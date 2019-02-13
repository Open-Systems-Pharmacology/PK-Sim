using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Qualification;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.CLI.Core.RunOptions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Services;
using PKSim.Presentation.Core;
using static PKSim.Assets.PKSimConstants.Error;
using Project = PKSim.Core.Snapshots.Project;

namespace PKSim.CLI.Core.Services
{
   public class QualificationRunner : IBatchRunner<QualificationRunOptions>
   {
      private readonly ISnapshotTask _snapshotTask;
      private readonly IJsonSerializer _jsonSerializer;
      private readonly IWorkspace _workspace;
      private readonly IWorkspacePersistor _workspacePersistor;
      private readonly ILogger _logger;
      private readonly IExportSimulationRunner _exportSimulationRunner;
      private readonly IDataRepositoryTask _dataRepositoryTask;
      private readonly IMarkdownReporterTask _markdownReporterTask;

      public QualificationRunner(ISnapshotTask snapshotTask,
         IJsonSerializer jsonSerializer,
         IWorkspace workspace,
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
         _logger.AddDebug("Starting qualification run");

         var config = await readConfigurationFrom(runOptions);
         if (config == null)
            throw new QualificationRunException(UnableToLoadQualificationConfigurationFromFile(runOptions.ConfigurationFile));

         var errorMessage = config.Validate().Message;
         if (!string.IsNullOrEmpty(errorMessage))
            throw new QualificationRunException(errorMessage);

         var snapshot = await _snapshotTask.LoadSnapshotFromFile<Project>(config.SnapshotFile);
         await performBuildingBlockSwap(snapshot, config.BuildingBlocks);

         //Retrieve charts and validate inputs before exiting validation to ensure that we can throw error messages if an element is not available
         var charts = retrieveChartDefinitionsFrom(snapshot, config);
         validateInputs(snapshot, config);

         if (runOptions.Validate)
         {
            _logger.AddInfo("Qualification run configuration valid");
            return;
         }

         var begin = DateTime.UtcNow;
         var project = await _snapshotTask.LoadProjectFromSnapshot(snapshot);
         var projectOutputFolder = createProjectOutputFolder(config.OutputFolder, project.Name);

         _logger.AddDebug($"Exporting project {project.Name} to '{projectOutputFolder}'", project.Name);

         var exportRunOtions = new ExportRunOptions
         {
            OutputFolder = projectOutputFolder,
            ExportMode = SimulationExportMode.All
         };


         var simulationExports = await _exportSimulationRunner.ExportSimulationsIn(project, exportRunOtions);
         var simulationMappings = simulationExports.Select(x => simulationMappingFrom(x, config)).ToArray();

         var observedDataMappings = exportObservedData(project, config);

         var inputMappings = await exportInputs(project, config);

         var mapping = new QualificationMapping
         {
            SimulationMappings = simulationMappings,
            ObservedDataMappings = observedDataMappings,
            Plots = charts,
            Inputs = inputMappings
         };

         await _jsonSerializer.Serialize(mapping, config.MappingFile);
         _logger.AddDebug($"Project mapping for '{project.Name}' exported to '{config.MappingFile}'", project.Name);

         var projectFile = Path.Combine(projectOutputFolder, $"{project.Name}{CoreConstants.Filter.PROJECT_EXTENSION}");
         _workspace.Project = project;
         _workspacePersistor.SaveSession(_workspace, projectFile);
         _logger.AddDebug($"Project saved to '{projectFile}'", project.Name);

         var snapshotFile = Path.Combine(projectOutputFolder, $"{project.Name}{Constants.Filter.JSON_EXTENSION}");
         await _snapshotTask.ExportModelToSnapshot(project, snapshotFile);
         _logger.AddDebug($"Project snapshot saved to '{snapshotFile}'", project.Name);

         var end = DateTime.UtcNow;
         var timeSpent = end - begin;
         _logger.AddInfo($"Project '{project.Name}' exported for qualification in {timeSpent.ToDisplay()}", project.Name);
      }

      private PlotMapping[] retrieveChartDefinitionsFrom(Project snapshotProject, QualifcationConfiguration configuration) =>
         configuration.SimulationPlots?.SelectMany(x => retrieveChartDefinitionsForSimulation(x, snapshotProject)).ToArray();

