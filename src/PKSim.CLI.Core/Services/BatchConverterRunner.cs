using System;
using System.IO;
using System.Threading.Tasks;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.CLI.Core.RunOptions;
using PKSim.Core;
using PKSim.Core.Batch;
using PKSim.Core.Commands;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Services;
using Simulation = PKSim.Core.Batch.Simulation;

namespace PKSim.CLI.Core.Services
{
   internal class BatchConverterRunner : IBatchRunner<BatchConverterRunOptions>
   {
      private readonly ILogger _logger;
      private readonly ISimulationLoader _simulationLoader;
      private readonly IEntitiesInContainerRetriever _entitiesInContainerRetriever;
      private readonly IExecutionContext _executionContext;
      private readonly ISnapshotTask _snapshotTask;

      public BatchConverterRunner(ILogger logger, ISimulationLoader simulationLoader, IEntitiesInContainerRetriever entitiesInContainerRetriever, 
         IExecutionContext executionContext, ISnapshotTask snapshotTask)
      {
         _logger = logger;
         _simulationLoader = simulationLoader;
         _entitiesInContainerRetriever = entitiesInContainerRetriever;
         _executionContext = executionContext;
         _snapshotTask = snapshotTask;
      }

      public async Task RunBatchAsync(BatchConverterRunOptions runOptions)
      {
         var inputFolder = runOptions.InputFolder;
         var outputFolder = runOptions.OutputFolder;
         var exportMode = runOptions.ExportMode;

         _logger.AddInSeparator($"Starting batch converter run: {DateTime.Now.ToIsoFormat()}", NotificationType.Info);

         var inputDirectory = new DirectoryInfo(inputFolder);
         if (!inputDirectory.Exists)
            throw new ArgumentException($"Input folder '{inputFolder}' does not exist");

         var allSimulationFiles = inputDirectory.GetFiles(Constants.Filter.JSON_FILTER);
         if (allSimulationFiles.Length == 0)
            throw new ArgumentException($"No simulation json file found in '{inputFolder}'");

         var outputDirectory = new DirectoryInfo(outputFolder);
         if (!outputDirectory.Exists)
         {
            _logger.AddDebug($"Creating folder '{outputFolder}'");
            outputDirectory.Create();
         }


         var begin = DateTime.UtcNow;
         _logger.AddInfo($"Found {allSimulationFiles.Length} files in '{inputFolder}'");

         foreach (var simulationFile in allSimulationFiles)
         {
            await exportSimulationTo(simulationFile, outputFolder);
         }

         var end = DateTime.UtcNow;
         var timeSpent = end - begin;

         _logger.AddInSeparator($"Batch run finished: {DateTime.Now.ToIsoFormat()}", NotificationType.Info);
      }

      private async Task exportSimulationTo(FileInfo simulationFile, string outputFolder)
      {
         _logger.AddInSeparator($"Starting batch simulation for file '{simulationFile}'");
         var outputFile = Path.Combine(outputFolder, simulationFile.Name);
         var project = new PKSimProject();
         try
         {
            var simForBatch = _simulationLoader.LoadSimulationFrom(simulationFile.FullName);
            var simText = simForBatch.NumberOfSimulations > 1 ? "simulations" : "simulation";
            _logger.AddInfo($"{simForBatch.NumberOfSimulations} {simText} found in file '{simulationFile}'");

            var simulation = simForBatch.Simulation;
            simulation.AllBuildingBlocks<IPKSimBuildingBlock>().Each(project.AddBuildingBlock);

            var defaultSimulationName = FileHelper.FileNameFromFileFullPath(simulationFile.FullName);
            simulation.Name = defaultSimulationName;
            project.AddBuildingBlock(simulation);

            foreach (var parameterValueSet in simForBatch.ParameterVariationSets)
            {
               var newSimulationBatch = _simulationLoader.LoadSimulationFrom(simulationFile.FullName);
               var allParameters = _entitiesInContainerRetriever.ParametersFrom(simulation);
               string currentName = $"{defaultSimulationName}_{parameterValueSet.Name}";
               var newSimulation = newSimulationBatch.Simulation;
               newSimulation.Name = currentName;
               updateParameters(allParameters, parameterValueSet);
               project.AddBuildingBlock(newSimulation);
            }

            await _snapshotTask.ExportModelToSnapshot(project, outputFile);
         }
         catch (Exception e)
         {
            _logger.AddException(e);
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