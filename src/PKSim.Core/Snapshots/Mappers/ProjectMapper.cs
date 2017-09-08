using System;
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
            snapshot.Individuals = mapBuildingBlocks<Individual>(project.All<Model.Individual>());
            snapshot.Compounds = mapBuildingBlocks<Compound>(project.All<Model.Compound>());
         });
      }

      private List<T> mapBuildingBlocks<T>(IReadOnlyCollection<IPKSimBuildingBlock> buildingBlocks)
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
         throw new NotImplementedException();
      }
   }
}