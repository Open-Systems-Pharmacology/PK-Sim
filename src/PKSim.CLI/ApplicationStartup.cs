using System.Globalization;
using System.Threading;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Serialization.Diagram;
using OSPSuite.Core.Services;
using OSPSuite.Presentation;
using OSPSuite.Presentation.Core;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Format;
using PKSim.CLI.Core;
using PKSim.CLI.Core.MinimalImplementations;
using PKSim.Core;
using PKSim.Infrastructure;
using PresenterRegister = PKSim.Presentation.PresenterRegister;

namespace PKSim.CLI
{
   public static class ApplicationStartup
   {
      public static void Initialize()
      {
         Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
         Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

         InfrastructureRegister.Initialize();
         var container = IoC.Container;
         container.RegisterImplementationOf(new SynchronizationContext());
         container.Register<IExceptionManager, CLIExceptionManager>(LifeStyle.Singleton);
      }

      public static void Start()
      {
         var container = IoC.Container;

         using (container.OptimizeDependencyResolution())
         {
            container.RegisterImplementationOf(NumericFormatterOptions.Instance);
            container.Register<IApplicationController, ApplicationController>();

            registerCLITypes(container);

            container.AddRegister(x => x.FromType<PresenterRegister>());
            container.AddRegister(x => x.FromType<CoreRegister>());
            container.AddRegister(x => x.FromType<OSPSuite.Presentation.PresenterRegister>());
            container.AddRegister(x => x.FromType<InfrastructureRegister>());
            container.AddRegister(x => x.FromType<CLIRegister>());

            InfrastructureRegister.RegisterSerializationDependencies();
         }
      }

      private static void registerCLITypes(IContainer container)
      {
         container.Register<IProgressUpdater, CLIProgressUpdater>();
         container.Register<IDialogCreator, CLIDialogCreator>();
         container.Register<IDisplayUnitRetriever, CLIDisplayUnitRetriever>();
         container.Register<IJournalDiagramManagerFactory, CLIJournalDiagramManagerFactory>();
         container.Register<IDiagramModel, CLIDiagramModel>();
         container.Register<IDiagramModelToXmlMapper, CLIDiagramModelToXmlMapper>(LifeStyle.Singleton);
         container.Register<IHistoryManager, HistoryManager<IExecutionContext>>();
         container.Register<ICoreUserSettings, OSPSuite.Core.ICoreUserSettings, IPresentationUserSettings, CLIUserSettings>(LifeStyle.Singleton);
         InfrastructureRegister.RegisterWorkspace<CLIWorkspace>();
      }
   }
}