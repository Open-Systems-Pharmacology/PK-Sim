using System;
using System.Threading;
using FakeItEasy;
using PKSim.BatchTool;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using PKSim.Matlab;
using PKSim.Presentation;
using PKSim.Presentation.Core;
using PKSim.Presentation.Services;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Exceptions;

namespace PKSim.IntegrationTests
{
   [IntegrationTests]
   public abstract class ContextForIntegration<T> : ContextSpecification<T>
   {
      public override void GlobalContext()
      {
         if (IoC.Container != null) return;

         InfrastructureRegister.Initialize();

         //use only in tests
         var container = IoC.Container;
         using (container.OptimizeDependencyResolution())
         {
            var configuration = container.Resolve<IPKSimConfiguration>();
//            configuration.PKSimDb = DomainHelperForSpecs.DbFilePath;
//            configuration.DimensionFilePath = DomainHelperForSpecs.DimensionFilePath;
//            configuration.PKParametersFilePath = DomainHelperForSpecs.PKParametersFilePath;

            //need to register these serives for which the default implementation is in the UI
            container.RegisterImplementationOf(new SynchronizationContext());
            container.Register<IApplicationController, ApplicationController>(LifeStyle.Singleton);
            container.Register<IExceptionManager, ExceptionManagerForSpecs>(LifeStyle.Singleton);
            container.Register<IDisplayUnitRetriever, MatlabDisplayUnitRetriever>();
            container.Register<IOntogenyFactorsRetriever, OntogenyFactorsRetriever>();

            container.RegisterImplementationOf(A.Fake<IProgressUpdater>());
            container.RegisterImplementationOf(A.Fake<IDialogCreator>());
            container.RegisterImplementationOf(A.Fake<IHistoryManager>());
            container.RegisterImplementationOf(A.Fake<IWorkspace>());
            container.RegisterImplementationOf(A.Fake<OSPSuite.Core.IWorkspace>());
            container.RegisterImplementationOf(A.Fake<IHeavyWorkManager>());
            container.RegisterImplementationOf(A.Fake<IChartTemplatingTask>());
            container.RegisterImplementationOf(A.Fake<IPresentationSettingsTask>());
            container.RegisterImplementationOf(A.Fake<IJournalDiagramManagerFactory>());

            container.AddRegister(x =>
            {
               x.FromInstance(new CoreRegister());
               x.FromType<InfrastructureRegister>();
               x.FromType<PresenterRegister>();
               x.FromType<OSPSuite.Presentation.PresenterRegister>();
               x.FromType<BatchRegister>();
            });

            var userSettings = container.Resolve<IUserSettings>();
            userSettings.AbsTol = 1e-10;
            userSettings.RelTol = 1e-5;
            //this is use to create reports
            userSettings.NumberOfBins = CoreConstants.DEFAULT_NUMBER_OF_BINS;


            InfrastructureRegister.RegisterSerializationDependencies();
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