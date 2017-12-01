using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters.Commands;
using OSPSuite.Presentation.Presenters.Main;
using OSPSuite.Presentation.Services;
using OSPSuite.UI;
using OSPSuite.Utility;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Format;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using PKSim.Infrastructure.Services;
using PKSim.Presentation;
using PKSim.Presentation.Core;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views;
using PKSim.UI.Views.Core;
using IContainer = OSPSuite.Utility.Container.IContainer;
using PresenterRegister = OSPSuite.Presentation.PresenterRegister;

namespace PKSim.UI.BootStrapping
{
   public class ApplicationStartup
   {
      public static void Initialize(LogLevel logLevel = LogLevel.Information)
      {
         new ApplicationStartup().InitializeForStartup(logLevel);
      }

      public void InitializeUserInterace()
      {
         //Register typed instance of shell and splash screen 
         UserInterfaceRegister.InitializeForStartup(IoC.Container);
      }

      public void InitializeForStartup(LogLevel logLevel)
      {
         Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
         Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

         updateGoDiagramKey();

         InfrastructureRegister.Initialize();
         var container = IoC.Container;
         container.RegisterImplementationOf(getCurrentContext());

         container.Register<IApplicationController, ApplicationController>(LifeStyle.Singleton);
         container.Register<PKSimApplication, PKSimApplication>(LifeStyle.Singleton);

         //UI and Presentation mandatory ojects for startup
         container.Register<IProgressUpdater, PKSimProgressUpdater>();
         container.RegisterImplementationOf(NumericFormatterOptions.Instance);
         container.Register<IExceptionManager, ExceptionManager>(LifeStyle.Singleton);

         container.AddRegister(x => x.FromType<UIRegister>());
         container.AddRegister(x => x.FromType<PresenterRegister>());

         //must be registered so that the ui thread can start the startup
         container.RegisterImplementationOf(this);

         container.Register<IConfigurableContainerLayoutView, AccordionLayoutView>(ViewLayouts.AccordionView.Id);
         container.Register<IConfigurableContainerLayoutView, TabbedLayoutView>(ViewLayouts.TabbedView.Id);

         configureLogger(container, logLevel);
      }

      private void configureLogger(IContainer container, LogLevel logLevel)
      {
         var loggerFactory = container.Resolve<ILoggerFactory>();
         loggerFactory
            .AddPresenter(logLevel)
            .AddDebug(logLevel);
      }

      private static void updateGoDiagramKey()
      {
         // This line is patched during creation of setup. Do not modify.
         UIRegister.GoDiagramKey = $"{Environment.GetEnvironmentVariable("GO_DIAGRAM_KEY")}";
      }

      private SynchronizationContext getCurrentContext()
      {
         var context = SynchronizationContext.Current;
         if (context == null)
         {
            context = new WindowsFormsSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(context);
         }
         return SynchronizationContext.Current;
      }

      public void Start()
      {
         var progressManager = IoC.Resolve<IProgressManager>();
         var container = IoC.Container;
         using (var progress = progressManager.Create())
         {
            progress.Initialize(8);

            using (container.OptimizeDependencyResolution())
            {
               showStatusMessage(progress, PKSimConstants.UI.RegisterAssembly(PKSimConstants.UI.Core));
               container.AddRegister(x => x.FromType<CoreRegister>());

               showStatusMessage(progress, PKSimConstants.UI.RegisterAssembly(PKSimConstants.UI.Infrastructure));
               container.AddRegister(x => x.FromType<InfrastructureRegister>());

               showStatusMessage(progress, message: PKSimConstants.UI.RegisterAssembly(PKSimConstants.UI.Presentation));
               container.AddRegister(x => x.FromType<Presentation.PresenterRegister>());

               showStatusMessage(progress, PKSimConstants.UI.RegisterAssembly(PKSimConstants.UI.UserInterface));
               container.AddRegister(x => x.FromType<UserInterfaceRegister>());

               showStatusMessage(progress, PKSimConstants.UI.RegisterAssembly(PKSimConstants.UI.Commands));
               RegisterCommands(container);

               showStatusMessage(progress, PKSimConstants.UI.RegisterSerializationDependencies);
               InfrastructureRegister.RegisterSerializationDependencies();

               finalizeRegistration(container);
            }

            showStatusMessage(progress, PKSimConstants.UI.StartingUserInterface);
            startStartableObject(container);
         }
      }

      /// <summary>
      ///    All specific registration that needs to be performed once all other registrations are done
      /// </summary>
      private void finalizeRegistration(IContainer container)
      {
         InfrastructureRegister.RegisterWorkspace();
         //Create one instance of the invokers so that the object is available in the application 
         //since the object is not created anywhere and is only used as event listener
         container.Resolve<ICloseSubjectPresenterInvoker>();
         container.Resolve<IExportToPDFInvoker>();

         var mainPresenter = container.Resolve<IMainViewPresenter>();
         container.RegisterImplementationOf((IChangePropagator) mainPresenter);

         //This runner is only register when running PKSim as an executable. All other implementation should use the ISimulationRunner
         container.Register<IInteractiveSimulationRunner,InteractiveSimulationRunner>(LifeStyle.Singleton);

      }

      private void startStartableObject(IContainer container)
      {
         var rep = container.ResolveAll<IStartable>().ToList();
         rep.Each(item => item.Start());
      }

      public static void RegisterCommands(IContainer container)
      {
         container.Register<IHistoryManager, HistoryManager<IExecutionContext>>();
         var historyBrowserConfiguration = container.Resolve<IHistoryBrowserConfiguration>();
         historyBrowserConfiguration.AddDynamicColumn(Constants.Command.BUILDING_BLOCK_TYPE, PKSimConstants.UI.BuildingBlockType);
         historyBrowserConfiguration.AddDynamicColumn(Constants.Command.BUILDING_BLOCK_NAME, PKSimConstants.UI.BuildingBlockName);
      }

      private void showStatusMessage(IProgressUpdater progressUpdater, string message)
      {
         progressUpdater.IncrementProgress($"{message}...");
      }
   }
}