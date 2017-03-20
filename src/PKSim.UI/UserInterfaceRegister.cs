using PKSim.Presentation;
using PKSim.Presentation.Presenters.Main;
using PKSim.Presentation.Views.Main;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Views;
using PKSim.UI.Views.Simulations;
using OSPSuite.Core;
using OSPSuite.Presentation;
using OSPSuite.Presentation.Views;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using ICoreUserSettings = PKSim.Core.ICoreUserSettings;

namespace PKSim.UI
{
   public class UserInterfaceRegister : Register
   {
      /// <summary>
      ///    Register in container the minimum required to be able to launch the application
      /// </summary>
      public static void InitializeForStartup(IContainer container)
      {
         container.Register<IPKSimMainView, Shell>(LifeStyle.Singleton);
         container.Register<ISplashView, SplashScreen>();
         container.Register<ISplashViewPresenter, SplashViewPresenter>();

         var shell = container.Resolve<IPKSimMainView>().DowncastTo<Shell>();
         container.RegisterImplementationOf(shell);
         container.RegisterImplementationOf((IShell) shell);
         container.RegisterImplementationOf((IMainView) shell);

         var exceptionView = container.Resolve<IExceptionView>();
         exceptionView.MainView = shell;
      }

      public override void RegisterInContainer(IContainer container)
      {
         //Register PKSim.UI
         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<UserInterfaceRegister>();
            scan.ExcludeType<UserSettings>();
            scan.WithConvention<OSPSuiteRegistrationConvention>();
         });

         //register open views
         container.Register(typeof(ISimulationCompoundProcessView<,>), typeof(SimulationCompoundProcessView<,>));

         container.Register<IUserSettings, IPresentationUserSettings, ICoreUserSettings, OSPSuite.Core.ICoreUserSettings, UserSettings>(LifeStyle.Singleton);
      }
   }
}