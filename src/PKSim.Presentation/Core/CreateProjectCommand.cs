using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Events;
using OSPSuite.Presentation.Core;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Presentation.Core
{
   public class CreateProjectCommand : PKSimCommand
   {
      private IWorkspace _workspace;

      public CreateProjectCommand(IWorkspace workspace)
      {
         _workspace = workspace;
         ObjectType = ObjectTypes.Project;
         CommandType = Command.CommandTypeAdd;
      }

      protected override void ExecuteWith(IExecutionContext context)
      {
         var metaDataFactory = context.Resolve<ICreationMetaDataFactory>();
         var project = new PKSimProject {Creation = metaDataFactory.Create()};
         _workspace.Project = project;
         _workspace.WorkspaceLayout = new WorkspaceLayout();
         Description = Command.CreateProjectDescription(project.Creation.Version);
         context.PublishEvent(new ProjectCreatedEvent(project));
      }

      protected override void ClearReferences()
      {
         _workspace = null;
      }
   }
}