using DevExpress.LookAndFeel;
using DevExpress.XtraBars;
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
using OSPSuite.Presentation.Mappers;
using PKSim.Presentation;
using PKSim.Presentation.Mappers;
using System;
using System.Threading;
using PKSim.UI;
using PresenterRegister = OSPSuite.Presentation.PresenterRegister;

namespace PKSim.Starter
{
   public class UIApplicationStartup : ApplicationStartup
   {
      static IContainer _container;

      public static IContainer Initialize()
      {
         if (_container != null)
            return _container;

         RedirectAssembly("System.ComponentModel.Annotations", new Version(4, 2, 1, 0), "b03f5f7f11d50a3a");
         _container = initializeForStartup(IoC.Container);

         return _container;
      }

      private static IContainer initializeForStartup(IContainer moBiContainer)
      {
         ApplicationIcons.DefaultIcon = ApplicationIcons.PKSim;

         var pkSimContainer = InfrastructureRegister.Initialize(registerContainerAsStatic: false);

         using (pkSimContainer.OptimizeDependencyResolution())
         {
            // Set SynchronizationContext using MoBi Application context before registering new DevExpress components
            // They will create and use a new context but will not start a message loop until a UI is loaded
            // There are some methods in the API that do not use a UI
            var synchronizationContext = moBiContainer.Resolve<SynchronizationContext>();
            SynchronizationContext.SetSynchronizationContext(synchronizationContext);
            pkSimContainer.RegisterImplementationOf(synchronizationContext);
            
            // Register base DevExpress components
            pkSimContainer.RegisterImplementationOf(new DockManager());
            pkSimContainer.RegisterImplementationOf(new RibbonBarManager(new RibbonControl()));
            pkSimContainer.RegisterImplementationOf(UserLookAndFeel.Default);
            pkSimContainer.RegisterImplementationOf(new BarManager());

            // Cross register the main view and presentation components from MoBi into the PKSim
            // container so that modal dialogs can be viewed within the MoBi shell
            IShell shell = moBiContainer.Resolve<IShell>();
            pkSimContainer.RegisterImplementationOf(shell as BaseShell);
            pkSimContainer.RegisterImplementationOf(shell);
            pkSimContainer.RegisterImplementationOf(moBiContainer.Resolve<IMainViewPresenter>());
            pkSimContainer.RegisterImplementationOf(moBiContainer.Resolve<IMainView>());
            pkSimContainer.RegisterImplementationOf(moBiContainer.Resolve<IJournalPresenter>());

            pkSimContainer.RegisterImplementationOf(NumericFormatterOptions.Instance);
            pkSimContainer.Register<IApplicationController, ApplicationController>(LifeStyle.Singleton);
            pkSimContainer.Register<ICoreWorkspace, OSPSuite.Core.IWorkspace, IWorkspace, Workspace>(LifeStyle.Singleton);
            pkSimContainer.Register<ICoreUserSettings, OSPSuite.Core.ICoreUserSettings, IPresentationUserSettings, IUserSettings, UserSettings>(LifeStyle.Singleton);
            pkSimContainer.Register<IClassificationTypeToRootNodeTypeMapper, PKSimClassificationTypeToRootNodeTypeMapper>();

            // Full registration of OSPSuite Core UI and Presentation
            pkSimContainer.AddRegister(x => x.FromType<UIRegister>());
            pkSimContainer.AddRegister(x => x.FromType<PresenterRegister>());

            // Full registration of PKSim Core and Infrastructure
            pkSimContainer.AddRegister(x => x.FromType<CoreRegister>());
            pkSimContainer.AddRegister(x => x.FromType<InfrastructureRegister>());

            // Registration of PKSim presentation and UI for starter
            pkSimContainer.AddRegister(x => x.FromType<PKSimStarterPresenterRegister>());
            pkSimContainer.AddRegister(x => x.FromType<PKSimStarterUserInterfaceRegister>());

            pkSimContainer.Register<IInteractiveSimulationRunner, InteractiveSimulationRunner>(LifeStyle.Singleton);
            InfrastructureRegister.RegisterSerializationDependencies(pkSimContainer);
            PKSim.Presentation.Infrastructure.PresentationSerializerInitializer.AddPresentationSerializers(pkSimContainer);
            InfrastructureRegister.LoadDefaultEntities(pkSimContainer);
            pkSimContainer.Register<IExceptionManager, ExceptionManager>(LifeStyle.Singleton);
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