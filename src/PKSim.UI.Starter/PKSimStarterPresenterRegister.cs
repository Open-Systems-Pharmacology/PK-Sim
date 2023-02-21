using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Presentation;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.UICommands;
using PKSim.UI.UICommands;

namespace PKSim.UI.Starter
{
   public class PKSimStarterPresenterRegister : BasePresenterRegister
   {
      public override void RegisterInContainer(IContainer container)
      {
         base.RegisterInContainer(container);
         container.Register<ICreateIndividualPresenter, CreateIndividualPresenterForMoBi>();
      }

      protected override void ExcludeTypes(IAssemblyScanner scan)
      {
         base.ExcludeTypes(scan);
         scan.ExcludeType<CreateIndividualPresenter>();
      }
      
      protected override void RegisterMainViewPresenters(IContainer container)
      {
         
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
         container.Register<OSPSuite.UI.Services.IToolTipCreator, IToolTipCreator, ToolTipCreator>(LifeStyle.Transient);
         container.Register<IExitCommand, ExitCommand>();
      }
   }
}