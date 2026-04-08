# Analysis of SQLite Database Corruption Issues in PK-Sim

**Date:** 2026-04-08
**Repository:** Open-Systems-Pharmacology/PK-Sim
**Related Issues:** #3365, #3366, #3445, #3454

## Executive Summary

This document provides a comprehensive analysis of SQLite database corruption issues that occur intermittently in PK-Sim when saving or loading projects. The analysis identifies seven root causes and provides detailed solutions for each, organized by priority.

---

## Background

PK-Sim uses NHibernate to serialize projects into SQLite databases. The corruption issues manifest as:
- "database disk image is malformed" errors during save operations
- "file is not a database" errors when opening projects
- "no such table" errors during schema migrations
- Random failures that are not reproducible

The issues occur across different Windows systems and appear to be related to timing, file system interactions, and SQLite configuration.

---

## Section 1: Lack of WAL (Write-Ahead Logging) Mode

### Possible Cause of the Error

SQLite uses a rollback journal by default, which can leave databases in a corrupted state if a crash or power failure occurs during a write operation. The current implementation in `SessionFactoryProvider.cs` doesn't configure SQLite to use WAL (Write-Ahead Logging) mode.

**Current connection string:**
```csharp
Data Source={path};Version=3;New=False;Compress=True;
```

WAL mode provides:
- **Better crash recovery:** Changes are written to a separate WAL file first, making corruption less likely
- **Reduced risk of corruption:** The main database file is only updated during checkpoints
- **Better performance:** Readers don't block writers and vice versa
- **Atomic commits:** All changes in a transaction are committed atomically

**Evidence from code:**
- File: `/home/runner/work/PK-Sim/PK-Sim/src/PKSim.Infrastructure/Services/SessionFactoryProvider.cs:55`
- No WAL mode configuration present
- No PRAGMA statements to configure journaling mode

### What Could/Should Be Done in PK-Sim to Fix the Error

**Modify:** `/home/runner/work/PK-Sim/PK-Sim/src/PKSim.Infrastructure/Services/SessionFactoryProvider.cs`

1. **Update the connection string** to enable WAL mode:

```csharp
private Configuration createSqlLiteConfigurationFor(string dataSource)
{
   var configuration = new Configuration();
   var path = dataSource.ToUNCPath();

   configuration.SetProperty("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
   configuration.SetProperty("connection.driver_class", "NHibernate.Driver.SQLite20Driver");
   configuration.SetProperty("dialect", "NHibernate.Dialect.SQLiteDialect");
   configuration.SetProperty("query.substitutions", "true=1;false=0");
   configuration.SetProperty("show_sql", "false");

   // Add WAL mode and synchronous mode for better reliability
   configuration.SetProperty("connection.connection_string",
      $"Data Source={path};Version=3;New=False;Compress=True;Journal Mode=WAL;Synchronous=FULL;");

   return Fluently.Configure(configuration)
      .Mappings(cfg =>
      {
         cfg.HbmMappings.AddFromAssemblyOf<SessionFactoryProvider>();
         cfg.FluentMappings.AddFromAssemblyOf<SessionFactoryProvider>();
      }).BuildConfiguration();
}
```

2. **Execute PRAGMAs after opening connections** to ensure they're properly set:

```csharp
public ISessionFactory InitalizeSessionFactoryFor(string dataSource)
{
   var cfg = createSqlLiteConfigurationFor(dataSource);
   //Create schema for database
   new SchemaExport(cfg).Execute(false, true, false);

   var sessionFactory = createSessionFactory(cfg);

   // Ensure WAL mode is enabled on new databases
   using (var session = sessionFactory.OpenSession())
   {
      session.CreateSQLQuery("PRAGMA journal_mode=WAL;").ExecuteUpdate();
      session.CreateSQLQuery("PRAGMA synchronous=FULL;").ExecuteUpdate();
   }

   return sessionFactory;
}

public ISessionFactory OpenSessionFactoryFor(string dataSource)
{
   var cfg = createSqlLiteConfigurationFor(dataSource);
   var update = new SchemaUpdate(cfg);
   update.Execute(useStdOut: false, doUpdate: true);

   var sessionFactory = createSessionFactory(cfg);

   // Ensure WAL mode is enabled on existing databases
   using (var session = sessionFactory.OpenSession())
   {
      session.CreateSQLQuery("PRAGMA journal_mode=WAL;").ExecuteUpdate();
      session.CreateSQLQuery("PRAGMA synchronous=FULL;").ExecuteUpdate();
   }

   return sessionFactory;
}
```

