# PK-Sim Project (SQLite) Corruption — Root-Cause Analysis and Fix Proposal

## Background

A PK-Sim project file (`.pksim5`) is a **SQLite database**. It is serialized/deserialized through **NHibernate + FluentNHibernate** in `PKSim.Infrastructure`, which is built on top of `OSPSuite.Core` / `OSPSuite.Infrastructure.Serialization`. Saving and loading of a project ultimately go through:

- `PKSim.Infrastructure` → [`CoreWorkspacePersistor`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/CoreWorkspacePersistor.cs), [`SessionFactoryProvider`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Services/SessionFactoryProvider.cs), [`ProjectFileCompressor`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Services/ProjectFileCompressor.cs), [`DatabaseSchemaMigrator`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/DatabaseSchemaMigrator.cs).
- `OSPSuite.Core` → [`ConnectionStringHelper`](https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/Extensions/ConnectionExtensions.cs), [`SessionManager`](https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/Services/SessionManager.cs), [`SQLiteProjectCommandExecuter`](https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/Services/SQLiteProjectCommandExecuter.cs), [`HistoryTask`](https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/Services/HistoryTask.cs).

### Symptoms reported in the referenced issues

| Issue | Error message | Operation in stack trace |
| --- | --- | --- |
| [#3365](https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3365) | `database disk image is malformed` | `INSERT INTO COMMANDS ...` while saving history |
| [#3366](https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3366) | `database disk image is malformed` | `clearCommand` / `HistoryTask` (`DELETE FROM HISTORY_ITEMS` + `VACUUM`) |
| [#3445](https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3445) | `no such table: SUMMARY_CHART` | `migrateTo5_3` during load (schema migration) |
| [#3454](https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3454) | `file is not a database` | `BeginTransaction` while saving |
| [#3583](https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3583) | `database disk image is malformed` | `SELECT ... FROM COMMANDS` during load |

These are **classic SQLite corruption / durability signatures**. `database disk image is malformed` and `file is not a database` are raised by SQLite when the page image on disk is inconsistent with the database header, or when a write-ahead-log (`-wal`) / rollback-journal side-car file was lost, truncated, or separated from the main file. `no such table: SUMMARY_CHART` indicates a partially-migrated / partially-written schema. All of them are non-deterministic and hardware/environment-dependent, which matches the "cannot reproduce, happens on different systems" observation.

### Fundamental architectural weaknesses (shared by both repos)

1. **The save path writes directly into the user's live file and then `VACUUM`s it in place.** There is no "write to a temporary file, then atomically replace the original" and no `.bak` backup. Any interruption during the save (crash, power loss, antivirus lock, cloud-sync lock, disk-full) leaves the *original* project unrecoverable.
2. **The connection string sets no durability or concurrency PRAGMAs** (`journal_mode`, `synchronous`, `busy_timeout`). The `Microsoft.Data.Sqlite` driver used in V13 defaults to `journal_mode=WAL` + `synchronous=NORMAL`, which creates `-wal`/`-shm` side-car files and reduces the fsync guarantees on the main file — a poor fit for a *single-file document* that users copy, move, and place on OneDrive / network shares.
3. **Multiple independent connections are opened against the same file for one logical operation** (NHibernate session for data, a separate `SqliteConnection` for `VACUUM`, another for schema migration, another for the journal). Because pooling is disabled and no busy-timeout is set, these connections can collide with each other and with external file locks.
4. **Destructive maintenance operations (`VACUUM`, mass `DELETE`) are run with no integrity check and no transactional safety net**, so a partial/failed maintenance directly corrupts the file.

The two sections below split the concrete remediation into the two code bases.

---

# Section 1 — Changes needed in OSPSuite.Core (`OSPSuite.Infrastructure.Serialization`)

> All code in this section lives under
> `https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/`.
> These changes benefit **every** OSP application (PK-Sim, MoBi), because the connection string, the session manager and the history/vacuum helpers are all defined here.

## 1.1 The connection string does not configure any SQLite durability/concurrency PRAGMAs

### Possible cause of the error

The single place that builds every SQLite connection string is [`ConnectionStringHelper.ConnectionStringFor`](https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/Extensions/ConnectionExtensions.cs):

```csharp
public static string ConnectionStringFor(string dataSource)
{
   return $"Data Source={dataSource};Foreign Keys=False;Pooling=False";
}
```

This connection string is used by **all** consumers:

- NHibernate (via [`PKSim.Infrastructure/Services/SessionFactoryProvider.cs#L55`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Services/SessionFactoryProvider.cs#L55)),
- the VACUUM executor ([`SQLiteProjectCommandExecuter`](https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/Services/SQLiteProjectCommandExecuter.cs)),
- the schema migrator ([`DatabaseSchemaMigrator.cs#L23`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/DatabaseSchemaMigrator.cs#L23)),
- and the journal connection provider ([`Journal/IConnectionProvider.cs`](https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/Journal/IConnectionProvider.cs)).

It specifies **no** `Journal Mode`, **no** `Synchronous`, **no** `Default Timeout` / busy timeout, and **no** `Cache` mode. Consequences:

- **WAL side-car files (`-wal`, `-shm`).** V13 uses `Microsoft.Data.Sqlite`, whose default `journal_mode` is **WAL** and default `synchronous` is **NORMAL**. WAL writes recent transactions into a separate `<project>.pksim5-wal` file. If the application/OS terminates without a clean checkpoint, or the user copies/moves/uploads only the main `.pksim5` file (very common — it's presented to the user as a single document), the committed data still sitting in the `-wal` file is lost or "detached", and SQLite then reports **`database disk image is malformed`** or **`file is not a database`** (issues [#3365](https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3365), [#3454](https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3454), [#3583](https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3583)).
- **`synchronous=NORMAL` weakens crash durability.** In NORMAL mode SQLite does not fsync on every commit; on a power loss or hard crash the file header/pages can be left inconsistent → `database disk image is malformed`.
- **No busy timeout.** With multiple connections opened for one operation, and with external agents (antivirus real-time scanning, Windows Search indexer, OneDrive/Dropbox/Google Drive sync) briefly locking the file, a write that hits a locked page fails immediately instead of retrying. A failed/aborted write mid-transaction is a direct corruption vector.

### What should be modified to fix the error

Modify `ConnectionStringHelper.ConnectionStringFor` (or add an overload) to produce a *durable, single-file-friendly* connection string, and route **all** OSP callers through it:

1. **Force rollback-journal mode instead of WAL** for the main project database. Add `Journal Mode=Delete` (or `Truncate`) to the connection string so that the whole project is always contained in a **single file** with no side-car. This eliminates the entire class of "user copied only the .pksim5" corruption.
   - Alternatively, keep WAL but guarantee a **checkpoint + WAL removal on close** (see §1.2). Journal-mode `Delete` is strongly preferred for a document-style application.
2. **Set `synchronous` to `FULL`** (via `PRAGMA synchronous=FULL` after opening, or the equivalent connection-string keyword) for the project database, so every commit is flushed to disk. The small performance cost is acceptable for a "Save" that happens on user action.
3. **Add a busy timeout** (e.g. `Default Timeout=30` seconds and/or `PRAGMA busy_timeout=30000`) so transient locks from antivirus / indexer / cloud-sync cause a *retry* instead of an immediate failure.
4. Because `Microsoft.Data.Sqlite` does not accept every PRAGMA as a connection-string keyword, introduce a small helper that, immediately after `connection.Open()`, executes the required `PRAGMA` statements (`journal_mode`, `synchronous`, `busy_timeout`, `foreign_keys`). Call it from every place that opens a project connection: `SQLiteProjectCommandExecuter`, `ConnectionProvider`, `DatabaseSchemaMigrator`, and configure NHibernate to run them via a connection interceptor / `Configuration` property. This keeps the settings in one place instead of duplicating literals.
5. Keep `Pooling=False` (already present) — pooling would keep file handles alive after `Close()` and defeat the goal of releasing the file promptly for the subsequent `VACUUM`/`File.Copy` steps.

## 1.2 `SessionManager` performs "Save As" via a raw `File.Copy` of a (possibly still-open / un-checkpointed) database

### Possible cause of the error

In [`SessionManager.CreateFactoryFor`](https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/Services/SessionManager.cs), when the currently-open factory points to a different file (the "Save As …" scenario), the code performs a raw `File.Copy(_currentFileName, fileName, true)` before closing the current factory and opening a new one against the copy.

- If a WAL is in use (the default, see §1.1), the `-wal` file is **not** copied — only the main `.pksim5` file is. The destination copy therefore misses whatever committed data still lives in the source `-wal`, producing an immediately-malformed target file.
- `File.Copy` is not atomic with respect to concurrent readers/writers. If any handle to the source file is still open, the copied bytes can be an inconsistent snapshot.

### What should be modified to fix the error

1. **Checkpoint and fully release the source database before copying.** Before `File.Copy`, ensure the source session factory is closed and a `PRAGMA wal_checkpoint(TRUNCATE)` has been executed (only relevant if WAL is retained; with §1.1's `Journal Mode=Delete` there is no side-car to worry about, but the source must still be closed first). Reorder the logic so `CloseFactory()` happens **before** the copy, then copy, then `OpenSessionFactoryFor`.
2. **Copy all associated side-car files** if WAL is retained (`-wal`, `-shm`) — or, preferably, avoid the problem entirely by switching to `Journal Mode=Delete` in §1.1.
3. Consider replacing "copy then reopen" with "**save into the new file**" (create a fresh factory/schema for the destination and serialize the in-memory project into it), which is inherently consistent and does not depend on the physical byte-state of the source file.

## 1.3 `SQLiteProjectCommandExecuter` runs `VACUUM` with no integrity check, no backup, and (with WAL) can corrupt on failure

### Possible cause of the error

[`SQLiteProjectCommandExecuter.ExecuteCommand`](https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/Services/SQLiteProjectCommandExecuter.cs) opens a brand-new `SqliteConnection` with the same PRAGMA-less connection string and runs an arbitrary command against the project file. Its main use from PK-Sim is `VACUUM` (see [`ProjectFileCompressor.cs#L28`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Services/ProjectFileCompressor.cs#L28)).

- `VACUUM` **rewrites the entire database file** by building a temporary copy and then overwriting the original. If it is interrupted (crash, disk full, antivirus/cloud-sync lock), the file can be left partially written → `database disk image is malformed`.
- `VACUUM` interacts badly with WAL mode and with any other still-open connection to the same file. Because the connection here is separate from the NHibernate session, ordering/locking issues are possible if the session's file handle has not been fully released.
- No `PRAGMA integrity_check` is run before `VACUUM`, so an already-slightly-damaged file is "compacted" (and thus made unrecoverable) instead of being detected.

### What should be modified to fix the error

1. Run **`PRAGMA integrity_check`** (or `quick_check`) *before* executing `VACUUM`. If it does not return `ok`, abort and surface a clear error rather than vacuuming a damaged file.
2. Ensure the NHibernate session/factory for the file is **fully closed** (file handle released) before this separate connection opens, so `VACUUM` is the sole writer.
3. Apply the durable connection settings from §1.1 to this connection as well (it currently inherits the PRAGMA-less string).
4. Preferably perform the whole save-then-compact sequence against a **temporary working file** (see Section 2.1) so that a failed `VACUUM` never touches the user's original.

## 1.4 `HistoryTask.clearCommand` issues mass `DELETE` + `VACUUM` on the live file without transactional safety

### Possible cause of the error

[`HistoryTask.clearCommand`](https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/Services/HistoryTask.cs) executes, on a fresh connection, a sequence of `DELETE FROM HISTORY_ITEMS`, `DELETE FROM COMMAND_PROPERTIES`, `DELETE FROM COMMANDS`, followed by `VACUUM` — each via a separate `ExecuteNonQuery`. This is exactly the code in the stack trace of issue [#3366](https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3366) (`database disk image is malformed`).

- The individual statements are **not wrapped in a single transaction**, so a failure between the `DELETE`s and the `VACUUM` leaves the file half-cleared.
- Immediately following mass deletes with `VACUUM` maximises file rewriting — the highest-risk moment for interruption-driven corruption.
- Same PRAGMA-less connection (§1.1), so no `synchronous=FULL`, no busy timeout.

### What should be modified to fix the error

1. Wrap the three `DELETE`s in a **single explicit transaction** and commit before doing anything else.
2. Run `PRAGMA integrity_check` before the operation; skip/abort `VACUUM` if the file is already damaged.
3. Use the durable connection settings from §1.1.
4. Consider decoupling `VACUUM` from the clear operation (or making it best-effort and non-fatal), because a failed compaction should never corrupt an otherwise-valid file.

## 1.5 The command-history table (`COMMANDS`) grows unbounded and is repeatedly rewritten in place

### Possible cause of the error

[`HistoryManagerPersistor.Save`](https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/ORM/History/HistoryManagerPersistor.cs) appends every new command to a `COMMANDS` table that keeps growing for the lifetime of the project. The insert path is the one that fails in issue [#3365](https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3365) (`INSERT INTO COMMANDS`) and the read path is the one that fails in issue [#3583](https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3583) (`SELECT … FROM COMMANDS`).

- A larger file and more write operations statistically increase the exposure to any single failed/aborted write, which — combined with the durability gaps in §1.1 — makes long-lived, heavily-edited projects the most likely to corrupt (consistent with the reports that corruption appears after extended editing sessions).
- The COMMANDS/history data is *audit/undo* data, not the model itself; it is disproportionately responsible for write volume.

### What should be modified to fix the error

1. This is primarily a **durability** problem (fix §1.1 first). In addition, evaluate bounding or pruning the persisted command history (e.g. cap the number of persisted commands, or make full-history persistence optional) to reduce write volume and file size.
2. Ensure history saving participates in the **same transaction** as the rest of the save (in PK-Sim it already does, see [`CoreWorkspacePersistor.cs#L59-L72`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/CoreWorkspacePersistor.cs#L59-L72)); confirm the OSPSuite.Core persistor never opens its own out-of-band connection for history writes.

---

# Section 2 — Changes needed in PK-Sim (`PKSim.Infrastructure` / `PKSim.Presentation`)

> All code in this section lives under
> `https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/`.

## 2.1 `CoreWorkspacePersistor.SaveSession` writes directly into the user's file and VACUUMs it in place — no atomic replace, no backup

### Possible cause of the error

[`CoreWorkspacePersistor.SaveSession`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/CoreWorkspacePersistor.cs#L48-L83) saves *straight into* `fileFullPath` (the user's real project file) and then compacts it in place:

```csharp
_sessionManager.CreateFactoryFor(fileFullPath);           // creates schema in the destination file
using (var session = _sessionManager.OpenSession())
using (var transaction = session.BeginTransaction())
{
   _projectPersistor.Save(workspace.Project, session);     // #3454 fails here (BeginTransaction / file is not a database)
   _historyManagerPersistor.Save(workspace.HistoryManager, session);  // #3365 fails here (INSERT INTO COMMANDS)
   SaveLayoutFor(workspace, session);
   transaction.Commit();
}
_projectFileCompressor.Compress(fileFullPath);             // in-place VACUUM
```

- Because the write targets the live file, **any** failure during schema creation, insert, commit, or the subsequent in-place `VACUUM` leaves the user's original project **damaged and unrecoverable**. There is no temporary file and no `.bak` copy to fall back to.
- `CreateFactoryFor` on an *existing* destination path (Save = overwrite same file) recreates schema on top of a file that is simultaneously the source of truth; combined with the durability gaps of §1.1, this is where issue [#3454](https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3454) (`file is not a database`) surfaces.
- The in-place `VACUUM` immediately after commit doubles the amount of file rewriting at the exact end of the save, the most likely moment for antivirus/cloud-sync to grab a lock.

### What should be modified to fix the error

Implement an **atomic, crash-safe save** in `SaveSession`:

1. **Save to a temporary file, then atomically replace the original.**
   - Serialize the project (schema create + inserts + commit + `VACUUM`) into a fresh temporary file next to the target (e.g. `<project>.pksim5.saving` on the *same volume*, so the replace is a rename, not a cross-device copy).
   - Only after the temporary file is fully written, committed, and passes a `PRAGMA integrity_check`, atomically move it into place using `File.Replace` (which preserves the ability to keep a backup of the previous file) or a `Delete original` + `Move temp → original` sequence.
2. **Keep a backup of the previous version.** Use `File.Replace(temp, target, target + ".bak")` (or copy the existing target to `<project>.pksim5.bak` before replacing). If replacement fails, the user still has both the old file and the freshly-written temp file.
3. **Only VACUUM the temporary file**, never the user's live file, so a failed compaction can never damage the original.
4. On any exception, **delete the temporary file and leave the original untouched**; surface a clear "save failed, your project was not modified" message.
5. Apply the durable connection settings from §1.1 (they come from OSPSuite.Core; PK-Sim just needs to consume the updated `ConnectionStringHelper` / NHibernate configuration — see §2.3).

> This single change neutralises the majority of the reported corruption reports, because it makes the *original* file immutable until a complete, integrity-checked replacement exists.

## 2.2 `DatabaseSchemaMigrator` performs raw DDL directly on the original file, un-transacted, and swallows probe exceptions

### Possible cause of the error

[`DatabaseSchemaMigrator.MigrateSchema`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/DatabaseSchemaMigrator.cs#L20-L29) opens the **user's project file directly** and runs schema-altering DDL against it *before* NHibernate ever opens it — and this happens on the load path ([`CoreWorkspacePersistor.cs#L93`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/CoreWorkspacePersistor.cs#L93)):

```csharp
migrateTo5_3(sqlite);   // ALTER TABLE SUMMARY_CHART RENAME ...; CREATE TABLE ...; INSERT loop
migrateTo6_2(sqlite);   // ALTER TABLE OBSERVED_DATA RENAME ...
```

- The multiple `ALTER TABLE` / `CREATE TABLE` / `INSERT` statements in [`migrateTo5_3`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/DatabaseSchemaMigrator.cs#L32-L54) are **not wrapped in a transaction**. If the process dies (or the file is already partially damaged) between the `RENAME` and the `CREATE`/`INSERT`, the schema is left half-migrated — exactly the `no such table: SUMMARY_CHART` state of issue [#3445](https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3445).
- [`hasTable`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/DatabaseSchemaMigrator.cs#L77-L88) **swallows all exceptions and returns `false`**. If the file is corrupt (not merely old-schema), the probe `SELECT COUNT(*) FROM SIMULATION_COMPARISONS` throws `malformed`, is caught, returns `false`, and the migrator then tries `ALTER TABLE SUMMARY_CHART RENAME …` on a table that no longer exists — masking the real corruption behind a misleading `no such table` error.
- The migration runs **in-place on the original file**, so a failed migration corrupts the source.
- The `INSERT` in [`migrateTo5_3`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/DatabaseSchemaMigrator.cs#L52) interpolates the id directly into SQL string (`… VALUES ('{allSummaryChartIds.StringAt("Id")}')`); while the ids are internal GUIDs, it is still string-built SQL and should be parameterised.

### What should be modified to fix the error

1. **Run the whole migration inside a single transaction** and commit only if all steps succeed; on failure, roll back so the file is never left half-migrated.
2. **Migrate a working copy, not the original.** Combine with §2.1: migrate against the temporary/working file (or make a `.bak` first) so a failed migration cannot destroy the user's project.
3. **Distinguish "table missing" from "database corrupt."** In `hasTable`, do not blanket-catch every exception as "table absent". Detect corruption errors (`malformed`, `file is not a database`, `disk I/O`) and **abort with a clear corruption message** instead of proceeding to `ALTER TABLE`. Run a `PRAGMA integrity_check` at the very start of `MigrateSchema` and refuse to migrate a damaged file.
4. Prefer `SELECT name FROM sqlite_master WHERE type='table' AND name=@name` (parameterised) to test table existence, instead of `SELECT COUNT(*) FROM <table>` inside a try/catch — this is both safer and cannot be confused with an I/O error.
5. Parameterise the `INSERT INTO INDIVIDUAL_SIMULATION_COMPARISONS` statement.
6. Apply the durable connection settings from §1.1 to the migrator's `SqliteConnection` ([line 23](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/DatabaseSchemaMigrator.cs#L23)).

## 2.3 NHibernate SQLite configuration lacks safe PRAGMAs / release-mode settings

### Possible cause of the error

[`SessionFactoryProvider.createSqlLiteConfigurationFor`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Services/SessionFactoryProvider.cs#L45-L63) configures NHibernate with the driver/dialect from `NHibernate.Extensions.Sqlite` and the connection string from `ConnectionStringHelper.ConnectionStringFor` ([line 55](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Services/SessionFactoryProvider.cs#L55)):

```csharp
configuration.SetProperty("connection.connection_string",
   ConnectionStringHelper.ConnectionStringFor(path));
```

- Because the connection string carries no durability PRAGMAs (§1.1), NHibernate opens the project database in WAL / `synchronous=NORMAL` with no busy timeout — the same weak configuration used everywhere else.
- There is no `connection.release_mode` / connection-release configuration, so it is not explicit when NHibernate releases the underlying file handle. If the handle lingers past `transaction.Commit()`, the following in-place `VACUUM` in [`CoreWorkspacePersistor.cs#L75`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/CoreWorkspacePersistor.cs#L75) can contend with a still-open connection.
- `InitializeSessionFactoryFor` runs `new SchemaExport(cfg).Execute(false, true, false)` — DDL against the (destination) file — inheriting the same weak settings.

### What should be modified to fix the error

1. Route NHibernate through the **hardened connection settings** from §1.1 (so it also gets `Journal Mode=Delete`, `synchronous=FULL`, `busy_timeout`). If those cannot all be expressed in the connection string for the `Microsoft.Data.Sqlite` provider, register an NHibernate **connection interceptor** (or a custom `IConnectionProvider`) that runs the required `PRAGMA` statements immediately after each connection is opened.
2. Set an explicit `connection.release_mode` (e.g. `on_close`) and ensure the session/factory is disposed and the file handle released **before** `ProjectFileCompressor.Compress` runs.
3. Keep `Pooling=False` so file handles are not retained by a pool after close.

## 2.4 The save/load flow keeps opening several independent connections to the same file with no coordination

### Possible cause of the error

For a single Save, the file is touched by: the NHibernate factory/session (`CreateFactoryFor` + `OpenSession`, [`CoreWorkspacePersistor.cs#L56-L58`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/CoreWorkspacePersistor.cs#L56-L58)), then a **separate** `SqliteConnection` for `VACUUM` ([`ProjectFileCompressor.cs`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Services/ProjectFileCompressor.cs)). For a Load, the file is first opened by a **separate** `SqliteConnection` in `DatabaseSchemaMigrator` ([line 23](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/DatabaseSchemaMigrator.cs#L23)) and then again by NHibernate, plus a journal connection.

With pooling disabled and **no busy timeout**, any overlap between these handles — or with an external agent (antivirus, indexer, OneDrive/Dropbox/Google Drive) — surfaces as an immediate lock failure mid-write, which is a corruption vector.

### What should be modified to fix the error

1. Guarantee **strict ordering**: each stage opens the file, completes, and fully closes/releases the handle before the next stage opens it. In particular, close the NHibernate factory before `VACUUM`, and close the migrator connection before opening the NHibernate factory on load.
2. Give every one of these connections the busy timeout from §1.1 so transient external locks are retried rather than fatal.
3. Combined with §2.1 (work on a temporary file) and §2.2 (migrate a copy), the number of live handles on the *user's* file is minimised.

## 2.5 No detection or warning for projects stored on network / cloud-synced / read-only locations

### Possible cause of the error

SQLite explicitly documents that its file locking is unreliable on network filesystems, and that real-time cloud-sync clients (OneDrive, Dropbox, Google Drive) can copy or lock a database mid-write. The "different systems, cannot reproduce" pattern in the issues strongly correlates with such environments. PK-Sim currently opens/saves the file wherever the user points it, with only a read-only check ([`CoreWorkspacePersistor.verifyProjectNotReadOnly`, lines 150-156](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/CoreWorkspacePersistor.cs#L150-L156)) — nothing detects network/cloud paths or `-wal`/`-shm` leftovers.

### What should be modified to fix the error

1. **Detect risky locations** (UNC/network drives, and known cloud-sync folders) when opening or saving, and **warn the user** that the project should be kept on a local disk while in use. This does not need to block the operation but should be surfaced prominently.
2. On load, **detect leftover `-wal`/`-shm` side-car files** for the project (a sign of a previous unclean shutdown) and either checkpoint/recover them or warn the user before proceeding. With the §1.1 switch to `Journal Mode=Delete`, new projects won't produce these, but older/interrupted files may still have them.
3. Optionally, offer/enable **automatic backups** (e.g. keep the last N `.bak` versions) so users can always roll back after a corruption event.

## 2.6 Failed loads/saves surface low-level SQLite errors instead of actionable, recoverable messages

### Possible cause of the error

- On load, [`CoreWorkspacePersistor.LoadSession`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/CoreWorkspacePersistor.cs#L128-L134) catches, closes the factory, and **rethrows the raw exception** — so the user sees `database disk image is malformed` with no guidance and no offer to restore a backup.
- [`ProjectPersistor.projectFromDatabase`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/ProjectPersistor.cs#L68-L77) treats a `Count == 0` result as "corrupt or not a pk-sim project" and returns `null`, conflating *empty/corrupt* with *not-a-project*, which can mask a genuine corruption as a "not a PK-Sim file" style failure.

### What should be modified to fix the error

1. On load, run a `PRAGMA integrity_check` (via the OSPSuite.Core connection helper) up-front and, if it fails, present a **dedicated "project file is corrupt" dialog** that offers to open the most recent `.bak`/backup (from §2.5/§2.1) instead of throwing a raw SQLite message.
2. Distinguish genuine corruption from "old/empty/foreign file" so the correct, actionable message is shown.
3. Keep the existing read-only guard, but add the risky-location and side-car warnings from §2.5.

---

# Summary of recommended change order

The fixes are ordered by impact-to-effort. The first two eliminate most reported corruption.

1. **OSPSuite.Core §1.1** — harden the connection string (`Journal Mode=Delete`, `synchronous=FULL`, `busy_timeout`) and route all callers through it. *Removes the WAL side-car loss class and weak durability.*
2. **PK-Sim §2.1** — atomic save (temp file + integrity check + `File.Replace` with `.bak` backup); never VACUUM the live file. *Makes the original immutable until a verified replacement exists.*
3. **PK-Sim §2.2** — transactional, copy-based, corruption-aware schema migration; stop swallowing I/O errors as "table missing" (fixes #3445-class symptoms).
4. **OSPSuite.Core §1.3 / §1.4** — `integrity_check` before `VACUUM`; wrap history-clear `DELETE`s in a transaction; decouple/soften `VACUUM`.
5. **OSPSuite.Core §1.2** — checkpoint + close before "Save As" `File.Copy` (or save into a fresh file).
6. **PK-Sim §2.3 / §2.4** — consume hardened settings in NHibernate config; enforce strict single-handle ordering across migrate/session/vacuum/journal.
7. **PK-Sim §2.5 / §2.6 & OSPSuite.Core §1.5** — network/cloud-path and side-car warnings, backup/restore UX, clearer corruption errors, and bounding of the growing `COMMANDS` history.

## Referenced issues

- [#3365 — database disk image is malformed (INSERT INTO COMMANDS)](https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3365)
- [#3366 — database disk image is malformed (HistoryTask clearCommand / DELETE + VACUUM)](https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3366)
- [#3445 — no such table: SUMMARY_CHART (migrateTo5_3)](https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3445)
- [#3454 — file is not a database (BeginTransaction during save)](https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3454)
- [#3583 — database disk image is malformed (SELECT FROM COMMANDS during load)](https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3583)
