# PK-Sim Database Corruption Analysis

## Executive Summary

This document analyzes potential causes of SQLite database corruption in PK-Sim projects and provides detailed recommendations for fixes. The analysis is based on examination of the V13 branch of both the PK-Sim and OSPSuite.Core repositories.

Database corruption in PK-Sim appears to be caused by multiple factors related to SQLite transaction management, connection handling, concurrent access patterns, and missing error recovery mechanisms. The issues are primarily in the OSPSuite.Core serialization infrastructure, with some PK-Sim-specific concerns.

---

## Section 1: Changes Needed in OSPSuite.Core

### 1.1 Missing Transaction Rollback on Save Failures

**Possible Cause:**

In [`SessionManager.cs`](https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/Services/SessionManager.cs), when creating a factory for an existing file (lines 77-82), the code performs a `File.Copy` operation while a session factory is still open for the source file. If an exception occurs during this operation or subsequently, there's no transaction rollback mechanism in place.

The transaction handling is done at a higher level (e.g., in WorkspacePersistor), but if an exception occurs during the NHibernate operations before `transaction.Commit()` is called, the transaction is not explicitly rolled back. While the transaction should auto-rollback on disposal, this is not guaranteed if the session or connection is in an inconsistent state.

**Code Location:**
- https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/Services/SessionManager.cs#L77-L82

**What Should Be Modified:**

1. Wrap all transactional operations in explicit try-catch-finally blocks with rollback handling
2. Ensure proper disposal order: transaction → session → factory

**Recommended Code Changes:**