**Benefits:**
- Significantly reduces corruption risk during crashes or power failures
- Improves concurrent access handling
- Better performance for read-heavy workloads
- Industry-standard approach for SQLite reliability

### What Could/Should Be Done in OSPSuite.Core to Fix the Error

If `OSPSuite.Core` has a similar `SessionFactoryProvider` or connection management for SQLite, apply the same WAL mode configuration. Additionally:

1. Ensure any direct SQLite connections created through `SQLiteProjectCommandExecuter` also enable WAL mode
2. Update connection string templates to include WAL mode
3. Add PRAGMA execution when opening connections in serialization services

---

## Section 2: Missing Busy Timeout Configuration

### Possible Cause of the Error

SQLite can return "database is locked" errors when multiple operations try to access the database simultaneously. Without a proper busy timeout, operations fail immediately instead of retrying. This is particularly problematic when:

- VACUUM is executed after committing transactions (as in `ProjectFileCompressor`)
- Multiple connections might be open simultaneously
- Background operations are running
- Antivirus software briefly locks files

**Current behavior:**
- No `busy_timeout` PRAGMA set
- No timeout in connection string
- Operations fail immediately on lock contention

**Evidence from issues:**
- Issue #3365: Corruption during COMMANDS table insert suggests possible lock contention
- Issue #3366: Failure during history clear operation

### What Could/Should Be Done in PK-Sim to Fix the Error

**Modify:** `/home/runner/work/PK-Sim/PK-Sim/src/PKSim.Infrastructure/Services/SessionFactoryProvider.cs`

1. **Update connection string** to include timeout:

```csharp
configuration.SetProperty("connection.connection_string",
   $"Data Source={path};Version=3;New=False;Compress=True;Journal Mode=WAL;Synchronous=FULL;Default Timeout=30000;");
```

2. **Set PRAGMA in session factory creation:**

```csharp
private static ISessionFactory createSessionFactory(Configuration cfg)
{
   var sessionFactory = cfg.BuildSessionFactory();
   sessionFactory.Evict(typeof(SimulationResults));
   sessionFactory.Evict(typeof(IndividualResults));

   // Set busy timeout on a test connection to ensure it's configured
   using (var session = sessionFactory.OpenSession())
   {
      session.CreateSQLQuery("PRAGMA busy_timeout=30000;").ExecuteUpdate();
   }

   return sessionFactory;
}
```

**Modify:** `/home/runner/work/PK-Sim/PK-Sim/src/PKSim.Infrastructure/Serialization/DatabaseSchemaMigrator.cs`

3. **Add timeout to migration connections:**

```csharp
public void MigrateSchema(string fileFullPath)
{
   var path = fileFullPath.ToUNCPath();
   using (var sqlLite = new SQLiteConnection($"Data Source={path};Journal Mode=WAL;Default Timeout=30000;"))
   {
      sqlLite.Open();

      // Set busy timeout
      using (var cmd = sqlLite.CreateCommand())
      {
         cmd.CommandText = "PRAGMA busy_timeout=30000;";
         cmd.ExecuteNonQuery();
      }

      migrateTo5_3(sqlLite);
      migrateTo6_2(sqlLite);
   }
}
```

**Benefits:**
- Operations retry automatically on lock contention instead of failing
- 30-second timeout provides ample time for transient locks to clear
- Handles antivirus and system interference gracefully
- Standard best practice for SQLite applications

### What Could/Should Be Done in OSPSuite.Core to Fix the Error

In `OSPSuite.Infrastructure.Serialization`:

1. Add busy timeout to all `SQLiteConnection` instances
2. Update `SQLiteProjectCommandExecuter` to set `PRAGMA busy_timeout`
3. Ensure NHibernate session configurations include timeout settings
4. Document the timeout value and rationale

---

## Section 3: VACUUM Running on Open Connection/Transaction

### Possible Cause of the Error

The `ProjectFileCompressor.Compress()` method runs `VACUUM` immediately after the main transaction is committed. However, the NHibernate session/connection might still be open or not fully flushed when VACUUM executes.

**Current flow in `WorkspacePersistor.SaveSession()`:**
```csharp
using (var session = _sessionManager.OpenSession())
using (var transaction = session.BeginTransaction())
{
   // ... save operations ...
   transaction.Commit();  // Line 76
}  // Session disposed here (line 77)

progress.IncrementProgress(PKSimConstants.UI.CompressionProject);
_projectFileCompressor.Compress(fileFullPath);  // Line 80 - VACUUM runs here
```

