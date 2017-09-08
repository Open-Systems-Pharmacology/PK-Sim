using SnapshotProject  = PKSim.Core.Snapshots.Project;
using ModelProject = PKSim.Core.Model.PKSimProject;


namespace PKSim.Core.Snapshots.Mappers
{
   public class ProjectMapper : ObjectBaseSnapshotMapperBase<ModelProject, SnapshotProject>
   {
      public override SnapshotProject MapToSnapshot(ModelProject project)
      {
         throw new System.NotImplementedException();
      }

      public override ModelProject MapToModel(SnapshotProject snapshot)
      {
         throw new System.NotImplementedException();
      }
   }
}