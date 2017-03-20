using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using OSPSuite.Core;
using OSPSuite.Presentation.Core;
using OSPSuite.Assets;

namespace PKSim.Presentation.Core
{
   public class CreateProjectCommand : PKSimCommand
   {
      private IPKSimProject _project;
      private IWorkspace _workspace;

      public CreateProjectCommand(IWorkspace workspace, IPKSimProject project)
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
      }

      protected override void ClearReferences()
      {
         _workspace = null;
         _project = null;
      }
   }
}