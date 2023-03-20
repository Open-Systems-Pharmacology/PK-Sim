using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Model;
using PKSim.Core.Services;


namespace PKSim.Core
{
   public abstract class concern_for_BuildConfigurationTask : ContextSpecification<ISimulationConfigurationTask>
   {
      protected IPKSimSpatialStructureFactory _spatialStructureFactory;
      protected Simulation _simulation;
      private IModelObserverQuery _modelObserverQuery;
      private IModelPassiveTransportQuery _modelPassiveTransportQuery;
      protected ISpatialStructure _spatialStructure;
      protected IPassiveTransportBuildingBlock _passiveTransportBuilingBlock;
      protected IObserverBuildingBlock _observerBuildingBlock;
      protected IPKSimParameterStartValuesCreator _parameterStartValueCreator;
      protected IMoleculesAndReactionsCreator _moleculesAndReactionsCreator;
      protected IEventBuildingBlockCreator _eventBuildingBlockCreator;
      protected Individual _individual;
      protected Compound _compound;
      protected Protocol _protocol;
      protected IEventGroupBuildingBlock _eventBuildingBlock;
      protected ParameterStartValuesBuildingBlock _parameterValuesBuildingBlock;
      protected IPKSimMoleculeStartValuesCreator _moleculeStartValueCreator;
      private MoleculeStartValuesBuildingBlock _moleculeStartValueBuildingBlock;
      protected IMoleculeCalculationRetriever _moleculeCalculationRetriever;
      protected ICoreCalculationMethod _cm1;
      protected ICoreCalculationMethod _cm2;
      protected IDistributedParameterToTableParameterConverter _distributedTableConverter;
      protected IParameterDefaultStateUpdater _parameterDefaultStateUpdater;
      private IObjectBaseFactory _objectBaseFactory;

      protected override void Context()
      {
         _spatialStructureFactory = A.Fake<IPKSimSpatialStructureFactory>();
         _modelObserverQuery = A.Fake<IModelObserverQuery>();
         _modelPassiveTransportQuery = A.Fake<IModelPassiveTransportQuery>();
         _parameterStartValueCreator = A.Fake<IPKSimParameterStartValuesCreator>();
         _moleculesAndReactionsCreator = A.Fake<IMoleculesAndReactionsCreator>();
         _eventBuildingBlockCreator = A.Fake<IEventBuildingBlockCreator>();
         _moleculeStartValueCreator = A.Fake<IPKSimMoleculeStartValuesCreator>();
         _moleculeCalculationRetriever = A.Fake<IMoleculeCalculationRetriever>();
         _distributedTableConverter = A.Fake<IDistributedParameterToTableParameterConverter>();
         _parameterDefaultStateUpdater = A.Fake<IParameterDefaultStateUpdater>();
         _objectBaseFactory= A.Fake<IObjectBaseFactory>();
         _cm1 = new CoreCalculationMethod();
         _cm2 = new CoreCalculationMethod();
         _simulation = new IndividualSimulation {Properties = new SimulationProperties()};
         _simulation.Settings = new SimulationSettings();
         _simulation.ModelConfiguration = new ModelConfiguration();
         _individual = new Individual().WithName("MyIndividuyal");
         _compound = A.Fake<Compound>().WithName("MyCompound");
         _protocol = new SimpleProtocol().WithName("MyProtocol");
         _spatialStructure = A.Fake<ISpatialStructure>();
         _passiveTransportBuilingBlock = A.Fake<IPassiveTransportBuildingBlock>();
         _observerBuildingBlock = A.Fake<IObserverBuildingBlock>();
         _eventBuildingBlock = A.Fake<IEventGroupBuildingBlock>();
         _parameterValuesBuildingBlock = A.Fake<ParameterStartValuesBuildingBlock>();
         _moleculeStartValueBuildingBlock = A.Fake<MoleculeStartValuesBuildingBlock>();
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Individual", PKSimBuildingBlockType.Individual) {BuildingBlock = _individual});
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Compound", PKSimBuildingBlockType.Compound) {BuildingBlock = _compound});
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Protocol", PKSimBuildingBlockType.Protocol) {BuildingBlock = _protocol});
         A.CallTo(() => _moleculeCalculationRetriever.AllMoleculeCalculationMethodsUsedBy(_simulation)).Returns(new[] {_cm1, _cm2});
         A.CallTo(() => _spatialStructureFactory.CreateFor(_individual, _simulation)).Returns(_spatialStructure);
         A.CallTo(() => _modelPassiveTransportQuery.AllPassiveTransportsFor(_simulation)).Returns(_passiveTransportBuilingBlock);
         A.CallTo(() => _modelObserverQuery.AllObserversFor(A<MoleculeBuildingBlock>.Ignored, _simulation)).Returns(_observerBuildingBlock);
         A.CallTo(() => _eventBuildingBlockCreator.CreateFor(_simulation)).Returns(_eventBuildingBlock);
         A.CallTo(() => _parameterStartValueCreator.CreateFor(A<SimulationConfiguration>.Ignored, A<Simulation>.Ignored)).Returns(_parameterValuesBuildingBlock);
         A.CallTo(() => _moleculeStartValueCreator.CreateFor(A<SimulationConfiguration>.Ignored, A<Simulation>.Ignored)).Returns(_moleculeStartValueBuildingBlock);
         sut = new SimulationConfigurationTask(_spatialStructureFactory, _modelObserverQuery, _modelPassiveTransportQuery, _parameterStartValueCreator,
            _moleculesAndReactionsCreator, _eventBuildingBlockCreator, _moleculeStartValueCreator, _moleculeCalculationRetriever,
            _distributedTableConverter, _parameterDefaultStateUpdater, _objectBaseFactory);
      }
   }

