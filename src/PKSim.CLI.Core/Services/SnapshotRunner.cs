using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Assets.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
using PKSim.CLI.Core.RunOptions;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Services;

namespace PKSim.CLI.Core.Services
{
   public enum SnapshotExportMode
   {
      /// <summary>
      ///    Exports project files from json files
      /// </summary>
      Project,

      /// <summary>
      ///    Exports json files from project files
      /// </summary>
      Snapshot
   }

   public class SnapshotRunner : IBatchRunner<SnapshotRunOptions>
   {
      private readonly ICoreWorkspace _workspace;
      private readonly ISnapshotTask _snapshotTask;
      private readonly IWorkspacePersistor _workspacePersistor;
      private readonly IOSPSuiteLogger _logger;

      //For testing purposes only
      public Func<string, string, FileInfo[]> AllFilesFrom { get; set; }



      public SnapshotRunner(
         ICoreWorkspace workspace,
         ISnapshotTask snapshotTask,
         IWorkspacePersistor workspacePersistor,
         IOSPSuiteLogger logger)
      {
         _workspace = workspace;
         _snapshotTask = snapshotTask;
         _workspacePersistor = workspacePersistor;
         _logger = logger;
         AllFilesFrom = allFilesFrom;
      }  

      public async Task RunBatchAsync(SnapshotRunOptions runOptions)
      {
         _logger.AddInfo($"Starting snapshot run: {DateTime.Now.ToIsoFormat()}");

         var allFilesToExports = allFilesToExportFrom(runOptions).ToList();

         await Task.Run(() => startSnapshotRun(allFilesToExports, runOptions.ExportMode));

         _logger.AddInfo($"Snapshot run finished: {DateTime.Now.ToIsoFormat()}");
      }

      private Task startSnapshotRun(IReadOnlyList<FileMap> fileMaps, SnapshotExportMode exportMode)
      {
         if (exportMode == SnapshotExportMode.Snapshot)
            return startSnapshotRun(fileMaps, createSnapshotFromProjectFile);

         return startSnapshotRun(fileMaps, createProjectFromSnapshotFile);
      }

      private async Task startSnapshotRun(IReadOnlyList<FileMap> fileMaps, Func<FileMap, Task> exportFunc)
      {
         var begin = DateTime.UtcNow;
         foreach (var fileMap in fileMaps)
         {
            try
            {
               await exportFunc(fileMap);
            }
            catch (Exception e)
            {
               _logger.AddException(e);
            }
            finally
            {
               //Ensure that we reset the project to avoid any leaks
               _workspace.Project = null;
            }
         }
         var end = DateTime.UtcNow;
         var timeSpent = end - begin;

         _logger.AddInfo($"{fileMaps.Count} {"project".PluralizeIf(fileMaps)} loaded and exported in {timeSpent.ToDisplay()}");
      }

      private async Task createProjectFromSnapshotFile(FileMap file)
      {
         _logger.AddInfo($"Starting project export for '{file.SnapshotFile}'");
         var project = await _snapshotTask.LoadProjectFromSnapshotFileAsync(file.SnapshotFile );
         if (project == null)
            return;

         _logger.AddDebug($"Snapshot loaded successfully from '{file.SnapshotFile}'");
         _workspace.Project = project;
         _workspacePersistor.SaveSession(_workspace, file.ProjectFile);
         _logger.AddInfo($"Project saved to '{file.ProjectFile};");
      }

      private async Task createSnapshotFromProjectFile(FileMap file)
      {
         _logger.AddInfo($"Starting snapshot export for '{file.ProjectFile}'");

         _workspacePersistor.LoadSession(_workspace, file.ProjectFile);
         _logger.AddDebug($"Project loaded successfully from '{file.ProjectFile}'");

         await _snapshotTask.ExportModelToSnapshotAsync(_workspace.Project, file.SnapshotFile);
         _logger.AddInfo($"Snapshot saved to '{file.SnapshotFile}'");
      }

      private IEnumerable<FileMap> allFilesToExportFrom(SnapshotRunOptions runOptions)
      {
         if (runOptions.Folders.Any())
            return allFilesFromFolderList(runOptions);

         return allFilesFromInputFolder(runOptions);
      }

      private IEnumerable<FileMap> allFilesFromFolderList(SnapshotRunOptions runOptions)
      {
         var (inputFilter, outputExtension) = inputFileFilterAndOutputFileExtensionFrom(runOptions);
         var allFiles = new List<FileMap>();
         runOptions.Folders.Each(f => { allFiles.AddRange(allFilesFrom(f, f, inputFilter, outputExtension, runOptions.ExportMode)); });

         return allFiles;
      }

      private IEnumerable<FileMap> allFilesFromInputFolder(SnapshotRunOptions runOptions)
      {
         var (inputFilter, outputExtension) = inputFileFilterAndOutputFileExtensionFrom(runOptions);

         var inputFolder = runOptions.InputFolder;
         var outputFolder = runOptions.OutputFolder;
         return allFilesFrom(inputFolder, outputFolder, inputFilter, outputExtension, runOptions.ExportMode);
      }

      private IEnumerable<FileMap> allFilesFrom(string inputFolder, string outputFolder, string inputFilter, string outputExtension, SnapshotExportMode exportMode)
      {
         var allInputFiles = AllFilesFrom(inputFolder, inputFilter);
         if (allInputFiles.Length == 0)
         {
            _logger.AddDebug($"No file found in '{inputFolder}'");
            yield break;
         }

         DirectoryHelper.CreateDirectory(outputFolder);

         foreach (var inputFile in allInputFiles)
         {
            var inputFileFullPath = inputFile.FullName;
            var projectName = FileHelper.FileNameFromFileFullPath(inputFileFullPath);
            var outputFileFullPath = Path.Combine(outputFolder, $"{projectName}{outputExtension}");

            yield return fileMapFor(inputFileFullPath, outputFileFullPath, exportMode);
         }
      }

      private FileInfo[] allFilesFrom(string folder, string filter)
      {
         var directory = new DirectoryInfo(folder);
         if (!directory.Exists)
            throw new OSPSuiteException($"Folder '{folder}' does not exist!");

         return directory.GetFiles(filter);
      }

      private static (string inputFilter, string outputExtension) inputFileFilterAndOutputFileExtensionFrom(SnapshotRunOptions runOptions)
      {
         return runOptions.ExportMode == SnapshotExportMode.Project ? (Constants.Filter.JSON_FILTER, CoreConstants.Filter.PROJECT_EXTENSION) : (CoreConstants.Filter.PROJECT_FILTER, Constants.Filter.JSON_EXTENSION);
      }

      private FileMap fileMapFor(string inputFileFullPath, string outputFileFullPath, SnapshotExportMode runOptionsExportMode)
      {
         return runOptionsExportMode == SnapshotExportMode.Project ? new FileMap(outputFileFullPath, inputFileFullPath) : new FileMap(inputFileFullPath, outputFileFullPath);
      }

      private class FileMap
      {
         public string ProjectFile { get; }
         public string SnapshotFile { get; }

         public FileMap(string projectFile, string snapshotFile)
         {
            ProjectFile = projectFile;
            SnapshotFile = snapshotFile;
         }
      }
   }
}