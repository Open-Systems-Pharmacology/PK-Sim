using System.Collections.Generic;
using System.Linq;
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

      public override SnapshotProject MapToSnapshot(ModelProject project)
      {
         return SnapshotFrom(project, snapshot =>
         {
            snapshot.Individuals = mapBuildingBlocksToSnapshots<Individual>(project.All<Model.Individual>());
            snapshot.Compounds = mapBuildingBlocksToSnapshots<Compound>(project.All<Model.Compound>());
            snapshot.Events = mapBuildingBlocksToSnapshots<Event>(project.All<PKSimEvent>());
            snapshot.Formulations = mapBuildingBlocksToSnapshots<Formulation>(project.All<Model.Formulation>());
            snapshot.Protocols = mapBuildingBlocksToSnapshots<Protocol>(project.All<Model.Protocol>());
            snapshot.Populations = mapBuildingBlocksToSnapshots<Population>(project.All<Model.Population>());
         });
      }

      private List<T> mapBuildingBlocksToSnapshots<T>(IReadOnlyCollection<IPKSimBuildingBlock> buildingBlocks)
      {
         //required to load the snapshot mapper via execution context to avoid circular references
         var snapshotMapper = _executionContext.Resolve<ISnapshotMapper>();
         return buildingBlocks.Select(bb => mapBuildingBlockToSnapshot<T>(bb, snapshotMapper)).ToList();
      }

      private T mapBuildingBlockToSnapshot<T>(IPKSimBuildingBlock buildingBlock, ISnapshotMapper snapshotMapper)
      {
         _executionContext.Load(buildingBlock);
         return snapshotMapper.MapToSnapshot(buildingBlock).DowncastTo<T>();
      }

      public override ModelProject MapToModel(SnapshotProject snapshot)
      {
         var project = new ModelProject();
         allBuidingBlocksFrom(snapshot).Each(project.AddBuildingBlock);
         return project;
      }

      private IEnumerable<IPKSimBuildingBlock> allBuidingBlocksFrom(SnapshotProject snapshot)
      {
         var allBuildingBlocks = new List<IPKSimBuildingBlock>();
         allBuildingBlocks.AddRange(mapSnapshotsToBuildingBlocks(snapshot.Individuals));
         allBuildingBlocks.AddRange(mapSnapshotsToBuildingBlocks(snapshot.Compounds));
         allBuildingBlocks.AddRange(mapSnapshotsToBuildingBlocks(snapshot.Events));
         allBuildingBlocks.AddRange(mapSnapshotsToBuildingBlocks(snapshot.Formulations));
         allBuildingBlocks.AddRange(mapSnapshotsToBuildingBlocks(snapshot.Protocols));
         allBuildingBlocks.AddRange(mapSnapshotsToBuildingBlocks(snapshot.Populations));
         return allBuildingBlocks;
      }

      private IEnumerable<IPKSimBuildingBlock> mapSnapshotsToBuildingBlocks(IEnumerable<object> snapshots)
      {
         var snapshotMapper = _executionContext.Resolve<ISnapshotMapper>();
         return snapshots.Select(snapshotMapper.MapToModel).Cast<IPKSimBuildingBlock>();
      }
   }
}