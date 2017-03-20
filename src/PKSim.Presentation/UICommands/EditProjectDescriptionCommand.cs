using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Services;
using PKSim.Presentation.Core;
using PKSim.Presentation.Services;
using OSPSuite.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class EditProjectDescriptionCommand : IUICommand
   {
      private readonly IWorkspace _workspace;
      private readonly IEntityTask _entityTask;

      public EditProjectDescriptionCommand(IWorkspace workspace, IEntityTask entityTask)
      {
         _workspace = workspace;
         _entityTask = entityTask;
      }

      public void Execute()
      {
         _workspace.AddCommand(_entityTask.EditDescription(_workspace.Project));
      }
   }
}