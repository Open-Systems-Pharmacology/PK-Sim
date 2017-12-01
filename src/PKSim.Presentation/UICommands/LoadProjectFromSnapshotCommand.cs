using OSPSuite.Presentation.MenuAndBars;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.UICommands
{
   public class LoadProjectFromSnapshotCommand : IUICommand
   {
      private readonly IProjectTask _projectTask;

      public LoadProjectFromSnapshotCommand(IProjectTask projectTask)
      {
         _projectTask = projectTask;
      }

      public void Execute()
      {
         _projectTask.LoadProjectFromSnapshot();
      }
   }
}