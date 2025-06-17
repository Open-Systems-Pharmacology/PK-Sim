using OSPSuite.Core.Snapshots.Mappers;
using PKSim.Core.Model;

namespace PKSim.Core.Snapshots.Mappers;

public interface IClassificationSnapshotTask : IClassificationSnapshotTask<PKSimProject>;

public class ClassificationSnapshotTask(ClassificationMapper classificationMapper) : ClassificationSnapshotTask<PKSimProject>(classificationMapper), IClassificationSnapshotTask
{
   protected override ClassificationSnapshotContext<PKSimProject> ContextFor<TClassifiable, TSubject>(SnapshotContext<PKSimProject> snapshotContext)
   {
      return new ClassificationSnapshotContext(ClassificationTypeFor<TClassifiable>(), snapshotContext);
   }
}