using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using AdvancedParameter = PKSim.Core.Snapshots.AdvancedParameter;
using Compound = PKSim.Core.Model.Compound;
using CompoundProperties = PKSim.Core.Model.CompoundProperties;
using DataRepository = OSPSuite.Core.Domain.Data.DataRepository;
using Individual = PKSim.Core.Model.Individual;
using OutputSchema = OSPSuite.Core.Domain.OutputSchema;
using OutputSelections = PKSim.Core.Snapshots.OutputSelections;
using PopulationAnalysisChart = PKSim.Core.Model.PopulationAnalyses.PopulationAnalysisChart;
using Simulation = PKSim.Core.Snapshots.Simulation;
using SimulationRunOptions = PKSim.Core.Services.SimulationRunOptions;
using SolverSettings = OSPSuite.Core.Domain.SolverSettings;

namespace PKSim.Core
{
   public abstract class concern_for_SimulationMapper : ContextSpecificationAsync<SimulationMapper>
   {
      protected IndividualSimulation _individualSimulation;
      protected SimulationProperties _simulationProperties;
      protected ISimulationSettings _settings;
      protected Simulation _snapshot;
      protected SolverSettingsMapper _solverSettingsMapper;
      protected OutputSchemaMapper _outputSchemaMapper;
      protected OutputSelectionsMapper _outputSelectionMapper;
      protected ParameterMapper _parameterMapper;
      protected Container _rootContainer;
      private CompoundPropertiesMapper _compoundPropertiesMapper;
      private CompoundProperties _compoundProperties;
      protected Snapshots.CompoundProperties _snaphotCompoundProperties;
      protected AdvancedParameterMapper _advancedParameterMapper;
      protected PKSimProject _project;
      protected ISimulationFactory _simulationFactory;
      private IExecutionContext _executionContext;
      private ISimulationModelCreator _simulationModelCreator;
      protected ISimulationBuildingBlockUpdater _simulationBuildingBlockUpdater;
      protected EventMappingMapper _eventMappingMapper;
      protected Individual _individual;
      protected Compound _compound;
      protected PKSimEvent _event;
      private OutputSelections _outputSelectionSnapshot;
      protected RandomPopulation _population;
      protected PopulationSimulation _populationSimulation;
      protected OSPSuite.Core.Domain.Model _model;
      protected AdvancedParameterCollection _avancedParameterCollection;
      protected DataRepository _observedData;
      protected IModelPropertiesTask _modelPropertiesTask;
      protected SimulationTimeProfileChartMapper _curveChartMapper;
      protected SimulationTimeProfileChart _simulationTimeProfile;
      protected CurveChart _snapshotSimulationTimeProfile;
      protected ISimulationRunner _simulationRunner;
      protected PopulationAnalysisChartMapper _populationAnalysisChartMapper;
      protected PopulationAnalysisChart _populationSimulationAnalysisChart;
      protected Snapshots.PopulationAnalysisChart _snapshotPopulationAnalysisChart;
      protected ProcessMappingMapper _processMappingMapper;
      protected InteractionSelection _intereactionSelection;
      protected CompoundProcessSelection _snapshotInteraction;
      protected InductionProcess _inductionProcess;
      protected EventMapping _eventMapping;
      protected EventSelection _eventSelection;
      protected ISimulationParameterOriginIdUpdater _simulationParameterOriginIdUpdater;

