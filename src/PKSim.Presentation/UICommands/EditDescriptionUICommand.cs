using OSPSuite.Core.Domain;
using OSPSuite.Presentation.UICommands;
using PKSim.Core;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class EditDescriptionUICommand : ObjectUICommand<IObjectBase>
   {
      private readonly ICoreWorkspace _workspace;
      private readonly IEntityTask _entityTask;

      public EditDescriptionUICommand(ICoreWorkspace workspace, IEntityTask entityTask)
      {
         _workspace = workspace;
         _entityTask = entityTask;
      }

      protected override void PerformExecute()
      {
         _workspace.AddCommand(_entityTask.EditDescription(Subject));
      }
   }
}