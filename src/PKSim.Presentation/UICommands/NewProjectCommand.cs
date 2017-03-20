using OSPSuite.Presentation.MenuAndBars;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.UICommands
{
   public class NewProjectCommand : IUICommand
   {
      private readonly IProjectTask _projectTask;

      public NewProjectCommand(IProjectTask projectTask)
      {
         _projectTask = projectTask;
      }

      public void Execute()
      {
         _projectTask.NewProject();
      }
   }
}