      protected override Task Context()
      {
         _solverSettingsMapper = A.Fake<SolverSettingsMapper>();
         _outputSchemaMapper = A.Fake<OutputSchemaMapper>();
         _outputSelectionMapper = A.Fake<OutputSelectionsMapper>();
         _parameterMapper = A.Fake<ParameterMapper>();
         _compoundPropertiesMapper = A.Fake<CompoundPropertiesMapper>();
         _advancedParameterMapper = A.Fake<AdvancedParameterMapper>();
         _eventMappingMapper = A.Fake<EventMappingMapper>();
         _curveChartMapper = A.Fake<SimulationTimeProfileChartMapper>();
         _processMappingMapper = A.Fake<ProcessMappingMapper>();
         _simulationFactory = A.Fake<ISimulationFactory>();
         _executionContext = A.Fake<IExecutionContext>();
         _simulationModelCreator = A.Fake<ISimulationModelCreator>();
         _simulationBuildingBlockUpdater = A.Fake<ISimulationBuildingBlockUpdater>();
         _modelPropertiesTask = A.Fake<IModelPropertiesTask>();
         _simulationRunner = A.Fake<ISimulationRunner>();
         _populationAnalysisChartMapper = A.Fake<PopulationAnalysisChartMapper>();
         _simulationParameterOriginIdUpdater= A.Fake<ISimulationParameterOriginIdUpdater>();

         sut = new SimulationMapper(_solverSettingsMapper, _outputSchemaMapper,
            _outputSelectionMapper, _compoundPropertiesMapper, _parameterMapper,
            _advancedParameterMapper, _eventMappingMapper, _curveChartMapper,
            _populationAnalysisChartMapper, _processMappingMapper,
            _simulationFactory, _executionContext, _simulationModelCreator,
            _simulationBuildingBlockUpdater, _modelPropertiesTask, _simulationRunner, _simulationParameterOriginIdUpdater);

         _project = new PKSimProject();
         _individual = new Individual {Name = "IND"};
         _compound = new Compound {Name = "COMP"};
         _inductionProcess = new InductionProcess().WithName("Interaction process");
         _compound.AddProcess(_inductionProcess);


         _event = new PKSimEvent {Name = "Event"};
         _population = new RandomPopulation() {Name = "POP"};
         _observedData = new DataRepository("OBS_ID").WithName("OBS");
         _project.AddBuildingBlock(_individual);
         _project.AddBuildingBlock(_compound);
         _project.AddBuildingBlock(_event);
         _project.AddBuildingBlock(_population);
         _project.AddObservedData(_observedData);

         _simulationProperties = new SimulationProperties
         {
            ModelProperties = new ModelProperties
            {
               ModelConfiguration = new ModelConfiguration {ModelName = "4Comp"}
            }
         };
         _intereactionSelection = new InteractionSelection {ProcessName = _inductionProcess.Name};
         _simulationProperties.InteractionProperties.AddInteraction(_intereactionSelection);

         _settings = new SimulationSettings();
         _rootContainer = new Container().WithName("Sim");
         _model = new OSPSuite.Core.Domain.Model {Root = _rootContainer};

         _individualSimulation = new IndividualSimulation
         {
            Name = "S1",
            Properties = _simulationProperties,
            SimulationSettings = _settings,
            Description = "Simulation Description",
            Model = _model
         };

         _simulationTimeProfile = new SimulationTimeProfileChart();
         _snapshotSimulationTimeProfile = new CurveChart();
         _individualSimulation.AddAnalysis(_simulationTimeProfile);

         A.CallTo(() => _curveChartMapper.MapToSnapshot(_simulationTimeProfile)).Returns(_snapshotSimulationTimeProfile);


         _populationSimulation = new PopulationSimulation
         {
            Properties = _simulationProperties,
            SimulationSettings = _settings,
            Model = _model
         };

         _avancedParameterCollection = new AdvancedParameterCollection();
         _populationSimulationAnalysisChart = new BoxWhiskerAnalysisChart();
         _populationSimulation.SetAdvancedParameters(_avancedParameterCollection);
         _populationSimulation.AddAnalysis(_populationSimulationAnalysisChart);
         _snapshotPopulationAnalysisChart = new Snapshots.PopulationAnalysisChart();

         A.CallTo(() => _populationAnalysisChartMapper.MapToSnapshot(_populationSimulationAnalysisChart)).Returns(_snapshotPopulationAnalysisChart);

         _snapshotInteraction = new CompoundProcessSelection();
         A.CallTo(() => _processMappingMapper.MapToSnapshot(_intereactionSelection)).Returns(_snapshotInteraction);
         _snapshotInteraction.CompoundName = _compound.Name;
         _snapshotInteraction.Name = _inductionProcess.Name;

         _compoundProperties = new CompoundProperties();
         _snaphotCompoundProperties = new Snapshots.CompoundProperties {Name = _compound.Name};
         _individualSimulation.Properties.AddCompoundProperties(_compoundProperties);

         _eventMapping = new EventMapping();
         _individualSimulation.EventProperties.AddEventMapping(_eventMapping);
         A.CallTo(() => _compoundPropertiesMapper.MapToSnapshot(_compoundProperties, _project)).Returns(_snaphotCompoundProperties);


         _eventSelection = new EventSelection
         {
            Name = _event.Name,
         };

         _individualSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("IndTemplateId", PKSimBuildingBlockType.Individual)
         {
            BuildingBlock = _individual
         });
         _populationSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("PopTemplateId", PKSimBuildingBlockType.Population)
         {
            BuildingBlock = _population
         });
         _individualSimulation.AddUsedObservedData(_observedData);

