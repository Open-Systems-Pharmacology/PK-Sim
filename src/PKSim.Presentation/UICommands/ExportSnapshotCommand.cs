using OSPSuite.Core.Domain;
using OSPSuite.Presentation.UICommands;
using PKSim.Core.Snapshots.Services;

namespace PKSim.Presentation.UICommands
{
   public class ExportSnapshotCommand<T>  : ObjectUICommand<T> where T : class, IObjectBase
   {
      private readonly ISnapshotTask _snapshotTask;

      public ExportSnapshotCommand(ISnapshotTask snapshotTask)
      {
         _snapshotTask = snapshotTask;
      }

      protected override void PerformExecute()
      {
         _snapshotTask.ExportSnapshot(Subject);
      }
   }
}