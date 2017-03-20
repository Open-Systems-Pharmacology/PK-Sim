using PKSim.Presentation.Services;
using OSPSuite.Presentation.MenuAndBars;

namespace PKSim.Presentation.UICommands
{
   public class OpenProjectCommand : IUICommand
   {
      private readonly IProjectTask _projectTask;

      public OpenProjectCommand(IProjectTask projectTask)
      {
         _projectTask = projectTask;
      }

      public void Execute()
      {
         _projectTask.OpenProject();
      }
   }
}