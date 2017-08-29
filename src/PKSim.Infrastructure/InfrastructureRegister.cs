using Castle.Facilities.TypedFactory;
using Microsoft.Extensions.Logging;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Serialization;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Infrastructure.Container.Castle;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.Infrastructure.Serialization.ORM.History;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Infrastructure.Services;
using OSPSuite.Presentation.Serialization.Extensions;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Compression;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.FileLocker;
using OSPSuite.Utility.Logging;
using OSPSuite.Utility.Logging.TextWriterLogging;
using PKSim.Core;
using PKSim.Core.Reporting;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.Mappers;
using PKSim.Infrastructure.ORM.Repositories;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v5_3;
using PKSim.Infrastructure.ProjectConverter.v6_2;
using PKSim.Infrastructure.Reporting.Summary;
using PKSim.Infrastructure.Reporting.TeX.Builders;
using PKSim.Infrastructure.Reporting.TeX.Reporters;
using PKSim.Infrastructure.Serialization;
using PKSim.Infrastructure.Serialization.Xml;
using PKSim.Infrastructure.Serialization.Xml.Serializers;
using PKSim.Infrastructure.Services;
using PKSim.Presentation;
using SimModelNET;
using IContainer = OSPSuite.Utility.Container.IContainer;
using ILogger = OSPSuite.Core.Services.ILogger;
using IWorkspace = PKSim.Presentation.Core.IWorkspace;

namespace PKSim.Infrastructure
{
   public class InfrastructureRegister : Register
   {
      public static void Initialize()
      {
         var container = initializeContainer();

         registerFactoryIn(container);

         registerConfigurationIn(container);

         container.Register<IEventPublisher, EventPublisher>(LifeStyle.Singleton);

         registerRunOptionsIn(container);

         registerLogging(container);

         EnvironmentHelper.ApplicationName = () => "pksim";
      }

      private static void registerLogging(IContainer container)
      {
         var loggerFactory = new LoggerFactory();
         container.RegisterImplementationOf((ILoggerFactory) loggerFactory);
         container.Register<ILogger, PKSimLogger>(LifeStyle.Singleton);
         //TODO REMOVE
         container.Register<ILogFactory, TextWriterLogFactory>();
      }

      private static void registerRunOptionsIn(IContainer container)
      {
         container.Register<StartOptions, IStartOptions, StartOptions>(LifeStyle.Singleton);
      }

      private static IContainer initializeContainer()
      {
         var container = new CastleWindsorContainer();
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
         CoreConstants.ProductDisplayName = configuration.ProductDisplayName;
      }

      private static void registerFactoryIn(IContainer container)
      {
         container.RegisterFactory<IProgressManager>();
         container.RegisterFactory<IHistoryManagerFactory>();
      }

      public static void RegisterSerializationDependencies(bool registerSimModelSchema = true)
      {
         var container = IoC.Container;

         container.Register<ISerializationManager, XmlSerializationManager>();
         container.Register<IStringSerializer, CompressedStringSerializer>(CoreConstants.Serialization.Compressed);
         container.Register<IPKSimXmlSerializerRepository, PKSimXmlSerializerRepository>(LifeStyle.Singleton);

         container.Register(typeof(IXmlReader<>), typeof(XmlReader<>));
         container.Register(typeof(IXmlWriter<>), typeof(XmlWriter<>));

         //load repository to trigger initialization
         container.Resolve<IPKSimXmlSerializerRepository>().PerformMapping();

         //register pk analyses values
         var pkParameterRepository = container.Resolve<IPKParameterRepository>();
         var pKParameterLoader = container.Resolve<IPKParameterRepositoryLoader>();
         var pkSimConfiguration = container.Resolve<IPKSimConfiguration>();
         pKParameterLoader.Load(pkParameterRepository, pkSimConfiguration.PKParametersFilePath);

         if (registerSimModelSchema)
            XMLSchemaCache.InitializeFromFile(pkSimConfiguration.SimModelSchemaFilePath);
      }

