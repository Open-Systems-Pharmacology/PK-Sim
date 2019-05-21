using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Qualification;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.CLI.Core.RunOptions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Services;
using static PKSim.Assets.PKSimConstants.Error;
using Project = PKSim.Core.Snapshots.Project;
using Simulation = PKSim.Core.Snapshots.Simulation;

namespace PKSim.CLI.Core.Services
{
   public class QualificationRunner : IBatchRunner<QualificationRunOptions>
   {
      private readonly ISnapshotTask _snapshotTask;
      private readonly IJsonSerializer _jsonSerializer;
      private readonly ICoreWorkspace _workspace;
      private readonly IWorkspacePersistor _workspacePersistor;
      private readonly ILogger _logger;
      private readonly IExportSimulationRunner _exportSimulationRunner;
      private readonly IDataRepositoryTask _dataRepositoryTask;
      private readonly IMarkdownReporterTask _markdownReporterTask;
      private readonly Cache<string, Project> _snapshotProjectCache = new Cache<string, Project>();

      public QualificationRunner(ISnapshotTask snapshotTask,
         IJsonSerializer jsonSerializer,
         ICoreWorkspace workspace,
         IWorkspacePersistor workspacePersistor,
         IExportSimulationRunner exportSimulationRunner,
         IDataRepositoryTask dataRepositoryTask,
         IMarkdownReporterTask markdownReporterTask,
         ILogger logger
      )
      {
         _snapshotTask = snapshotTask;
         _jsonSerializer = jsonSerializer;
         _workspace = workspace;
         _workspacePersistor = workspacePersistor;
         _logger = logger;
         _exportSimulationRunner = exportSimulationRunner;
         _dataRepositoryTask = dataRepositoryTask;
         _markdownReporterTask = markdownReporterTask;
      }

      public async Task RunBatchAsync(QualificationRunOptions runOptions)
      {
         _snapshotProjectCache.Clear();
         _logger.AddDebug("Starting qualification run");

         var config = await readConfigurationFrom(runOptions);
         if (config == null)
            throw new QualificationRunException(UnableToLoadQualificationConfigurationFromFile(runOptions.ConfigurationFile));

         var errorMessage = config.Validate().Message;
         if (!string.IsNullOrEmpty(errorMessage))
            throw new QualificationRunException(errorMessage);

         _logger.AddDebug($"Loading project from snapshot file '{config.SnapshotFile}'...");
         var snapshot = await snapshotProjectFromFile(config.SnapshotFile);

         //Ensures that the name of the snapshot is also the name of the project as defined in the configuration
         snapshot.Name = config.Project;
         _logger.AddDebug($"Project {snapshot.Name} loaded from snapshot file '{config.SnapshotFile}'.");

         await performBuildingBlockSwap(snapshot, config.BuildingBlocks);

         await performSimulationParameterSwap(snapshot, config.SimulationParameters);

         //Retrieve charts and validate inputs before exiting validation to ensure that we can throw error messages if an element is not available
         var plots = retrievePlotDefinitionsFrom(snapshot, config);
         validateInputs(snapshot, config);

         if (runOptions.Validate)
         {
            _logger.AddInfo("Qualification run configuration valid");
            return;
         }

         var begin = DateTime.UtcNow;
         var project = await _snapshotTask.LoadProjectFromSnapshot(snapshot);
         _workspace.Project = project;

         var projectOutputFolder = createProjectOutputFolder(config.OutputFolder, project.Name);

         _logger.AddDebug($"Exporting project {project.Name} to '{projectOutputFolder}'", project.Name);

         var exportRunOptions = new ExportRunOptions
         {
            OutputFolder = projectOutputFolder,
            ExportMode = SimulationExportMode.Xml | SimulationExportMode.Csv
         };

         var simulationExports = await _exportSimulationRunner.ExportSimulationsIn(project, exportRunOptions);
         var simulationMappings = simulationExports.Select(x => simulationMappingFrom(x, config)).ToArray();

         var observedDataMappings = await exportAllObservedData(project, config);

         var inputMappings = await exportInputs(project, config);

         var mapping = new QualificationMapping
         {
            SimulationMappings = simulationMappings,
            ObservedDataMappings = observedDataMappings,
            Plots = plots,
            Inputs = inputMappings
         };

         await _jsonSerializer.Serialize(mapping, config.MappingFile);
         _logger.AddDebug($"Project mapping for '{project.Name}' exported to '{config.MappingFile}'", project.Name);

         var projectFile = Path.Combine(config.TempFolder, $"{project.Name}{CoreConstants.Filter.PROJECT_EXTENSION}");
         _workspacePersistor.SaveSession(_workspace, projectFile);
         _logger.AddDebug($"Project saved to '{projectFile}'", project.Name);

         var snapshotFile = Path.Combine(config.TempFolder, $"{project.Name}{Constants.Filter.JSON_EXTENSION}");
         await _snapshotTask.ExportModelToSnapshot(project, snapshotFile);
         _logger.AddDebug($"Project snapshot saved to '{snapshotFile}'", project.Name);

         var end = DateTime.UtcNow;
         var timeSpent = end - begin;
         _logger.AddInfo($"Project '{project.Name}' exported for qualification in {timeSpent.ToDisplay()}", project.Name);
      }

