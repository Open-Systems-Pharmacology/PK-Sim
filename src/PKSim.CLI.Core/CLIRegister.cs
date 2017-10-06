using OSPSuite.Utility.Container;
using PKSim.CLI.Core.Services;
using PKSim.Core;

namespace PKSim.CLI.Core
{
   public class CLIRegister : Register
   {
      public override void RegisterInContainer(IContainer container)
      {
         container.AddScanner(x =>
         {
            x.AssemblyContainingType<CLIRegister>();

            //Register services
            x.IncludeNamespaceContainingType<SnapshotRunner>();
            x.WithConvention<PKSimRegistrationConvention>();
         });
      }
   }
}