         A.CallTo(() => _eventMappingMapper.MapToSnapshot(_eventMapping, _project)).Returns(_eventSelection);

         _outputSelectionSnapshot = new OutputSelections();
         A.CallTo(() => _outputSelectionMapper.MapToSnapshot(_individualSimulation.OutputSelections)).Returns(_outputSelectionSnapshot);

         A.CallTo(() => _processMappingMapper.MapToModel(_snapshotInteraction, _inductionProcess)).Returns(_intereactionSelection);

         return _completed;
      }
   }

   public class When_exporting_a_simulation_thatt_does_not_come_from_pksim_to_snapshot : concern_for_SimulationMapper
   {
      private Model.Simulation _mobiSimulation;

      protected override async Task Context()
      {
         await base.Context();
         _mobiSimulation = new IndividualSimulation {Properties = new SimulationProperties {Origin = Origins.MoBi}};
      }

      [Observation]
      public void should_throw_an_exception()
      {
         TheAsync.Action(() => sut.MapToSnapshot(_mobiSimulation, _project)).ShouldThrowAnAsync<OSPSuiteException>();
      }
   }

   public class When_mapping_an_individual_simulation_to_snapshot : concern_for_SimulationMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_individualSimulation, _project);
      }

      [Observation]
      public void should_returns_snapshot_with_the_expected_default_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_individualSimulation.Name);
         _snapshot.Description.ShouldBeEqualTo(_individualSimulation.Description);
      }

      [Observation]
      public void should_save_the_name_of_the_individual_used_to_create_the_simulation()
      {
         _snapshot.Individual.ShouldBeEqualTo("IND");
         _snapshot.Population.ShouldBeNull();
      }

      [Observation]
      public void should_save_the_compound_properties_to_snapshot()
      {
         _snapshot.Compounds.ShouldContain(_snaphotCompoundProperties);
      }

      [Observation]
      public void should_save_the_used_events_to_snapshot()
      {
         _snapshot.Events.ShouldContain(_eventSelection);
      }

      [Observation]
      public void should_not_save_any_advanced_parameters()
      {
         _snapshot.AdvancedParameters.ShouldBeNull();
      }

      [Observation]
      public void should_save_used_observed_data()
      {
         _snapshot.ObservedData.ShouldContain(_observedData.Name);
      }

      [Observation]
      public void should_save_individual_analysis()
      {
         _snapshot.IndividualAnalyses.ShouldContain(_snapshotSimulationTimeProfile);
      }

      [Observation]
      public void should_set_population_analysis_to_null()
      {
         _snapshot.PopulationAnalyses.ShouldBeNull();
      }

      [Observation]
      public void should_save_interactions()
      {
         _snapshot.Interactions.ShouldContain(_snapshotInteraction);
      }
   }

   public class When_mapping_a_population_simulation_to_snapshot : concern_for_SimulationMapper
   {
      private AdvancedParameter _snapshotAdvancedParameter;
      private Model.AdvancedParameter _advancedParameter;

      protected override async Task Context()
      {
         await base.Context();
         _advancedParameter = new Model.AdvancedParameter();
         _avancedParameterCollection.AddAdvancedParameter(_advancedParameter);
         _snapshotAdvancedParameter = new AdvancedParameter();
         A.CallTo(() => _advancedParameterMapper.MapToSnapshot(_advancedParameter)).Returns(_snapshotAdvancedParameter);
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_populationSimulation, _project);
      }

      [Observation]
      public void should_save_the_name_of_the_population_used_to_create_the_simulation()
      {
         _snapshot.Population.ShouldBeEqualTo("POP");
         _snapshot.Individual.ShouldBeNull();
      }

      [Observation]
      public void should_save_the_advanced_parameters_defined_in_the_simulation()
      {
         _snapshot.AdvancedParameters.ShouldContain(_snapshotAdvancedParameter);
      }

      [Observation]
      public void should_save_population_analysis()
      {
         _snapshot.PopulationAnalyses.ShouldContain(_snapshotPopulationAnalysisChart);
      }

      [Observation]
      public void should_set_individual_analysis_to_null()
      {
         _snapshot.IndividualAnalyses.ShouldBeNull();
      }
   }

   public class When_mapping_an_individual_simulation_snapshot_to_simulation : concern_for_SimulationMapper
   {
      private Model.Simulation _simulation;
      private ModelProperties _modelProperties;
      private OSPSuite.Core.Domain.OutputSelections _outputSelection;
      private SolverSettings _solver;
      private OutputSchema _outputSchema;
      private SimulationAnalysisContext _context;
      private DataRepository _calculatedDataRepository;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_individualSimulation, _project);
         var individualSimulation = new IndividualSimulation
         {
            Properties = _simulationProperties,
            SimulationSettings = _settings,
            Model = _model
         };

         _modelProperties = new ModelProperties();
         A.CallTo(() => _modelPropertiesTask.DefaultFor(_individual.OriginData, _snapshot.Model)).Returns(_modelProperties);
         A.CallTo(() => _simulationFactory.CreateFrom(_individual, A<IReadOnlyList<Compound>>._, _modelProperties, null)).Returns(individualSimulation);

         _outputSelection = new OSPSuite.Core.Domain.OutputSelections();
         _outputSelection.AddOutput(new QuantitySelection("PATH", QuantityType.BaseGrid));
         A.CallTo(() => _outputSelectionMapper.MapToModel(_snapshot.OutputSelections, individualSimulation)).Returns(_outputSelection);

         _solver = new SolverSettings();
         A.CallTo(() => _solverSettingsMapper.MapToModel(_snapshot.Solver)).Returns(_solver);

         _outputSchema = new OutputSchema();
         A.CallTo(() => _outputSchemaMapper.MapToModel(_snapshot.OutputSchema)).Returns(_outputSchema);

         A.CallTo(() => _curveChartMapper.MapToModel(_snapshotSimulationTimeProfile, A<SimulationAnalysisContext>._))
            .Invokes(x => _context = x.GetArgument<SimulationAnalysisContext>(1))
            .Returns(_simulationTimeProfile);


         //ensure that run will be performed
         _snapshot.HasResults = true;
         _calculatedDataRepository = DomainHelperForSpecs.ObservedData("Calculated");

         A.CallTo(() => _simulationRunner.RunSimulation(individualSimulation, A<SimulationRunOptions>._))
            .Invokes(x => { individualSimulation.DataRepository = _calculatedDataRepository; });

         A.CallTo(() => _eventMappingMapper.MapToModel(_eventSelection, _project)).Returns(_eventMapping);
      }

      protected override async Task Because()
      {
         _simulation = await sut.MapToModel(_snapshot, _project);
      }

      [Observation]
      public void should_return_a_simulation_with_the_expected_properties()
      {
         _simulation.Name.ShouldBeEqualTo(_snapshot.Name);
         _simulation.Description.ShouldBeEqualTo(_snapshot.Description);
      }

      [Observation]
      public void should_update_building_blocks_in_simulation()
      {
         A.CallTo(() => _simulationBuildingBlockUpdater.UpdateProtocolsInSimulation(_simulation)).MustHaveHappened();
         A.CallTo(() => _simulationBuildingBlockUpdater.UpdateFormulationsInSimulation(_simulation)).MustHaveHappened();
         A.CallTo(() => _simulationBuildingBlockUpdater.UpdateMultipleUsedBuildingBlockInSimulationFromTemplate(_simulation, A<IEnumerable<PKSimEvent>>._, PKSimBuildingBlockType.Event)).MustHaveHappened();
      }

      [Observation]
      public void should_have_updated_the_output_selection()
      {
         _simulation.OutputSelections.ShouldBeEqualTo(_outputSelection);
      }

      [Observation]
      public void should_have_updated_the_solver_settings()
      {
         _simulation.Solver.ShouldBeEqualTo(_solver);
      }

      [Observation]
      public void should_have_updated_the_output_schema()
      {
         _simulation.OutputSchema.ShouldBeEqualTo(_outputSchema);
      }

      [Observation]
      public void should_use_observed_data()
      {
         _simulation.UsesObservedData(_observedData).ShouldBeTrue();
      }

      [Observation]
      public void should_update_analysis()
      {
         _simulation.Analyses.ShouldContain(_simulationTimeProfile);
      }

      [Observation]
      public void should_run_the_simulation()
      {
         A.CallTo(() => _simulationRunner.RunSimulation(_simulation, A<SimulationRunOptions>._)).MustHaveHappened();
      }

      [Observation]
      public void should_have_added_the_observed_data_from_project_and_the_current_simulation_results_to_the_curve_context()
      {
         _context.DataRepositories.ShouldContain(_observedData, _calculatedDataRepository);
      }

      [Observation]
      public void should_update_interactions()
      {
         _simulation.InteractionProperties.Interactions.ShouldContain(_intereactionSelection);
      }

      [Observation]
      public void should_have_updated_the_event_mapping()
      {
         _simulation.EventProperties.EventMappings.ShouldContain(_eventMapping);
      }

      [Observation]
      public void should_update_parameter_origin_from_simulation()
      {
         A.CallTo(() => _simulationParameterOriginIdUpdater.UpdateSimulationId(_simulation)).MustHaveHappened();
      }
   }

   public class when_mapping_a_population_simulation_snapshot_to_simulation : concern_for_SimulationMapper
   {
      private Model.Simulation _simulation;
      private SimulationAnalysisContext _context;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_populationSimulation, _project);

         var populationSimulation = new PopulationSimulation
         {
            Properties = _simulationProperties,
            SimulationSettings = _settings,
            Model = _model
         };

         A.CallTo(() => _simulationFactory.CreateFrom(_population, A<IReadOnlyList<Compound>>._, A<ModelProperties>._, null)).Returns(populationSimulation);

         A.CallTo(() => _populationAnalysisChartMapper.MapToModel(_snapshotPopulationAnalysisChart, A<SimulationAnalysisContext>._))
            .Invokes(x => _context = x.GetArgument<SimulationAnalysisContext>(1))
            .Returns(_populationSimulationAnalysisChart);
      }

      protected override async Task Because()
      {
         _simulation = await sut.MapToModel(_snapshot, _project);
      }

      [Observation]
      public void should_return_a_population_simulation_with_advanced_parameters_updated_from_snapshot()
      {
         _simulation.ShouldBeAnInstanceOf<PopulationSimulation>();
         var populationSimulation = _simulation.DowncastTo<PopulationSimulation>();
         A.CallTo(() => _advancedParameterMapper.MapToModel(_snapshot.AdvancedParameters, populationSimulation)).MustHaveHappened();
      }

      [Observation]
      public void should_update_analysis()
      {
         _simulation.Analyses.ShouldContain(_populationSimulationAnalysisChart);
      }
   }
}