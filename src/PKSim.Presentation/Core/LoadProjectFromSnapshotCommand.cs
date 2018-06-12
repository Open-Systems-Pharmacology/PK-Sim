using OSPSuite.Assets;
using OSPSuite.Core;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Presentation.Core
{
   public class LoadProjectFromSnapshotCommand : PKSimCommand
   {
      private PKSimProject _project;
      private readonly string _snapshotFile;
      private IWorkspace _workspace;

      public LoadProjectFromSnapshotCommand(IWorkspace workspace, PKSimProject project, string snapshotFile)
      {
         _workspace = workspace;
         _project = project;
         _snapshotFile = snapshotFile;
         ObjectType = ObjectTypes.Project;
         CommandType = Command.CommandTypeAdd;
      }

      protected override void ExecuteWith(IExecutionContext context)
      {
         _workspace.LoadProject(_project);
         var configuration = context.Resolve<IApplicationConfiguration>();
         Description = PKSimConstants.Command.LoadProjectFromSnapshotDescription(_snapshotFile, configuration.Version, _project.Creation.Version);
      }

      protected override void ClearReferences()
      {
         _workspace = null;
         _project = null;
      }
   }
}