   public class When_the_simulation_configuration_task_is_creating_a_simulation_configuration_for_a_simulation : concern_for_BuildConfigurationTask
   {
      private SimulationConfiguration _simulationConfiguration;

      protected override void Because()
      {
         _simulationConfiguration = sut.CreateFor(_simulation, true, true);
      }

      [Observation]
      public void should_create_a_spatial_structure_based_on_the_simulation_individual_and_model_properties()
      {
         _simulationConfiguration.SpatialStructure.ShouldBeEqualTo(_spatialStructure);
      }

      [Observation]
      public void should_update_the_default_parameter_state_for_the_spatial_structure()
      {
         A.CallTo(() => _parameterDefaultStateUpdater.UpdateDefaultFor(_simulationConfiguration.SpatialStructure)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_passive_transports_for_the_model_defined_for_the_simulation()
      {
         _simulationConfiguration.PassiveTransports.ShouldBeEqualTo(_passiveTransportBuilingBlock);
      }

      [Observation]
      public void should_add_the_observers_defined_for_the_given_model()
      {
         _simulationConfiguration.Observers.ShouldBeEqualTo(_observerBuildingBlock);
      }

      [Observation]
      public void should_add_the_event_defined_for_the_given_simulation()
      {
         _simulationConfiguration.EventGroups.ShouldBeEqualTo(_eventBuildingBlock);
      }

      [Observation]
      public void should_update_the_default_parameter_state_for_the_event_building_block_creator()
      {
         A.CallTo(() => _parameterDefaultStateUpdater.UpdateDefaultFor(_simulationConfiguration.EventGroups)).MustHaveHappened();
      }

      [Observation]
      public void should_create_the_default_parameter_values_for_the_available_parameters()
      {
         A.CallTo(() => _parameterStartValueCreator.CreateFor(_simulationConfiguration, _simulation)).MustHaveHappened();
      }

      [Observation]
      public void should_create_the_default_molecule_start_values()
      {
         A.CallTo(() => _moleculesAndReactionsCreator.CreateFor(_simulationConfiguration, _simulation)).MustHaveHappened();
      }

      [Observation]
      public void should_create_the_default_molecule_start_value_creator()
      {
         A.CallTo(() => _moleculeStartValueCreator.CreateFor(_simulationConfiguration, _simulation)).MustHaveHappened();
      }

      [Observation]
      public void should_have_set_the_used_calculation_method_in_the_build_configuration()
      {
         _simulationConfiguration.AllCalculationMethods.ShouldOnlyContain(_cm1, _cm2);
      }

      [Observation]
      public void should_have_set_the_simulation_settings_in_the_build_configuration()
      {
         _simulationConfiguration.SimulationSettings.ShouldBeEqualTo(_simulation.Settings);
      }

      [Observation]
      public void should_have_updated_the_distributed_parameter()
      {
         A.CallTo(() => _distributedTableConverter.UpdateSimulationConfigurationForAging(_simulationConfiguration, _simulation, true)).MustHaveHappened();
      }
   }
}