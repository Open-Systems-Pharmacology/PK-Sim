using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Model;
using PKSim.Core.Services;


namespace PKSim.Core
{
   public abstract class concern_for_BuildConfigurationTask : ContextSpecification<IBuildConfigurationTask>
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
      protected IParameterStartValuesBuildingBlock _parameterValuesBuildingBlock;
      protected IPKSimMoleculeStartValuesCreator _moleculeStartValueCreator;
      private IMoleculeStartValuesBuildingBlock _moleculeStartValueBuildingBlock;
      protected IMoleculeCalculationRetriever _moleculeCalculationRetriever;
      protected ICoreCalculationMethod _cm1;
      protected ICoreCalculationMethod _cm2;
      protected IDistributedParameterToTableParameterConverter _distributedTableConverter;
      protected IParameterDefaultStateUpdater _parameterDefaultStateUpdater;

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
         _cm1 = new CoreCalculationMethod();
         _cm2 = new CoreCalculationMethod();
         _simulation = new IndividualSimulation {Properties = new SimulationProperties()};
         _simulation.SimulationSettings = new SimulationSettings();
         _simulation.ModelConfiguration = new ModelConfiguration();
         _individual = new Individual().WithName("MyIndividuyal");
         _compound = A.Fake<Compound>().WithName("MyCompound");
         _protocol = new SimpleProtocol().WithName("MyProtocol");
         _spatialStructure = A.Fake<ISpatialStructure>();
         _passiveTransportBuilingBlock = A.Fake<IPassiveTransportBuildingBlock>();
         _observerBuildingBlock = A.Fake<IObserverBuildingBlock>();
         _eventBuildingBlock = A.Fake<IEventGroupBuildingBlock>();
         _parameterValuesBuildingBlock = A.Fake<IParameterStartValuesBuildingBlock>();
         _moleculeStartValueBuildingBlock = A.Fake<IMoleculeStartValuesBuildingBlock>();
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Individual", PKSimBuildingBlockType.Individual) {BuildingBlock = _individual});
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Compound", PKSimBuildingBlockType.Compound) {BuildingBlock = _compound});
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Protocol", PKSimBuildingBlockType.Protocol) {BuildingBlock = _protocol});
         A.CallTo(() => _moleculeCalculationRetriever.AllMoleculeCalculationMethodsUsedBy(_simulation)).Returns(new[] {_cm1, _cm2});
         A.CallTo(() => _spatialStructureFactory.CreateFor(_individual, _simulation)).Returns(_spatialStructure);
         A.CallTo(() => _modelPassiveTransportQuery.AllPassiveTransportsFor(_simulation)).Returns(_passiveTransportBuilingBlock);
         A.CallTo(() => _modelObserverQuery.AllObserversFor(A<IMoleculeBuildingBlock>.Ignored, _simulation)).Returns(_observerBuildingBlock);
         A.CallTo(() => _eventBuildingBlockCreator.CreateFor(_simulation)).Returns(_eventBuildingBlock);
         A.CallTo(() => _parameterStartValueCreator.CreateFor(A<IBuildConfiguration>.Ignored, A<Simulation>.Ignored)).Returns(_parameterValuesBuildingBlock);
         A.CallTo(() => _moleculeStartValueCreator.CreateFor(A<IBuildConfiguration>.Ignored, A<Simulation>.Ignored)).Returns(_moleculeStartValueBuildingBlock);
         sut = new BuildConfigurationTask(_spatialStructureFactory, _modelObserverQuery, _modelPassiveTransportQuery, _parameterStartValueCreator,
            _moleculesAndReactionsCreator, _eventBuildingBlockCreator, _moleculeStartValueCreator, _moleculeCalculationRetriever,
            _distributedTableConverter, _parameterDefaultStateUpdater);
      }
   }

   public class When_the_build_configuration_task_is_creating_a_build_configuation_for_a_simulation : concern_for_BuildConfigurationTask
   {
      private IBuildConfiguration _buildConfiguration;

      protected override void Because()
      {
         _buildConfiguration = sut.CreateFor(_simulation, true, true);
      }

      [Observation]
      public void should_create_a_spatial_structure_based_on_the_simulation_individual_and_model_properties()
      {
         _buildConfiguration.SpatialStructure.ShouldBeEqualTo(_spatialStructure);
      }

      [Observation]
      public void should_update_the_default_parameter_state_for_the_spatial_structure()
      {
         A.CallTo(() => _parameterDefaultStateUpdater.UpdateDefaultFor(_buildConfiguration.SpatialStructure)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_passsive_transports_for_the_model_defined_for_the_simulation()
      {
         _buildConfiguration.PassiveTransports.ShouldBeEqualTo(_passiveTransportBuilingBlock);
      }

      [Observation]
      public void should_add_the_observers_defined_for_the_given_model()
      {
         _buildConfiguration.Observers.ShouldBeEqualTo(_observerBuildingBlock);
      }

      [Observation]
      public void should_add_the_event_defined_for_the_given_simulation()
      {
         _buildConfiguration.EventGroups.ShouldBeEqualTo(_eventBuildingBlock);
      }

      [Observation]
      public void should_update_the_default_parameter_state_for_the_event_building_block_creator()
      {
         A.CallTo(() => _parameterDefaultStateUpdater.UpdateDefaultFor(_buildConfiguration.EventGroups)).MustHaveHappened();
      }

      [Observation]
      public void should_create_the_default_parameter_values_for_the_available_parameters()
      {
         A.CallTo(() => _parameterStartValueCreator.CreateFor(_buildConfiguration, _simulation)).MustHaveHappened();
      }

      [Observation]
      public void should_create_the_default_molecule_start_values()
      {
         A.CallTo(() => _moleculesAndReactionsCreator.CreateFor(_buildConfiguration, _simulation)).MustHaveHappened();
      }

      [Observation]
      public void should_create_the_default_molecule_start_value_creator()
      {
         A.CallTo(() => _moleculeStartValueCreator.CreateFor(_buildConfiguration, _simulation)).MustHaveHappened();
      }

      [Observation]
      public void should_have_set_the_used_calculation_method_in_the_build_configuration()
      {
         _buildConfiguration.AllCalculationMethods().ShouldOnlyContain(_cm1, _cm2);
      }

      [Observation]
      public void should_have_set_the_simulation_settings_in_the_build_configuration()
      {
         _buildConfiguration.SimulationSettings.ShouldBeEqualTo(_simulation.SimulationSettings);
      }

      [Observation]
      public void should_have_updated_the_distributed_parameter()
      {
         A.CallTo(() => _distributedTableConverter.UpdateBuildConfigurationForAging(_buildConfiguration, _simulation, true)).MustHaveHappened();
      }
   }
}