using OSPSuite.Assets;
using OSPSuite.Core;
using OSPSuite.Core.Events;
using OSPSuite.Presentation.Core;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Presentation.Core
{
   public class CreateProjectCommand : PKSimCommand
   {
      private PKSimProject _project;
      private IWorkspace _workspace;

      public CreateProjectCommand(IWorkspace workspace, PKSimProject project)
      {
         _workspace = workspace;
         _project = project;
         ObjectType = ObjectTypes.Project;
         CommandType = Command.CommandTypeAdd;
      }

      protected override void ExecuteWith(IExecutionContext context)
      {
         _workspace.Project = _project;
         _workspace.WorkspaceLayout = new WorkspaceLayout();
         var configuration = context.Resolve<IApplicationConfiguration>();
         Description = Command.CreateProjectDescription(configuration.Version);
         context.PublishEvent(new ProjectCreatedEvent(_project));
      }

      protected override void ClearReferences()
      {
         _workspace = null;
         _project = null;
      }
   }
}