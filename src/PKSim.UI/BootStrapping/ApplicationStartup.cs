using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using OSPSuite.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Infrastructure.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Mappers;
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
using PKSim.Presentation;
using PKSim.Presentation.Core;
using PKSim.Presentation.Infrastructure.Services;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Presenters.Main;
using PKSim.Presentation.Views;
using PKSim.UI.Views.Core;
using IContainer = OSPSuite.Utility.Container.IContainer;
using PresenterRegister = OSPSuite.Presentation.PresenterRegister;

namespace PKSim.UI.BootStrapping
{
   public class ApplicationStartup
   {
      public static void Initialize(LogLevel logLevel = LogLevel.Information, Action<IContainer> registrationAction = null)
      {
         new ApplicationStartup().InitializeForStartup(logLevel, registrationAction);
      }

      public void InitializeUserInterface()
      {
         //Register typed instance of shell and splash screen 
         UserInterfaceRegister.InitializeForStartup(IoC.Container);
      }

      public void InitializeForStartup(LogLevel logLevel, Action<IContainer> registrationAction)
      {
         ApplicationIcons.DefaultIcon = ApplicationIcons.PKSim;

         updateGoDiagramKey();
         initializeSynchronizationContext();

         var container = InfrastructureRegister.Initialize();
         container.RegisterImplementationOf(SynchronizationContext.Current);
         // Register the UI thread so the EventPublisher singleton captures it (via its explicit-thread
         // constructor) and dispatches synchronously (Send) from the UI thread rather than deferred (Post).
         container.RegisterImplementationOf(Thread.CurrentThread);

         container.Register<IApplicationController, ApplicationController>(LifeStyle.Singleton);
         container.Register<PKSimApplication, PKSimApplication>(LifeStyle.Singleton);

         //UI and Presentation mandatory objects for startup
         container.Register<IProgressUpdater, PKSimProgressUpdater>();
         container.RegisterImplementationOf(NumericFormatterOptions.Instance);
         container.Register<IExceptionManager, ExceptionManager>(LifeStyle.Singleton);
         container.Register<IClassificationTypeToRootNodeTypeMapper, PKSimClassificationTypeToRootNodeTypeMapper>();

         container.AddRegister(x => x.FromType<UIRegister>());
         container.AddRegister(x => x.FromType<PresenterRegister>());

         //must be registered so that the ui thread can start the startup
         container.RegisterImplementationOf(this);

         container.Register<IConfigurableContainerLayoutView, AccordionLayoutView>(ViewLayouts.AccordionView.Id);
         container.Register<IConfigurableContainerLayoutView, TabbedLayoutView>(ViewLayouts.TabbedView.Id);

         configureLogger(container, logLevel);

         registrationAction?.Invoke(container);
      }

      private void configureLogger(IContainer container, LogLevel logLevel)
      {
         var loggerCreator = container.Resolve<ILoggerCreator>();

         loggerCreator
            .AddLoggingBuilderConfiguration(builder =>
               builder
                  .SetMinimumLevel(logLevel)
                  .AddDebug()
                  .AddPresenter()
            );
      }

      private static void updateGoDiagramKey()
      {
         // This line is patched during creation of setup. Do not modify.
         UIRegister.GoDiagramKey = $"{Environment.GetEnvironmentVariable("GO_DIAGRAM_KEY")}";
      }

      private void initializeSynchronizationContext()
      {
         var context = new WindowsFormsSynchronizationContext();
         SynchronizationContext.SetSynchronizationContext(context);
      }

