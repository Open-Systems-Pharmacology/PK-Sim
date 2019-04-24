using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;

namespace PKSim.CLI.Services
{
   public class CLIWithWorkspaceLayout : IWithWorkspaceLayout
   {
      public IWorkspaceLayout WorkspaceLayout { get; set; }
   }
}