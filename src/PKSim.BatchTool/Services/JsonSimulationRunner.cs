using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Batch;
using PKSim.Core.Commands;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;

namespace PKSim.BatchTool.Services
{
   public class JsonSimulationRunner : IBatchRunner
   {
      private readonly ISimulationLoader _simulationLoader;
      private readonly ISimulationExporter _simulationExporter;
      private readonly IBatchLogger _logger;
      private readonly IEntitiesInContainerRetriever _entitiesInContainerRetriever;
      private readonly IExecutionContext _executionContext;
      private readonly ICommandTask _commandTask;
      private readonly IList<string> _allSimulationNames = new List<string>();

      public JsonSimulationRunner(ISimulationLoader simulationLoader, ISimulationExporter simulationExporter, IBatchLogger logger,
         IEntitiesInContainerRetriever entitiesInContainerRetriever, IExecutionContext executionContext, ICommandTask commandTask)
      {
         _simulationLoader = simulationLoader;
         _simulationExporter = simulationExporter;
         _logger = logger;
         _entitiesInContainerRetriever = entitiesInContainerRetriever;
         _executionContext = executionContext;
         _commandTask = commandTask;
      }

      public Task RunBatch(dynamic parameters)
      {
         string inputFolder = parameters.inputFolder;
         string outputFolder = parameters.outputFolder;
         BatchExportMode exportMode = parameters.exportMode;

         clear();

         return Task.Run(() =>
         {

            _logger.AddInSeparator($"Starting batch run: {DateTime.Now.ToIsoFormat()}");

            var inputDirectory = new DirectoryInfo(inputFolder);
            if (!inputDirectory.Exists)
               throw new ArgumentException($"Input folder '{inputFolder}' does not exist");

            var allSimulationFiles = inputDirectory.GetFiles(CoreConstants.Filter.JSON_FILTER);
            if (allSimulationFiles.Length == 0)
               throw new ArgumentException($"No simulation json file found in '{inputFolder}'");

            var outputDirectory = new DirectoryInfo(outputFolder);
            if (!outputDirectory.Exists)
            {
               _logger.AddInfo($"Creating folder '{outputFolder}'");
               outputDirectory.Create();
            }

            var allExistingFiles = outputDirectory.GetFiles("*.csv");
            allExistingFiles.Each(outputFile =>
            {
               FileHelper.DeleteFile(outputFile.FullName);
               _logger.AddDebug($"Deleting file '{outputFile.FullName}'");
            });

            var begin = DateTime.UtcNow;
            allSimulationFiles.Each(s => exportSimulationTo(s, outputFolder, exportMode));
            var end = DateTime.UtcNow;
            var timeSpent = end - begin;
            _logger.AddInSeparator($"{_allSimulationNames.Count} simulations calculated and exported in '{timeSpent.ToDisplay()}'");

            createSummaryFilesIn(outputDirectory);

            _logger.AddInSeparator($"Batch run finished: {DateTime.Now.ToIsoFormat()}");

            exportLogFile(outputDirectory);
         });
      }

      private void clear()
      {
         _logger.Clear();
         _allSimulationNames.Clear();
      }

      private void exportLogFile(DirectoryInfo outputDirectory)
      {
         using (var file = new StreamWriter(Path.Combine(outputDirectory.FullName, "log.txt")))
         {
            _logger.Entries.Each(file.WriteLine);
         }
      }

      private void createSummaryFilesIn(DirectoryInfo outputDirectory)
      {
         var dataTable = new DataTable(outputDirectory.Name);
         dataTable.Columns.Add("Sim_id", typeof (string));
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

      private void exportSimulationTo(FileInfo simulationFile, string outputFolder, BatchExportMode batchExportMode)
      {
         _logger.AddInSeparator($"Starting batch simulation for file '{simulationFile}'");
         try
         {
            var simForBatch = _simulationLoader.LoadSimulationFrom(simulationFile.FullName);
            string simText = simForBatch.NumberOfSimulations > 1 ? "simulations" : "simulation";
            _logger.AddInfo($"{simForBatch.NumberOfSimulations} {simText} found in file '{simulationFile}'");

            var simulation = simForBatch.Simulation;

            var defaultSimulationName = FileHelper.FileNameFromFileFullPath(simulationFile.FullName);
            var allParameters = _entitiesInContainerRetriever.ParametersFrom(simulation);
            _simulationExporter.RunAndExport(simulation, outputFolder, defaultSimulationName, simForBatch.Configuration, batchExportMode);
            _allSimulationNames.Add(defaultSimulationName);
            foreach (var parameterValueSet in simForBatch.ParameterVariationSets)
            {
               string currentName = $"{defaultSimulationName}_{parameterValueSet.Name}";
               var command = updateParameters(allParameters, parameterValueSet);
               _simulationExporter.RunAndExport(simulation, outputFolder, currentName, simForBatch.Configuration, batchExportMode);
               _allSimulationNames.Add(currentName);
               _commandTask.ResetChanges(command, _executionContext);
            }

            //when simulation is not used anymore, unregister simulation
            _executionContext.Unregister(simulation);
         }
         catch (Exception e)
         {
            _logger.AddError($"{e.FullMessage()}\n{e.FullStackTrace()}");
         }
      }

      private IPKSimMacroCommand updateParameters(PathCache<IParameter> allParameters, ParameterVariationSet parameterVariationSet)
      {
         var macroCommand = new PKSimMacroCommand();

         foreach (var parameterValue in parameterVariationSet.ParameterValues)
         {
            var parameterPath = parameterValue.ParameterPath;

            var parameter = allParameters[parameterPath];
            if (parameter == null)
            {
               //try with adding the name of the simulation at first
               parameterPath = $"{Simulation.Name}{ObjectPath.PATH_DELIMITER}{parameterPath}";
               parameter = allParameters[parameterPath];
               if (parameter == null)
               {
                  _logger.AddWarning($"Parameter with path '{parameterValue.ParameterPath}' not found!");
                  continue;
               }
            }

            _logger.AddDebug($"Parameter '{parameterValue.ParameterPath}' value set from '{parameter.Value} to '{parameterValue.Value}'.");
            macroCommand.Add(new SetParameterValueCommand(parameter, parameterValue.Value));
         }

         return macroCommand.Run(_executionContext);
      }
   }
}