using OSPSuite.Presentation.MenuAndBars;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.UICommands
{
   public class SaveProjectAsCommand : IUICommand
   {
      private readonly IProjectTask _projectTask;

      public SaveProjectAsCommand(IProjectTask projectTask)
      {
         _projectTask = projectTask;
      }

      public void Execute()
      {
         _projectTask.SaveCurrentProjectAs();
      }
   }
}