      private void validateInputs(Project snapshotProject, QualifcationConfiguration configuration)
      {
         configuration.Inputs?.Each(x =>
         {
            var buildingBlock = snapshotProject.BuildingBlockByTypeAndName(x.Type, x.Name);
            if (buildingBlock == null)
               throw new QualificationRunException(CannotFindBuildingBlockInSpahsot(x.Type.ToString(), x.Name, snapshotProject.Name));
         });
      }

      private IEnumerable<PlotMapping> retrieveChartDefinitionsForSimulation(SimulationPlot simulationPlot, Project snapshotProject)
      {
         var simuationName = simulationPlot.RefSimulation;
         var simulation = snapshotProject.Simulations?.FindByName(simuationName);
         if (simulation == null)
            throw new QualificationRunException($"Cannot export charts as simulation '{simuationName}' in not defined in project '{snapshotProject.Name}'.");

         return simulation.Analyses.Select(chart => new PlotMapping
         {
            Plot = chart,
            SectionId = simulationPlot.SectionId,
            RefSimulation = simuationName,
            RefProject = snapshotProject.Name
         });
      }

      private SimulationMapping simulationMappingFrom(SimulationExport simulationExport, QualifcationConfiguration configuration) =>
         new SimulationMapping
         {
            Path = relativePath(simulationExport.SimulationFolder, configuration.ReportConfigurationFile),
            RefProject = simulationExport.ProjectName,
            RefSimulation = simulationExport.SimulationName
         };

      private ObservedDataMapping[] exportObservedData(PKSimProject project, QualifcationConfiguration configuration)
      {
         if (!project.AllObservedData.Any())
            return null;

         var observedDataOutputFolder = configuration.ObservedDataFolder;
         DirectoryHelper.CreateDirectory(observedDataOutputFolder);

         return project.AllObservedData.Select(obs =>
         {
            var fileFullPath = Path.Combine(observedDataOutputFolder, $"{FileHelper.RemoveIllegalCharactersFrom(obs.Name)}{Constants.Filter.XLSX_EXTENSION}");
            _logger.AddDebug($"Observed data '{obs.Name}' exported to '{fileFullPath}'", project.Name);
            _dataRepositoryTask.ExportToExcel(obs, fileFullPath, launchExcel: false);
            return new ObservedDataMapping
            {
               Id = obs.Name,
               Path = relativePath(fileFullPath, configuration.ReportConfigurationFile)
            };
         }).ToArray();
      }

      private Task<InputMapping[]> exportInputs(PKSimProject project, QualifcationConfiguration configuration)
      {
         if (configuration.Inputs == null)
            return Task.FromResult<InputMapping[]>(null);

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

         var fileFullPath = Path.Combine(targetFolder, $"{buildingBlockName}{CoreConstants.Filter.MARDOWN_EXTENSION}");
         await _markdownReporterTask.ExportToMarkdown(buildingBlock, fileFullPath);
         _logger.AddDebug($"Input data for {input.Type} '{input.Name}' exported to '{fileFullPath}'", project.Name);

         return new InputMapping
         {
            SectionId = input.SectionId,
            Path = relativePath(fileFullPath, configuration.ReportConfigurationFile)
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

         return Task.WhenAll(buildingBlockSwaps.Select(swap => swapBuildingBlockIn(projectSnapshot, swap)));
      }

      private async Task swapBuildingBlockIn(Project projectSnapshot, BuildingBlockSwap buildingBlockSwap)
      {
         var (type, name, snapshotPath) = buildingBlockSwap;
         var referenceSnasphot = await _snapshotTask.LoadSnapshotFromFile<Project>(snapshotPath);
         if (referenceSnasphot == null)
            throw new QualificationRunException(CannotLoadSnapshotFromFile(snapshotPath));

         var buildiingBlockToUse = referenceSnasphot.BuildingBlockByTypeAndName(type, name);
         if (buildiingBlockToUse == null)
            throw new QualificationRunException(CannotFindBuildingBlockInSnapshot(type.ToString(), name, snapshotPath));

         var buildingBlock = projectSnapshot.BuildingBlockByTypeAndName(type, name);
         if (buildingBlock == null)
            throw new QualificationRunException(CannotFindBuildingBlockInSnapshot(type.ToString(), name, projectSnapshot.Name));

         projectSnapshot.Swap(buildiingBlockToUse);
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