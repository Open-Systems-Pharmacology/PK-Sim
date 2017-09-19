using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Repositories;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Extensions;
using AdvancedParameter = PKSim.Core.Snapshots.AdvancedParameter;
using Parameter = PKSim.Core.Snapshots.Parameter;
using Simulation = PKSim.Core.Snapshots.Simulation;

namespace PKSim.Core
{
   public abstract class concern_for_SimulationMapper : ContextSpecificationAsync<SimulationMapper>
   {
      protected IndividualSimulation _individualSimulation;
      protected SimulationProperties _simulationProperties;
      protected ISimulationSettings _settings;
      protected Simulation _snapshot;
      protected SimulationPropertiesMapper _simulationPropertiesMapper;
      protected IObservedDataRepository _observedDataRepository;
      protected SolverSettingsMapper _solverSettingsMapper;
      protected OutputSchemaMapper _outputSchemaMapper;
      protected OutputSelectionsMapper _outputSelectionMapper;
      protected ParameterMapper _parameterMapper;
      protected Container _rootContainer;
      private CompoundPropertiesMapper _compoundPropertiesMapper;
      private IBuildingBlockRepository _buildingBlockRepository;
      private CompoundProperties _compoundProperties;
      protected Snapshots.CompoundProperties _snaphotCompoundProperties;
      protected PKSimEvent _event;
      protected Parameter _snapshotEventStartTime1;
      protected Parameter _snapshotEventStartTime2;
      protected AdvancedParameterMapper _advancedParameterMapper;

      protected override Task Context()
      {
         _simulationPropertiesMapper = A.Fake<SimulationPropertiesMapper>();
         _observedDataRepository = A.Fake<IObservedDataRepository>();
         _solverSettingsMapper = A.Fake<SolverSettingsMapper>();
         _outputSchemaMapper = A.Fake<OutputSchemaMapper>();
         _outputSelectionMapper = A.Fake<OutputSelectionsMapper>();
         _parameterMapper = A.Fake<ParameterMapper>();
         _compoundPropertiesMapper = A.Fake<CompoundPropertiesMapper>();
         _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();
         _advancedParameterMapper = A.Fake<AdvancedParameterMapper>();

         sut = new SimulationMapper(_simulationPropertiesMapper, _solverSettingsMapper, _outputSchemaMapper,
            _outputSelectionMapper, _observedDataRepository, _compoundPropertiesMapper, _parameterMapper,
            _buildingBlockRepository, _advancedParameterMapper);

         _simulationProperties = new SimulationProperties();
         _settings = new SimulationSettings();

         _rootContainer = new Container().WithName("Sim");
         _individualSimulation = new IndividualSimulation
         {
            Name = "S1",
            Properties = _simulationProperties,
            SimulationSettings = _settings,
            Description = "Simulation Description",
            Model = new OSPSuite.Core.Domain.Model
            {
               Root = _rootContainer
            }
         };


         _compoundProperties = new CompoundProperties();
         _snaphotCompoundProperties = new Snapshots.CompoundProperties();
         A.CallTo(() => _compoundPropertiesMapper.MapToSnapshot(_compoundProperties)).ReturnsAsync(_snaphotCompoundProperties);
         _individualSimulation.Properties.AddCompoundProperties(_compoundProperties);

         _individualSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("IndTemplateId", PKSimBuildingBlockType.Individual)
         {
            BuildingBlock = new Individual {Name = "IND"}
         });

         _event = new PKSimEvent()
            .WithName("E1")
            .WithId("EventId");

         _individualSimulation.EventProperties.AddEventMapping(new EventMapping
         {
            TemplateEventId = _event.Id,
            StartTime = DomainHelperForSpecs.ConstantParameterWithValue(1)
         });

         _individualSimulation.EventProperties.AddEventMapping(new EventMapping
         {
            TemplateEventId = _event.Id,
            StartTime = DomainHelperForSpecs.ConstantParameterWithValue(1)
         });

         _snapshotEventStartTime1 = new Parameter();
         _snapshotEventStartTime2 = new Parameter();

         A.CallTo(() => _buildingBlockRepository.ById(_event.Id)).Returns(_event);
         A.CallTo(() => _parameterMapper.MapToSnapshot(_individualSimulation.EventProperties.EventMappings[0].StartTime)).ReturnsAsync(_snapshotEventStartTime1);
         A.CallTo(() => _parameterMapper.MapToSnapshot(_individualSimulation.EventProperties.EventMappings[1].StartTime)).ReturnsAsync(_snapshotEventStartTime2);


         return Task.FromResult(true);
      }
   }

   public class When_mapping_an_individual_simulation_to_snapshot : concern_for_SimulationMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_individualSimulation);
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
         _snapshot.Events.Length.ShouldBeEqualTo(2);
         _snapshot.Events[0].Name.ShouldBeEqualTo(_event.Name);
         _snapshot.Events[0].StartTime.ShouldBeEqualTo(_snapshotEventStartTime1);
         _snapshot.Events[1].Name.ShouldBeEqualTo(_event.Name);
         _snapshot.Events[1].StartTime.ShouldBeEqualTo(_snapshotEventStartTime2);
      }

      [Observation]
      public void should_not_save_any_advanced_parameters()
      {
         _snapshot.AdvancedParameters.ShouldBeNull();
      }
   }

   public class When_mapping_a_population_simulation_to_snapshot : concern_for_SimulationMapper
   {
      private PopulationSimulation _populationSimulation;
      private AdvancedParameter _snapshotAdvancedParameter;
      private Model.AdvancedParameter _advancedParameter;

      protected override async Task Context()
      {
         await base.Context();
         _populationSimulation = new PopulationSimulation
         {
            Properties = _simulationProperties,
            SimulationSettings = _settings,
            Model = new OSPSuite.Core.Domain.Model
            {
               Root = _rootContainer
            }
         };

         _populationSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("PopTemplateId", PKSimBuildingBlockType.Population)
         {
            BuildingBlock = new ImportPopulation {Name = "POP"}
         });

         _advancedParameter = new Model.AdvancedParameter();
         _populationSimulation.SetAdvancedParameters(new AdvancedParameterCollection{_advancedParameter});
         _snapshotAdvancedParameter = new AdvancedParameter();
         A.CallTo(() => _advancedParameterMapper.MapToSnapshot(_advancedParameter)).ReturnsAsync(_snapshotAdvancedParameter);
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_populationSimulation);
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
}