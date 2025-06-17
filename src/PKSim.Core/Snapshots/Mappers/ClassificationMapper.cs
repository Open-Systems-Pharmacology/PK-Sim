using OSPSuite.Core.Domain;
using OSPSuite.Core.Snapshots.Mappers;
using PKSim.Core.Model;

namespace PKSim.Core.Snapshots.Mappers;

public class ClassificationSnapshotContext : ClassificationSnapshotContext<PKSimProject>
{
   public ClassificationSnapshotContext(ClassificationType classificationType, SnapshotContext<PKSimProject> baseContext) : base(classificationType, baseContext, ProjectVersions.Current)
   {
      
   }
   
}

public class ClassificationMapper : ClassificationMapper<PKSimProject>;