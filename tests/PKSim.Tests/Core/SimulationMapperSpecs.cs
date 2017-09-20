using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Extensions;
using AdvancedParameter = PKSim.Core.Snapshots.AdvancedParameter;
using Compound = PKSim.Core.Model.Compound;
using CompoundProperties = PKSim.Core.Model.CompoundProperties;
using Individual = PKSim.Core.Model.Individual;
using OutputSchema = OSPSuite.Core.Domain.OutputSchema;
using OutputSelections = PKSim.Core.Snapshots.OutputSelections;
using Simulation = PKSim.Core.Snapshots.Simulation;
using SolverSettings = OSPSuite.Core.Domain.SolverSettings;

namespace PKSim.Core
{
   public abstract class concern_for_SimulationMapper : ContextSpecificationAsync<SimulationMapper>
   {
      protected IndividualSimulation _individualSimulation;
      protected SimulationProperties _simulationProperties;
      protected ISimulationSettings _settings;
      protected Simulation _snapshot;
      protected SimulationPropertiesMapper _simulationPropertiesMapper;
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
      private EventPropertiesMapper _eventPropertiesMapper;
      protected EventSelections _eventSelections;
      protected Individual _individual;
      protected Compound _compound;
      protected PKSimEvent _event;
      private OutputSelections _outputSelectionSnapshot;
      protected RandomPopulation _population;
      protected PopulationSimulation _populationSimulation;
      protected OSPSuite.Core.Domain.Model _model;
      protected AdvancedParameterCollection _avancedParameterCollection;

      protected override Task Context()
      {
         _simulationPropertiesMapper = A.Fake<SimulationPropertiesMapper>();
         _solverSettingsMapper = A.Fake<SolverSettingsMapper>();
         _outputSchemaMapper = A.Fake<OutputSchemaMapper>();
         _outputSelectionMapper = A.Fake<OutputSelectionsMapper>();
         _parameterMapper = A.Fake<ParameterMapper>();
         _compoundPropertiesMapper = A.Fake<CompoundPropertiesMapper>();
         _advancedParameterMapper = A.Fake<AdvancedParameterMapper>();
         _eventPropertiesMapper = A.Fake<EventPropertiesMapper>();
         _simulationFactory = A.Fake<ISimulationFactory>();
         _executionContext = A.Fake<IExecutionContext>();
         _simulationModelCreator = A.Fake<ISimulationModelCreator>();
         _simulationBuildingBlockUpdater = A.Fake<ISimulationBuildingBlockUpdater>();

         sut = new SimulationMapper(_simulationPropertiesMapper, _solverSettingsMapper, _outputSchemaMapper,
            _outputSelectionMapper, _compoundPropertiesMapper, _parameterMapper, _advancedParameterMapper, _eventPropertiesMapper,
            _simulationFactory, _executionContext, _simulationModelCreator, _simulationBuildingBlockUpdater);

         _simulationProperties = new SimulationProperties();
         _settings = new SimulationSettings();
         _rootContainer = new Container().WithName("Sim");
         _model = new OSPSuite.Core.Domain.Model {Root = _rootContainer };

         _individualSimulation = new IndividualSimulation
         {
            Name = "S1",
            Properties = _simulationProperties,
            SimulationSettings = _settings,
            Description = "Simulation Description",
            Model = _model
         };

         _populationSimulation = new PopulationSimulation
         {
            Properties = _simulationProperties,
            SimulationSettings = _settings,
            Model =_model
         };

         _avancedParameterCollection = new AdvancedParameterCollection();
         _populationSimulation.SetAdvancedParameters(_avancedParameterCollection);

         _project = new PKSimProject();
         _individual = new Individual {Name = "IND"};
         _compound = new Compound {Name = "COMP"};
         _event = new PKSimEvent {Name = "Event"};
         _population = new RandomPopulation() {Name = "POP"};
         _project.AddBuildingBlock(_individual);
         _project.AddBuildingBlock(_compound);
         _project.AddBuildingBlock(_event);
         _project.AddBuildingBlock(_population);

         _compoundProperties = new CompoundProperties();
         _snaphotCompoundProperties = new Snapshots.CompoundProperties {Name = _compound.Name};
         A.CallTo(() => _compoundPropertiesMapper.MapToSnapshot(_compoundProperties, _project)).ReturnsAsync(_snaphotCompoundProperties);
         _individualSimulation.Properties.AddCompoundProperties(_compoundProperties);

         _eventSelections = new EventSelections();
         _eventSelections.AddEventSelection(new EventSelection
         {
            Name = _event.Name,
         });

         _individualSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("IndTemplateId", PKSimBuildingBlockType.Individual)
         {
            BuildingBlock = _individual
         });
         _populationSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("PopTemplateId", PKSimBuildingBlockType.Population)
         {
            BuildingBlock = _population
         });


         A.CallTo(() => _eventPropertiesMapper.MapToSnapshot(_individualSimulation.EventProperties, _project)).ReturnsAsync(_eventSelections);

         _outputSelectionSnapshot = new OutputSelections();
         A.CallTo(() => _outputSelectionMapper.MapToSnapshot(_individualSimulation.OutputSelections)).ReturnsAsync(_outputSelectionSnapshot);
         return Task.FromResult(true);
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
         _snapshot.Events.ShouldBeEqualTo(_eventSelections);
      }

      [Observation]
      public void should_not_save_any_advanced_parameters()
      {
         _snapshot.AdvancedParameters.ShouldBeNull();
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
         A.CallTo(() => _advancedParameterMapper.MapToSnapshot(_advancedParameter)).ReturnsAsync(_snapshotAdvancedParameter);
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
   }

   public class When_mapping_an_individual_simulation_snapshot_to_simulation : concern_for_SimulationMapper
   {
      private Model.Simulation _simulation;
      private ModelProperties _modelProperties;
      private OSPSuite.Core.Domain.OutputSelections _outputSelection;
      private SolverSettings _solver;
      private OutputSchema _outputSchema;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_individualSimulation, _project);
         var individualSimulation = new IndividualSimulation
         {
            Properties = _simulationProperties,
            SimulationSettings =_settings,
            Model = new OSPSuite.Core.Domain.Model {Root = new Container()}
         };

         _modelProperties = new ModelProperties();
         A.CallTo(() => _simulationPropertiesMapper.ModelPropertiesFrom(_snapshot.Configuration, _individual)).Returns(_modelProperties);
         A.CallTo(() => _simulationFactory.CreateFrom(_individual, A<IReadOnlyList<Compound>>._, _modelProperties, null)).Returns(individualSimulation);

         _outputSelection = new OSPSuite.Core.Domain.OutputSelections();
         _outputSelection.AddOutput(new QuantitySelection("PATH", QuantityType.BaseGrid));
         A.CallTo(() => _outputSelectionMapper.MapToModel(_snapshot.OutputSelections, individualSimulation)).ReturnsAsync(_outputSelection);

         _solver = new SolverSettings();
         A.CallTo(() => _solverSettingsMapper.MapToModel(_snapshot.Solver)).ReturnsAsync(_solver);

         _outputSchema = new OutputSchema();
         A.CallTo(() => _outputSchemaMapper.MapToModel(_snapshot.OutputSchema)).ReturnsAsync(_outputSchema);
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
   }

   public class when_mapping_a_population_simulation_snapshot_to_simulation : concern_for_SimulationMapper
   {
      private Model.Simulation _simulation;

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
   }
}