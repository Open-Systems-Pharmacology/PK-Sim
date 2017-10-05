using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Batch;
using PKSim.Presentation.Core;
using PKSim.Presentation.Services;

namespace PKSim.BatchTool.Services
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

   public class SnapshotsRunner : IBatchRunner<SnapshotRunOptions>
   {
      private readonly IWorkspace _workspace;
      private readonly IProjectTask _projectTask;
      private readonly IWorkspacePersistor _workspacePersistor;
      private readonly IBatchLogger _logger;

      public SnapshotsRunner(
         IWorkspace workspace,
         IProjectTask projectTask,
         IWorkspacePersistor workspacePersistor,
         IBatchLogger logger)
      {
         _workspace = workspace;
         _projectTask = projectTask;
         _workspacePersistor = workspacePersistor;
         _logger = logger;
      }

      public async Task RunBatchAsync(SnapshotRunOptions runOptions)
      {
         var allFilesToExports = allFilesToExportFrom(runOptions).ToList();
         var notificationType = NotificationType.Info | NotificationType.Error | NotificationType.Warning;

         using (new BatchLoggerDisposer(_logger, runOptions.LogFileFullPath, notificationType))
         {
            _logger.AddInSeparator($"Starting snapshot run: {DateTime.Now.ToIsoFormat()}", NotificationType.Info);

            await startSnapshotRun(allFilesToExports, runOptions.ExportMode);

            _logger.AddInSeparator($"Snapshot run finished: {DateTime.Now.ToIsoFormat()}", NotificationType.Info);
         }
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
               _logger.AddError(e.ExceptionMessageWithStackTrace());
            }
            finally
            {
               //Ensure that we reset the project to avoid any leaks
               _workspace.Project = null;
            }
         }
         var end = DateTime.UtcNow;
         var timeSpent = end - begin;

         _logger.AddInSeparator($"{fileMaps.Count} projects loaded and exported in '{timeSpent.ToDisplay()}'", NotificationType.Info);
      }

      private async Task createProjectFromSnapshotFile(FileMap file)
      {
         _logger.AddInSeparator($"Starting project export for '{file.SnapshotFile}'", NotificationType.Info);
         var project = await _projectTask.LoadProjectFromSnapshotFile(file.SnapshotFile);
         if (project == null)
            return;

         _logger.AddDebug($"Snapshot loaded successfuly from '{file.SnapshotFile}'");
         _workspace.Project = project;
         _workspacePersistor.SaveSession(_workspace, file.ProjectFile);
         _logger.AddInfo($"Project saved to '{file.ProjectFile};");
      }

      private async Task createSnapshotFromProjectFile(FileMap file)
      {
         _logger.AddInSeparator($"Starting snapshot export for '{file.ProjectFile}'", NotificationType.Info);

         //need to suply registration task to workspace so that project will be registered when deserialized
         _workspacePersistor.LoadSession(_workspace, file.ProjectFile);
         _logger.AddDebug($"Project loaded successfuly from '{file.ProjectFile}'");

         await _projectTask.ExportProjectToSnapshot(_workspace.Project, file.SnapshotFile);
         _logger.AddInfo($"Snapshot saved to '{file.SnapshotFile}'");
      }

      private IEnumerable<FileMap> allFilesToExportFrom(SnapshotRunOptions runOptions)
      {
         //TODO define logic here
         var inputFolder = runOptions.InputFolder;
         var outputFolder = runOptions.OutputFolder;
         var inputDirectory = new DirectoryInfo(inputFolder);

         var ( inputFilter, outputExtension ) = inputFileFilterAndOutputFileExtensionFrom(runOptions);

         var allInputFiles = inputDirectory.GetFiles(inputFilter);
         if (allInputFiles.Length == 0)
            throw new ArgumentException($"No file found in '{inputFolder}'");

         DirectoryHelper.CreateDirectory(outputFolder);

         foreach (var inputFile in allInputFiles)
         {
            var inputFileFullPath = inputFile.FullName;
            var projectName = FileHelper.FileNameFromFileFullPath(inputFileFullPath);
            var outputFileFullPath = Path.Combine(outputFolder, $"{projectName}{outputExtension}");

            yield return fileMapFor(inputFileFullPath, outputFileFullPath, runOptions.ExportMode);
         }
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