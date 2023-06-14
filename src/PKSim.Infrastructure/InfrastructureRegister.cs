using System;
using Castle.Facilities.TypedFactory;
using Microsoft.Extensions.Logging;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Serialization;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Infrastructure.Container.Castle;
using OSPSuite.Infrastructure.Export;
using OSPSuite.Infrastructure.Import;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.Infrastructure.Serialization;
using OSPSuite.Infrastructure.Serialization.ORM.History;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Infrastructure.Serialization.Services;
using OSPSuite.Presentation.Serialization.Extensions;
using OSPSuite.Presentation.Services;
using OSPSuite.TeXReporting;
using OSPSuite.Utility;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.FileLocker;
using PKSim.Core;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.Mappers;
using PKSim.Infrastructure.ORM.Repositories;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.Reporting.Markdown;
using PKSim.Infrastructure.Reporting.Markdown.Builders;
using PKSim.Infrastructure.Reporting.Summary;
using PKSim.Infrastructure.Reporting.TeX.Builders;
using PKSim.Infrastructure.Reporting.TeX.Reporters;
using PKSim.Infrastructure.Serialization;
using PKSim.Infrastructure.Serialization.Xml;
using PKSim.Infrastructure.Serialization.Xml.Serializers;
using PKSim.Infrastructure.Services;
using PKSim.Presentation;
using IContainer = OSPSuite.Utility.Container.IContainer;
using IWorkspace = PKSim.Presentation.IWorkspace;

namespace PKSim.Infrastructure
{
   public class InfrastructureRegister : Register
   {
      public static IContainer Initialize(bool registerContainerAsStatic = true)
      {
         var container = initializeContainer(registerContainerAsStatic);

         container.AddRegister(x => x.FromType<OSPSuite.Infrastructure.InfrastructureRegister>());

         registerFactoryIn(container);

         registerConfigurationIn(container);

         container.Register<IEventPublisher, EventPublisher>(LifeStyle.Singleton);
         container.Register<IDbGateway, SimpleDbGateway>(LifeStyle.Singleton);

         registerRunOptionsIn(container);

         EnvironmentHelper.ApplicationName = () => "pksim";
         return container;
      }
      private static void registerRunOptionsIn(IContainer container)
      {
         container.Register<StartOptions, IStartOptions, StartOptions>(LifeStyle.Singleton);
      }

      private static IContainer initializeContainer(bool registerContainerAsStatic)
      {
         var container = new CastleWindsorContainer();
         if (registerContainerAsStatic) 
            IoC.InitializeWith(container);

         container.WindsorContainer.AddFacility<EventRegisterFacility>();
         container.WindsorContainer.AddFacility<SerializationFacility>();

         //required to used abstract factory pattern with container
         container.WindsorContainer.AddFacility<TypedFactoryFacility>();

         //Register container into container to avoid any reference to dependency in code
         container.RegisterImplementationOf(container.DowncastTo<IContainer>());
         return container;
      }

      private static void registerConfigurationIn(IContainer container)
      {
         container.Register<IPKSimConfiguration, IApplicationConfiguration, PKSimConfiguration>(LifeStyle.Singleton);

         var configuration = container.Resolve<IPKSimConfiguration>();
         CoreConstants.ProductDisplayName = configuration.ProductDisplayName; }

      private static void registerFactoryIn(IContainer container)
      {
         container.RegisterFactory<IProgressManager>();
         container.RegisterFactory<IHistoryManagerFactory>();
      }

      public static void LoadSerializers(IContainer container)
      {
         RegisterSerializationDependencies(container);
         LoadDefaultEntities(container);
      }

