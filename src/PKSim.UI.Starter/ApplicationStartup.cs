using System.Globalization;
using System.Threading;
using DevExpress.LookAndFeel;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Ribbon;
using OSPSuite.Assets;
using OSPSuite.Presentation;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters.Journal;
using OSPSuite.Presentation.Presenters.Main;
using OSPSuite.Presentation.Services;
using OSPSuite.Presentation.Views;
using OSPSuite.UI;
using OSPSuite.UI.Views;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Format;
using PKSim.CLI.Core.MinimalImplementations;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using PKSim.Presentation;
using PKSim.Presentation.UICommands;

namespace PKSim.UI.Starter
{
   public static class ApplicationStartup
   {
      static IContainer _container;

      public static IContainer Initialize()
      {
         if (_container != null)
            return _container;

         _container = initializeForStartup(IoC.Container);

         return _container;
      }

      private static IContainer initializeForStartup(IContainer moBiContainer)
      {
         Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
         Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
         ApplicationIcons.DefaultIcon = ApplicationIcons.PKSim;

         var pkSimContainer = InfrastructureRegister.Initialize(registerContainerAsStatic: false);

         using (pkSimContainer.OptimizeDependencyResolution())
         {
            // Register base DevExpress components
            pkSimContainer.RegisterImplementationOf(new DockManager());
            pkSimContainer.RegisterImplementationOf(new RibbonBarManager(new RibbonControl()));
            pkSimContainer.RegisterImplementationOf(UserLookAndFeel.Default);
            pkSimContainer.RegisterImplementationOf(new DevExpress.XtraBars.BarManager());

            // Cross register the main view and presentation components from MoBi into the PKSim
            // container so that modal dialogs can be viewed within the MoBi shell
            IShell shell = moBiContainer.Resolve<IShell>();
            pkSimContainer.RegisterImplementationOf(shell as BaseShell);
            pkSimContainer.RegisterImplementationOf(shell);
            pkSimContainer.RegisterImplementationOf(moBiContainer.Resolve<IMainViewPresenter>());
            pkSimContainer.RegisterImplementationOf(moBiContainer.Resolve<IMainView>());
            pkSimContainer.RegisterImplementationOf(moBiContainer.Resolve<IJournalPresenter>());
            
            pkSimContainer.RegisterImplementationOf(NumericFormatterOptions.Instance);
            pkSimContainer.RegisterImplementationOf(SynchronizationContext.Current);
            pkSimContainer.Register<IApplicationController, ApplicationController>(LifeStyle.Singleton);
            pkSimContainer.Register<ICoreWorkspace, OSPSuite.Core.IWorkspace, IWorkspace, Workspace>(LifeStyle.Singleton);
            pkSimContainer.Register<ICoreUserSettings, OSPSuite.Core.ICoreUserSettings, IPresentationUserSettings, IUserSettings, UserSettings>(LifeStyle.Singleton);

            // Full registration of OSPSuite Core UI and Presentation
            pkSimContainer.AddRegister(x => x.FromType<UIRegister>());
            pkSimContainer.AddRegister(x => x.FromType<OSPSuite.Presentation.PresenterRegister>());

            // Full registration of PKSim Core and Infrastructure
            pkSimContainer.AddRegister(x => x.FromType<CoreRegister>());
            pkSimContainer.AddRegister(x => x.FromType<InfrastructureRegister>());

            // Registration of PKSim presentation and UI for starter
            pkSimContainer.AddRegister(x => x.FromType<PKSimStarterPresenterRegister>());
            pkSimContainer.AddRegister(x => x.FromType<PKSimStarterUserInterfaceRegister>());
            
            pkSimContainer.Register<IInteractiveSimulationRunner, InteractiveSimulationRunner>(LifeStyle.Singleton);
            InfrastructureRegister.LoadSerializers(pkSimContainer);
            pkSimContainer.Register<IExceptionManager, ExceptionManager>(LifeStyle.Singleton);
            InfrastructureRegister.RegisterWorkspace(pkSimContainer);
         }

         return pkSimContainer;
      }
   }

   internal class Workspace : CLIWorkspace, IWorkspace
   {
      public Workspace(IRegistrationTask registrationTask) : base(registrationTask)
      {
      }

      public IWorkspaceLayout WorkspaceLayout { get; set; }
   }
}