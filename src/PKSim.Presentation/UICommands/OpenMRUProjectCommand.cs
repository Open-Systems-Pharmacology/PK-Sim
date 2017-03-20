using OSPSuite.Presentation.MenuAndBars;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.UICommands
{
   public class OpenMRUProjectCommand : IUICommand
   {
      private readonly IProjectTask _projectTask;
      public string ProjectPath { get; set; }

      public OpenMRUProjectCommand(IProjectTask projectTask)
      {
         _projectTask = projectTask;
      }

      public void Execute()
      {
         _projectTask.OpenProjectFrom(ProjectPath);
      }
   }
}