      public static void RegisterWorkspace()
      {
         RegisterWorkspace<Workspace>();
      }

      public static void RegisterWorkspace<TWorkspace>() where TWorkspace : IWorkspace
      {
         var container = IoC.Container;
         container.Register<IWorkspace, IWithWorkspaceLayout, OSPSuite.Core.IWorkspace, TWorkspace>(LifeStyle.Singleton);
      }

      private void registerORMDependencies()
      {
         var container = IoC.Container;
         container.Register<IDbGateway, SimpleDbGateway>(LifeStyle.Singleton);
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
            scan.ExcludeNamespaceContainingType<SimulationReporter>(); //tex reporter
            scan.ExcludeNamespaceContainingType<IndividualTeXBuilder>(); //tex builder
            scan.ExcludeNamespaceContainingType<PKSimXmlSerializerRepository>(); //Serializer

            scan.ExcludeType<CommandMetaDataRepository>();
            scan.ExcludeType<DefaultIndividualRetriever>();
            scan.ExcludeType<SessionManager>();
            scan.ExcludeType<TemplateDatabase>();
            scan.ExcludeType<ProteinExpressionDatabase>();
            scan.ExcludeType<ProteinExpressionQueries>();
            scan.ExcludeType<ModelDatabase>();
            scan.ExcludeType<VersionChecker>();
            scan.ExcludeType<Workspace>();
            scan.ExcludeType<PKSimLogger>();

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
            scan.IncludeType<ProteinExpressionDatabase>();
            scan.IncludeType<ProteinExpressionQueries>();
            scan.IncludeType<ModelDatabase>();

            scan.IncludeType<VersionChecker>();

            scan.RegisterAs(LifeStyle.Singleton);
            scan.WithConvention<OSPSuiteRegistrationConvention>();
         });


         registerConverters(container);

         registerReportBuilders(container);

         registerTexReporters(container);

         container.Register<ICompression, SharpLibCompression>();
         container.Register<IStringCompression, StringCompression>();
         container.Register<IProjectRetriever, PKSimProjectRetriever>();
         container.Register<IObservedDataConfiguration, ImportObservedDataTask>();
         container.Register<IFlatContainerIdToContainerMapperSpecification, FlatContainerIdToFormulationMapper>();
         container.Register<IFileLocker, FileLocker>(LifeStyle.Singleton);

         registerORMDependencies();

         var xmlRegister = new CoreSerializerRegister();
         container.AddRegister(x => x.FromInstance(xmlRegister));
         var sbsuiteSerializerRepository = container.Resolve<IOSPSuiteXmlSerializerRepository>();
         sbsuiteSerializerRepository.AddPresentationSerializers();
         xmlRegister.PerformMappingForSerializerIn(container);

         container.AddRegister(x => x.FromType<ReportingRegister>());
         container.AddRegister(x => x.FromType<OSPSuite.TeXReporting.ReportingRegister>());
         container.AddRegister(x => x.FromType<OSPSuite.Infrastructure.InfrastructureRegister>());

         //register factory also as IObjectBaseFactoryIBuildTrackerFactory
         var factory = container.Resolve<IPKSimObjectBaseFactory>() as IObjectBaseFactory;
         container.RegisterImplementationOf(factory);

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
            scan.ExcludeType<Converter52To531>();
            scan.ExcludeType<Converter612To621>();
            scan.WithConvention<PKSimRegistrationConvention>();
         });

         //required as singleton because of element caching
         container.Register<Converter52To531, IObjectConverter, Converter52To531>(LifeStyle.Singleton);
         container.Register<Converter612To621, IObjectConverter, Converter612To621>(LifeStyle.Singleton);
         container.Register<IObjectConverterFinder, ObjectConverterFinder>(LifeStyle.Singleton);
         container.Register<IProjectConverterLogger, ProjectConverterLogger>(LifeStyle.Singleton);
      }
   }
}