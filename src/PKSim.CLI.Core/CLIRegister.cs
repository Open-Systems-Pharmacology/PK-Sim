using OSPSuite.Utility.Container;
using PKSim.CLI.Core.Services;
using PKSim.Core;
using PKSim.Core.Batch;

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

            //TODO DELETE
            x.IncludeNamespaceContainingType<SimulationConstructor>();

            x.WithConvention<PKSimRegistrationConvention>();
         });
      }
   }
}