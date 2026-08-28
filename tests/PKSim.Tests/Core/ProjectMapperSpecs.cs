using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Core.Snapshots;
using OSPSuite.Core.Snapshots.Mappers;
using PKSim.Assets;
using PKSim.Core.Chart;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using Classification = OSPSuite.Core.Domain.Classification;
using Compound = PKSim.Core.Model.Compound;
using DataRepository = OSPSuite.Core.Domain.Data.DataRepository;
using Event = PKSim.Core.Snapshots.Event;
using ExpressionProfile = PKSim.Core.Model.ExpressionProfile;
using Formulation = PKSim.Core.Model.Formulation;
using Individual = PKSim.Core.Model.Individual;
using ObserverSet = PKSim.Core.Model.ObserverSet;
using ParameterIdentificationMapper = OSPSuite.Core.Snapshots.Mappers.ParameterIdentificationMapper;
using Population = PKSim.Core.Model.Population;
using Project = PKSim.Core.Snapshots.Project;
using Protocol = PKSim.Core.Model.Protocol;
using QualificationPlan = OSPSuite.Core.Domain.QualificationPlan;
using Simulation = PKSim.Core.Snapshots.Simulation;
using ModelSimulation = PKSim.Core.Model.Simulation;

namespace PKSim.Core
{
   public abstract class concern_for_ProjectMapper : ContextSpecificationAsync<ProjectMapper>
   {
      protected PKSimProject _project;
      protected Individual _individual;
      protected Compound _compound;
      protected PKSimEvent _event;
      protected Formulation _formulation;
      protected Protocol _protocol;
      protected IndividualSimulation _simulation;
      protected Project _snapshot;
      protected ISnapshotMapper _snapshotMapper;
      protected Snapshots.Compound _compoundSnapshot;
      protected Snapshots.Individual _individualSnapshot;
      protected IExecutionContext _executionContext;
      protected Event _eventSnapshot;
      protected Snapshots.Formulation _formulationSnapshot;
      protected Snapshots.Protocol _protocolSnapshot;
      protected Population _population;
      protected ObserverSet _observerSet;
      protected Snapshots.Population _populationSnapshot;
      protected DataRepository _observedData;
      protected OSPSuite.Core.Snapshots.DataRepository _observedDataSnapshot;
      protected SimulationMapper _simulationMapper;
      protected Simulation _simulationSnapshot;
      protected ClassificationMapper _classificationMapper;
      protected ClassifiableObservedData _classifiableObservedData;
      protected Classification _classification;
      protected OSPSuite.Core.Snapshots.Classification _observedDataClassificationSnapshot;
      protected IClassificationSnapshotTask _classificationSnapshotTask;
      protected SimulationComparison _simulationComparisonSnapshot;
      protected ParameterIdentification _parameterIdentificationSnapshot;
      protected ISimulationComparison _simulationComparison;
      protected SimulationComparisonMapper _simulationComparisonMapper;
      protected OSPSuite.Core.Snapshots.Classification _simulationClassificationSnapshot;
      protected OSPSuite.Core.Snapshots.Classification _comparisonClassificationSnapshot;
      protected OSPSuite.Core.Snapshots.Classification _parameterIdentificationClassificationSnapshot;
      protected OSPSuite.Core.Snapshots.Classification _qualificationPlanClassificationSnapshot;
      protected ILazyLoadTask _lazyLoadTask;
      protected ParameterIdentificationMapper _parameterIdentificationMapper;
      protected OSPSuite.Core.Domain.ParameterIdentifications.ParameterIdentification _parameterIdentification;
      protected QualificationPlanMapper _qualificationPlanMapper;
      protected QualificationPlan _qualificationPlan;
      protected Snapshots.QualificationPlan _qualificationPlanSnapshot;
      protected IOSPSuiteLogger _logger;
      protected ICreationMetaDataFactory _creationMetaDataFactory;
      protected Snapshots.ObserverSet _observerSetSnapshot;
      protected ExpressionProfile _expressionProfile;
      protected Snapshots.ExpressionProfile _expressionProfileSnapshot;
      protected ICoreUserSettings _userSettings;
      protected ISimulationRunner _simulationRunner;
      protected SimulationTimeProfileChartMapper _simulationTimeProfileChartMapper;
      protected PopulationAnalysisChartMapper _populationAnalysisChartMapper;
      protected IStartableWarmup _startableWarmup;