      public void Start()
      {
         var container = IoC.Container;
         using (var progress = createSplashProgressUpdater(container))
         {
            progress.Initialize(8);

            using (container.OptimizeDependencyResolution())
            {
               showStatusMessage(progress, PKSimConstants.UI.RegisterAssembly(PKSimConstants.UI.Core));
               container.AddRegister(x => x.FromType<CoreRegister>());

               showStatusMessage(progress, PKSimConstants.UI.RegisterAssembly(PKSimConstants.UI.Infrastructure));
               container.AddRegister(x => x.FromType<InfrastructureRegister>());

               showStatusMessage(progress, PKSimConstants.UI.RegisterAssembly(PKSimConstants.UI.Presentation));
               container.AddRegister(x => x.FromType<Presentation.PresenterRegister>());

               showStatusMessage(progress, PKSimConstants.UI.RegisterAssembly(PKSimConstants.UI.UserInterface));
               container.AddRegister(x => x.FromType<UserInterfaceRegister>());

               showStatusMessage(progress, PKSimConstants.UI.RegisterAssembly(PKSimConstants.UI.Commands));
               RegisterCommands(container);

               showStatusMessage(progress, PKSimConstants.UI.RegisterSerializationDependencies);
               InfrastructureRegister.RegisterSerializationDependencies(container);
               Presentation.Infrastructure.PresentationSerializerInitializer.AddPresentationSerializers(container);
               InfrastructureRegister.LoadDefaultEntities(container);

               finalizeRegistration(container);
            }

            showStatusMessage(progress, PKSimConstants.UI.StartingUserInterface);
            startStartableObject(container);
         }
      }

      //The splash runs on its own UI thread, so its progress updates must be dispatched to that thread. Bind a
      //dedicated EventPublisher to the splash's own synchronization context and drive a ProgressUpdater through it, so
      //the splash presenter handles the events on the splash thread - updating incrementally and without a cross-thread.
      private static IProgressUpdater createSplashProgressUpdater(IContainer container)
      {
         var splashPresenter = container.Resolve<ISplashViewPresenter>();
         var splashControl = (Control) splashPresenter.View;

         // Wait until the control is created, up to a maximum of 5 seconds
         SpinWait.SpinUntil(() => splashControl.IsHandleCreated, TimeSpan.FromSeconds(5));
         if (!splashControl.IsHandleCreated)
            return container.Resolve<IProgressManager>().Create();

         var splashContext = splashControl.Invoke(() => SynchronizationContext.Current);
         var splashThread = splashControl.Invoke(() => Thread.CurrentThread);
         var splashPublisher = new EventPublisher(splashContext, splashThread, container.Resolve<IExceptionManager>());
         splashPublisher.AddListener(splashPresenter);
         return new PKSimProgressUpdater(splashPublisher);
      }

      /// <summary>
      ///    All specific registration that needs to be performed once all other registrations are done
      /// </summary>
      private void finalizeRegistration(IContainer container)
      {
         //Create one instance of the invokers so that the object is available in the application
         //since the object is not created anywhere and is only used as event listener
         container.Resolve<ICloseSubjectPresenterInvoker>();
         container.Resolve<IJournalPageEditorActivator>();

         var mainPresenter = container.Resolve<IMainViewPresenter>();
         container.RegisterImplementationOf((IChangePropagator)mainPresenter);

         //This runner is only register when running PKSim as an executable. All other implementation should use the ISimulationRunner
         container.Register<IInteractiveSimulationRunner, InteractiveSimulationRunner>(LifeStyle.Singleton);
      }

      private void startStartableObject(IContainer container)
      {
         //Resolve all startables on the UI thread (Castle Windsor resolution stays single-threaded), then warm the
         //DB-backed repositories on a background thread so their loading overlaps the main window construction.
         var startables = container.ResolveAll<IStartable>().ToList();

         //Settings are read while the main window is built (e.g. layout restore, comparison settings), so they must
         //be loaded synchronously before construction; only the (DB-backed) repositories are deferred.
         var synchronousStartables = startables.OfType<SettingsLoader>().ToList();
         synchronousStartables.Each(item => item.Start());

         container.Resolve<IStartableWarmup>().Begin(startables.Except(synchronousStartables).ToList());
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