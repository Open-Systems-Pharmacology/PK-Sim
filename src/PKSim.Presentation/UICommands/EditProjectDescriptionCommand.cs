using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Presentation.Core;

namespace PKSim.Presentation.UICommands
{
   public class EditProjectDescriptionCommand : IUICommand
   {
      private readonly ICoreWorkspace _workspace;
      private readonly IEntityTask _entityTask;

      public EditProjectDescriptionCommand(ICoreWorkspace workspace, IEntityTask entityTask)
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