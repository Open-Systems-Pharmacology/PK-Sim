using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using OSPSuite.Assets.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
using PKSim.CLI.Core.RunOptions;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Services;
using SimulationRunOptions = PKSim.Core.Services.SimulationRunOptions;

namespace PKSim.CLI.Core.Services
{
   public class JsonSimulationRunner : IBatchRunner<JsonRunOptions>
   {
      private readonly ISimulationExporter _simulationExporter;
      private readonly IOSPSuiteLogger _logger;
      private readonly ISnapshotTask _snapshotTask;
      private readonly IList<string> _allSimulationNames = new List<string>();
      private readonly SimulationRunOptions _simulationRunOptions;

      public JsonSimulationRunner(
         ISimulationExporter simulationExporter,
         IOSPSuiteLogger logger,
         ISnapshotTask snapshotTask
      )
      {
         _simulationExporter = simulationExporter;
         _logger = logger;
         _snapshotTask = snapshotTask;
         _simulationRunOptions = new SimulationRunOptions
         {
            Validate = false,
            CheckForNegativeValues = false,
         };
      }

      public async Task RunBatchAsync(JsonRunOptions runOptions)
      {
         _logger.AddInfo($"Starting batch run: {DateTime.Now.ToIsoFormat()}");
         
         _simulationRunOptions.RunForAllOutputs = runOptions.RunForAllOutputs;
         await Task.Run(() => startJsonSimulationRun(runOptions));

         _logger.AddInfo($"Batch run finished: {DateTime.Now.ToIsoFormat()}");
      }

      private async Task startJsonSimulationRun(JsonRunOptions runOptions)
      {
         var inputFolder = runOptions.InputFolder;
         var outputFolder = runOptions.OutputFolder;
         var exportMode = runOptions.ExportMode;

         clear();
         
         var inputDirectory = new DirectoryInfo(inputFolder);
         if (!inputDirectory.Exists)
            throw new OSPSuiteException($"Input folder '{inputFolder}' does not exist");

         var allSnapshotFiles = inputDirectory.GetFiles(Constants.Filter.JSON_FILTER);
         if (allSnapshotFiles.Length == 0)
            throw new OSPSuiteException($"No snapshot file found in '{inputFolder}'");

         var outputDirectory = new DirectoryInfo(outputFolder);
         if (!outputDirectory.Exists)
         {
            _logger.AddDebug($"Creating folder '{outputFolder}'");
            outputDirectory.Create();
         }

         var allExistingFiles = outputDirectory.GetFiles("*.csv");
         allExistingFiles.Each(outputFile =>
         {
            FileHelper.DeleteFile(outputFile.FullName);
            _logger.AddDebug($"Deleting file '{outputFile.FullName}'");
         });

         var begin = DateTime.UtcNow;
         _logger.AddInfo($"Found {allSnapshotFiles.Length} files in '{inputFolder}'");

         foreach (var snapshotFile in allSnapshotFiles)
         {
            await runAndExportSimulationsInSnapshotFile(snapshotFile, outputFolder, exportMode)
               .ConfigureAwait(false);
         }

         var end = DateTime.UtcNow;
         var timeSpent = end - begin;
         _logger.AddInfo($"{_allSimulationNames.Count} simulations calculated and exported in {timeSpent.ToDisplay()}");

         createSummaryFilesIn(outputDirectory, exportMode);
      }

      private void clear()
      {
         _allSimulationNames.Clear();
      }

      private void createSummaryFilesIn(DirectoryInfo outputDirectory, SimulationExportMode exportMode)
      {
         if (!exportMode.HasFlag(SimulationExportMode.Csv))
            return;

         var dataTable = new DataTable(outputDirectory.Name);
         dataTable.Columns.Add("Sim_id", typeof(string));
         foreach (var simulationName in _allSimulationNames)
         {
            var row = dataTable.NewRow();
            row[0] = simulationName;
            dataTable.Rows.Add(row);
         }
         var fileName = Path.Combine(outputDirectory.FullName, $"{outputDirectory.Name}.csv");
         if (FileHelper.FileExists(fileName))
         {
            var tmp = FileHelper.FileNameFromFileFullPath(FileHelper.GenerateTemporaryFileName());
            tmp = Path.Combine(outputDirectory.FullName, $"{tmp}.csv");
            _logger.AddWarning($"File '{fileName}' already exists and will be saved under '{tmp}'");
            fileName = tmp;
         }

         _logger.AddDebug($"Exporting simulation file names to '{fileName}'");
         dataTable.ExportToCSV(fileName);
      }

      private async Task runAndExportSimulationsInSnapshotFile(FileInfo projectFile, string outputFolder, SimulationExportMode simulationExportMode)
      {
         _logger.AddInfo($"Starting batch simulation export for file '{projectFile}'");
         try
         {
            var project = await _snapshotTask.LoadProjectFromSnapshotFile(projectFile.FullName);
            var simulations = project.All<Simulation>();
            var numberOfSimulations = simulations.Count;
            _logger.AddInfo($"{numberOfSimulations} {"simulation".PluralizeIf(numberOfSimulations)} found in file '{projectFile}'", project.Name);

            var simulationExportOptions = new SimulationExportOptions
            {
               OutputFolder = outputFolder,
               ExportMode = simulationExportMode,
               ProjectName = project.Name,
               PrependProjectName = true
            };

            foreach (var simulation in simulations)
            {
               _logger.AddDebug($"Starting batch simulation export for simulation '{simulation.Name}'", project.Name);

               await _simulationExporter.RunAndExport(simulation, _simulationRunOptions, simulationExportOptions);
               _allSimulationNames.Add(simulation.Name);
            }
         }
         catch (Exception e)
         {
            _logger.AddException(e);
         }
      }
   }
}