**Problems:**
1. VACUUM requires exclusive access to the database
2. Connection pooling might keep connections alive briefly after disposal
3. WAL checkpointing might not have completed
4. File handles might not be fully released

**Evidence:**
- File: `/home/runner/work/PK-Sim/PK-Sim/src/PKSim.Infrastructure/Serialization/WorkspacePersistor.cs:76-80`
- File: `/home/runner/work/PK-Sim/PK-Sim/src/PKSim.Infrastructure/Services/ProjectFileCompressor.cs:28`

### What Could/Should Be Done in PK-Sim to Fix the Error

**Option A: Close factory before VACUUM** (Recommended)

**Modify:** `/home/runner/work/PK-Sim/PK-Sim/src/PKSim.Infrastructure/Serialization/WorkspacePersistor.cs`

```csharp
public void SaveSession(ICoreWorkspace workspace, string fileFullPath)
{
   using (var progress = _progressManager.Create())
   {
      progress.Initialize(5);

      progress.IncrementProgress(PKSimConstants.UI.CreatingProjectDatabase);

      _sessionManager.CreateFactoryFor(fileFullPath);

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
      }  // Ensure session is fully disposed here

      // IMPORTANT: Close the session factory to release all connections
      // before running VACUUM
      _sessionManager.CloseFactory();

      progress.IncrementProgress(PKSimConstants.UI.CompressionProject);
      _projectFileCompressor.Compress(fileFullPath);

      // Re-open the factory for any subsequent operations if needed
      // This ensures UpdateProjectAfterSave can execute
      _sessionManager.OpenFactoryFor(fileFullPath);

      //once saved, we can update the project
      _projectPersistor.UpdateProjectAfterSave(workspace.Project);

      // Close again after update
      _sessionManager.CloseFactory();
   }

   workspace.Project.Name = FileHelper.FileNameFromFileFullPath(fileFullPath);
}
```

**Option B: Add WAL checkpoint before VACUUM**

**Modify:** `/home/runner/work/PK-Sim/PK-Sim/src/PKSim.Infrastructure/Services/ProjectFileCompressor.cs`

```csharp
private static void vacuum(DbConnection sqlLite)
{
   // Ensure all pending writes are completed and WAL is checkpointed
   sqlLite.ExecuteNonQuery("PRAGMA wal_checkpoint(FULL);");

   // Small delay to ensure all file handles are released
   System.Threading.Thread.Sleep(100);

   sqlLite.ExecuteNonQuery("VACUUM;");
}
```

**Benefits:**
- Eliminates race condition between connection closure and VACUUM
- Ensures database is fully flushed before compression
- Reduces corruption risk significantly
- Standard pattern for SQLite maintenance operations

### What Could/Should Be Done in OSPSuite.Core to Fix the Error

In `OSPSuite.Infrastructure.Serialization.Services.SQLiteProjectCommandExecuter`:

1. Ensure `ExecuteCommand` properly isolates connections for VACUUM
2. Add WAL checkpoint before VACUUM operations
3. Consider making VACUUM optional or running asynchronously
4. Document the requirement for exclusive access during VACUUM

---

## Section 4: Lack of Transaction Rollback on Error

### Possible Cause of the Error

