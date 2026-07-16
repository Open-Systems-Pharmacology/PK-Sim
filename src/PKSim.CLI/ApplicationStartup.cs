using System.Threading;
using OSPSuite.Core;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Format;
using PKSim.CLI.Core;
using PKSim.CLI.Core.MinimalImplementations;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using PKSim.Infrastructure.Serialization;
using PKSim.Infrastructure.Serialization.Xml.Serializers;
using PKSim.Infrastructure.Services;
using CoreRegister = PKSim.Core.CoreRegister;
using ICoreUserSettings = PKSim.Core.ICoreUserSettings;

namespace PKSim.CLI
{
   public static class ApplicationStartup
   {
      public static void Initialize()
      {
         var container = InfrastructureRegister.Initialize();
         container.RegisterImplementationOf(new SynchronizationContext());
      }

      public static void Start()
      {
         var container = IoC.Container;

         using (container.OptimizeDependencyResolution())
         {
            container.RegisterImplementationOf(NumericFormatterOptions.Instance);

            registerCLITypes(container);

            container.AddRegister(x => x.FromType<CoreRegister>());
            container.AddRegister(x => x.FromType<InfrastructureRegister>());
            container.AddRegister(x => x.FromType<CLIRegister>());

            InfrastructureRegister.RegisterSerializationDependencies(container);
            PKSim.Presentation.Infrastructure.PresentationSerializerInitializer.AddPresentationSerializers(container);
            InfrastructureRegister.LoadDefaultEntities(container);
         }
      }

      private static void registerCLITypes(IContainer container)
      {
         container.Register<IProgressUpdater, NoneProgressUpdater>();
         container.Register<IOntogenyTask, CLIIndividualOntogenyTask>();
         container.Register<IHistoryManager, HistoryManager<IExecutionContext>>();
         container.Register<ICoreUserSettings, OSPSuite.Core.ICoreUserSettings, CLIUserSettings>(LifeStyle.Singleton);
         container.Register<ICoreWorkspace, IWorkspace, CLIWorkspace>(LifeStyle.Singleton);
         container.Register<IPKSimXmlSerializerRepository, CorePKSimXmlSerializerRepository>(LifeStyle.Singleton);
         container.Register<IWorkspacePersistor, CoreWorkspacePersistor>(LifeStyle.Singleton);
         container.Register<IObservedDataTask, CoreObservedDataTask>();
         container.Register<ISimulationChartsLoader, CLISimulationChartsLoader>();
      }
   }
}