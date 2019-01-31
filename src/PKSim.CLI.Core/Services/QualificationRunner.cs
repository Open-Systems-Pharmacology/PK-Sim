using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using PKSim.CLI.Core.RunOptions;
using PKSim.Core;
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
      public string SnapshotPath { get; set; }
      public string OutputFolder { get; set; }
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

      public QualificationRunner(ISnapshotTask snapshotTask,
         IJsonSerializer jsonSerializer,
         IWorkspace workspace,
         IWorkspacePersistor workspacePersistor,
         IExportSimulationRunner exportSimulationRunner,
         ILogger logger
      )
      {
         _snapshotTask = snapshotTask;
         _jsonSerializer = jsonSerializer;
         _workspace = workspace;
         _workspacePersistor = workspacePersistor;
         _logger = logger;
         _exportSimulationRunner = exportSimulationRunner;
      }

      public async Task RunBatchAsync(QualificationRunOptions runOptions)
      {
         _logger.AddDebug("Starting qualification run");

         var config = await readConfigurationFrom(runOptions);
         if (config == null)
         {
            _logger.AddError(UnableToLoadQualificationConfigurationFromOptions);
            return;
         }

         if (string.IsNullOrEmpty(config.OutputFolder))
         {
            _logger.AddError(QualificationOutputFolderNotDefined);
            return;
         }

         var begin = DateTime.UtcNow;
         if (!FileHelper.FileExists(config.SnapshotPath))
         {
            _logger.AddError(CannotLoadSnapshotFromFile(config.SnapshotPath));
            return;
         }

         var snapshot = await _snapshotTask.LoadSnapshotFromFile<Project>(config.SnapshotPath);
         snapshot.Name = FileHelper.FileNameFromFileFullPath(config.SnapshotPath);

         await performBuildingBlockSwap(snapshot, config.BuildingBlocks);

         var project = await _snapshotTask.LoadProjectFromSnapshot(snapshot);
         var projectOutputFolder = createProjectOutputFolder(config.OutputFolder, project.Name);

         var exportRunOtions = new ExportRunOptions
         {
            OutputFolder = projectOutputFolder,
            ExportMode = SimulationExportMode.All
         };

         await _exportSimulationRunner.ExportSimulationsIn(project, exportRunOtions);

         var projectFile = Path.Combine(projectOutputFolder, $"{project.Name}{CoreConstants.Filter.PROJECT_EXTENSION}");
         _workspace.Project = project;
         _workspacePersistor.SaveSession(_workspace, projectFile);

         var end = DateTime.UtcNow;
         var timeSpent = end - begin;
         _logger.AddInfo($"Project '{project.Name}' exported for qualification in {timeSpent.ToDisplay()}");
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
         {
            _logger.AddError(CannotLoadSnapshotFromFile(snapshotPath));
            return;
         }

         var buildiingBlockToUse = referenceSnasphot.BuildingBlockByTypeAndName(type, name);
         if (buildiingBlockToUse == null)
         {
            _logger.AddError(CannotFindBuildingBlockInSnapshot(type.ToString(), name, snapshotPath));
            return;
         }


         var buildingBlock = projectSnapshot.BuildingBlockByTypeAndName(type, name);
         if (buildingBlock == null)
         {
            _logger.AddError(CannotFindBuildingBlockInSnapshot(type.ToString(), name, projectSnapshot.Name));
            return;
         }

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