In any code that uses transactions (primarily in consumer code like PK-Sim's WorkspacePersistor), implement:

```csharp
ITransaction transaction = null;
try
{
    transaction = session.BeginTransaction();
    // ... perform operations
    transaction.Commit();
}
catch (Exception)
{
    transaction?.Rollback();
    throw;
}
finally
{
    transaction?.Dispose();
}
```

### 1.2 Missing SQLite PRAGMA Settings for Data Integrity

**Possible Cause:**

The connection string in [`ConnectionExtensions.cs`](https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/Extensions/ConnectionExtensions.cs#L7) does not configure critical SQLite settings for data integrity:

```csharp
return $"Data Source={dataSource};Foreign Keys=False;Pooling=False";
```

Missing settings include:
- **journal_mode**: Not set (defaults to DELETE mode which is less crash-safe than WAL)
- **synchronous**: Not set (defaults to FULL, but should be explicitly set)
- **busy_timeout**: Not configured, leading to immediate failures on locks
- **cache_size**: Uses default, which may be inadequate for large transactions

When SQLite writes data in DELETE journal mode, there's a small window where a crash or power failure can corrupt the database. WAL (Write-Ahead Logging) mode is significantly more robust.

**What Should Be Modified:**

Enhance the connection string with proper SQLite PRAGMA settings to ensure data integrity and better handle concurrent access scenarios.

**Recommended Code Changes:**

Modify [`ConnectionExtensions.cs`](https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/Extensions/ConnectionExtensions.cs):

```csharp
public static string ConnectionStringFor(string dataSource)
{
    // WAL mode is more crash-resistant and allows better concurrency
    // NORMAL synchronous is a good balance between safety and performance
    // busy_timeout gives time for locks to be released instead of immediate failure
    return $"Data Source={dataSource};Foreign Keys=False;Pooling=False;Journal Mode=WAL;Synchronous=NORMAL;Busy Timeout=5000;Cache Size=-2000";
}
```

Additionally, add initialization code to set PRAGMAs that cannot be set via connection string:

```csharp
public class SQLiteProjectCommandExecuter
{
    public virtual void ExecuteCommand(string projectFile, Action<DbConnection> command)
    {
        string file = projectFile.ToUNCPath();
        using (var sqlLite = new SqliteConnection(ConnectionStringHelper.ConnectionStringFor(file)))
        {
            sqlLite.Open();
            // Set additional PRAGMAs for data integrity
            using (var pragmaCmd = sqlLite.CreateCommand())
            {
                pragmaCmd.CommandText = "PRAGMA journal_mode=WAL; PRAGMA synchronous=NORMAL; PRAGMA busy_timeout=5000;";
                pragmaCmd.ExecuteNonQuery();
            }
            command(sqlLite);
        }
    }
}
```

### 1.3 VACUUM Execution While NHibernate Session May Still Be Active

**Possible Cause:**

In the save workflow, VACUUM is executed immediately after the transaction commits but potentially before the session and session factory are fully closed. Looking at the flow in PK-Sim's WorkspacePersistor:

1. Session and transaction opened (using statements)
2. Data saved
3. Transaction committed
4. Using blocks exit (session disposed)
5. **VACUUM executed via a NEW connection**

While this creates a new connection, if the NHibernate session factory still has the database file locked or has pending operations, VACUUM might fail or cause issues. VACUUM requires exclusive access to the database and rewrites the entire database file.

**Code Location:**
- https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/Services/SQLiteProjectCommandExecuter.cs

**What Should Be Modified:**

1. Ensure all NHibernate resources are fully disposed before VACUUM
2. Add retry logic for VACUUM operations
3. Handle VACUUM failures gracefully without corrupting the database

**Recommended Code Changes:**

Modify [`SQLiteProjectCommandExecuter.cs`](https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/Services/SQLiteProjectCommandExecuter.cs):

```csharp
public virtual void ExecuteCommand(string projectFile, Action<DbConnection> command)
{
    string file = projectFile.ToUNCPath();

    // Add retry logic for operations that might encounter locks
    int retryCount = 0;
    const int maxRetries = 3;

    while (retryCount < maxRetries)
    {
        try
        {
            using (var sqlLite = new SqliteConnection(ConnectionStringHelper.ConnectionStringFor(file)))
            {
                sqlLite.Open();

                // Set PRAGMAs for data integrity
                using (var pragmaCmd = sqlLite.CreateCommand())
                {
                    pragmaCmd.CommandText = "PRAGMA busy_timeout=5000;";
                    pragmaCmd.ExecuteNonQuery();
                }

                command(sqlLite);
                return; // Success
            }
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 5) // SQLITE_BUSY
        {
            retryCount++;
            if (retryCount >= maxRetries)
                throw;

            System.Threading.Thread.Sleep(100 * retryCount); // Exponential backoff
        }
    }
}
```

### 1.4 File Copy During Active Session (Save As scenario)

**Possible Cause:**

In [`SessionManager.cs`](https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/Services/SessionManager.cs#L77-L82), the "Save As" scenario copies the database file while the session factory might still have locks:

```csharp
if (sessionFactoryIsOpen())
{
    File.Copy(_currentFileName, fileName, true);
    CloseFactory();
    _sessionFactory = _sessionFactoryProvider.OpenSessionFactoryFor(fileName);
}
```

If `File.Copy` is interrupted or fails partway through, the destination file could be corrupted. Additionally, copying a SQLite database while it might be in use can lead to inconsistent state if not all data has been flushed to disk.

**What Should Be Modified:**

1. Close the session factory BEFORE copying the file
2. Add proper error handling and cleanup if copy fails
3. Ensure SQLite checkpoint is performed before copy (especially in WAL mode)

**Recommended Code Changes:**

Modify [`SessionManager.cs`](https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/Services/SessionManager.cs):

```csharp
public void CreateFactoryFor(string fileName)
{
    //Project file is already open for the same filename
    if (sessionFactoryIsAlreadyOpenFor(fileName)) return;

    //Project file open, but for another name => save as
    if (sessionFactoryIsOpen())
    {
        string sourceFile = _currentFileName;

        // CRITICAL: Close factory first to release all locks
        CloseFactory();

        // Perform WAL checkpoint on source file to ensure all data is in main db
        checkpointDatabase(sourceFile);

        try
        {
            // Now safe to copy
            File.Copy(sourceFile, fileName, true);
        }
        catch (Exception)
        {
            // If copy fails, reopen the source
            _sessionFactory = _sessionFactoryProvider.OpenSessionFactoryFor(sourceFile);
            _currentFileName = sourceFile;
            throw;
        }

        _sessionFactory = _sessionFactoryProvider.OpenSessionFactoryFor(fileName);
    }
    //new project from scratch. save file
    else
    {
        FileHelper.DeleteFile(fileName);
        _sessionFactory = _sessionFactoryProvider.InitializeSessionFactoryFor(fileName);
    }

    _currentFileName = fileName;
}

private void checkpointDatabase(string databasePath)
{
    try
    {
        using (var connection = new SqliteConnection(ConnectionStringHelper.ConnectionStringFor(databasePath)))
        {
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "PRAGMA wal_checkpoint(FULL);";
                cmd.ExecuteNonQuery();
            }
        }
    }
    catch
    {
        // If checkpoint fails, log but don't prevent operation
        // In non-WAL mode this will fail harmlessly
    }
}
```

### 1.5 No Validation After Critical Operations

**Possible Cause:**

There is no database integrity validation after save operations or after VACUUM. If corruption occurs during these operations, it goes undetected until the next time the file is opened, at which point it may be too late to recover.

SQLite provides `PRAGMA integrity_check` which can detect many types of corruption.

**What Should Be Modified:**

Add integrity checks after critical operations like save and VACUUM to detect corruption early.

**Recommended Code Changes:**

Add a new validation service in OSPSuite.Infrastructure.Serialization:

```csharp
public interface IDatabaseIntegrityChecker
{
    bool VerifyIntegrity(string databasePath);
    IEnumerable<string> GetIntegrityErrors(string databasePath);
}

public class DatabaseIntegrityChecker : IDatabaseIntegrityChecker
{
    private readonly SQLiteProjectCommandExecuter _commandExecuter;

    public DatabaseIntegrityChecker(SQLiteProjectCommandExecuter commandExecuter)
    {
        _commandExecuter = commandExecuter;
    }

    public bool VerifyIntegrity(string databasePath)
    {
        return GetIntegrityErrors(databasePath).Count() == 0;
    }

    public IEnumerable<string> GetIntegrityErrors(string databasePath)
    {
        return _commandExecuter.ExecuteCommand(databasePath, connection =>
        {
            var errors = new List<string>();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "PRAGMA integrity_check;";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var result = reader.GetString(0);
                        if (result != "ok")
                            errors.Add(result);
                    }
                }
            }
            return errors;
        });
    }
}
```

### 1.6 Missing Proper Flush Before Transaction Commit

**Possible Cause:**

NHibernate batches SQL statements for performance. If a transaction is committed before all changes are flushed to the database, some operations might be lost. While NHibernate typically flushes automatically before commit, explicit flushing ensures all changes are persisted and helps identify issues earlier.

Looking at the code, there are no explicit `session.Flush()` calls before `transaction.Commit()` in the save workflow.

**What Should Be Modified:**

Add explicit flush operations before committing transactions to ensure all NHibernate changes are written to the database.

**Recommended Code Changes:**

This change would be in the consumer code (see Section 2), but document the pattern here:

```csharp
using (var session = _sessionManager.OpenSession())
using (var transaction = session.BeginTransaction())
{
    // ... perform all save operations

    // Explicitly flush to ensure all changes are written
    session.Flush();

    transaction.Commit();
}
```

### 1.7 Cascade Delete Operations Without Explicit Ordering

**Possible Cause:**

The NHibernate mappings use `Cascade.AllDeleteOrphan` extensively (seen in ProjectMetaDataMapping and others). When updating complex object graphs with many relationships, NHibernate might execute DELETE and INSERT operations in an order that temporarily violates referential integrity constraints.

While foreign keys are disabled in the connection string (`Foreign Keys=False`), this means constraint violations are not caught, which can lead to orphaned records or referential integrity issues that manifest as corruption.

**What Should Be Modified:**

1. Consider enabling foreign keys and handling constraint violations properly
2. Use explicit save/update ordering for complex operations
3. Implement proper cascade handling in UpdateFrom methods

**Recommended Code Changes:**

Modify [`ConnectionExtensions.cs`](https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/Extensions/ConnectionExtensions.cs):

```csharp
public static string ConnectionStringFor(string dataSource)
{
    // Enable foreign keys to catch referential integrity violations early
    return $"Data Source={dataSource};Foreign Keys=True;Pooling=False;Journal Mode=WAL;Synchronous=NORMAL;Busy Timeout=5000";
}
```

Then ensure all mappings and update operations respect foreign key constraints properly.

### 1.8 Collection Update Logic May Leave Orphaned Records

**Possible Cause:**

In [`UpdatableCollectionExtensions.cs`](https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/ORM/MetaData/UpdatableCollectionExtensions.cs#L9-L32), the UpdateFrom method removes items from collections:

```csharp
foreach (var child in targetCollection.ToList())
{
    if (sourceCollection.Contains(child)) continue;
    //does not exist. Remove
    targetCollection.Remove(child);
}
```

If the cascade settings are not properly configured, or if there's an exception during the removal, this could leave orphaned records in the database that reference deleted parent records.

**What Should Be Modified:**

Improve the update logic to be more transactional and handle errors properly.

**Recommended Code Changes:**

Modify [`UpdatableCollectionExtensions.cs`](https://github.com/Open-Systems-Pharmacology/OSPSuite.Core/blob/V13/src/OSPSuite.Infrastructure.Serialization/ORM/MetaData/UpdatableCollectionExtensions.cs):

```csharp
public static void UpdateFrom<TKey, TValue>(this ICollection<TValue> targetCollection, ICollection<TValue> sourceCollection, ISession session)
    where TValue : MetaData<TKey>, IUpdatableFrom<TValue>
{
    var targetDictionary = targetCollection.ToDictionary(entity => entity.Id);
    var itemsToRemove = new List<TValue>();

    // First pass: add new items and update existing ones
    foreach (var sourceChild in sourceCollection)
    {
        if (!targetCollection.Contains(sourceChild))
        {
            targetCollection.Add(sourceChild);
        }
        else
        {
            targetDictionary[sourceChild.Id].UpdateFrom(sourceChild, session);
        }
    }

    // Second pass: identify items to remove
    foreach (var child in targetCollection.ToList())
    {
        if (!sourceCollection.Contains(child))
        {
            itemsToRemove.Add(child);
        }
    }

    // Third pass: remove items (after flush to ensure consistency)
    session.Flush();

    foreach (var itemToRemove in itemsToRemove)
    {
        targetCollection.Remove(itemToRemove);
        // Explicitly delete if cascade is not configured
        session.Delete(itemToRemove);
    }
}
```

---

## Section 2: Changes Needed in PK-Sim

### 2.1 No Error Handling Around VACUUM Operation

**Possible Cause:**

In [`WorkspacePersistor.cs`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/WorkspacePersistor.cs#L79-L80), the VACUUM operation is called without any error handling:

```csharp
progress.IncrementProgress(PKSimConstants.UI.CompressionProject);
_projectFileCompressor.Compress(fileFullPath);
```

If VACUUM fails (due to disk space, locks, or corruption), the exception propagates up but the project has already been saved. This can leave the database in an un-vacuumed state with wasted space, or worse, if VACUUM partially completes, it could corrupt the database.

VACUUM is a non-critical optimization operation and should not cause save operations to fail.

**Code Location:**
- https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/WorkspacePersistor.cs#L79-L80

**What Should Be Modified:**

Wrap VACUUM in a try-catch block and handle failures gracefully. Log the error but don't fail the save operation.

**Recommended Code Changes:**

Modify [`WorkspacePersistor.cs`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/WorkspacePersistor.cs):

```csharp
progress.IncrementProgress(PKSimConstants.UI.CompressionProject);
try
{
    _projectFileCompressor.Compress(fileFullPath);
}
catch (Exception ex)
{
    // Log the error but don't fail the save
    // The project is already saved; VACUUM is just an optimization
    _logger.AddToLog($"Warning: Failed to compress database: {ex.Message}");
}

//once saved, we can
_projectPersistor.UpdateProjectAfterSave(workspace.Project);
```

### 2.2 Missing Explicit Session Flush Before Transaction Commit

**Possible Cause:**

In [`WorkspacePersistor.cs`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/WorkspacePersistor.cs#L62-L77), the save workflow does not explicitly flush the NHibernate session before committing:

```csharp
using (var session = _sessionManager.OpenSession())
using (var transaction = session.BeginTransaction())
{
    progress.IncrementProgress(PKSimConstants.UI.SavingProject);
    workspace.UpdateJournalPathRelativeTo(fileFullPath);
    _projectPersistor.Save(workspace.Project, session);

    progress.IncrementProgress(PKSimConstants.UI.SavingHistory);
    _historyManagerPersistor.Save(workspace.HistoryManager, session);

    progress.IncrementProgress(PKSimConstants.UI.SavingLayout);
    if (workspace is IWithWorkspaceLayout withWorkspaceLayout)
        _workspaceLayoutPersistor.Save(withWorkspaceLayout.WorkspaceLayout, session);

    transaction.Commit();
}
```

While NHibernate should auto-flush before commit, making this explicit ensures all changes are persisted and can help catch errors earlier in the process.

**What Should Be Modified:**

Add explicit flush calls before transaction commit.

**Recommended Code Changes:**

Modify [`WorkspacePersistor.cs`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/WorkspacePersistor.cs):

```csharp
using (var session = _sessionManager.OpenSession())
using (var transaction = session.BeginTransaction())
{
    try
    {
        progress.IncrementProgress(PKSimConstants.UI.SavingProject);
        workspace.UpdateJournalPathRelativeTo(fileFullPath);
        _projectPersistor.Save(workspace.Project, session);

        progress.IncrementProgress(PKSimConstants.UI.SavingHistory);
        _historyManagerPersistor.Save(workspace.HistoryManager, session);

        progress.IncrementProgress(PKSimConstants.UI.SavingLayout);
        if (workspace is IWithWorkspaceLayout withWorkspaceLayout)
            _workspaceLayoutPersistor.Save(withWorkspaceLayout.WorkspaceLayout, session);

        // Explicitly flush to ensure all changes are written to the database
        session.Flush();

        transaction.Commit();
    }
    catch (Exception)
    {
        transaction.Rollback();
        throw;
    }
}
```

### 2.3 No Database Integrity Check After Save

**Possible Cause:**

After saving and compressing the database, there is no verification that the database is still valid and not corrupted. If corruption occurs during the save process, it won't be detected until the next load attempt.

**What Should Be Modified:**

Add an integrity check after the save operation completes (after VACUUM).

**Recommended Code Changes:**

Modify [`WorkspacePersistor.cs`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/WorkspacePersistor.cs):

```csharp
private readonly IDatabaseIntegrityChecker _integrityChecker;

// In constructor
public WorkspacePersistor(
    IProjectPersistor projectPersistor,
    IHistoryManagerPersistor historyManagerPersistor,
    IWorkspaceLayoutPersistor workspaceLayoutPersistor,
    ISessionManager sessionManager,
    IProgressManager progressManager,
    IProjectFileCompressor projectFileCompressor,
    IDatabaseSchemaMigrator databaseSchemaMigrator,
    IJournalLoader journalLoader,
    IProjectClassifiableUpdaterAfterDeserialization projectClassifiableUpdaterAfterDeserialization,
    IDatabaseIntegrityChecker integrityChecker)
{
    // ... existing assignments
    _integrityChecker = integrityChecker;
}

public void SaveSession(ICoreWorkspace workspace, string fileFullPath)
{
    using (var progress = _progressManager.Create())
    {
        progress.Initialize(6); // One more step

        progress.IncrementProgress(PKSimConstants.UI.CreatingProjectDatabase);
        _sessionManager.CreateFactoryFor(fileFullPath);

        using (var session = _sessionManager.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            try
            {
                progress.IncrementProgress(PKSimConstants.UI.SavingProject);
                workspace.UpdateJournalPathRelativeTo(fileFullPath);
                _projectPersistor.Save(workspace.Project, session);

                progress.IncrementProgress(PKSimConstants.UI.SavingHistory);
                _historyManagerPersistor.Save(workspace.HistoryManager, session);

                progress.IncrementProgress(PKSimConstants.UI.SavingLayout);
                if (workspace is IWithWorkspaceLayout withWorkspaceLayout)
                    _workspaceLayoutPersistor.Save(withWorkspaceLayout.WorkspaceLayout, session);

                session.Flush();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        // Close the session factory to release all locks before VACUUM
        _sessionManager.CloseFactory();

        progress.IncrementProgress(PKSimConstants.UI.CompressionProject);
        try
        {
            _projectFileCompressor.Compress(fileFullPath);
        }
        catch (Exception ex)
        {
            _logger.AddToLog($"Warning: Failed to compress database: {ex.Message}");
        }

        // Verify database integrity after save
        progress.IncrementProgress("Verifying database integrity...");
        if (!_integrityChecker.VerifyIntegrity(fileFullPath))
        {
            var errors = _integrityChecker.GetIntegrityErrors(fileFullPath);
            throw new DatabaseCorruptedException($"Database integrity check failed: {string.Join(", ", errors)}");
        }

        // Reopen the factory for continued use
        _sessionManager.OpenFactoryFor(fileFullPath);

        //once saved, we can
        _projectPersistor.UpdateProjectAfterSave(workspace.Project);
    }

    workspace.Project.Name = FileHelper.FileNameFromFileFullPath(fileFullPath);
}
```

### 2.4 Session Factory Not Properly Closed Before VACUUM

**Possible Cause:**

The VACUUM operation in [`ProjectFileCompressor.cs`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Services/ProjectFileCompressor.cs) opens a new connection, but the NHibernate session factory might still have the database file locked or have pending operations.

Looking at the flow in SaveSession:
1. Session and transaction are disposed (using blocks exit)
2. VACUUM is called
3. Session factory is NOT closed

The session factory maintains connection pools and locks even after individual sessions are closed.

**What Should Be Modified:**

Ensure the session factory is closed before VACUUM, then reopened if needed.

**Recommended Code Changes:**

See modification in 2.3 above - the session factory should be closed before VACUUM and reopened after integrity check.

### 2.5 Database Schema Migration Without Transaction Protection

**Possible Cause:**

In [`DatabaseSchemaMigrator.cs`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/DatabaseSchemaMigrator.cs#L20-L29), schema migrations are performed without transaction protection:

```csharp
public void MigrateSchema(string fileFullPath)
{
    var path = fileFullPath.ToUNCPath();
    using (var sqlLite = new SqliteConnection(ConnectionStringHelper.ConnectionStringFor(path)))
    {
        sqlLite.Open();
        migrateTo5_3(sqlLite);
        migrateTo6_2(sqlLite);
    }
}
```

If a migration partially completes and then fails, the database could be left in an inconsistent state between schema versions.

**What Should Be Modified:**

Wrap each migration in a transaction so that failures roll back cleanly.

**Recommended Code Changes:**

Modify [`DatabaseSchemaMigrator.cs`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/DatabaseSchemaMigrator.cs):

```csharp
public void MigrateSchema(string fileFullPath)
{
    var path = fileFullPath.ToUNCPath();
    using (var sqlLite = new SqliteConnection(ConnectionStringHelper.ConnectionStringFor(path)))
    {
        sqlLite.Open();

        // Enable WAL mode for better crash safety during migration
        using (var pragmaCmd = sqlLite.CreateCommand())
        {
            pragmaCmd.CommandText = "PRAGMA journal_mode=WAL;";
            pragmaCmd.ExecuteNonQuery();
        }

        // Wrap each migration in a transaction
        using (var transaction = sqlLite.BeginTransaction())
        {
            try
            {
                migrateTo5_3(sqlLite);
                migrateTo6_2(sqlLite);
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
```

### 2.6 NHibernate Second-Level Cache Not Properly Evicted

**Possible Cause:**

In [`SessionFactoryProvider.cs`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Services/SessionFactoryProvider.cs#L33-L35), SimulationResults and IndividualResults are evicted from the second-level cache:

```csharp
sessionFactory.Evict(typeof(SimulationResults));
sessionFactory.Evict(typeof(IndividualResults));
```

However, if other entities are cached and become stale, this could lead to inconsistencies when saving. If the cached entity differs from what's in the database, the UpdateFrom operations might produce incorrect results.

**What Should Be Modified:**

Consider disabling second-level caching for all entities involved in serialization, or ensure cache is properly cleared at appropriate times.

**Recommended Code Changes:**

Modify [`SessionFactoryProvider.cs`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Services/SessionFactoryProvider.cs):

```csharp
private Configuration createSqlLiteConfigurationFor(string dataSource)
{
    var configuration = new Configuration();
    var path = dataSource.ToUNCPath();

    configuration.SetProperty("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
    configuration.SetProperty("connection.driver_class", typeof(SqliteDriver).AssemblyQualifiedName);
    configuration.SetProperty("dialect", typeof(SqliteDialect).AssemblyQualifiedName);
    configuration.SetProperty("query.substitutions", "true=1;false=0");
    configuration.SetProperty("show_sql", "false");
    configuration.SetProperty("connection.connection_string", ConnectionStringHelper.ConnectionStringFor(path));

    // Disable second-level cache to prevent stale data issues
    configuration.SetProperty("cache.use_second_level_cache", "false");
    configuration.SetProperty("cache.use_query_cache", "false");

    return Fluently.Configure(configuration)
        .Mappings(cfg =>
        {
            cfg.HbmMappings.AddFromAssemblyOf<SessionFactoryProvider>();
            cfg.FluentMappings.AddFromAssemblyOf<SessionFactoryProvider>();
        }).BuildConfiguration();
}
```

### 2.7 Exception Handling in Load Does Not Clean Up Properly

**Possible Cause:**

In [`WorkspacePersistor.cs`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/WorkspacePersistor.cs#L137-L143), if an exception occurs during loading, the session factory is closed but the workspace might be left in a partially-loaded state:

```csharp
catch (Exception)
{
    //Exception occurs while opening the project!
    //close the file and rethrow the exception
    _sessionManager.CloseFactory();
    throw;
}
```

This is good, but the workspace object itself might have partial data (e.g., HistoryManager loaded but Project failed). This could cause issues if the calling code doesn't properly handle the exception.

**What Should Be Modified:**

Ensure the workspace is reset to a clean state on load failure.

**Recommended Code Changes:**

Modify [`WorkspacePersistor.cs`](https://github.com/Open-Systems-Pharmacology/PK-Sim/blob/V13/src/PKSim.Infrastructure/Serialization/WorkspacePersistor.cs):

```csharp
catch (Exception)
{
    //Exception occurs while opening the project!
    //Reset workspace to clean state
    workspace.HistoryManager = null;
    workspace.Project = null;
    workspace.Journal = null;
    if (workspace is IWithWorkspaceLayout withWorkspaceLayout)
        withWorkspaceLayout.WorkspaceLayout = null;

    //close the file and rethrow the exception
    _sessionManager.CloseFactory();
    throw;
}
```

---

## Additional Recommendations

### Create Automatic Backups Before Save

Consider implementing automatic backup creation before overwriting existing project files:

```csharp
public void SaveSession(ICoreWorkspace workspace, string fileFullPath)
{
    // Create backup of existing file
    if (File.Exists(fileFullPath))
    {
        var backupPath = $"{fileFullPath}.backup";
        File.Copy(fileFullPath, backupPath, true);
    }

    try
    {
        // ... existing save logic

        // Delete backup on successful save
        if (File.Exists($"{fileFullPath}.backup"))
            File.Delete($"{fileFullPath}.backup");
    }
    catch (Exception)
    {
        // Restore from backup on failure
        if (File.Exists($"{fileFullPath}.backup"))
        {
            File.Copy($"{fileFullPath}.backup", fileFullPath, true);
            File.Delete($"{fileFullPath}.backup");
        }
        throw;
    }
}
```

### Implement Database Recovery Tools

Add a database repair utility that users can run if corruption is detected:

1. **Dump and Restore**: Export data to SQL and reimport to a fresh database
2. **SQLite Recovery**: Use `.recover` command to extract data from corrupted databases
3. **Partial Recovery**: Attempt to load and re-save individual building blocks

### Add Comprehensive Logging

Implement detailed logging around all database operations to help diagnose issues:

```csharp
_logger.AddToLog($"Beginning save to {fileFullPath}");
_logger.AddToLog($"Session factory created");
_logger.AddToLog($"Transaction started");
_logger.AddToLog($"Project saved");
// ... etc
```

### Consider Periodic Integrity Checks

Add an option for users to run periodic integrity checks on their open projects, possibly:
- On load (optional, as it adds time)
- On save (as shown in recommendations)
- As a manual menu option
- Automatically in the background every N minutes

---

## Summary of Critical Fixes

**Highest Priority (OSPSuite.Core):**
1. Enable WAL mode and proper SQLite PRAGMAs (Section 1.2)
2. Fix File.Copy during active session (Section 1.4)
3. Add transaction rollback handling (Section 1.1)
4. Ensure session factory is closed before VACUUM (Section 1.3)

**Highest Priority (PK-Sim):**
1. Close session factory before VACUUM (Section 2.4)
2. Add explicit flush before commit (Section 2.2)
3. Wrap VACUUM in try-catch (Section 2.1)
4. Protect schema migrations with transactions (Section 2.5)

**Medium Priority:**
- Add database integrity validation (Sections 1.5, 2.3)
- Improve collection update logic (Section 1.8)
- Disable second-level cache (Section 2.6)

**Low Priority (but recommended):**
- Implement automatic backups
- Add recovery tools
- Enhance logging

These changes should significantly reduce the occurrence of database corruption in PK-Sim projects.
