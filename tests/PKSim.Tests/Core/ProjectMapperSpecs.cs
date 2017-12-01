using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using Classification = OSPSuite.Core.Domain.Classification;
using Compound = PKSim.Core.Model.Compound;
using DataRepository = OSPSuite.Core.Domain.Data.DataRepository;
using Event = PKSim.Core.Snapshots.Event;
using Formulation = PKSim.Core.Model.Formulation;
using Individual = PKSim.Core.Model.Individual;
using Population = PKSim.Core.Model.Population;
using Project = PKSim.Core.Snapshots.Project;
using Protocol = PKSim.Core.Model.Protocol;
using Simulation = PKSim.Core.Snapshots.Simulation;

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
      protected Snapshots.Population _populationSnapshot;
      protected DataRepository _observedData;
      protected Snapshots.DataRepository _observedDataSnapshot;
      protected SimulationMapper _simulationMapper;
      protected Simulation _simulationSnapshot;
      protected ClassificationMapper _classificationMapper;
      protected ClassifiableObservedData _classifiableObservedData;
      protected Classification _classification;
      protected Snapshots.Classification _observedDataClassificationSnapshot;
      protected IClassificationSnapshotTask _classificationSnapshotTask;
      protected SimulationComparison _simulationComparisonSnapshot;
      protected ISimulationComparison _simulationComparison;
      protected SimulationComparisonMapper _simulationComparisonMapper;
      protected Snapshots.Classification _simulationClassificationSnapshot;
      protected Snapshots.Classification _comparisonClassificationSnapshot;
      protected ILazyLoadTask _lazyLoadTask;

      protected override Task Context()
      {
         _classificationMapper = A.Fake<ClassificationMapper>();
         _snapshotMapper = A.Fake<ISnapshotMapper>();
         _executionContext = A.Fake<IExecutionContext>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _simulationMapper = A.Fake<SimulationMapper>();
         _simulationComparisonMapper = A.Fake<SimulationComparisonMapper>();
         _classificationSnapshotTask = A.Fake<IClassificationSnapshotTask>();
         sut = new ProjectMapper(_simulationMapper, _simulationComparisonMapper, _executionContext, _classificationSnapshotTask, _lazyLoadTask);
         A.CallTo(() => _executionContext.Resolve<ISnapshotMapper>()).Returns(_snapshotMapper);
         _individual = new Individual().WithName("IND");
         _compound = new Compound().WithName("COMP");
         _event = new PKSimEvent().WithName("EVENT");
         _formulation = new Formulation().WithName("FORM");
         _protocol = new SimpleProtocol().WithName("PROTO");
         _population = new RandomPopulation().WithName("POP");
         _observedData = new DataRepository().WithName("OD");
         _classifiableObservedData = new ClassifiableObservedData {Subject = _observedData};
         _classification = new Classification {ClassificationType = ClassificationType.ObservedData}.WithName("OD Classification");
         _simulationComparison = new IndividualSimulationComparison().WithName("COMP").WithId("SimComp");
         _simulation = new IndividualSimulation().WithName("IND_SIM").WithId("IndSim");
         _project = new PKSimProject();
         _project.AddBuildingBlock(_individual);
         _project.AddBuildingBlock(_compound);
         _project.AddBuildingBlock(_event);
         _project.AddBuildingBlock(_formulation);
         _project.AddBuildingBlock(_protocol);
         _project.AddBuildingBlock(_population);
         _project.AddObservedData(_observedData);
         _project.AddBuildingBlock(_simulation);
         _project.AddClassifiable(_classifiableObservedData);
         _project.AddClassification(_classification);
         _project.AddSimulationComparison(_simulationComparison);

         _compoundSnapshot = new Snapshots.Compound();
         _individualSnapshot = new Snapshots.Individual();
         _eventSnapshot = new Event();
         _formulationSnapshot = new Snapshots.Formulation();
         _protocolSnapshot = new Snapshots.Protocol();
         _populationSnapshot = new Snapshots.Population();
         _observedDataSnapshot = new Snapshots.DataRepository();
         _observedDataClassificationSnapshot = new Snapshots.Classification();
         _simulationComparisonSnapshot = new SimulationComparison();
         _simulationClassificationSnapshot = new Snapshots.Classification();
         _comparisonClassificationSnapshot = new Snapshots.Classification();
         _simulationSnapshot = new Simulation();

         A.CallTo(() => _snapshotMapper.MapToSnapshot(_compound)).Returns(_compoundSnapshot);
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_individual)).Returns(_individualSnapshot);
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_event)).Returns(_eventSnapshot);
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_formulation)).Returns(_formulationSnapshot);
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_protocol)).Returns(_protocolSnapshot);
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_population)).Returns(_populationSnapshot);
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_observedData)).Returns(_observedDataSnapshot);

         A.CallTo(() => _simulationComparisonMapper.MapToSnapshot(_simulationComparison)).Returns(_simulationComparisonSnapshot);
         A.CallTo(() => _classificationSnapshotTask.MapClassificationsToSnapshots<ClassifiableObservedData>(_project)).Returns(new[] {_observedDataClassificationSnapshot});
         A.CallTo(() => _classificationSnapshotTask.MapClassificationsToSnapshots<ClassifiableSimulation>(_project)).Returns(new[] {_simulationClassificationSnapshot});
         A.CallTo(() => _classificationSnapshotTask.MapClassificationsToSnapshots<ClassifiableComparison>(_project)).Returns(new[] {_comparisonClassificationSnapshot});

         A.CallTo(() => _simulationMapper.MapToSnapshot(_simulation, _project)).Returns(_simulationSnapshot);

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
      public void should_retrieve_the_snapshot_for_all_underlying_models()
      {
         _snapshot.Compounds.ShouldContain(_compoundSnapshot);
         _snapshot.Individuals.ShouldContain(_individualSnapshot);
         _snapshot.Protocols.ShouldContain(_protocolSnapshot);
         _snapshot.Formulations.ShouldContain(_formulationSnapshot);
         _snapshot.Events.ShouldContain(_eventSnapshot);
         _snapshot.Populations.ShouldContain(_populationSnapshot);
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
      public void should_save_simulation_comparison_classification()
      {
         _snapshot.SimulationComparisonClassifications.ShouldContain(_comparisonClassificationSnapshot);
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
      public void should_load_the_exported_building_blocks()
      {
         A.CallTo(() => _lazyLoadTask.Load((IPKSimBuildingBlock) _compound)).MustHaveHappened();
         A.CallTo(() => _lazyLoadTask.Load((IPKSimBuildingBlock) _formulation)).MustHaveHappened();
         A.CallTo(() => _lazyLoadTask.Load((IPKSimBuildingBlock) _event)).MustHaveHappened();
         A.CallTo(() => _lazyLoadTask.Load((IPKSimBuildingBlock) _individual)).MustHaveHappened();
         A.CallTo(() => _lazyLoadTask.Load((IPKSimBuildingBlock) _population)).MustHaveHappened();
         A.CallTo(() => _lazyLoadTask.Load((IPKSimBuildingBlock) _protocol)).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_simulation_comparison_results()
      {
         A.CallTo(() => _lazyLoadTask.Load((ILazyLoadable) _simulationComparison)).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_exported_simulation_results()
      {
         A.CallTo(() => _lazyLoadTask.Load((ILazyLoadable) _simulation)).MustHaveHappened();
         A.CallTo(() => _lazyLoadTask.LoadResults((Model.Simulation) _simulation)).MustHaveHappened();
      }
   }

   public class When_converting_a_project_snapshot_to_project : concern_for_ProjectMapper
   {
      private PKSimProject _newProject;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_project);
         A.CallTo(() => _snapshotMapper.MapToModel(_compoundSnapshot)).Returns(_compound);
         A.CallTo(() => _snapshotMapper.MapToModel(_individualSnapshot)).Returns(_individual);
         A.CallTo(() => _snapshotMapper.MapToModel(_protocolSnapshot)).Returns(_protocol);
         A.CallTo(() => _snapshotMapper.MapToModel(_formulationSnapshot)).Returns(_formulation);
         A.CallTo(() => _snapshotMapper.MapToModel(_eventSnapshot)).Returns(_event);
         A.CallTo(() => _snapshotMapper.MapToModel(_populationSnapshot)).Returns(_population);
         A.CallTo(() => _snapshotMapper.MapToModel(_observedDataSnapshot)).Returns(_observedData);

         A.CallTo(() => _simulationMapper.MapToModel(_simulationSnapshot, A<PKSimProject>._)).Returns(_simulation);
         A.CallTo(() => _simulationComparisonMapper.MapToModel(_simulationComparisonSnapshot, A<PKSimProject>._)).Returns(_simulationComparison);
      }

      protected override async Task Because()
      {
         _newProject = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_return_a_project_with_the_expected_building_blocks()
      {
         _newProject.All<Compound>().ShouldContain(_compound);
         _newProject.All<Individual>().ShouldContain(_individual);
         _newProject.All<PKSimEvent>().ShouldContain(_event);
         _newProject.All<Formulation>().ShouldContain(_formulation);
         _newProject.All<Protocol>().ShouldContain(_protocol);
         _newProject.All<Population>().ShouldContain(_population);
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
      public void should_udpate_project_classification_for_observed_data()
      {
         A.CallTo(() => _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableObservedData, DataRepository>(_snapshot.ObservedDataClassifications, _newProject, _newProject.AllObservedData)).MustHaveHappened();
      }

      [Observation]
      public void should_udpate_project_classification_for_simulation()
      {
         A.CallTo(() => _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableSimulation, Model.Simulation>(_snapshot.SimulationClassifications, _newProject, A<IReadOnlyCollection<Model.Simulation>>._)).MustHaveHappened();
      }

      [Observation]
      public void should_udpate_project_classification_for_simulation_comparison()
      {
         A.CallTo(() => _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableComparison, ISimulationComparison>(_snapshot.SimulationComparisonClassifications, _newProject, _newProject.AllSimulationComparisons)).MustHaveHappened();
      }
   }
}