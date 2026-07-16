using OSPSuite.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Format;
using PKSim.CLI.Core.MinimalImplementations;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using PKSim.Infrastructure.Serialization;
using PKSim.Infrastructure.Serialization.Xml.Serializers;
using PKSim.Infrastructure.Services;
using PKSim.Presentation;
using System;
using System.Threading;
using OSPSuite.Presentation;

namespace PKSim.Starter;

public class CLIApplicationStartup : ApplicationStartup
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

         pkSimContainer.RegisterImplementationOf(NumericFormatterOptions.Instance);
         pkSimContainer.Register<IApplicationController, ApplicationController>(LifeStyle.Singleton);

         // Full registration of PKSim Core and Infrastructure
         pkSimContainer.AddRegister(x => x.FromType<CoreRegister>());
         pkSimContainer.AddRegister(x => x.FromType<InfrastructureRegister>());
         pkSimContainer.AddRegister(x => x.FromType<CLI.Core.CLIRegister>());

         pkSimContainer.Register<IInteractiveSimulationRunner, InteractiveSimulationRunner>(LifeStyle.Singleton);
         pkSimContainer.Register<IPKSimXmlSerializerRepository, CorePKSimXmlSerializerRepository>(LifeStyle.Singleton);
         InfrastructureRegister.RegisterSerializationDependencies(pkSimContainer);
         PKSim.Presentation.Infrastructure.PresentationSerializerInitializer.AddPresentationSerializers(pkSimContainer);
         InfrastructureRegister.LoadDefaultEntities(pkSimContainer);
         pkSimContainer.Register<IExceptionManager, ExceptionManager>(LifeStyle.Singleton);

         pkSimContainer.Register<IMRUProvider, MRUProvider>();

         registerCLITypes(pkSimContainer);
      }

      return pkSimContainer;
   }

   private static void registerCLITypes(IContainer container)
   {
      container.Register<IProgressUpdater, NoneProgressUpdater>();
      container.Register<IOntogenyTask, CLIIndividualOntogenyTask>();
      container.Register<IHistoryManager, HistoryManager<IExecutionContext>>();
      container.Register<ICoreUserSettings, OSPSuite.Core.ICoreUserSettings, IPresentationUserSettings, CLIStarterUserSettings>(LifeStyle.Singleton);
      container.Register<ICoreWorkspace, IWorkspace, CLIWorkspace>(LifeStyle.Singleton);
      container.Register<IWorkspacePersistor, CoreWorkspacePersistor>(LifeStyle.Singleton);
      container.Register<IObservedDataTask, CoreObservedDataTask>();
      container.Register<ISimulationChartsLoader, CLISimulationChartsLoader>();
   }
}