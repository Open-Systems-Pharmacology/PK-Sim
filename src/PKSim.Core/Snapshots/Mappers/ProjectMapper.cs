using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using SnapshotProject = PKSim.Core.Snapshots.Project;
using ModelProject = PKSim.Core.Model.PKSimProject;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ProjectMapper : ObjectBaseSnapshotMapperBase<ModelProject, SnapshotProject>
   {
      private readonly IExecutionContext _executionContext;

      public ProjectMapper(IExecutionContext executionContext)
      {
         _executionContext = executionContext;
      }

      public override async Task<SnapshotProject> MapToSnapshot(ModelProject project)
      {
         var snapshot = await SnapshotFrom(project);
         snapshot.Individuals = await mapBuildingBlocksToSnapshots<Individual>(project.All<Model.Individual>());
         snapshot.Compounds = await mapBuildingBlocksToSnapshots<Compound>(project.All<Model.Compound>());
         snapshot.Events = await mapBuildingBlocksToSnapshots<Event>(project.All<PKSimEvent>());
         snapshot.Formulations = await mapBuildingBlocksToSnapshots<Formulation>(project.All<Model.Formulation>());
         snapshot.Protocols = await mapBuildingBlocksToSnapshots<Protocol>(project.All<Model.Protocol>());
         snapshot.Populations = await mapBuildingBlocksToSnapshots<Population>(project.All<Model.Population>());
         return snapshot;
      }

      private async Task<T[]> mapBuildingBlocksToSnapshots<T>(IReadOnlyCollection<IPKSimBuildingBlock> buildingBlocks)
      {
         //required to load the snapshot mapper via execution context to avoid circular references
         var snapshotMapper = _executionContext.Resolve<ISnapshotMapper>();
         var tasks = buildingBlocks.Select(bb => mapBuildingBlockToSnapshot(bb, snapshotMapper));
         var snapshots = await Task.WhenAll(tasks);
         return snapshots.OfType<T>().ToArray();
      }

      private Task<object> mapBuildingBlockToSnapshot(IPKSimBuildingBlock buildingBlock, ISnapshotMapper snapshotMapper)
      {
         _executionContext.Load(buildingBlock);
         return snapshotMapper.MapToSnapshot(buildingBlock);
      }

      public override async Task<ModelProject> MapToModel(SnapshotProject snapshot)
      {
         var project = new ModelProject();
         var buildingBlocks = await allBuidingBlocksFrom(snapshot);
         buildingBlocks.Each(project.AddBuildingBlock);
         return project;
      }

      private async Task<IEnumerable<IPKSimBuildingBlock>> allBuidingBlocksFrom(SnapshotProject snapshot)
      {
         var tasks = new List<Task<object>>();
         tasks.AddRange(mapSnapshotsToBuildingBlocks(snapshot.Individuals));
         tasks.AddRange(mapSnapshotsToBuildingBlocks(snapshot.Compounds));
         tasks.AddRange(mapSnapshotsToBuildingBlocks(snapshot.Events));
         tasks.AddRange(mapSnapshotsToBuildingBlocks(snapshot.Formulations));
         tasks.AddRange(mapSnapshotsToBuildingBlocks(snapshot.Protocols));
         tasks.AddRange(mapSnapshotsToBuildingBlocks(snapshot.Populations));

         var buildingBlocks = await Task.WhenAll(tasks);
         return buildingBlocks.Cast<IPKSimBuildingBlock>();
      }

      private IEnumerable<Task<object>> mapSnapshotsToBuildingBlocks(IEnumerable<object> snapshots)
      {
         var snapshotMapper = _executionContext.Resolve<ISnapshotMapper>();
         return snapshots.Select(snapshotMapper.MapToModel);
      }
   }
}