      public static void RegisterSerializationDependencies(IContainer container)
      {
         container.Register<ISerializationManager, XmlSerializationManager>();
         container.Register<IStringSerializer, StringSerializer>();
         container.Register<IStringSerializer, CompressedStringSerializer>(CoreConstants.Serialization.Compressed);
         container.Register<IPKSimXmlSerializerRepository, PKSimXmlSerializerRepository>(LifeStyle.Singleton);

         container.Register(typeof(IXmlReader<>), typeof(XmlReader<>));
         container.Register(typeof(IXmlWriter<>), typeof(XmlWriter<>));

         //Core serialization to serialize to pkml compatible with core
         container.AddRegister(x => x.FromType<CoreSerializerRegister>());
      }

      public static void LoadDefaultEntities(IContainer container)
      {
         //load unit serializer
         container.Resolve<IUnitSystemXmlSerializerRepository>().PerformMapping();
         //Then trigger dimension load to load the dimensions
         loadDimensionRepository(container);

         //Load pkml serializers
         var ospSuiteXmlSerializerRepository = container.Resolve<IOSPSuiteXmlSerializerRepository>();
         ospSuiteXmlSerializerRepository.AddPresentationSerializers();
         ospSuiteXmlSerializerRepository.PerformMapping();
         //then load pk parameters
         loadPKParameters(container);

         //last load all serializer
         container.Resolve<IPKSimXmlSerializerRepository>().PerformMapping();
      }

      private static void loadDimensionRepository(IContainer container)
      {
         //Loading the dimension will trigger the load
         container.Resolve<IDimensionRepository>().All();
      }

      private static void loadPKParameters(IContainer container)
      {
         //register pk analyses values
         var pkParameterRepository = container.Resolve<IPKParameterRepository>();
         var pKParameterLoader = container.Resolve<IPKParameterRepositoryLoader>();
         var pkSimConfiguration = container.Resolve<IPKSimConfiguration>();
         pKParameterLoader.Load(pkParameterRepository, pkSimConfiguration.PKParametersFilePath);
      }

      public static void RegisterWorkspace(IContainer container)
      {
         container.Register<IWorkspace, IWithWorkspaceLayout, ICoreWorkspace, OSPSuite.Core.IWorkspace, Workspace>(LifeStyle.Singleton);
      }

      private void registerORMDependencies(IContainer container)
      {
         container.Register(typeof(IDataTableToMetaDataMapper<>), typeof(DataTableToMetaDataMapper<>));
      }

      public override void RegisterInContainer(IContainer container)
      {
         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<InfrastructureRegister>();

            //ORM Repository should be registered as singleton
            scan.ExcludeNamespaceContainingType<SpeciesRepository>();

            //this type will be registered using another convention
            scan.ExcludeNamespaceContainingType<IObjectConverter>(); //Converter
            scan.ExcludeNamespaceContainingType<IReportBuilder>(); //report builder
            scan.ExcludeNamespaceContainingType<IMarkdownBuilder>(); //Markdown builder
            scan.ExcludeNamespaceContainingType<SimulationReporter>(); //tex reporter
            scan.ExcludeNamespaceContainingType<IndividualTeXBuilder>(); //tex builder
            scan.ExcludeNamespaceContainingType<PKSimXmlSerializerRepository>(); //Serializer

            scan.ExcludeType<CommandMetaDataRepository>();
            scan.ExcludeType<DefaultIndividualRetriever>();
            scan.ExcludeType<SessionManager>();
            scan.ExcludeType<TemplateDatabase>();
            scan.ExcludeType<GeneExpressionDatabase>();
            scan.ExcludeType<GeneExpressionQueries>();
            scan.ExcludeType<ModelDatabase>();
            scan.ExcludeType<VersionChecker>();
            scan.ExcludeType<Workspace>();
            scan.ExcludeType<StringSerializer>();
            scan.ExcludeType<CompressedStringSerializer>();

            //already registered
            scan.ExcludeType<PKSimXmlSerializerRepository>();
            scan.ExcludeType<PKSimConfiguration>();
            scan.ExcludeType<SerializationContext>();

            scan.WithConvention<PKSimRegistrationConvention>();
         });

         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<InfrastructureRegister>();
            scan.IncludeNamespaceContainingType<SpeciesRepository>(); //Repository

