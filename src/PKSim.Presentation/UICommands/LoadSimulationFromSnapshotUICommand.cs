using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Snapshots.Services;

namespace PKSim.Presentation.UICommands
{
   public class LoadSimulationFromSnapshotUICommand : IUICommand
   {
      private readonly ISnapshotTask _snapshotTask;

      public LoadSimulationFromSnapshotUICommand(ISnapshotTask snapshotTask)
      {
         _snapshotTask = snapshotTask;
      }

      public void Execute()
      {
         throw new System.NotImplementedException();
      }
   }
}