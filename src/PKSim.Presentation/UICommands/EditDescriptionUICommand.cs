using PKSim.Core.Services;
using PKSim.Presentation.Core;
using PKSim.Presentation.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class EditDescriptionUICommand : ObjectUICommand<IObjectBase>
   {
      private readonly IWorkspace _workspace;
      private readonly IEntityTask _entityTask;

      public EditDescriptionUICommand(IWorkspace workspace, IEntityTask entityTask)
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