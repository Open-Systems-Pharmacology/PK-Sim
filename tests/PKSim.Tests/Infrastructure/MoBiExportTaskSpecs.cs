using System;
using System.Linq;
using FakeItEasy;
using FluentNHibernate.Utils;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Serialization.Exchange;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Core.Snapshots;
using PKSim.Infrastructure.Services;
using DataRepository = OSPSuite.Core.Domain.Data.DataRepository;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;
using Simulation = PKSim.Core.Model.Simulation;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_MoBiExportTask : ContextSpecification<MoBiExportTask>
   {
      protected ISimulationConfigurationTask _simulationConfigurationTask;
      protected ISimulationToModelCoreSimulationMapper _simulationMapper;
      protected IRepresentationInfoRepository _representationInfoRepository;
      protected IPKSimConfiguration _configuration;
      protected ILazyLoadTask _lazyLoadTask;
      protected IDialogCreator _dialogCreator;
      protected ISimulationPersistor _simulationPersistor;
      protected IProjectRetriever _projectRetriever;
      protected IObjectIdResetter _objectIdResetter;
      private IJournalRetriever _journalRetriever;
      protected IApplicationSettings _applicationSettings;
      protected IStartableProcessFactory _startableProcessFactory;
      protected IModelCoreSimulationSnapshotUpdater _modelCoreSimulationSnapshotUpdater;
      protected Simulation _sim;
      private IModelCoreSimulation _modelCoreSimulation;

      protected override void Context()
      {
         _simulationConfigurationTask = A.Fake<ISimulationConfigurationTask>();
         _simulationMapper = A.Fake<ISimulationToModelCoreSimulationMapper>();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         _configuration = A.Fake<IPKSimConfiguration>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _simulationPersistor = A.Fake<ISimulationPersistor>();
         _projectRetriever = A.Fake<IProjectRetriever>();
         _objectIdResetter = A.Fake<IObjectIdResetter>();
         _journalRetriever = A.Fake<IJournalRetriever>();
         _applicationSettings = A.Fake<IApplicationSettings>();
         _startableProcessFactory = A.Fake<IStartableProcessFactory>();
         _modelCoreSimulationSnapshotUpdater = A.Fake<IModelCoreSimulationSnapshotUpdater>();
         _sim = A.Fake<Simulation>();

         _modelCoreSimulation = new ModelCoreSimulation
         {
            Configuration = new SimulationConfiguration(),
            Model = new Model
            {
               Root = new Container()
            }
         };
         _modelCoreSimulation.Configuration.AddModuleConfiguration(new ModuleConfiguration(new Module()));
         _modelCoreSimulation.Configuration.Individual = [];

         A.CallTo(() => _simulationMapper.MapFrom(_sim, A<SimulationConfiguration>._, true)).Returns(_modelCoreSimulation);

         sut = new MoBiExportTask(_simulationConfigurationTask, _simulationMapper, _representationInfoRepository,
            _configuration, _lazyLoadTask, _dialogCreator, _simulationPersistor, _projectRetriever, _objectIdResetter, _journalRetriever, _applicationSettings, _startableProcessFactory, _modelCoreSimulationSnapshotUpdater);
      }
   }

   public class When_exporting_a_simulation_to_pkml_file_that_was_imported_from_pkml : concern_for_MoBiExportTask
   {
      
      private string _fileName;

      protected override void Context()
      {
         base.Context();
         _fileName = "toto";
         A.CallTo(() => _sim.IsImported).Returns(true);
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.ExportSimulationToPkmlFile(_sim, _fileName)).ShouldThrowAn<PKSimException>();
      }
   }

   public class When_exporting_a_simulation_to_pkml_file : concern_for_MoBiExportTask
   {
      private string _fileName;
      private SimulationTransfer _simulationTransfer;
      private DataRepository _observedData1;

      protected override void Context()
      {
         base.Context();
         _fileName = "toto";
         _observedData1 = new DataRepository();
         A.CallTo(() => _sim.UsedObservedData).Returns(new[] {new UsedObservedData {Id = "OBS"}});
         A.CallTo(() => _sim.IsImported).Returns(false);
         A.CallTo(() => _simulationPersistor.Save(A<SimulationTransfer>._, _fileName))
            .Invokes(x => _simulationTransfer = x.GetArgument<SimulationTransfer>(0));

         A.CallTo(() => _projectRetriever.CurrentProject.Favorites).Returns(new Favorites {"FAV1"});
         A.CallTo(() => _projectRetriever.CurrentProject.ObservedDataBy("OBS")).Returns(_observedData1);
      }

      protected override void Because()
      {
         sut.ExportSimulationToPkmlFile(_sim, _fileName);
      }

      [Observation]
      public void should_export_the_observed_data_defined_in_the_current_project()
      {
         _simulationTransfer.AllObservedData.ShouldOnlyContain(_observedData1);
      }

      [Observation]
      public void should_export_the_favorites_data_defined_in_the_current_project()
      {
         _simulationTransfer.Favorites.ShouldOnlyContain("FAV1");
      }

      [Observation]
      public void snapshots_should_be_set_by_the_snapshot_task()
      {
         A.CallTo(() => _modelCoreSimulationSnapshotUpdater.AddSnapshotsToModelCoreSimulation(A<Simulation>._, A<IModelCoreSimulation>._)).MustHaveHappened();
      }

      [Observation]
      public void should_have_reset_the_id_of_the_simulation()
      {
         A.CallTo(() => _objectIdResetter.ResetIdFor(_simulationTransfer.Simulation)).MustHaveHappened();
      }
   }

   public class When_exporting_a_simulation_to_MoBi_and_the_application_was_installed_using_the_setup : concern_for_MoBiExportTask
   {
      private readonly string _moBiConfigPath = "MoBiConfigPath";
      private Func<string, bool> _oldFileHelper;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _oldFileHelper = FileHelper.FileExists;
         FileHelper.FileExists = s => s == _moBiConfigPath;
      }

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _configuration.MoBiPath).Returns(_moBiConfigPath);
      }

      protected override void Because()
      {
         sut.StartWith(_sim);
      }

      [Observation]
      public void should_start_mobi_with_the_simulation_file()
      {
         A.CallTo(() => _startableProcessFactory.CreateStartableProcess(_moBiConfigPath, A<string[]>._)).MustHaveHappened();
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         FileHelper.FileExists = _oldFileHelper;
      }
   }

   public class When_exporting_a_simulation_to_MoBi_and_the_application_was_installed_using_a_portable_setup_and_mobi_executable_path_can_be_found_on_system_using_the_application_settings : concern_for_MoBiExportTask
   {
      private readonly string _moBiAppSettingsPath = "MoBiAppSettingsPath";
      private Func<string, bool> _oldFileHelper;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _oldFileHelper = FileHelper.FileExists;
         FileHelper.FileExists = s => s == _moBiAppSettingsPath;
      }

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _applicationSettings.MoBiPath).Returns(_moBiAppSettingsPath);
      }

      protected override void Because()
      {
         sut.StartWith(_sim);
      }

      [Observation]
      public void should_start_mobi_with_the_simulation_file()
      {
         A.CallTo(() => _startableProcessFactory.CreateStartableProcess(_moBiAppSettingsPath, A<string[]>._)).MustHaveHappened();
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         FileHelper.FileExists = _oldFileHelper;
      }
   }

   public class When_exporting_a_simulation_to_MoBi_and_the_application_was_installed_using_setup_and_mobi_executable_path_can_be_found_on_system_using_the_application_settings : concern_for_MoBiExportTask
   {
      private Simulation _simulation;
      private readonly string _moBiAppSettingsPath = "MoBiAppSettingsPath";
      private readonly string _moBiConfigPath = "MoBiConfigPath";
      private Func<string, bool> _oldFileHelper;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _oldFileHelper = FileHelper.FileExists;
         FileHelper.FileExists = s => s == _moBiAppSettingsPath || s== _moBiConfigPath;
      }

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
         //the two files are defined
         A.CallTo(() => _applicationSettings.MoBiPath).Returns(_moBiAppSettingsPath);
         A.CallTo(() => _configuration.MoBiPath).Returns(_moBiConfigPath);
      }

      protected override void Because()
      {
         sut.StartWith(_simulation);
      }

      [Observation]
      public void should_start_mobi_from_the_application_settings()
      {
         A.CallTo(() => _startableProcessFactory.CreateStartableProcess(_moBiAppSettingsPath, A<string[]>._)).MustHaveHappened();
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         FileHelper.FileExists = _oldFileHelper;
      }
   }

   public class When_exporting_a_simulation_to_MoBi_and_mobi_is_not_found_on_the_system : concern_for_MoBiExportTask
   {
      private Simulation _simulation;
      private Func<string, bool> _oldFileHelper;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _oldFileHelper = FileHelper.FileExists;
         FileHelper.FileExists = s => false;
      }

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
      }

      [Observation]
      public void should_thrown_an_exception()
      {
         The.Action(() => sut.StartWith(_simulation)).ShouldThrowAn<OSPSuiteException>();
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         FileHelper.FileExists = _oldFileHelper;
      }
   }
}