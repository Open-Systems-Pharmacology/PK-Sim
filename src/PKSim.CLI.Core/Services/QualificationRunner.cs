using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using PKSim.CLI.Core.RunOptions;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Services;
using Project = PKSim.Core.Snapshots.Project;

namespace PKSim.CLI.Core.Services
{
   public class BuildingBlockSwap : IWithName
   {
      public PKSimBuildingBlockType Type { get; set; }
      public string Name { get; set; }
      public string SnapshotPath { get; set; }
   }

   public class QualifcationConfiguration
   {
      public string SnapshotPath { get; set; }
      public string Output { get; set; }
      public BuildingBlockSwap[] BuildingBlocks { get; set; }
   }

   public class QualificationRunner : IBatchRunner<QualificationRunOptions>
   {
      private readonly ISnapshotTask _snapshotTask;
      private readonly IJsonSerializer _jsonSerializer;
      private readonly ILogger _logger;

      public QualificationRunner(ISnapshotTask snapshotTask, IJsonSerializer jsonSerializer, ILogger logger)
      {
         _snapshotTask = snapshotTask;
         _jsonSerializer = jsonSerializer;
         _logger = logger;
      }

      public async Task RunBatchAsync(QualificationRunOptions runOptions)
      {
         _logger.AddDebug("Starting qualification run");

         //TODO Check string or fi;e
         var config = await _jsonSerializer.DeserializeFromString<QualifcationConfiguration>(runOptions.Configuration);

         var snapshot = await _snapshotTask.LoadSnapshotFromFile<Project>(config.SnapshotPath);
         performBuildingBlockSwap(snapshot, config.BuildingBlocks);

         var project = await _snapshotTask.LoadProjectFromSnapshot(snapshot);

         project.Name = "toto";

      }

      private void performBuildingBlockSwap(Project snapshot, BuildingBlockSwap[] buildingBlockSwaps)
      {
         if (buildingBlockSwaps == null || buildingBlockSwaps.Length == 0)
            return;

         //TODO 
         return;
      }
   }
}