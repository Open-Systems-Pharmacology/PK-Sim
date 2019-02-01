using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
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
   public class BuildingBlockSwap : IWithName
   {
      public PKSimBuildingBlockType Type { get; set; }
      public string Name { get; set; }
      public string SnapshotPath { get; set; }

      public void Deconstruct(out PKSimBuildingBlockType type, out string name, out string snapshotPath)
      {
         type = Type;
         name = Name;
         snapshotPath = SnapshotPath;
      }
   }

   public class QualifcationConfiguration
   {
      /// <summary>
      /// Path of project snapshot file used for this qualificaiton run
      /// </summary>
      public string SnapshotPath { get; set; }

      /// <summary>
      /// Output folder where project artefacts will be exported. It will be created if it does not exist
      /// </summary>
      public string OutputFolder { get; set; }

      /// <summary>
      /// Folder were observed data will be exported
      /// </summary>
      public string ObservedDataFolder { get; set; } 

      /// <summary>
      /// Path of mapping file that will be created for the project.
      /// </summary>
      public string MappingPath { get; set; } 

      public BuildingBlockSwap[] BuildingBlocks { get; set; }
   }

   public class QualificationRunner : IBatchRunner<QualificationRunOptions>
   {
      private readonly ISnapshotTask _snapshotTask;
      private readonly IJsonSerializer _jsonSerializer;
      private readonly IWorkspace _workspace;
      private readonly IWorkspacePersistor _workspacePersistor;
      private readonly ILogger _logger;
      private readonly IExportSimulationRunner _exportSimulationRunner;
      private readonly IDataRepositoryTask _dataRepositoryTask;

      public QualificationRunner(ISnapshotTask snapshotTask,
         IJsonSerializer jsonSerializer,
         IWorkspace workspace,
         IWorkspacePersistor workspacePersistor,
         IExportSimulationRunner exportSimulationRunner,
         IDataRepositoryTask dataRepositoryTask,
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
      }

      public async Task RunBatchAsync(QualificationRunOptions runOptions)
      {
         _logger.AddDebug("Starting qualification run");

         var config = await readConfigurationFrom(runOptions);
         if (config == null)
            throw new QualificationRunException(UnableToLoadQualificationConfigurationFromOptions);

         if (string.IsNullOrEmpty(config.OutputFolder))
            throw new QualificationRunException(QualificationOutputFolderNotDefined);

         if (!FileHelper.FileExists(config.SnapshotPath))
            throw new QualificationRunException(CannotLoadSnapshotFromFile(config.SnapshotPath));

         var snapshot = await _snapshotTask.LoadSnapshotFromFile<Project>(config.SnapshotPath);
         await performBuildingBlockSwap(snapshot, config.BuildingBlocks);

         if (runOptions.Validate)
         {
            _logger.AddInfo("Qualification run configuration valid");
            return;
         }

         var begin = DateTime.UtcNow;
         var project = await _snapshotTask.LoadProjectFromSnapshot(snapshot);
         var projectOutputFolder = createProjectOutputFolder(config.OutputFolder, project.Name);

         _logger.AddDebug($"Exporting project {project.Name} to '{projectOutputFolder}'");

         var exportRunOtions = new ExportRunOptions
         {
            OutputFolder = projectOutputFolder,
            ExportMode = SimulationExportMode.All
         };

         await _exportSimulationRunner.ExportSimulationsIn(project, exportRunOtions);

         exportObservedData(project, config);

         var projectFile = Path.Combine(projectOutputFolder, $"{project.Name}{CoreConstants.Filter.PROJECT_EXTENSION}");
         _workspace.Project = project;
         _workspacePersistor.SaveSession(_workspace, projectFile);

         var end = DateTime.UtcNow;
         var timeSpent = end - begin;
         _logger.AddInfo($"Project '{project.Name}' exported for qualification in {timeSpent.ToDisplay()}");
      }

      private void exportObservedData(PKSimProject project, QualifcationConfiguration qualifcationConfiguration)
      {
         if (!project.AllObservedData.Any())
            return;

         var observedDataOutputFolder = qualifcationConfiguration.ObservedDataFolder;
         DirectoryHelper.CreateDirectory(observedDataOutputFolder);

         project.AllObservedData.Each(obs =>
         {
            var fileFullPath = Path.Combine(observedDataOutputFolder, $"{obs.Name}{Constants.Filter.XLSX_EXTENSION}");
            _logger.AddDebug($"Observed data '{obs.Name}' exported to '{fileFullPath}'");
            _dataRepositoryTask.ExportToExcel(obs, fileFullPath, launchExcel: false);
         });
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
         if (!string.IsNullOrEmpty(runOptions.ConfigurationFile))
         {
            _logger.AddDebug($"Reading configuration from file '{runOptions.ConfigurationFile}'");
            return _jsonSerializer.Deserialize<QualifcationConfiguration>(runOptions.ConfigurationFile);
         }

         if (!string.IsNullOrEmpty(runOptions.Configuration))
         {
            _logger.AddDebug("Reading configuration from string");
            return _jsonSerializer.DeserializeFromString<QualifcationConfiguration>(runOptions.Configuration);
         }

         return Task.FromResult<QualifcationConfiguration>(null);
      }
   }
}