      private PlotMapping[] retrievePlotDefinitionsFrom(Project snapshotProject, QualifcationConfiguration configuration)
      {
         var plotMappings = configuration.SimulationPlots?.SelectMany(x => retrievePlotDefinitionsForSimulation(x, snapshotProject));
         return plotMappings?.ToArray() ?? Array.Empty<PlotMapping>();
      }

      private void validateInputs(Project snapshotProject, QualifcationConfiguration configuration)
      {
         configuration.Inputs?.Each(x =>
         {
            var buildingBlock = snapshotProject.BuildingBlockByTypeAndName(x.Type, x.Name);
            if (buildingBlock == null)
               throw new QualificationRunException(CannotFindBuildingBlockInSnapshot(x.Type.ToString(), x.Name, snapshotProject.Name));
         });
      }

      private IEnumerable<PlotMapping> retrievePlotDefinitionsForSimulation(SimulationPlot simulationPlot, Project snapshotProject)
      {
         var simulationName = simulationPlot.Simulation;
         var simulation = simulationFrom(snapshotProject, simulationName);

         return simulation.Analyses.Select(plot => new PlotMapping
         {
            Plot = plot,
            SectionId = simulationPlot.SectionId,
            Simulation = simulationName,
            Project = snapshotProject.Name
         });
      }

      private SimulationMapping simulationMappingFrom(SimulationExport simulationExport, QualifcationConfiguration configuration) =>
         new SimulationMapping
         {
            Path = relativePath(simulationExport.SimulationFolder, configuration.OutputFolder),
            Project = simulationExport.Project,
            Simulation = simulationExport.Simulation
         };

      private Task<ObservedDataMapping[]> exportAllObservedData(PKSimProject project, QualifcationConfiguration configuration)
      {
         if (!project.AllObservedData.Any())
            return Task.FromResult(Array.Empty<ObservedDataMapping>());

         var observedDataOutputFolder = configuration.ObservedDataFolder;
         DirectoryHelper.CreateDirectory(observedDataOutputFolder);

         return Task.WhenAll(project.AllObservedData.Select(x => exportObservedData(x, configuration, project)));
      }

      private async Task<ObservedDataMapping> exportObservedData(DataRepository observedData, QualifcationConfiguration configuration, PKSimProject project)
      {
         var observedDataOutputFolder = configuration.ObservedDataFolder;
         var removeIllegalCharactersFrom = FileHelper.RemoveIllegalCharactersFrom(observedData.Name);
         var csvFullPath = Path.Combine(observedDataOutputFolder, $"{removeIllegalCharactersFrom}{Constants.Filter.CSV_EXTENSION}");
         var xlsFullPath = Path.Combine(observedDataOutputFolder, $"{removeIllegalCharactersFrom}{Constants.Filter.XLSX_EXTENSION}");
         _logger.AddDebug($"Observed data '{observedData.Name}' exported to '{csvFullPath}'", project.Name);
         await _dataRepositoryTask.ExportToCsvAsync(observedData, csvFullPath);

         _logger.AddDebug($"Observed data '{observedData.Name}' exported to '{xlsFullPath}'", project.Name);
         await _dataRepositoryTask.ExportToExcelAsync(observedData, xlsFullPath, launchExcel: false);

         return new ObservedDataMapping
         {
            Id = observedData.Name,
            Path = relativePath(csvFullPath, configuration.OutputFolder)
         };
      }

      private Task<InputMapping[]> exportInputs(PKSimProject project, QualifcationConfiguration configuration)
      {
         if (configuration.Inputs == null)
            return Task.FromResult(Array.Empty<InputMapping>());

         return Task.WhenAll(configuration.Inputs.Select(x => exportInput(project, configuration, x)));
      }

      private async Task<InputMapping> exportInput(PKSimProject project, QualifcationConfiguration configuration, Input input)
      {
         var buildingBlock = project.BuildingBlockByName(input.Name, input.Type);

         var inputsFolder = configuration.InputsFolder;
         var projectName = FileHelper.RemoveIllegalCharactersFrom(project.Name);
         var buildingBlockName = FileHelper.RemoveIllegalCharactersFrom(input.Name);
         var targetFolder = Path.Combine(inputsFolder, projectName, input.Type.ToString());
         DirectoryHelper.CreateDirectory(targetFolder);

         var fileFullPath = Path.Combine(targetFolder, $"{buildingBlockName}{CoreConstants.Filter.MARKDOWN_EXTENSION}");
         await _markdownReporterTask.ExportToMarkdown(buildingBlock, fileFullPath);
         _logger.AddDebug($"Input data for {input.Type} '{input.Name}' exported to '{fileFullPath}'", project.Name);

         return new InputMapping
         {
            SectionId = input.SectionId,
            Path = relativePath(fileFullPath, configuration.OutputFolder)
         };
      }

