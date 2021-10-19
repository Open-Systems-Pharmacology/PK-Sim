using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
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
using ObserverSet = PKSim.Core.Model.ObserverSet;
using OutputSchema = OSPSuite.Core.Domain.OutputSchema;
using OutputSelections = PKSim.Core.Snapshots.OutputSelections;
using PopulationAnalysisChart = PKSim.Core.Model.PopulationAnalyses.PopulationAnalysisChart;
using Protocol = PKSim.Core.Model.Protocol;
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
      protected Snapshots.CompoundProperties _snapshotCompoundProperties;
      protected AdvancedParameterMapper _advancedParameterMapper;
      protected PKSimProject _project;
      protected ISimulationFactory _simulationFactory;
      protected IExecutionContext _executionContext;
      private ISimulationModelCreator _simulationModelCreator;
      protected ISimulationBuildingBlockUpdater _simulationBuildingBlockUpdater;
      protected EventMappingMapper _eventMappingMapper;
      protected Individual _individual;
      protected Compound _compound;
      protected Protocol _protocol;
      protected PKSimEvent _event;
      private OutputSelections _outputSelectionSnapshot;
      protected RandomPopulation _population;
      protected PopulationSimulation _populationSimulation;
      protected OSPSuite.Core.Domain.Model _model;
      protected AdvancedParameterCollection _advancedParameterCollection;
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
      protected InteractionSelection _interactionSelection;
      protected CompoundProcessSelection _snapshotInteraction;
      protected InductionProcess _inductionProcess;
      protected EventMapping _eventMapping;
      protected ObserverSetMapping _observerSetMapping;
      protected EventSelection _eventSelection;
      protected ObserverSetSelection _observerSetSelection;
      protected ISimulationParameterOriginIdUpdater _simulationParameterOriginIdUpdater;
      protected IOSPSuiteLogger _logger;
      protected IContainerTask _containerTask;
      protected IEntityPathResolver _entityPathResolver;
      protected CompoundProcessSelection _noSelectionSnapshotInteraction;
      protected InteractionSelection _noInteractionSelection;
      protected ObserverSetMappingMapper _observerSetMappingMapper;
      protected ObserverSet _observerSet;

      protected override Task Context()
      {
         _solverSettingsMapper = A.Fake<SolverSettingsMapper>();
         _outputSchemaMapper = A.Fake<OutputSchemaMapper>();
         _outputSelectionMapper = A.Fake<OutputSelectionsMapper>();
         _parameterMapper = A.Fake<ParameterMapper>();
         _compoundPropertiesMapper = A.Fake<CompoundPropertiesMapper>();
         _advancedParameterMapper = A.Fake<AdvancedParameterMapper>();
         _eventMappingMapper = A.Fake<EventMappingMapper>();
         _observerSetMappingMapper = A.Fake<ObserverSetMappingMapper>();
         _curveChartMapper = A.Fake<SimulationTimeProfileChartMapper>();
         _processMappingMapper = A.Fake<ProcessMappingMapper>();
         _simulationFactory = A.Fake<ISimulationFactory>();
         _executionContext = A.Fake<IExecutionContext>();
         _simulationModelCreator = A.Fake<ISimulationModelCreator>();
         _simulationBuildingBlockUpdater = A.Fake<ISimulationBuildingBlockUpdater>();
         _modelPropertiesTask = A.Fake<IModelPropertiesTask>();
         _simulationRunner = A.Fake<ISimulationRunner>();
         _populationAnalysisChartMapper = A.Fake<PopulationAnalysisChartMapper>();
         _simulationParameterOriginIdUpdater = A.Fake<ISimulationParameterOriginIdUpdater>();
         _logger = A.Fake<IOSPSuiteLogger>();
         _containerTask = A.Fake<IContainerTask>();
         _entityPathResolver = A.Fake<IEntityPathResolver>();

         sut = new SimulationMapper(_solverSettingsMapper, _outputSchemaMapper,
            _outputSelectionMapper, _compoundPropertiesMapper, _parameterMapper,
            _advancedParameterMapper, _eventMappingMapper, _observerSetMappingMapper, _curveChartMapper,
            _populationAnalysisChartMapper, _processMappingMapper,
            _simulationFactory, _executionContext, _simulationModelCreator,
            _simulationBuildingBlockUpdater, _modelPropertiesTask,
            _simulationRunner, _simulationParameterOriginIdUpdater,
            _logger, _containerTask, _entityPathResolver
         );

         _project = new PKSimProject();
         _individual = new Individual {Name = "IND", Id = "IndTemplateId"};
         _compound = new Compound {Name = "COMP", Id = "COMP"};
         _observerSet = new ObserverSet {Name = "OBS_SET", Id = "OBS_SET"};
         _protocol = new SimpleProtocol {Name = "PROT", Id = "PROT"};
         _inductionProcess = new InductionProcess().WithName("Interaction process");
         _compound.AddProcess(_inductionProcess);


         _event = new PKSimEvent {Name = "Event"};
         _population = new RandomPopulation() {Name = "POP", Id="PopTemplateId"};
         _observedData = new DataRepository("OBS_ID").WithName("OBS");
         _project.AddBuildingBlock(_individual);
         _project.AddBuildingBlock(_compound);
         _project.AddBuildingBlock(_event);
         _project.AddBuildingBlock(_population);
         _project.AddBuildingBlock(_observerSet);
         _project.AddObservedData(_observedData);

         _simulationProperties = new SimulationProperties
         {
            ModelProperties = new ModelProperties
            {
               ModelConfiguration = new ModelConfiguration {ModelName = "4Comp"}
            }
         };
         _interactionSelection = new InteractionSelection {ProcessName = _inductionProcess.Name};
         _noInteractionSelection = new InteractionSelection {MoleculeName = "CYP2D6"};

         _simulationProperties.InteractionProperties.AddInteraction(_interactionSelection);
         _simulationProperties.InteractionProperties.AddInteraction(_noInteractionSelection);

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

         _advancedParameterCollection = new AdvancedParameterCollection();
         _populationSimulationAnalysisChart = new BoxWhiskerAnalysisChart();
         _populationSimulation.SetAdvancedParameters(_advancedParameterCollection);
         _populationSimulation.AddAnalysis(_populationSimulationAnalysisChart);
         _populationSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("IndTemplateId", PKSimBuildingBlockType.Individual)
         {
            BuildingBlock = _individual,
         });

         _snapshotPopulationAnalysisChart = new Snapshots.PopulationAnalysisChart();

         A.CallTo(() => _populationAnalysisChartMapper.MapToSnapshot(_populationSimulationAnalysisChart)).Returns(_snapshotPopulationAnalysisChart);

         _snapshotInteraction = new CompoundProcessSelection();
         A.CallTo(() => _processMappingMapper.MapToSnapshot(_interactionSelection)).Returns(_snapshotInteraction);
         _snapshotInteraction.CompoundName = _compound.Name;
         _snapshotInteraction.Name = _inductionProcess.Name;

         _noSelectionSnapshotInteraction = new CompoundProcessSelection();
         A.CallTo(() => _processMappingMapper.MapToSnapshot(_noInteractionSelection)).Returns(_noSelectionSnapshotInteraction);
         _noSelectionSnapshotInteraction.MoleculeName = _noInteractionSelection.MoleculeName;

         _compoundProperties = new CompoundProperties();
         _snapshotCompoundProperties = new Snapshots.CompoundProperties {Name = _compound.Name};
         _individualSimulation.Properties.AddCompoundProperties(_compoundProperties);

         _eventMapping = new EventMapping();
         _individualSimulation.EventProperties.AddEventMapping(_eventMapping);

         _observerSetMapping = new ObserverSetMapping();
         _individualSimulation.ObserverSetProperties.AddObserverSetMapping(_observerSetMapping);

         A.CallTo(() => _compoundPropertiesMapper.MapToSnapshot(_compoundProperties, _project)).Returns(_snapshotCompoundProperties);


         _eventSelection = new EventSelection
         {
            Name = _event.Name,
         };

         _observerSetSelection = new ObserverSetSelection
         {
            Name = _observerSet.Name,
         };

         _individualSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("IndTemplateId", PKSimBuildingBlockType.Individual)
         {
            BuildingBlock = _individual,
            Altered = true
         });

         _individualSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("CompTemplateId", PKSimBuildingBlockType.Compound)
         {
            BuildingBlock = _compound
         });

         _individualSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("ProtTemplateId", PKSimBuildingBlockType.Protocol)
         {
            BuildingBlock = _protocol
         });

         _individualSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("ObserveSetTemplateId", PKSimBuildingBlockType.ObserverSet)
         {
            BuildingBlock = _observerSet
         });

         _populationSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("PopTemplateId", PKSimBuildingBlockType.Population)
         {
            BuildingBlock = _population
         });


         _individualSimulation.AddUsedObservedData(_observedData);

         A.CallTo(() => _eventMappingMapper.MapToSnapshot(_eventMapping, _project)).Returns(_eventSelection);
         A.CallTo(() => _observerSetMappingMapper.MapToSnapshot(_observerSetMapping, _project)).Returns(_observerSetSelection);

         _outputSelectionSnapshot = new OutputSelections();
         A.CallTo(() => _outputSelectionMapper.MapToSnapshot(_individualSimulation.OutputSelections)).Returns(_outputSelectionSnapshot);

         A.CallTo(() => _processMappingMapper.MapToModel(_snapshotInteraction, _inductionProcess)).Returns(_interactionSelection);

         return _completed;
      }
   }

   public class When_exporting_a_simulation_that_does_not_come_from_pksim_to_snapshot : concern_for_SimulationMapper
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
         The.Action(() => sut.MapToSnapshot(_mobiSimulation, _project)).ShouldThrowAn<OSPSuiteException>();
      }
   }

   public class When_mapping_an_individual_simulation_to_snapshot : concern_for_SimulationMapper
   {
      private readonly LocalizedParameter _simulationParameterSnapshot = new LocalizedParameter();
      private readonly LocalizedParameter _individualChangedParameterSnapshot = new LocalizedParameter();
      private readonly LocalizedParameter _protocolParameterSnapshot = new LocalizedParameter();

      private IParameter _individualParameter;
      private IParameter _individualParameterTemplate;

      private IParameter _individualParameterChanged;
      private IParameter _individualParameterChangedTemplate;

      private IParameter _simulationParameter;

      private IParameter _protocolParameter;

      private LocalizedParameter[] _localizedParameters;
      private Protocol _protocolTemplateBuildingBlock;

      protected override async Task Context()
      {
         await base.Context();
         _individualParameter = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("IndParam");
         _individualParameterTemplate = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("IndParam");
         _individualParameter.BuildingBlockType = PKSimBuildingBlockType.Individual;
         _individualParameter.Origin.BuilingBlockId = _individual.Id;
         _individualParameter.Origin.ParameterId = "IndParamTemplateId";

         _individualParameterChanged = DomainHelperForSpecs.ConstantParameterWithValue(2).WithName("IndParamChanged");
         _individualParameterChangedTemplate = DomainHelperForSpecs.ConstantParameterWithValue(10000).WithName("IndParamChanged");
         _individualParameterChanged.BuildingBlockType = PKSimBuildingBlockType.Individual;
         _individualParameterChanged.Origin.BuilingBlockId = _individual.Id;
         _individualParameterChanged.Origin.ParameterId = "IndParamChangedTemplateId";

         _simulationParameter = DomainHelperForSpecs.ConstantParameterWithValue(2).WithName("SimParam");
         _simulationParameter.BuildingBlockType = PKSimBuildingBlockType.Simulation;

         _protocolParameter = DomainHelperForSpecs.ConstantParameterWithValue(2).WithName("ProtocolParam");
         _protocolParameter.BuildingBlockType = PKSimBuildingBlockType.Protocol;
         _protocolParameter.Origin.BuilingBlockId = _protocol.Id;
         _protocolParameter.Origin.ParameterId = "ProtocolParamTemplateId";

         _rootContainer.Add(_individualParameter);
         _rootContainer.Add(_individualParameterChanged);
         _rootContainer.Add(_simulationParameter);
         _rootContainer.Add(_protocolParameter);

         _localizedParameters = new[] {_individualChangedParameterSnapshot, _simulationParameterSnapshot, _protocolParameterSnapshot};

         A.CallTo(() => _parameterMapper.LocalizedParametersFrom(A<IEnumerable<IParameter>>.That.Matches(x => x.ContainsAll(new[] {_individualParameterChanged, _simulationParameter, _protocolParameter}))))
            .Returns(_localizedParameters);

         A.CallTo(() => _executionContext.Get<IParameter>(_individualParameterChanged.Origin.ParameterId)).Returns(_individualParameterChangedTemplate);
         A.CallTo(() => _executionContext.Get<IParameter>(_individualParameter.Origin.ParameterId)).Returns(_individualParameterTemplate);

         _protocolTemplateBuildingBlock = new SimpleProtocol {Id = "ProtTemplateId"};
         _project.AddBuildingBlock(_protocolTemplateBuildingBlock);

         var allIndividualTemplateParameters = new PathCacheForSpecs<IParameter>();
         A.CallTo(() => _containerTask.CacheAllChildren<IParameter>(_individual)).Returns(allIndividualTemplateParameters);
         allIndividualTemplateParameters.Add(_individualParameterTemplate.Name, _individualParameterTemplate);
         allIndividualTemplateParameters.Add(_individualParameterChangedTemplate.Name, _individualParameterChangedTemplate);

         A.CallTo(() => _entityPathResolver.PathFor(_individualParameterTemplate)).Returns(_individualParameterTemplate.Name);
         A.CallTo(() => _entityPathResolver.PathFor(_individualParameterChangedTemplate)).Returns(_individualParameterChangedTemplate.Name);
      }

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
         _snapshot.Individual.ShouldBeEqualTo(_individual.Name);
         _snapshot.Population.ShouldBeNull();
      }

      [Observation]
      public void should_save_the_compound_properties_to_snapshot()
      {
         _snapshot.Compounds.ShouldContain(_snapshotCompoundProperties);
      }

      [Observation]
      public void should_save_the_used_events_to_snapshot()
      {
         _snapshot.Events.ShouldContain(_eventSelection);
      }

      [Observation]
      public void should_save_the_used_observer_sets_to_snapshot()
      {
         _snapshot.ObserverSets.ShouldContain(_observerSetSelection);
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
         _snapshot.Interactions.ShouldContain(_snapshotInteraction, _noSelectionSnapshotInteraction);
      }

      [Observation]
      public void should_only_export_building_block_parameters_that_have_changed_from_their_original_value_in_template_building_block()
      {
         _snapshot.Parameters.ShouldOnlyContain(_simulationParameterSnapshot, _individualChangedParameterSnapshot, _protocolParameterSnapshot);
      }

      [Observation]
      public void should_export_all_building_block_that_were_altered_in_the_simulation()
      {
         _snapshot.AlteredBuildingBlocks.Length.ShouldBeEqualTo(1);
         _snapshot.AlteredBuildingBlocks[0].Type.ShouldBeEqualTo(PKSimBuildingBlockType.Individual);
         _snapshot.AlteredBuildingBlocks[0].Name.ShouldBeEqualTo(_snapshot.Individual);
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
         _advancedParameterCollection.AddAdvancedParameter(_advancedParameter);
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

         individualSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("IndTemplateId", PKSimBuildingBlockType.Individual)
         {
            Name = _individual.Name,
            BuildingBlock = _individual
         });

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
         A.CallTo(() => _observerSetMappingMapper.MapToModel(_observerSetSelection, _project)).Returns(_observerSetMapping);
      }

      protected override async Task Because()
      {
         _simulation = await sut.MapToModel(_snapshot, new SimulationContext{Project = _project, Run = true});
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
         _simulation.InteractionProperties.Interactions.ShouldContain(_interactionSelection, _noInteractionSelection);
      }

      [Observation]
      public void should_have_updated_the_event_mapping()
      {
         _simulation.EventProperties.EventMappings.ShouldContain(_eventMapping);
      }

      [Observation]
      public void should_have_updated_the_observer_set_mapping()
      {
         _simulation.ObserverSetProperties.ObserverSetMappings.ShouldContain(_observerSetMapping);
      }

      [Observation]
      public void should_update_parameter_origin_from_simulation()
      {
         A.CallTo(() => _simulationParameterOriginIdUpdater.UpdateSimulationId(_simulation)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_altered_flag_for_each_altered_building_block()
      {
         _simulation.UsedBuildingBlockInSimulation<Individual>().Altered.ShouldBeTrue();
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

         populationSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("IndTemplateId", PKSimBuildingBlockType.Individual)
         {
            BuildingBlock = _individual
         });

         A.CallTo(() => _simulationFactory.CreateFrom(_population, A<IReadOnlyList<Compound>>._, A<ModelProperties>._, null)).Returns(populationSimulation);

         A.CallTo(() => _populationAnalysisChartMapper.MapToModel(_snapshotPopulationAnalysisChart, A<SimulationAnalysisContext>._))
            .Invokes(x => _context = x.GetArgument<SimulationAnalysisContext>(1))
            .Returns(_populationSimulationAnalysisChart);
      }

      protected override async Task Because()
      {
         _simulation = await sut.MapToModel(_snapshot, new SimulationContext {Project = _project, Run = false});
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