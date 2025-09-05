using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Container;
using PKSim.CLI.Core;
using PKSim.Core;
using PKSim.R.Services;

namespace PKSim.R
{
   internal class RRegister : Register
   {
      public override void RegisterInContainer(IContainer container)
      {
         container.AddRegister(x => x.FromType<CLIRegister>());

         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<RRegister>();

            //Register Services
            scan.IncludeNamespaceContainingType<IOntogenyFactorsRetriever>();

            scan.WithConvention<PKSimRegistrationConvention>();
         });
         registerCLITypes(container);
      }

      private static void registerCLITypes(IContainer container)
      {
         container.Register<IHistoryManager, HistoryManager<IExecutionContext>>();
      }
   }
}