      protected override Task Context()
      {
         _classificationMapper = A.Fake<ClassificationMapper>();
         _snapshotMapper = A.Fake<ISnapshotMapper>();
         _executionContext = A.Fake<IExecutionContext>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _simulationMapper = A.Fake<SimulationMapper>();
         _simulationComparisonMapper = A.Fake<SimulationComparisonMapper>();
         _parameterIdentificationMapper = A.Fake<ParameterIdentificationMapper>();
         _classificationSnapshotTask = A.Fake<IClassificationSnapshotTask>();
         _qualificationPlanMapper = A.Fake<QualificationPlanMapper>();
         _creationMetaDataFactory = A.Fake<ICreationMetaDataFactory>();
         _logger = A.Fake<IOSPSuiteLogger>();
         _userSettings = A.Fake<ICoreUserSettings>();
         _simulationRunner = A.Fake<ISimulationRunner>();
         _simulationTimeProfileChartMapper = A.Fake<SimulationTimeProfileChartMapper>();
         _populationAnalysisChartMapper = A.Fake<PopulationAnalysisChartMapper>();
         _startableWarmup = A.Fake<IStartableWarmup>();
         A.CallTo(() => _startableWarmup.AwaitCompletion()).Returns(true);

         sut = new ProjectMapper(
            _simulationMapper,
            _simulationComparisonMapper,
            _parameterIdentificationMapper,
            _qualificationPlanMapper,
            _executionContext,
            _classificationSnapshotTask,
            _lazyLoadTask,
            _creationMetaDataFactory,
            _logger,
            _userSettings,
            _simulationRunner,
            _simulationTimeProfileChartMapper,
            _populationAnalysisChartMapper,
            _startableWarmup);


         A.CallTo(() => _executionContext.Resolve<ISnapshotMapper>()).Returns(_snapshotMapper);
         _individual = new Individual().WithName("IND");
         _compound = new Compound().WithName("COMP");
         _event = new PKSimEvent().WithName("EVENT");
         _formulation = new Formulation().WithName("FORM");
         _protocol = new SimpleProtocol().WithName("PROTO");
         _population = new RandomPopulation().WithName("POP");
         _observerSet = new ObserverSet().WithName("OBS_SET");
         _observedData = new DataRepository().WithName("OD");
         _expressionProfile = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>();
         _parameterIdentification = new OSPSuite.Core.Domain.ParameterIdentifications.ParameterIdentification().WithName("PI").WithId("PI_ID");
         _classifiableObservedData = new ClassifiableObservedData { Subject = _observedData };
         _classification = new Classification { ClassificationType = ClassificationType.ObservedData }.WithName("OD Classification");
         _simulationComparison = new IndividualSimulationComparison().WithName("COMP").WithId("SimComp");
         _simulation = new IndividualSimulation().WithName("IND_SIM").WithId("IndSim");
         _qualificationPlan = new QualificationPlan().WithName("QP").WithId("QP_ID");
         _project = new PKSimProject();
         _project.AddBuildingBlock(_individual);
         _project.AddBuildingBlock(_compound);
         _project.AddBuildingBlock(_event);
         _project.AddBuildingBlock(_formulation);
         _project.AddBuildingBlock(_protocol);
         _project.AddBuildingBlock(_population);
         _project.AddBuildingBlock(_observerSet);
         _project.AddObservedData(_observedData);
         _project.AddBuildingBlock(_simulation);
         _project.AddBuildingBlock(_expressionProfile);
         _project.AddClassifiable(_classifiableObservedData);
         _project.AddClassification(_classification);
         _project.AddSimulationComparison(_simulationComparison);
         _project.AddParameterIdentification(_parameterIdentification);
         _project.AddQualificationPlan(_qualificationPlan);

         _compoundSnapshot = new Snapshots.Compound();
         _individualSnapshot = new Snapshots.Individual();
         _eventSnapshot = new Event();
         _observerSetSnapshot = new Snapshots.ObserverSet();
         _formulationSnapshot = new Snapshots.Formulation();
         _protocolSnapshot = new Snapshots.Protocol();
         _populationSnapshot = new Snapshots.Population();
         _observedDataSnapshot = new OSPSuite.Core.Snapshots.DataRepository();
         _parameterIdentificationSnapshot = new ParameterIdentification();
         _observedDataClassificationSnapshot = new OSPSuite.Core.Snapshots.Classification();
         _simulationComparisonSnapshot = new SimulationComparison();
         _simulationClassificationSnapshot = new OSPSuite.Core.Snapshots.Classification();
         _comparisonClassificationSnapshot = new OSPSuite.Core.Snapshots.Classification();
         _parameterIdentificationClassificationSnapshot = new OSPSuite.Core.Snapshots.Classification();
         _qualificationPlanClassificationSnapshot = new OSPSuite.Core.Snapshots.Classification();
         _qualificationPlanSnapshot = new Snapshots.QualificationPlan();
         _expressionProfileSnapshot = new Snapshots.ExpressionProfile();
         _simulationSnapshot = new Simulation();


         A.CallTo(() => _snapshotMapper.MapToSnapshot(_compound)).Returns(_compoundSnapshot);
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_individual)).Returns(_individualSnapshot);
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_event)).Returns(_eventSnapshot);
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_formulation)).Returns(_formulationSnapshot);
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_protocol)).Returns(_protocolSnapshot);
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_population)).Returns(_populationSnapshot);
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_observedData)).Returns(_observedDataSnapshot);
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_observerSet)).Returns(_observerSetSnapshot);
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_expressionProfile)).Returns(_expressionProfileSnapshot);
         A.CallTo(() => _simulationMapper.MapToSnapshot(_simulation, _project)).Returns(_simulationSnapshot);
         A.CallTo(() => _simulationComparisonMapper.MapToSnapshot(_simulationComparison)).Returns(_simulationComparisonSnapshot);
         A.CallTo(() => _parameterIdentificationMapper.MapToSnapshot(_parameterIdentification)).Returns(_parameterIdentificationSnapshot);
         A.CallTo(() => _qualificationPlanMapper.MapToSnapshot(_qualificationPlan)).Returns(_qualificationPlanSnapshot);

         A.CallTo(() => _classificationSnapshotTask.MapClassificationsToSnapshots<ClassifiableObservedData>(_project)).Returns(new[] { _observedDataClassificationSnapshot });
         A.CallTo(() => _classificationSnapshotTask.MapClassificationsToSnapshots<ClassifiableSimulation>(_project)).Returns(new[] { _simulationClassificationSnapshot });
         A.CallTo(() => _classificationSnapshotTask.MapClassificationsToSnapshots<ClassifiableComparison>(_project)).Returns(new[] { _comparisonClassificationSnapshot });
         A.CallTo(() => _classificationSnapshotTask.MapClassificationsToSnapshots<ClassifiableParameterIdentification>(_project)).Returns(new[] { _parameterIdentificationClassificationSnapshot });
         A.CallTo(() => _classificationSnapshotTask.MapClassificationsToSnapshots<ClassifiableQualificationPlan>(_project)).Returns(new[] { _qualificationPlanClassificationSnapshot });

         return _completed;
      }
   }

   public class When_exporting_a_project_to_snapshot : concern_for_ProjectMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_project);
      }

      [Observation]
      public void should_save_the_current_project_version()
      {
         _snapshot.Version.ShouldBeEqualTo(ProjectVersions.Current);
      }

      [Observation]
      public void should_identify_PKSim_as_the_application_that_created_the_snapshot()
      {
         _snapshot.ApplicationName.ShouldBeEqualTo("PK-Sim");
      }

      [Observation]
      public void should_retrieve_the_snapshot_for_all_underlying_models()
      {
         _snapshot.Compounds.ShouldContain(_compoundSnapshot);
         _snapshot.Individuals.ShouldContain(_individualSnapshot);
         _snapshot.Protocols.ShouldContain(_protocolSnapshot);
         _snapshot.Formulations.ShouldContain(_formulationSnapshot);
         _snapshot.Events.ShouldContain(_eventSnapshot);
         _snapshot.Populations.ShouldContain(_populationSnapshot);
         _snapshot.ObserverSets.ShouldContain(_observerSetSnapshot);
         _snapshot.ExpressionProfiles.ShouldContain(_expressionProfileSnapshot);
      }

      [Observation]
      public void should_save_observed_data_classification()
      {
         _snapshot.ObservedDataClassifications.ShouldContain(_observedDataClassificationSnapshot);
      }

      [Observation]
      public void should_save_simulation_classification()
      {
         _snapshot.SimulationClassifications.ShouldContain(_simulationClassificationSnapshot);
      }

      [Observation]
      public void should_save_parameter_identification_classification()
      {
         _snapshot.ParameterIdentificationClassifications.ShouldContain(_parameterIdentificationClassificationSnapshot);
      }

      [Observation]
      public void should_save_simulation_comparison_classification()
      {
         _snapshot.SimulationComparisonClassifications.ShouldContain(_comparisonClassificationSnapshot);
      }

      [Observation]
      public void should_save_qualification_plan_classification()
      {
         _snapshot.QualificationPlanClassifications.ShouldContain(_qualificationPlanClassificationSnapshot);
      }

      [Observation]
      public void should_retrieve_the_snapshot_for_all_simulations_used_in_the_project()
      {
         _snapshot.Simulations.ShouldContain(_simulationSnapshot);
      }

      [Observation]
      public void should_retrieve_the_snapshot_for_all_comparison_used_in_the_project()
      {
         _snapshot.SimulationComparisons.ShouldContain(_simulationComparisonSnapshot);
      }

      [Observation]
      public void should_retrieve_the_snapshot_for_all_parameter_identification_used_in_the_project()
      {
         _snapshot.ParameterIdentifications.ShouldContain(_parameterIdentificationSnapshot);
      }

      [Observation]
      public void should_retrieve_the_snapshot_for_all_qualification_plan_used_in_the_project()
      {
         _snapshot.QualificationPlans.ShouldContain(_qualificationPlanSnapshot);
      }

      [Observation]
      public void should_load_the_exported_building_blocks()
      {
         A.CallTo(() => _lazyLoadTask.Load((IPKSimBuildingBlock)_compound)).MustHaveHappened();
         A.CallTo(() => _lazyLoadTask.Load((IPKSimBuildingBlock)_formulation)).MustHaveHappened();
         A.CallTo(() => _lazyLoadTask.Load((IPKSimBuildingBlock)_event)).MustHaveHappened();
         A.CallTo(() => _lazyLoadTask.Load((IPKSimBuildingBlock)_individual)).MustHaveHappened();
         A.CallTo(() => _lazyLoadTask.Load((IPKSimBuildingBlock)_population)).MustHaveHappened();
         A.CallTo(() => _lazyLoadTask.Load((IPKSimBuildingBlock)_observerSet)).MustHaveHappened();
         A.CallTo(() => _lazyLoadTask.Load((IPKSimBuildingBlock)_protocol)).MustHaveHappened();
         A.CallTo(() => _lazyLoadTask.Load((IPKSimBuildingBlock)_expressionProfile)).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_simulation_comparison_results()
      {
         A.CallTo(() => _lazyLoadTask.Load((ILazyLoadable)_simulationComparison)).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_parameter_identification_results()
      {
         A.CallTo(() => _lazyLoadTask.Load((ILazyLoadable)_parameterIdentification)).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_exported_simulation_results()
      {
         A.CallTo(() => _lazyLoadTask.Load((ILazyLoadable)_simulation)).MustHaveHappened();
         A.CallTo(() => _lazyLoadTask.LoadResults((Model.Simulation)_simulation)).MustHaveHappened();
      }
   }

   public class When_converting_a_project_snapshot_to_project : concern_for_ProjectMapper
   {
      protected PKSimProject _newProject;
      private Simulation _corruptedSimulationSnapshot;
      private CreationMetaData _creationMetaData;
      private ISnapshotMapper _defaultMapper;

      protected override async Task Context()
      {
         await base.Context();
         _creationMetaData = new CreationMetaData();
         A.CallTo(() => _creationMetaDataFactory.Create()).Returns(_creationMetaData);
         _snapshot = await sut.MapToSnapshot(_project);
         _snapshot.Version = ProjectVersions.V7_1_0;
         _corruptedSimulationSnapshot = new Simulation();
         _snapshot.Simulations = new[] { _snapshot.Simulations[0], _corruptedSimulationSnapshot, };
         _defaultMapper = A.Fake<ISnapshotMapper>();
         A.CallTo(() => _snapshotMapper.MapperFor(_individualSnapshot)).Returns(_defaultMapper);
         A.CallTo(() => _defaultMapper.MapToModel(_individualSnapshot, A<SnapshotContext>._)).Returns(_individual);

         A.CallTo(() => _snapshotMapper.MapperFor(_expressionProfileSnapshot)).Returns(_defaultMapper);
         A.CallTo(() => _defaultMapper.MapToModel(_expressionProfileSnapshot, A<SnapshotContext>._)).Returns(_expressionProfile);

         A.CallTo(() => _snapshotMapper.MapperFor(_compoundSnapshot)).Returns(_defaultMapper);
         A.CallTo(() => _defaultMapper.MapToModel(_compoundSnapshot, A<SnapshotContext>._)).Returns(_compound);

         A.CallTo(() => _snapshotMapper.MapperFor(_protocolSnapshot)).Returns(_defaultMapper);
         A.CallTo(() => _defaultMapper.MapToModel(_protocolSnapshot, A<SnapshotContext>._)).Returns(_protocol);

         A.CallTo(() => _snapshotMapper.MapperFor(_formulationSnapshot)).Returns(_defaultMapper);
         A.CallTo(() => _defaultMapper.MapToModel(_formulationSnapshot, A<SnapshotContext>._)).Returns(_formulation);

         A.CallTo(() => _snapshotMapper.MapperFor(_eventSnapshot)).Returns(_defaultMapper);
         A.CallTo(() => _defaultMapper.MapToModel(_eventSnapshot, A<SnapshotContext>._)).Returns(_event);

         A.CallTo(() => _snapshotMapper.MapperFor(_populationSnapshot)).Returns(_defaultMapper);
         A.CallTo(() => _defaultMapper.MapToModel(_populationSnapshot, A<SnapshotContext>._)).Returns(_population);

         A.CallTo(() => _snapshotMapper.MapperFor(_observerSetSnapshot)).Returns(_defaultMapper);
         A.CallTo(() => _defaultMapper.MapToModel(_observerSetSnapshot, A<SnapshotContext>._)).Returns(_observerSet);

         A.CallTo(() => _snapshotMapper.MapToModel(_observedDataSnapshot, A<SnapshotContext>._)).Returns(_observedData);
         A.CallTo(() => _simulationMapper.MapToModel(_simulationSnapshot, A<SimulationContext>._)).Returns(_simulation);
         A.CallTo(() => _simulationMapper.MapToModel(_corruptedSimulationSnapshot, A<SimulationContext>._)).Throws(new Exception());
         A.CallTo(() => _simulationComparisonMapper.MapToModel(_simulationComparisonSnapshot, A<SnapshotContext>._)).Returns(_simulationComparison);
         A.CallTo(() => _parameterIdentificationMapper.MapToModel(_parameterIdentificationSnapshot, A<SnapshotContext>._)).Returns(_parameterIdentification);
         A.CallTo(() => _qualificationPlanMapper.MapToModel(_qualificationPlanSnapshot, A<SnapshotContext>._)).Returns(_qualificationPlan);
      }

      protected override async Task Because()
      {
         _newProject = await sut.MapToModel(_snapshot, new ProjectContext(new PKSimProject(), runSimulations: true));
      }

      [Observation]
      public void should_return_a_project_with_the_expected_building_blocks()
      {
         _newProject.All<Compound>().ShouldContain(_compound);
         _newProject.All<PKSimEvent>().ShouldContain(_event);
         _newProject.All<Formulation>().ShouldContain(_formulation);
         _newProject.All<Protocol>().ShouldContain(_protocol);
         _newProject.All<Population>().ShouldContain(_population);
         _newProject.All<ObserverSet>().ShouldContain(_observerSet);
         _newProject.All<ExpressionProfile>().ShouldContain(_expressionProfile);
         _newProject.All<Individual>().ShouldContain(_individual);
      }

      [Observation]
      public void should_have_created_a_new_meta_data_updating_the_version_to_the_internal_version()
      {
         _newProject.Creation.ShouldBeEqualTo(_creationMetaData);
         _newProject.Creation.Version.ShouldBeEqualTo(ProjectVersions.V7_1_0.VersionDisplay);
         _newProject.Creation.InternalVersion.ShouldBeEqualTo(_snapshot.Version);
      }

      [Observation]
      public void should_have_mapped_the_observed_data()
      {
         _newProject.AllObservedData.ShouldContain(_observedData);
      }

      [Observation]
      public void should_have_mapped_the_simulation_comparison()
      {
         _newProject.AllSimulationComparisons.ShouldContain(_simulationComparison);
      }

      [Observation]
      public void should_have_mapped_the_simulations()
      {
         _newProject.All<Model.Simulation>().ShouldContain(_simulation);
      }

      [Observation]
      public void should_have_mapped_the_parameter_identification()
      {
         _newProject.AllParameterIdentifications.ShouldContain(_parameterIdentification);
      }

      [Observation]
      public void should_have_mapped_the_qualification_plan()
      {
         _newProject.AllQualificationPlans.ShouldContain(_qualificationPlan);
      }

      [Observation]
      public void should_update_project_classification_for_observed_data()
      {
         A.CallTo(() => _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableObservedData, DataRepository>(_snapshot.ObservedDataClassifications, A<SnapshotContext>._, _newProject.AllObservedData)).MustHaveHappened();
      }

      [Observation]
      public void should_update_project_classification_for_simulation()
      {
         A.CallTo(() => _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableSimulation, Model.Simulation>(_snapshot.SimulationClassifications, A<SnapshotContext>._, A<IReadOnlyCollection<Model.Simulation>>._)).MustHaveHappened();
      }

      [Observation]
      public void should_update_project_classification_for_simulation_comparison()
      {
         A.CallTo(() => _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableComparison, ISimulationComparison>(_snapshot.SimulationComparisonClassifications, A<SnapshotContext>._, _newProject.AllSimulationComparisons)).MustHaveHappened();
      }

      [Observation]
      public void should_update_project_classification_for_qualification_plan()
      {
         A.CallTo(() => _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableQualificationPlan, QualificationPlan>(_snapshot.QualificationPlanClassifications, A<SnapshotContext>._, _newProject.AllQualificationPlans)).MustHaveHappened();
      }

      [Observation]
      public void should_update_project_classification_for_parameter_identification()
      {
         A.CallTo(() =>
               _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableParameterIdentification, OSPSuite.Core.Domain.ParameterIdentifications.ParameterIdentification>(_snapshot.ParameterIdentificationClassifications, A<SnapshotContext>._, _newProject.AllParameterIdentifications))
            .MustHaveHappened();
      }

      [Observation]
      public void should_log_an_error_for_simulation_that_could_not_be_loaded_from_snapshot()
      {
         A.CallTo(() => _logger.AddToLog(A<string>._, LogLevel.Error, A<string>._)).MustHaveHappened();
      }
   }

   public class When_converting_a_project_snapshot_with_multiple_simulations_to_project : concern_for_ProjectMapper
   {
      private PKSimProject _newProject;
      private Simulation _simulationSnapshot1;
      private Simulation _simulationSnapshot2;
      private Simulation _simulationSnapshot3;

      protected override async Task Context()
      {
         await base.Context();
         _simulationSnapshot1 = new Simulation {Name = "S1"};
         _simulationSnapshot2 = new Simulation {Name = "S2"};
         _simulationSnapshot3 = new Simulation {Name = "S3"};
         _snapshot = new Project
         {
            Simulations = new[] {_simulationSnapshot1, _simulationSnapshot2, _simulationSnapshot3}
         };

         //the parallel path must actually run for the ordering to be at stake
         A.CallTo(() => _userSettings.MaximumNumberOfCoresToUse).Returns(3);

         //the mappings complete out of order to verify that the simulations are nevertheless added in snapshot order
         A.CallTo(() => _simulationMapper.MapToModel(_simulationSnapshot1, A<SimulationContext>._)).ReturnsLazily(async () =>
         {
            await Task.Delay(100);
            return (Model.Simulation) new IndividualSimulation().WithName("S1");
         });
         A.CallTo(() => _simulationMapper.MapToModel(_simulationSnapshot2, A<SimulationContext>._)).ReturnsLazily(async () =>
         {
            await Task.Delay(50);
            return (Model.Simulation) new IndividualSimulation().WithName("S2");
         });
         A.CallTo(() => _simulationMapper.MapToModel(_simulationSnapshot3, A<SimulationContext>._)).Returns(new IndividualSimulation().WithName("S3"));
      }

      protected override async Task Because()
      {
         _newProject = await sut.MapToModel(_snapshot, new ProjectContext(new PKSimProject(), runSimulations: false));
      }

      [Observation]
      public void should_add_the_simulations_to_the_project_in_the_snapshot_order()
      {
         _newProject.All<Model.Simulation>().AllNames().ShouldOnlyContainInOrder("S1", "S2", "S3");
      }
   }

   public class When_converting_a_corrupted_project_snapshot_to_project : concern_for_ProjectMapper
   {
      private PKSimProject _newProject;
      private CreationMetaData _creationMetaData;
      private ISnapshotMapper _defaultMapper;

      protected override async Task Context()
      {
         await base.Context();
         _creationMetaData = new CreationMetaData();
         A.CallTo(() => _creationMetaDataFactory.Create()).Returns(_creationMetaData);
         _snapshot = new Project
         {
            ObservedData = new[] { _observedDataSnapshot, _observedDataSnapshot },
            Individuals = new[] { _individualSnapshot, _individualSnapshot }
         };

         _defaultMapper = A.Fake<ISnapshotMapper>();
         A.CallTo(() => _snapshotMapper.MapperFor(_individualSnapshot)).Returns(_defaultMapper);
         A.CallTo(() => _snapshotMapper.MapperFor(_observedDataSnapshot)).Returns(_snapshotMapper);
         A.CallTo(() => _defaultMapper.MapToModel(_individualSnapshot, A<SnapshotContext>._)).Returns(_individual);
         A.CallTo(() => _snapshotMapper.MapToModel(_observedDataSnapshot, A<SnapshotContext>._)).Returns(_observedData);
      }

      protected override async Task Because()
      {
         _newProject = await sut.MapToModel(_snapshot, new ProjectContext(new PKSimProject(), runSimulations: false));
      }

      [Observation]
      public void should_return_a_project_with_the_expected_building_blocks()
      {
         _newProject.All<Individual>().ShouldContain(_individual);
      }

      [Observation]
      public void should_have_mapped_the_observed_data()
      {
         _newProject.AllObservedData.ShouldContain(_observedData);
      }

      [Observation]
      public void should_log_an_error_for_duplicate_entries()
      {
         A.CallTo(() => _logger.AddToLog(A<string>._, LogLevel.Error, A<string>._)).MustHaveHappenedTwiceExactly();
      }
   }

   public class When_running_parallel_simulations : concern_for_ProjectMapper
   {
      private List<(ModelSimulation, Simulation)> _simulationsWithSnapshots;
      private SnapshotContext _snapshotContext;

      protected override async Task Context()
      {
         await base.Context();
         _simulationsWithSnapshots = new List<(ModelSimulation, Simulation)>
         {
            (new IndividualSimulation().WithName("Sim1"), new Simulation()),
            (new IndividualSimulation().WithName("Sim2"), new Simulation())
         };
         _snapshotContext = A.Fake<SnapshotContext>();
         A.CallTo(() => _userSettings.MaximumNumberOfCoresToUse).Returns(2);
      }

      protected override async Task Because()
      {
         var method = typeof(ProjectMapper).GetMethod("runParallelSimulations", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
         await (Task)method.Invoke(sut, new object[] { _simulationsWithSnapshots, _snapshotContext });
      }

      [Observation]
      public void should_run_simulation_for_each_simulation()
      {
         A.CallTo(() => _simulationRunner.RunSimulation(A<ModelSimulation>._, null, A<CancellationToken>._))
            .MustHaveHappened(_simulationsWithSnapshots.Count, Times.Exactly);
      }
   }

   public class When_running_parallel_simulations_and_a_run_fails : concern_for_ProjectMapper
   {
      private List<(ModelSimulation, Simulation)> _simulationsWithSnapshots;
      private SnapshotContext _snapshotContext;

      protected override async Task Context()
      {
         await base.Context();
         _simulationsWithSnapshots = new List<(ModelSimulation, Simulation)>
         {
            (new IndividualSimulation().WithName("Sim1"), new Simulation()),
            (new IndividualSimulation().WithName("Sim2"), new Simulation())
         };
         _snapshotContext = A.Fake<SnapshotContext>();
         //sequential so that the remaining counts are deterministic
         A.CallTo(() => _userSettings.MaximumNumberOfCoresToUse).Returns(1);
         //matched by name: simulations without an id would otherwise all compare equal
         A.CallTo(() => _simulationRunner.RunSimulation(A<ModelSimulation>.That.Matches(x => x.Name == "Sim1"), null, A<CancellationToken>._)).Throws<Exception>();
      }

      protected override async Task Because()
      {
         var method = typeof(ProjectMapper).GetMethod("runParallelSimulations", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
         await (Task)method.Invoke(sut, new object[] { _simulationsWithSnapshots, _snapshotContext });
      }

      //the failed run counts as completed, so the last success reports zero simulations remaining
      [Observation]
      public void should_report_the_remaining_count_including_the_failed_run()
      {
         A.CallTo(() => _logger.AddToLog(PKSimConstants.UI.SimulationFinishedMessage("Sim2", 0), LogLevel.Information, A<string>._)).MustHaveHappened();
      }
   }

   public class When_running_parallel_simulations_and_a_run_is_cancelled : concern_for_ProjectMapper
   {
      private List<(ModelSimulation, Simulation)> _simulationsWithSnapshots;
      private SnapshotContext _snapshotContext;

      protected override async Task Context()
      {
         await base.Context();
         _simulationsWithSnapshots = new List<(ModelSimulation, Simulation)>
         {
            (new IndividualSimulation().WithName("Sim1"), new Simulation()),
            (new IndividualSimulation().WithName("Sim2"), new Simulation())
         };
         _snapshotContext = A.Fake<SnapshotContext>();
         A.CallTo(() => _userSettings.MaximumNumberOfCoresToUse).Returns(1);
         //matched by name: simulations without an id would otherwise all compare equal
         A.CallTo(() => _simulationRunner.RunSimulation(A<ModelSimulation>.That.Matches(x => x.Name == "Sim1"), null, A<CancellationToken>._)).Throws<OperationCanceledException>();
      }

      protected override async Task Because()
      {
         var method = typeof(ProjectMapper).GetMethod("runParallelSimulations", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
         await (Task)method.Invoke(sut, new object[] { _simulationsWithSnapshots, _snapshotContext });
      }

      [Observation]
      public void should_log_the_cancelled_run()
      {
         A.CallTo(() => _logger.AddToLog(PKSimConstants.UI.SimulationRunCancelledMessage("Sim1"), LogLevel.Information, A<string>._)).MustHaveHappened();
      }

      //"All Simulations Finished Running." would be misleading after a cancellation
      [Observation]
      public void should_not_report_that_all_simulations_finished()
      {
         A.CallTo(() => _logger.AddToLog(PKSimConstants.UI.AllSimulationsFinishedMessage(), A<LogLevel>._, A<string>._)).MustNotHaveHappened();
      }
   }

   public class When_running_parallel_simulations_when_the_warmup_could_not_start_every_repository : concern_for_ProjectMapper
   {
      private List<(ModelSimulation, Simulation)> _simulationsWithSnapshots;
      private SnapshotContext _snapshotContext;

      protected override async Task Context()
      {
         await base.Context();
         _simulationsWithSnapshots = new List<(ModelSimulation, Simulation)>
         {
            (new IndividualSimulation().WithName("Sim1"), new Simulation()),
            (new IndividualSimulation().WithName("Sim2"), new Simulation())
         };
         _snapshotContext = A.Fake<SnapshotContext>();
         A.CallTo(() => _userSettings.MaximumNumberOfCoresToUse).Returns(4);
         A.CallTo(() => _startableWarmup.AwaitCompletion()).Returns(false);
      }

      protected override async Task Because()
      {
         var method = typeof(ProjectMapper).GetMethod("runParallelSimulations", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
         await (Task)method.Invoke(sut, new object[] { _simulationsWithSnapshots, _snapshotContext });
      }

      //runs touch lazily initialized services too: a cold repository degrades the run phase to sequential as well
      [Observation]
      public void should_run_the_simulations_on_a_single_core()
      {
         A.CallTo(() => _logger.AddToLog(PKSimConstants.UI.RunningSimulationsWithCoresMessage(1), LogLevel.Debug, A<string>._)).MustHaveHappened();
      }
   }


   public class When_converting_a_project_snapshot_whose_first_simulation_cannot_be_loaded : When_converting_a_project_snapshot_to_project
   {
      protected override async Task Context()
      {
         await base.Context();
         //the corrupted simulation comes first: mapping keeps going sequentially until one simulation
         //succeeds, so the parallel phase never starts without warmed-up services
         _snapshot.Simulations = new[] {_snapshot.Simulations[1], _snapshot.Simulations[0]};
      }
   }

   public class When_converting_a_project_snapshot_whose_leading_simulations_cannot_be_loaded : concern_for_ProjectMapper
   {
      private PKSimProject _newProject;
      private Simulation _failingSnapshot1;
      private Simulation _failingSnapshot2;
      private Simulation _validSnapshot;
      private ManualResetEventSlim _secondMappingStarted;
      private ManualResetEventSlim _releaseSecondMapping;
      private ManualResetEventSlim _thirdMappingStarted;
      private volatile bool _secondMappingCompleted;
      private bool _thirdStartedBeforeSecondCompleted;

      protected override async Task Context()
      {
         await base.Context();
         _secondMappingStarted = new ManualResetEventSlim();
         _releaseSecondMapping = new ManualResetEventSlim();
         _thirdMappingStarted = new ManualResetEventSlim();

         _failingSnapshot1 = new Simulation {Name = "S1"};
         _failingSnapshot2 = new Simulation {Name = "S2"};
         _validSnapshot = new Simulation {Name = "S3"};
         _snapshot = new Project
         {
            Simulations = new[] {_failingSnapshot1, _failingSnapshot2, _validSnapshot}
         };

         //cores are available: a fan-out that started before the first success would map S3 while S2 is still running
         A.CallTo(() => _userSettings.MaximumNumberOfCoresToUse).Returns(3);

         A.CallTo(() => _simulationMapper.MapToModel(_failingSnapshot1, A<SimulationContext>._)).Throws<Exception>();
         Model.Simulation failWhenReleased()
         {
            _secondMappingStarted.Set();
            _releaseSecondMapping.Wait(TimeSpan.FromSeconds(5));
            _secondMappingCompleted = true;
            throw new Exception();
         }

         A.CallTo(() => _simulationMapper.MapToModel(_failingSnapshot2, A<SimulationContext>._)).ReturnsLazily(() => Task.Run(failWhenReleased));
         A.CallTo(() => _simulationMapper.MapToModel(_validSnapshot, A<SimulationContext>._)).ReturnsLazily(() =>
         {
            //under an early fan-out the third mapping starts while the second still blocks, so the flag reads false
            _thirdStartedBeforeSecondCompleted = !_secondMappingCompleted;
            _thirdMappingStarted.Set();
            return Task.FromResult((Model.Simulation) new IndividualSimulation().WithName("S3"));
         });
      }

      protected override async Task Because()
      {
         var mapping = sut.MapToModel(_snapshot, new ProjectContext(new PKSimProject(), runSimulations: false));
         _secondMappingStarted.Wait(TimeSpan.FromSeconds(5)).ShouldBeTrue();

         //an early fan-out would start the third mapping within this window (it returns immediately in that
         //case); the guard was verified by mutation - an immediate fan-out fails the observation below
         _thirdMappingStarted.Wait(TimeSpan.FromMilliseconds(500));

         _releaseSecondMapping.Set();
         _newProject = await mapping;
      }

      public override async Task Cleanup()
      {
         await base.Cleanup();
         _secondMappingStarted.Dispose();
         _releaseSecondMapping.Dispose();
         _thirdMappingStarted.Dispose();
      }

      //no other mapping may begin while the second one - not yet successful - is still running
      [Observation]
      public void should_map_sequentially_until_a_simulation_succeeds()
      {
         _thirdStartedBeforeSecondCompleted.ShouldBeFalse();
      }

      [Observation]
      public void should_add_the_simulation_that_could_be_loaded()
      {
         _newProject.All<Model.Simulation>().AllNames().ShouldOnlyContainInOrder("S3");
      }
   }

   public class When_converting_a_project_snapshot_where_a_simulation_maps_to_nothing : concern_for_ProjectMapper
   {
      private PKSimProject _newProject;
      private Simulation _simulationSnapshot1;
      private Simulation _simulationSnapshot2;
      private Simulation _simulationSnapshot3;

      protected override async Task Context()
      {
         await base.Context();
         _simulationSnapshot1 = new Simulation {Name = "S1"};
         _simulationSnapshot2 = new Simulation {Name = "S2"};
         _simulationSnapshot3 = new Simulation {Name = "S3"};
         _snapshot = new Project
         {
            Simulations = new[] {_simulationSnapshot1, _simulationSnapshot2, _simulationSnapshot3}
         };
         A.CallTo(() => _userSettings.MaximumNumberOfCoresToUse).Returns(3);

         A.CallTo(() => _simulationMapper.MapToModel(_simulationSnapshot1, A<SimulationContext>._)).Returns(new IndividualSimulation().WithName("S1"));
         A.CallTo(() => _simulationMapper.MapToModel(_simulationSnapshot2, A<SimulationContext>._)).Returns((Model.Simulation) null);
         A.CallTo(() => _simulationMapper.MapToModel(_simulationSnapshot3, A<SimulationContext>._)).Returns(new IndividualSimulation().WithName("S3"));
      }

      protected override async Task Because()
      {
         _newProject = await sut.MapToModel(_snapshot, new ProjectContext(new PKSimProject(), runSimulations: false));
      }

      [Observation]
      public void should_add_only_the_simulations_that_were_mapped()
      {
         _newProject.All<Model.Simulation>().AllNames().ShouldOnlyContainInOrder("S1", "S3");
      }

      //a null mapping is skipped when the project is filled and must not count as loaded
      [Observation]
      public void should_warn_that_not_all_simulations_were_loaded()
      {
         A.CallTo(() => _logger.AddToLog(PKSimConstants.UI.OnlySomeSimulationsLoadedMessage(2, 3), LogLevel.Warning, A<string>._)).MustHaveHappened();
      }
   }

   public class When_converting_a_project_snapshot_with_a_single_simulation : concern_for_ProjectMapper
   {
      private Simulation _simulationSnapshot;

      protected override async Task Context()
      {
         await base.Context();
         _simulationSnapshot = new Simulation {Name = "S1"};
         _snapshot = new Project {Simulations = new[] {_simulationSnapshot}};
         A.CallTo(() => _userSettings.MaximumNumberOfCoresToUse).Returns(4);
         A.CallTo(() => _simulationMapper.MapToModel(_simulationSnapshot, A<SimulationContext>._)).Returns(new IndividualSimulation().WithName("S1"));
      }

      protected override async Task Because()
      {
         await sut.MapToModel(_snapshot, new ProjectContext(new PKSimProject(), runSimulations: false));
      }

      //a sequential load initializes lazily exactly what it touches instead of paying the full warm-up
      [Observation]
      public void should_not_await_the_startable_warmup()
      {
         A.CallTo(() => _startableWarmup.AwaitCompletion()).MustNotHaveHappened();
      }
   }

   public class When_converting_a_project_snapshot_when_the_warmup_could_not_start_every_repository : concern_for_ProjectMapper
   {
      private PKSimProject _newProject;
      private Simulation _simulationSnapshot1;
      private Simulation _simulationSnapshot2;

      protected override async Task Context()
      {
         await base.Context();
         _simulationSnapshot1 = new Simulation {Name = "S1"};
         _simulationSnapshot2 = new Simulation {Name = "S2"};
         _snapshot = new Project {Simulations = new[] {_simulationSnapshot1, _simulationSnapshot2}};
         A.CallTo(() => _userSettings.MaximumNumberOfCoresToUse).Returns(4);
         A.CallTo(() => _startableWarmup.AwaitCompletion()).Returns(false);
         A.CallTo(() => _simulationMapper.MapToModel(_simulationSnapshot1, A<SimulationContext>._)).Returns(new IndividualSimulation().WithName("S1"));
         A.CallTo(() => _simulationMapper.MapToModel(_simulationSnapshot2, A<SimulationContext>._)).Returns(new IndividualSimulation().WithName("S2"));
      }

      protected override async Task Because()
      {
         _newProject = await sut.MapToModel(_snapshot, new ProjectContext(new PKSimProject(), runSimulations: false));
      }

      //a cold repository is not safe to initialize from parallel workers: the load degrades to sequential instead of failing
      [Observation]
      public void should_still_load_every_simulation()
      {
         _newProject.All<Model.Simulation>().AllNames().ShouldOnlyContainInOrder("S1", "S2");
      }

      [Observation]
      public void should_construct_the_simulations_on_a_single_core()
      {
         A.CallTo(() => _logger.AddToLog(PKSimConstants.UI.ConstructingSimulationsWithCoresMessage(1), LogLevel.Debug, A<string>._)).MustHaveHappened();
      }

      //the fallback must be diagnosable above debug level, pointing at the error naming the failed repository
      [Observation]
      public void should_warn_that_the_load_falls_back_to_sequential_processing()
      {
         A.CallTo(() => _logger.AddToLog(PKSimConstants.UI.SequentialProcessingAfterFailedWarmupMessage, LogLevel.Warning, A<string>._)).MustHaveHappened();
      }
   }

   public class When_converting_a_project_snapshot_where_a_simulation_runs_out_of_memory : concern_for_ProjectMapper
   {
      private Simulation _simulationSnapshot1;
      private Simulation _simulationSnapshot2;
      private Exception _failure;

      protected override async Task Context()
      {
         await base.Context();
         _simulationSnapshot1 = new Simulation {Name = "S1"};
         _simulationSnapshot2 = new Simulation {Name = "S2"};
         _snapshot = new Project
         {
            Simulations = new[] {_simulationSnapshot1, _simulationSnapshot2}
         };
         A.CallTo(() => _userSettings.MaximumNumberOfCoresToUse).Returns(2);

         A.CallTo(() => _simulationMapper.MapToModel(_simulationSnapshot1, A<SimulationContext>._)).Returns(new IndividualSimulation().WithName("S1"));

         //wrapped the way a blocking wait would deliver it: the load must still fail rather than degrade silently
         A.CallTo(() => _simulationMapper.MapToModel(_simulationSnapshot2, A<SimulationContext>._))
            .ReturnsLazily(() => Task.FromException<Model.Simulation>(new AggregateException(new OutOfMemoryException())));
      }

      protected override async Task Because()
      {
         try
         {
            await sut.MapToModel(_snapshot, new ProjectContext(new PKSimProject(), runSimulations: false));
         }
         catch (Exception e)
         {
            _failure = e;
         }
      }

      [Observation]
      public void should_fail_the_load_with_the_out_of_memory_in_the_exception_chain()
      {
         _failure.ShouldBeAnInstanceOf<AggregateException>();
         _failure.IsOutOfMemory().ShouldBeTrue();
      }

      //no silent degradation: an out-of-memory failure must not be reported as a per-simulation load error
      [Observation]
      public void should_not_log_the_out_of_memory_as_a_simulation_load_error()
      {
         A.CallTo(() => _logger.AddToLog(PKSimConstants.Error.CannotLoadSimulation("S2"), A<LogLevel>._, A<string>._)).MustNotHaveHappened();
      }
   }

   public class When_loading_a_project_snapshot_without_running_simulations : When_converting_a_project_snapshot_to_project
   {
      protected override async Task Because()
      {
         _newProject = await sut.MapToModel(_snapshot, new ProjectContext(new PKSimProject(), runSimulations: false));
      }

      [Observation]
      public void should_not_run_any_simulations()
      {
         A.CallTo(() => _simulationRunner.RunSimulation(A<ModelSimulation>._, A<PKSim.Core.Services.SimulationRunOptions>._, A<CancellationToken>._))
            .MustNotHaveHappened();
      }

      [Observation]
      public void should_not_log_warnings_about_missing_quantities()
      {
         // Fixes #3467: when simulations are not run, CurveMapper must not warn about
         // missing data columns since no results are expected. The partial-load warning for the
         // corrupted simulation in this fixture is the only expected warning
         A.CallTo(() => _logger.AddToLog(
               A<string>.That.Matches(message => message != PKSimConstants.UI.OnlySomeSimulationsLoadedMessage(1, 2)),
               LogLevel.Warning, A<string>._))
            .MustNotHaveHappened();
      }

      [Observation]
      public void should_warn_that_not_all_simulations_were_loaded()
      {
         A.CallTo(() => _logger.AddToLog(PKSimConstants.UI.OnlySomeSimulationsLoadedMessage(1, 2), LogLevel.Warning, A<string>._))
            .MustHaveHappened();
      }
   }

   public class ExpressionProfileEqualityComparer : GenericEqualityComparer<ExpressionProfile>
   {
   }

   public class RandomPopulationEqualityComparer : GenericEqualityComparer<RandomPopulation>
   {
   }

   public class FormulationEqualityComparer : GenericEqualityComparer<Formulation>
   {
   }

   public class PKSimEventEqualityComparer : GenericEqualityComparer<PKSimEvent>
   {
   }

   public class ObserverSetEqualityComparer : GenericEqualityComparer<ObserverSet>
   {
   }
}