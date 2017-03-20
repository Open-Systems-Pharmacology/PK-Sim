using OSPSuite.Presentation.MenuAndBars;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.UICommands
{
   public class CloseProjectCommand : IUICommand
   {
      private readonly IProjectTask _projectTask;

      public CloseProjectCommand(IProjectTask projectTask)
      {
         _projectTask = projectTask;
      }

      public void Execute()
      {
         _projectTask.CloseCurrentProject();
      }
   }
}