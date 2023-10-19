using System;
using System.Threading;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Services;
using OSPSuite.Infrastructure.Import.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Exceptions;
using PKSim.BatchTool;
using PKSim.CLI.Core;
using PKSim.CLI.Core.MinimalImplementations;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using PKSim.Matlab;
using PKSim.Presentation;
using PKSim.Presentation.Services;

namespace PKSim.IntegrationTests
{
   [IntegrationTests]
   public abstract class ContextForIntegration<T> : ContextSpecification<T>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         if (IoC.Container != null) return;

         var container = InfrastructureRegister.Initialize();

         //use only in tests
         using (container.OptimizeDependencyResolution())
         {
            //need to register these series for which the default implementation is in the UI
            container.RegisterImplementationOf(new SynchronizationContext());
            container.Register<IApplicationController, ApplicationController>(LifeStyle.Singleton);
            container.Register<IExceptionManager, ExceptionManagerForSpecs>(LifeStyle.Singleton);
            container.Register<IDisplayUnitRetriever, CLIDisplayUnitRetriever>();
            container.Register<IOntogenyFactorsRetriever, OntogenyFactorsRetriever>();
            container.Register<ISimulationConstructor, SimulationConstructor>();
            container.RegisterImplementationOf(A.Fake<IProgressUpdater>());
            container.RegisterImplementationOf(A.Fake<IDialogCreator>());
            container.RegisterImplementationOf(A.Fake<IHistoryManager>());
            container.RegisterImplementationOf(A.Fake<IHeavyWorkManager>());
            container.RegisterImplementationOf(A.Fake<IChartTemplatingTask>());
            container.RegisterImplementationOf(A.Fake<IPresentationSettingsTask>());
            container.RegisterImplementationOf(A.Fake<IJournalDiagramManagerFactory>());
            container.RegisterImplementationOf(A.Fake<IDataImporter>());


            container.AddRegister(x =>
            {
               x.FromType<CoreRegister>();
               x.FromType<CLIRegister>();
               x.FromType<InfrastructureRegister>();
               x.FromType<PresenterRegister>();
               x.FromType<OSPSuite.Presentation.PresenterRegister>();
               x.FromType<BatchRegister>();
            });

            //Register an other type that was already registered previously to ensure that we do use the presentation implementation
            container.Register<IEntityValidationTask, CLIEntityValidationTask>();


            var userSettings = container.Resolve<IUserSettings>();
            userSettings.AbsTol = 1e-10;
            userSettings.RelTol = 1e-5;
            //this is use to create reports
            userSettings.NumberOfBins = CoreConstants.DEFAULT_NUMBER_OF_BINS;


            InfrastructureRegister.LoadSerializers(container);
            InfrastructureRegister.RegisterWorkspace(container);
         }

         //Required for usage with nunit 3
         Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
      }

      protected override void Context()
      {
         sut = IoC.Resolve<T>();
      }

      protected void Unregister(IWithId objectWithId)
      {
         if (objectWithId == null)
            return;

         var registrationTask = IoC.Resolve<IRegistrationTask>();
         registrationTask.Unregister(objectWithId);
      }
   }
}