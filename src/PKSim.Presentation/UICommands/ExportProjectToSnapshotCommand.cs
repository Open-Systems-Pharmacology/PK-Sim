using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Services;

namespace PKSim.Presentation.UICommands
{
   public class ExportProjectToSnapshotCommand : IUICommand
   {
      private readonly ISnapshotTask _snapshotTask;
      private readonly IPKSimProjectRetriever _projectRetriever;

      public ExportProjectToSnapshotCommand(ISnapshotTask snapshotTask, IPKSimProjectRetriever projectRetriever)
      {
         _snapshotTask = snapshotTask;
         _projectRetriever = projectRetriever;
      }

      public void Execute()
      {
         _snapshotTask.ExportSnapshot(_projectRetriever.Current);
      }
   }
}