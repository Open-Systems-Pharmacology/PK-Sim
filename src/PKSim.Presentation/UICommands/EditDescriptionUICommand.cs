using PKSim.Core.Services;
using PKSim.Presentation.Core;
using PKSim.Presentation.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;
using PKSim.Core;

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