            scan.IncludeType<CommandMetaDataRepository>();
            scan.IncludeType<DefaultIndividualRetriever>();
            scan.IncludeType<SessionManager>();
            scan.IncludeType<TemplateDatabase>();
            scan.IncludeType<GeneExpressionDatabase>();
            scan.IncludeType<GeneExpressionQueries>();
            scan.IncludeType<ModelDatabase>();

            scan.IncludeType<VersionChecker>();

            scan.RegisterAs(LifeStyle.Singleton);
            scan.WithConvention<OSPSuiteRegistrationConvention>();
         });


         registerConverters(container);

         registerReportBuilders(container);

         registerTexReporters(container);

         registerMarkdownBuilders(container);

         container.Register<IProjectRetriever, PKSimProjectRetriever>();
         container.Register<IObservedDataConfiguration, ImportObservedDataTask>();
         container.Register<IFlatContainerIdToContainerMapperSpecification, FlatContainerIdToFormulationMapper>();
         container.Register<IFileLocker, FileLocker>(LifeStyle.Singleton);

         registerORMDependencies(container);

         container.AddRegister(x => x.FromType<ReportingRegister>());
         container.AddRegister(x => x.FromType<InfrastructureSerializationRegister>());
         container.AddRegister(x => x.FromType<InfrastructureReportingRegister>());
         container.AddRegister(x => x.FromType<InfrastructureExportRegister>());
         container.AddRegister(x => x.FromType<InfrastructureImportRegister>());

         var configuration = container.Resolve<IPKSimConfiguration>();
         var versionChecker = container.Resolve<IVersionChecker>();
         versionChecker.VersionFileUrl = CoreConstants.VERSION_FILE_URL;
         versionChecker.CurrentVersion = configuration.Version;
         versionChecker.ProductName = CoreConstants.PRODUCT_NAME;
      }

      private static void registerReportBuilders(IContainer container)
      {
         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<InfrastructureRegister>();
            scan.IncludeNamespaceContainingType<IReportBuilder>();
            scan.ExcludeType<ReportGenerator>();
            scan.ExcludeType<ReportBuilderRepository>();
            scan.WithConvention<RegisterTypeConvention<IReportBuilder>>();
         });
         container.Register<IReportGenerator, ReportGenerator>(LifeStyle.Singleton);
         container.Register<IReportBuilderRepository, ReportBuilderRepository>(LifeStyle.Singleton);
      }

      private static void registerMarkdownBuilders(IContainer container)
      {
         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<InfrastructureRegister>();
            scan.IncludeNamespaceContainingType<IMarkdownBuilder>();
            scan.ExcludeType<MarkdownBuilderRepository>();
            scan.WithConvention<RegisterTypeConvention<IMarkdownBuilder>>();
         });
         container.Register<IMarkdownBuilderRepository, MarkdownBuilderRepository>(LifeStyle.Singleton);
      }

      private static void registerTexReporters(IContainer container)
      {
         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<InfrastructureRegister>();
            scan.IncludeNamespaceContainingType<IndividualTeXBuilder>();
            scan.IncludeNamespaceContainingType<SimulationReporter>();
            scan.WithConvention<ReporterRegistrationConvention>();
         });
      }

      private static void registerConverters(IContainer container)
      {
         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<InfrastructureRegister>();
            scan.IncludeNamespaceContainingType<IObjectConverter>();
            scan.ExcludeType<ProjectConverterLogger>();
            scan.ExcludeType<ObjectConverterFinder>();
            scan.WithConvention<PKSimRegistrationConvention>();
         });

         //required as singleton because of element caching
         container.Register<IObjectConverterFinder, ObjectConverterFinder>(LifeStyle.Singleton);
         container.Register<IProjectConverterLogger, ProjectConverterLogger>(LifeStyle.Singleton);
      }
   }
}