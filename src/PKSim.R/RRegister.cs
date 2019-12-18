using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.R.Services;

namespace PKSim.R
{
   internal class RRegister : Register
   {
      public override void RegisterInContainer(IContainer container)
      {
         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<RRegister>();

            //Register Services
            scan.IncludeNamespaceContainingType<IOntogenyFactorsRetriever>();

            scan.WithConvention<PKSimRegistrationConvention>();
         });
      }
   }
}