When an exception occurs during save operations (as seen in issue #3365), the transaction may not be properly rolled back, leaving the database in an inconsistent state. The current implementation relies on NHibernate's automatic rollback on disposal, but this may not be sufficient for all error scenarios.

**Current implementation:**
```csharp
using (var session = _sessionManager.OpenSession())
using (var transaction = session.BeginTransaction())
{
   // ... save operations ...
   transaction.Commit();
}  // Automatic disposal - rollback may not occur properly on commit failures
```

**Problems:**
1. Exception during commit itself leaves state unclear
2. No explicit rollback call
3. No session flush before commit to catch errors early
4. Cascading saves might partially complete before failure

**Evidence:**
- Issue #3365: Exception during COMMANDS table insert suggests partial transaction completion
- File: `/home/runner/work/PK-Sim/PK-Sim/src/PKSim.Infrastructure/Serialization/WorkspacePersistor.cs:62-77`

### What Could/Should Be Done in PK-Sim to Fix the Error

**Modify:** `/home/runner/work/PK-Sim/PK-Sim/src/PKSim.Infrastructure/Serialization/WorkspacePersistor.cs`

```csharp
public void SaveSession(ICoreWorkspace workspace, string fileFullPath)
{
   using (var progress = _progressManager.Create())
   {
      progress.Initialize(5);

      progress.IncrementProgress(PKSimConstants.UI.CreatingProjectDatabase);

      _sessionManager.CreateFactoryFor(fileFullPath);

      ITransaction transaction = null;
      ISession session = null;

      try
      {
         session = _sessionManager.OpenSession();
         transaction = session.BeginTransaction();

         progress.IncrementProgress(PKSimConstants.UI.SavingProject);
         workspace.UpdateJournalPathRelativeTo(fileFullPath);
         _projectPersistor.Save(workspace.Project, session);

         progress.IncrementProgress(PKSimConstants.UI.SavingHistory);
         _historyManagerPersistor.Save(workspace.HistoryManager, session);

         progress.IncrementProgress(PKSimConstants.UI.SavingLayout);
         if (workspace is IWithWorkspaceLayout withWorkspaceLayout)
            _workspaceLayoutPersistor.Save(withWorkspaceLayout.WorkspaceLayout, session);

         // Flush the session to catch any errors before commit
         session.Flush();

         transaction.Commit();
      }
      catch (Exception)
      {
         // Explicitly rollback on any error
         transaction?.Rollback();
         throw;
      }
      finally
      {
         transaction?.Dispose();
         session?.Dispose();
      }

      // Close factory before VACUUM
      _sessionManager.CloseFactory();

      progress.IncrementProgress(PKSimConstants.UI.CompressionProject);
      _projectFileCompressor.Compress(fileFullPath);

      //once saved, we can update project after save
      _projectPersistor.UpdateProjectAfterSave(workspace.Project);
   }

   workspace.Project.Name = FileHelper.FileNameFromFileFullPath(fileFullPath);
}
```

**Key improvements:**
1. Explicit try-catch-finally blocks
2. `session.Flush()` before commit catches constraint violations early
3. Explicit `transaction.Rollback()` on errors
4. Proper disposal in finally block

**Benefits:**
- Ensures database consistency on errors
- Catches errors before commit when possible
- Explicit rollback prevents partial transactions
- Standard pattern for transaction management

### What Could/Should Be Done in OSPSuite.Core to Fix the Error

Review all transaction usage in `OSPSuite.Infrastructure.Serialization.ORM.History` and related classes:

1. Add explicit try-catch-finally blocks around transaction operations
2. Call `Rollback()` on exceptions
3. Call `Session.Flush()` before commit
4. Document transaction handling patterns
5. Consider creating a transaction helper utility

---

## Section 5: Database Schema Migration Issues During Corruption

### Possible Cause of the Error

Issue #3445 shows a failure during schema migration where the `SUMMARY_CHART` table doesn't exist, causing the migration to fail with "no such table: SUMMARY_CHART". The migration code assumes tables exist without verification.

**Current migration logic in `DatabaseSchemaMigrator.migrateTo5_3()`:**
```csharp
private void migrateTo5_3(SQLiteConnection sqlite)
{
   if (!needsConversionTo5_3(sqlite)) return;

   //rename table SUMMARY_CHART to SIMULATION_COMPARISONS
   sqlite.ExecuteNonQuery("ALTER TABLE SUMMARY_CHART RENAME TO SIMULATION_COMPARISONS");
   // ... rest of migration
}
```

**Problems:**
1. Assumes `SUMMARY_CHART` table exists
2. No error handling for missing tables
3. Partial migrations can leave database in broken state
4. No integrity checking before migrations
5. Unclear error messages when migration fails

**Evidence:**
- Issue #3445: "no such table: SUMMARY_CHART" error
- File: `/home/runner/work/PK-Sim/PK-Sim/src/PKSim.Infrastructure/Serialization/DatabaseSchemaMigrator.cs:36`

### What Could/Should Be Done in PK-Sim to Fix the Error

**Modify:** `/home/runner/work/PK-Sim/PK-Sim/src/PKSim.Infrastructure/Serialization/DatabaseSchemaMigrator.cs`

1. **Add comprehensive table existence checks:**

```csharp
private void migrateTo5_3(SQLiteConnection sqlite)
{
   if (!needsConversionTo5_3(sqlite)) return;

   // Verify the source table exists before attempting migration
   if (!hasTable(sqlite, "SUMMARY_CHART"))
   {
      // Table doesn't exist - might be a new database or already migrated
      // Check if target table exists
      if (!hasTable(sqlite, "SIMULATION_COMPARISONS"))
      {
         // Neither table exists - this is likely a new database, skip migration
         return;
      }
      // Target exists but source doesn't - already migrated
      return;
   }

   try
   {
      //rename table SUMMARY_CHART to SIMULATION_COMPARISONS
      sqlite.ExecuteNonQuery("ALTER TABLE SUMMARY_CHART RENAME TO SIMULATION_COMPARISONS");

      //create new table for individual sim comparisons
      var query = @"CREATE TABLE INDIVIDUAL_SIMULATION_COMPARISONS (
                     Id TEXT NOT NULL,
                     PRIMARY KEY (Id),
                     CONSTRAINT fk_SimulationComparision_IndividualSimulationComparison FOREIGN KEY (Id) REFERENCES SIMULATION_COMPARISONS)";

      sqlite.ExecuteNonQuery(query);

      //Copy all previous data from SimulationComparisons into INDIVIDUAL_SIMULATION_COMPARISONS
      query = "SELECT Id FROM SIMULATION_COMPARISONS";
      foreach (DataRow allSummaryChartIds in sqlite.ExecuteQueryForDataTable(query).Rows)
      {
         sqlite.ExecuteNonQuery($"INSERT INTO INDIVIDUAL_SIMULATION_COMPARISONS (Id) VALUES ('{allSummaryChartIds.StringAt("Id")}')");
      }
   }
   catch (Exception ex)
   {
      throw new InvalidOperationException(
         "Failed to migrate database schema from version 5.2 to 5.3. The database may be corrupted. " +
         "Please restore from a backup or contact support.", ex);
   }
}

private void migrateTo6_2(SQLiteConnection sqlite)
{
   if (!needsConversionTo6_2(sqlite)) return;

   // Verify source table exists
   if (!hasTable(sqlite, "OBSERVED_DATA"))
   {
      // Check if already migrated
      if (hasTable(sqlite, "USED_OBSERVED_DATA"))
         return;

      // Neither exists - might be new database
      return;
   }

   try
   {
      //rename table OBSERVED_DATA to USED_OBSERVED_DATA
      sqlite.ExecuteNonQuery("ALTER TABLE OBSERVED_DATA RENAME TO USED_OBSERVED_DATA");
   }
   catch (Exception ex)
   {
      throw new InvalidOperationException(
         "Failed to migrate database schema from version 6.1 to 6.2. The database may be corrupted. " +
         "Please restore from a backup or contact support.", ex);
   }
}
```

2. **Add database integrity checking:**

```csharp
public void MigrateSchema(string fileFullPath)
{
   var path = fileFullPath.ToUNCPath();
   using (var sqlLite = new SQLiteConnection($"Data Source={path};Journal Mode=WAL;Default Timeout=30000;"))
   {
      sqlLite.Open();

      // Set busy timeout
      using (var cmd = sqlLite.CreateCommand())
      {
         cmd.CommandText = "PRAGMA busy_timeout=30000;";
         cmd.ExecuteNonQuery();
      }

      // Check database integrity before attempting migrations
      if (!checkDatabaseIntegrity(sqlLite))
      {
         throw new InvalidOperationException(
            $"Database integrity check failed for '{fileFullPath}'. The database may be corrupted. " +
            "Please restore from a backup or contact support.");
      }

      migrateTo5_3(sqlLite);
      migrateTo6_2(sqlLite);
   }
}

private bool checkDatabaseIntegrity(SQLiteConnection sqlite)
{
   try
   {
      var result = sqlite.ExecuteQueryForSingleRow("PRAGMA integrity_check;");
      var status = result.StringAt(0);
      return status.Equals("ok", StringComparison.OrdinalIgnoreCase);
   }
   catch (Exception)
   {
      // If integrity check fails, assume corruption
      return false;
   }
}
```

**Benefits:**
- Handles missing tables gracefully
- Prevents migration failures on corrupted databases
- Detects corruption before migration attempts
- Provides clear, actionable error messages
- Handles partial migrations correctly

### What Could/Should Be Done in OSPSuite.Core to Fix the Error

If OSPSuite.Core has similar schema migration logic:

1. Add table existence checks before all ALTER TABLE operations
2. Add database integrity checks before migrations
3. Provide clear error messages distinguishing corruption from migration issues
4. Consider adding a migration history table
5. Implement idempotent migrations (can be run multiple times safely)

---

## Section 6: File "Not a Database" Error (Issue #3454)

### Possible Cause of the Error

Issue #3454 shows "file is not a database" error when trying to begin a transaction. This typically occurs when:

1. The file is corrupted beyond recognition as a SQLite database
2. The file was partially written (power failure, disk full, etc.)
3. Another process has locked or modified the file
4. The file exists but is empty or contains invalid data

**Root causes:**
- Previous save operation crashed mid-write
- Antivirus software interfered with file writes
- Disk errors occurred
- File accessed over network share with caching issues
- System crash during database write

**Evidence:**
- Issue #3454: "file is not a database" during `BeginTransaction()`
- File: `/home/runner/work/PK-Sim/PK-Sim/src/PKSim.Infrastructure/Serialization/WorkspacePersistor.cs` (no validation)

### What Could/Should Be Done in PK-Sim to Fix the Error

**Modify:** `/home/runner/work/PK-Sim/PK-Sim/src/PKSim.Infrastructure/Serialization/WorkspacePersistor.cs`

1. **Add database validation before opening:**

```csharp
public void LoadSession(ICoreWorkspace workspace, string fileFullPath)
{
   using (var progress = _progressManager.Create())
   {
      progress.Initialize(5);
      progress.IncrementProgress(PKSimConstants.UI.OpeningProjectDatabase);

      verifyProjectNotReadOnly(fileFullPath);

      // Verify the file is a valid SQLite database before attempting to open
      if (!isValidSQLiteDatabase(fileFullPath))
      {
         throw new PKSimException(
            $"The file '{fileFullPath}' is not a valid PK-Sim project database. " +
            "The file may be corrupted. Please restore from a backup.");
      }

      _databaseSchemaMigrator.MigrateSchema(fileFullPath);

      // ... rest of the code
   }
}

private bool isValidSQLiteDatabase(string fileFullPath)
{
   try
   {
      // Check file exists and has content
      var fileInfo = new FileInfo(fileFullPath);
      if (!fileInfo.Exists || fileInfo.Length < 100)
         return false;

      // Check SQLite magic header
      using (var fs = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
      using (var reader = new BinaryReader(fs))
      {
         var header = reader.ReadBytes(16);
         // SQLite files start with "SQLite format 3\0"
         var expectedHeader = new byte[] { 0x53, 0x51, 0x4c, 0x69, 0x74, 0x65, 0x20, 0x66, 0x6f, 0x72, 0x6d, 0x61, 0x74, 0x20, 0x33, 0x00 };

         for (int i = 0; i < expectedHeader.Length; i++)
         {
            if (header[i] != expectedHeader[i])
               return false;
         }
      }

      // Try to open and query the database
      var path = fileFullPath.ToUNCPath();
      using (var conn = new SQLiteConnection($"Data Source={path};Read Only=True;"))
      {
         conn.Open();
         using (var cmd = conn.CreateCommand())
         {
            cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master;";
            cmd.ExecuteScalar();
         }
      }

      return true;
   }
   catch (Exception)
   {
      return false;
   }
}
```

2. **Add automatic backup before save:**

```csharp
public void SaveSession(ICoreWorkspace workspace, string fileFullPath)
{
   using (var progress = _progressManager.Create())
   {
      progress.Initialize(5);

      // Create backup of existing file before overwriting
      createBackupIfExists(fileFullPath);

      progress.IncrementProgress(PKSimConstants.UI.CreatingProjectDatabase);

      // ... rest of save logic
   }
}

private void createBackupIfExists(string fileFullPath)
{
   if (!File.Exists(fileFullPath))
      return;

   try
   {
      var backupPath = $"{fileFullPath}.backup";
      // Keep only the most recent backup
      if (File.Exists(backupPath))
         File.Delete(backupPath);

      File.Copy(fileFullPath, backupPath, true);
   }
   catch (Exception)
   {
      // Don't fail the save if backup creation fails
      // But log the error
   }
}
```

**Benefits:**
- Detects corruption before attempting operations
- Provides clear error messages to users
- Automatic backups enable recovery
- Validates file format integrity
- Prevents cascading failures from invalid files

### What Could/Should Be Done in OSPSuite.Core to Fix the Error

1. Add database validation methods to detect corruption early
2. Implement automatic backup mechanisms before risky operations
3. Add recovery options to restore from `.backup` files
4. Consider "safe save" pattern: write to temp file, verify, then move
5. Add file format validation utilities

---

## Section 7: Network/Antivirus Interference

### Possible Cause of the Error

SQLite is designed for local file systems and can exhibit issues when:

1. **Files are on network shares** with caching/locking issues
2. **Antivirus software** scans files during write operations
3. **Cloud sync services** (OneDrive, Dropbox, etc.) interfere with file writes
4. **File system limitations** - some network file systems don't support proper locking

**Why this causes corruption:**
- Network file systems may not guarantee atomic writes
- Caching can cause stale data to be read/written
- File locking may not work correctly across network
- Antivirus can hold file locks during scanning
- Cloud sync can modify files while application is using them

**Evidence:**
- Intermittent nature of issues suggests external interference
- Issues occur on different systems (environmental factors)
- No retry logic for transient failures in current code

### What Could/Should Be Done in PK-Sim to Fix the Error

**Modify:** `/home/runner/work/PK-Sim/PK-Sim/src/PKSim.Infrastructure/Workspace.cs`

1. **Add location validation and warnings:**

```csharp
public void SaveProject(string fileFullPath)
{
   // Warn if saving to potentially problematic location
   warnIfProblematicLocation(fileFullPath);

   //try to lock the file if it exists or is not lock already
   LockFile(fileFullPath);

   //notify the action project saving
   _eventPublisher.PublishEvent(new ProjectSavingEvent(_project));

   _workspacePersistor.SaveSession(this, fileFullPath);
   // ... rest of method
}

private void warnIfProblematicLocation(string fileFullPath)
{
   try
   {
      var drive = new DriveInfo(Path.GetPathRoot(fileFullPath));

      // Check if on network drive
      if (drive.DriveType == DriveType.Network)
      {
         _dialogCreator.MessageBoxInfo(
            "You are saving the project to a network location. " +
            "This may cause database corruption issues. " +
            "It is strongly recommended to save projects to a local drive.");
      }

      // Check if in cloud sync folder (OneDrive, Dropbox, Google Drive, etc.)
      var lowerPath = fileFullPath.ToLower();
      if (lowerPath.Contains("onedrive") ||
          lowerPath.Contains("dropbox") ||
          lowerPath.Contains("google drive") ||
          lowerPath.Contains("icloud"))
      {
         _dialogCreator.MessageBoxInfo(
            "You are saving the project to a cloud-synced folder. " +
            "This may cause database corruption issues. " +
            "It is strongly recommended to save projects to a non-synced local folder.");
      }
   }
   catch (Exception)
   {
      // Ignore errors in location checking
   }
}
```

**Modify:** `/home/runner/work/PK-Sim/PK-Sim/src/PKSim.Infrastructure/Services/SessionFactoryProvider.cs`

2. **Add retry logic for transient failures:**

```csharp
public ISessionFactory InitalizeSessionFactoryFor(string dataSource)
{
   return retryOperation(() =>
   {
      var cfg = createSqlLiteConfigurationFor(dataSource);
      new SchemaExport(cfg).Execute(false, true, false);
      return createSessionFactory(cfg);
   });
}

public ISessionFactory OpenSessionFactoryFor(string dataSource)
{
   return retryOperation(() =>
   {
      var cfg = createSqlLiteConfigurationFor(dataSource);
      var update = new SchemaUpdate(cfg);
      update.Execute(useStdOut: false, doUpdate: true);
      return createSessionFactory(cfg);
   });
}

private T retryOperation<T>(Func<T> operation, int maxRetries = 3)
{
   for (int i = 0; i < maxRetries; i++)
   {
      try
      {
         return operation();
      }
      catch (Exception) when (i < maxRetries - 1)
      {
         // Wait before retry (exponential backoff)
         System.Threading.Thread.Sleep(100 * (i + 1));
      }
   }

   // Last attempt without catching
   return operation();
}
```

**Benefits:**
- Users are warned about risky save locations
- Retry logic handles transient failures from antivirus/network
- Reduces support burden from environmental issues
- Follows SQLite best practices for deployment

### What Could/Should Be Done in OSPSuite.Core to Fix the Error

1. Add similar location validation and warnings
2. Implement retry logic for all database operations
3. Add "test write" operation before saving to detect issues
4. Document supported and unsupported storage locations
5. Consider adding configuration option to disable warnings for advanced users

---

## Priority Recommendations

### High Priority (Critical - Implement Immediately)

1. **Enable WAL Mode** (Section 1)
   - Most important for preventing corruption
   - Industry-standard solution
   - Minimal risk, high reward

2. **Add Busy Timeout** (Section 2)
   - Prevents lock-related failures
   - Simple to implement
   - Handles concurrent access gracefully

3. **Fix VACUUM Timing** (Section 3)
   - Addresses immediate corruption risk
   - Well-defined solution
   - Critical for database maintenance

4. **Add Transaction Rollback** (Section 4)
   - Ensures consistency on errors
   - Standard best practice
   - Low risk, high value

### Medium Priority (Important - Implement Soon)

5. **Improve Migration Robustness** (Section 5)
   - Prevents migration-related corruption
   - Makes system more resilient
   - Better error messages for users

6. **Add Database Validation** (Section 6)
   - Detects corruption early
   - Enables recovery options
   - Improves user experience

### Low Priority (Nice to Have - Implement When Possible)

7. **Add Location Warnings** (Section 7)
   - Helps users avoid problems
   - Educational value
   - Reduces support burden

---

## Implementation Strategy

### Phase 1: Core Reliability (Week 1-2)
Implement high-priority fixes (Sections 1-4) in PK-Sim:
- Day 1-2: WAL mode configuration
- Day 3-4: Busy timeout settings
- Day 5-6: VACUUM timing fixes
- Day 7-8: Transaction rollback
- Day 9-10: Testing and validation

### Phase 2: Resilience (Week 3)
Implement medium-priority fixes (Sections 5-6):
- Day 1-3: Schema migration improvements
- Day 4-5: Database validation
- Day 6-7: Automatic backups

### Phase 3: User Experience (Week 4)
Implement low-priority fixes and documentation (Section 7):
- Day 1-2: Location warnings
- Day 3-4: Retry logic
- Day 5-7: Testing and documentation

### Phase 4: OSPSuite.Core Coordination
Work with OSPSuite.Core maintainers to apply similar fixes

---

## Testing Requirements

### Unit Tests
- WAL mode is correctly set on new databases
- Busy timeout is configured properly
- Transaction rollback occurs on errors
- Validation methods detect corrupted files
- Backup files are created correctly

### Integration Tests
- Save and load large projects multiple times
- Simulate crashes during save operations
- Test migration from old database versions
- Test with corrupted database files
- Test on network drives (with warnings)

### Performance Tests
- Measure save/load times before and after
- Verify VACUUM duration with WAL checkpoint
- Check memory usage during operations
- Monitor database file size with WAL mode

### Compatibility Tests
- Projects created with new version open in old version
- Old projects migrate correctly to new version
- WAL mode doesn't break existing functionality

---

## Success Metrics

1. **Zero corruption reports** in testing with new implementation
2. **Successful crash recovery** - database consistent after process termination
3. **All migrations work** correctly from version 5.2 onwards
4. **No performance regression** - operations within 10% of baseline
5. **Clear error messages** when corruption is detected
6. **Automatic backup/restore** works reliably

---

## Risk Assessment

### Low Risk Changes
- Adding busy timeout ✓
- Adding transaction rollback ✓
- Adding validation checks ✓
- Adding backups ✓

### Medium Risk Changes
- WAL mode (changes file format, but compatible) ⚠
- VACUUM timing (could affect performance) ⚠
- Retry logic (could increase operation time) ⚠

### High Risk Changes
- Schema migration logic (must be thoroughly tested) ⚠⚠

---

## Conclusion

The SQLite database corruption issues in PK-Sim stem from multiple factors, primarily:
1. Lack of proper SQLite configuration (WAL mode, timeouts)
2. Timing issues with VACUUM operations
3. Inadequate error handling and transaction management
4. Environmental factors (network storage, antivirus)

The proposed solutions are well-established best practices for SQLite applications and should significantly reduce corruption occurrences. Implementation should proceed in phases, starting with the highest-priority fixes that provide the most value with the least risk.

All changes maintain backward compatibility with existing project files while improving reliability and providing better error messages when issues occur. The combination of preventive measures (WAL mode, timeouts), defensive programming (validation, rollback), and user guidance (warnings, backups) creates a robust system that minimizes corruption risk.

---

## References

- **PK-Sim Issues:**
  - #3365: Database corruption during COMMANDS table save
  - #3366: Corruption during history clear operation
  - #3445: Missing table during schema migration
  - #3454: "File is not a database" error

- **Code Locations:**
  - `/home/runner/work/PK-Sim/PK-Sim/src/PKSim.Infrastructure/Services/SessionFactoryProvider.cs`
  - `/home/runner/work/PK-Sim/PK-Sim/src/PKSim.Infrastructure/Serialization/WorkspacePersistor.cs`
  - `/home/runner/work/PK-Sim/PK-Sim/src/PKSim.Infrastructure/Serialization/DatabaseSchemaMigrator.cs`
  - `/home/runner/work/PK-Sim/PK-Sim/src/PKSim.Infrastructure/Services/ProjectFileCompressor.cs`

- **SQLite Documentation:**
  - Write-Ahead Logging: https://www.sqlite.org/wal.html
  - PRAGMA Statements: https://www.sqlite.org/pragma.html
  - How To Corrupt An SQLite Database: https://www.sqlite.org/howtocorrupt.html