      private string createProjectOutputFolder(string outputPath, string projectName)
      {
         var projectOutputFolder = Path.Combine(outputPath, projectName);

         if (DirectoryHelper.DirectoryExists(projectOutputFolder))
            DirectoryHelper.DeleteDirectory(projectOutputFolder, true);

         DirectoryHelper.CreateDirectory(projectOutputFolder);

         return projectOutputFolder;
      }

      private Task performBuildingBlockSwap(Project projectSnapshot, BuildingBlockSwap[] buildingBlockSwaps)
      {
         if (buildingBlockSwaps == null)
            return Task.CompletedTask;

         return Task.WhenAll(buildingBlockSwaps.Select(x => swapBuildingBlockIn(projectSnapshot, x)));
      }

      private Task performSimulationParameterSwap(Project projectSnapshot, SimulationParameterSwap[] simulationParameters)
      {
         if (simulationParameters == null)
            return Task.CompletedTask;

         return Task.WhenAll(simulationParameters.Select(x => swapSimulationParametersIn(projectSnapshot, x)));
      }

      private async Task swapBuildingBlockIn(Project projectSnapshot, BuildingBlockSwap buildingBlockSwap)
      {
         var (buildingBlockType, name, snapshotPath) = buildingBlockSwap;
         var referenceSnapshot = await snapshotProjectFromFile(snapshotPath);
         var typeDisplay = buildingBlockType.ToString();

         var buildingBlockToUse = referenceSnapshot.BuildingBlockByTypeAndName(buildingBlockType, name);
         if (buildingBlockToUse == null)
            throw new QualificationRunException(CannotFindBuildingBlockInSnapshot(typeDisplay, name, referenceSnapshot.Name));

         var buildingBlock = projectSnapshot.BuildingBlockByTypeAndName(buildingBlockType, name);
         if (buildingBlock == null)
            throw new QualificationRunException(CannotFindBuildingBlockInSnapshot(typeDisplay, name, projectSnapshot.Name));

         projectSnapshot.Swap(buildingBlockToUse);
      }

      private async Task swapSimulationParametersIn(Project projectSnapshot, SimulationParameterSwap simulationParameter)
      {
         var (parameterPath, simulationName, snapshotPath) = simulationParameter;
         var referenceSnapshot = await snapshotProjectFromFile(snapshotPath);

         var referenceSimulation = simulationFrom(referenceSnapshot, simulationName);

         var referenceParameter = referenceSimulation.ParameterByPath(parameterPath);
         if (referenceParameter == null)
            throw new QualificationRunException(CannotFindSimulationParameterInSnapshot(parameterPath, simulationName, referenceSnapshot.Name));

         simulationParameter.TargetSimulations?.Each(targetSimulationName =>
         {
            var targetSimulation = simulationFrom(projectSnapshot, targetSimulationName);
            targetSimulation.AddOrUpdate(referenceParameter);
         });
      }

      private async Task<Project> snapshotProjectFromFile(string snapshotPath)
      {
         if (!_snapshotProjectCache.Contains(snapshotPath))
         {
            var snapshot = await _snapshotTask.LoadSnapshotFromFile<Project>(snapshotPath);
            _snapshotProjectCache[snapshotPath] = snapshot ?? throw new QualificationRunException(CannotLoadSnapshotFromFile(snapshotPath));
         }

         return _snapshotProjectCache[snapshotPath];
      }

      private Simulation simulationFrom(Project snapshotProject, string simulationName)
      {
         var referenceSimulation = snapshotProject.Simulations?.FindByName(simulationName);
         return referenceSimulation ?? throw new QualificationRunException(CannotFindSimulationInSnapshot(simulationName, snapshotProject.Name));
      }

      private Task<QualifcationConfiguration> readConfigurationFrom(QualificationRunOptions runOptions)
      {
         if (!FileHelper.FileExists(runOptions.ConfigurationFile))
            throw new QualificationRunException(FileDoesNotExist(runOptions.ConfigurationFile));

         _logger.AddDebug($"Reading configuration from file '{runOptions.ConfigurationFile}'");
         return _jsonSerializer.Deserialize<QualifcationConfiguration>(runOptions.ConfigurationFile);
      }

      private string relativePath(string path, string relativeTo) =>
         FileHelper.CreateRelativePath(path, relativeTo, useUnixPathSeparator: true);
   }
}