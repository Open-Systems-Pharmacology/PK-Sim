using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Presentation;
using PKSim.Presentation.Presenters.Individuals;

namespace PKSim.UI.Starter
{
   public class PKSimStarterPresenterRegister : Register
   {
      public override void RegisterInContainer(IContainer container)
      {
         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<PresenterRegister>();

            //not registered because we are explicitly registering CreateIndividualPresenterForMoBi
            scan.ExcludeType<CreateIndividualPresenter>();
            scan.WithConvention<PKSimStarterRegistrationConvention>();
         });
      }
   }

   public class PKSimStarterUserInterfaceRegister : Register
   {
      public override void RegisterInContainer(IContainer container)
      {
         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<UserInterfaceRegister>();
            scan.WithConvention<PKSimStarterRegistrationConvention>();
         });
      }
   }
}