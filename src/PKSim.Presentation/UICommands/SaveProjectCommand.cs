using OSPSuite.Presentation.MenuAndBars;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.UICommands
{
   public class SaveProjectCommand : IUICommand
   {
      private readonly IProjectTask _projectTask;

      public SaveProjectCommand(IProjectTask projectTask)
      {
         _projectTask = projectTask;
      }

      public void Execute()
      {
         _projectTask.SaveCurrentProject();
      }
   }
}