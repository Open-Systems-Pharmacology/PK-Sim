using OSPSuite.Presentation.MenuAndBars;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.UICommands
{
   public class LoadProjectFromSnapshotUICommand : IUICommand
   {
      private readonly IProjectTask _projectTask;

      public LoadProjectFromSnapshotUICommand(IProjectTask projectTask)
      {
         _projectTask = projectTask;
      }

      public void Execute()
      {
         _projectTask.LoadProjectFromSnapshot();
      }
   }
}