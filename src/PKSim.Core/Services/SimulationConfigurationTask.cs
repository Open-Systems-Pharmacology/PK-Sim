using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationConfigurationTask
   {
      /// <summary>
      ///    Create a build configuration based on the selected building blocks and model properties from the simulation
      /// </summary>
      /// <param name="simulation">Simulation used to create a building block configuration</param>
      /// <param name="shouldValidate">True if validation should be performed otherwise false</param>
      /// <param name="createAgingDataInSimulation">
      ///    True if aging data should be created in the simulation. False if they should
      ///    stay as is (typically for MoBi Export)
      /// </param>
      SimulationConfiguration CreateFor(Simulation simulation, bool shouldValidate, bool createAgingDataInSimulation);
   }

   public class SimulationConfigurationTask : ISimulationConfigurationTask
   {
      private readonly IEventBuildingBlockCreator _eventBuildingBlockCreator;
      private readonly IPKSimMoleculeStartValuesCreator _moleculeStartValuesCreator;
      private readonly IMoleculeCalculationRetriever _moleculeCalculationRetriever;
      private readonly IDistributedParameterToTableParameterConverter _distributedParameterToTableParameterConverter;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IIndividualToIndividualBuildingBlockMapper _individualBuildingBlockMapper;
      private readonly IExpressionProfileToExpressionProfileBuildingBlockMapper _expressionProfileBuildingBlockMapper;
      private readonly IApplicationConfiguration _applicationConfiguration;
      private readonly IModelObserverQuery _modelObserverQuery;
      private readonly IModelPassiveTransportQuery _modelPassiveTransportQuery;
      private readonly IMoleculesAndReactionsCreator _moleculesAndReactionsCreator;
      private readonly IPKSimParameterStartValuesCreator _parameterStartValuesCreator;
      private readonly IPKSimSpatialStructureFactory _spatialStructureFactory;

      public SimulationConfigurationTask(
         IPKSimSpatialStructureFactory spatialStructureFactory,
         IModelObserverQuery modelObserverQuery,
         IModelPassiveTransportQuery modelPassiveTransportQuery,
         IPKSimParameterStartValuesCreator parameterStartValuesCreator,
         IMoleculesAndReactionsCreator moleculesAndReactionsCreator,
         IEventBuildingBlockCreator eventBuildingBlockCreator,
         IPKSimMoleculeStartValuesCreator moleculeStartValuesCreator,
         IMoleculeCalculationRetriever moleculeCalculationRetriever,
         IDistributedParameterToTableParameterConverter distributedParameterToTableParameterConverter,
         IObjectBaseFactory objectBaseFactory,
         IIndividualToIndividualBuildingBlockMapper individualBuildingBlockMapper,
         IExpressionProfileToExpressionProfileBuildingBlockMapper expressionProfileBuildingBlockMapper,
         IApplicationConfiguration applicationConfiguration
      )
      {
         _moleculeCalculationRetriever = moleculeCalculationRetriever;
         _distributedParameterToTableParameterConverter = distributedParameterToTableParameterConverter;
         _objectBaseFactory = objectBaseFactory;
         _spatialStructureFactory = spatialStructureFactory;
         _modelObserverQuery = modelObserverQuery;
         _modelPassiveTransportQuery = modelPassiveTransportQuery;
         _parameterStartValuesCreator = parameterStartValuesCreator;
         _moleculesAndReactionsCreator = moleculesAndReactionsCreator;
         _eventBuildingBlockCreator = eventBuildingBlockCreator;
         _moleculeStartValuesCreator = moleculeStartValuesCreator;
         _moleculeCalculationRetriever = moleculeCalculationRetriever;
         _individualBuildingBlockMapper = individualBuildingBlockMapper;
         _expressionProfileBuildingBlockMapper = expressionProfileBuildingBlockMapper;
         _applicationConfiguration = applicationConfiguration;
      }

      public SimulationConfiguration CreateFor(Simulation simulation, bool shouldValidate, bool createAgingDataInSimulation)
      {
         var module = _objectBaseFactory.Create<Module>().WithName(simulation.Name);
         module.AddExtendedProperty(CoreConstants.PK_SIM_VERSION, _applicationConfiguration.FullVersion);
         var individual = simulation.Individual;
         var simulationConfiguration = new SimulationConfiguration
         {
            ShouldValidate = shouldValidate,
         };
         var moduleConfiguration = new ModuleConfiguration(module);
         simulationConfiguration.AddModuleConfiguration(moduleConfiguration);
         //STEP1: Create spatial structure
         module.SpatialStructure = _spatialStructureFactory.CreateFor(individual, simulation);

         //STEP2: add used calculation method to the build configuration
         _moleculeCalculationRetriever.AllMoleculeCalculationMethodsUsedBy(simulation).Each(simulationConfiguration.AddCalculationMethod);

         //STEP3: Add Individual
         simulationConfiguration.Individual = _individualBuildingBlockMapper.MapFrom(individual);

         //STEP4: Add expression profiles
         individual.AllExpressionProfiles().MapAllUsing(_expressionProfileBuildingBlockMapper).Each(simulationConfiguration.AddExpressionProfile);

         //STEP5: Add Passive Transports 
         module.PassiveTransports = _modelPassiveTransportQuery.AllPassiveTransportsFor(simulation);

         //STEP6 : Molecules, and Molecule Start and reactions are generated in one go from the molecule and reaction creator
         var (molecules, reactions) = _moleculesAndReactionsCreator.CreateFor(module, simulation);
         module.Molecules = molecules;
         module.Reactions = reactions;

         //STEP7 Add Application, Formulation and events
         module.EventGroups = _eventBuildingBlockCreator.CreateFor(simulation);

         //STEP8: Add Observers
         module.Observers = _modelObserverQuery.AllObserversFor(module.Molecules, simulation);

         //STEP9 once all building blocks have been created, we need to create the default parameter and molecule values values 
         var moleculeStartValues = _moleculeStartValuesCreator.CreateFor(module, simulation);
         module.AddMoleculeStartValueBlock(moleculeStartValues);

         var parameterStartValues = _parameterStartValuesCreator.CreateFor(simulation);
         module.AddParameterStartValueBlock(parameterStartValues);

         moduleConfiguration.SelectedParameterStartValues = parameterStartValues;
         moduleConfiguration.SelectedMoleculeStartValues = moleculeStartValues;

         //STEP10 update simulation settings
         simulationConfiguration.SimulationSettings = simulation.Settings.WithName(simulation.Name);

         //STEP11 Convert all parameters to table if required
         _distributedParameterToTableParameterConverter.UpdateSimulationConfigurationForAging(simulationConfiguration, simulation, createAgingDataInSimulation);


         return simulationConfiguration;
      }
   }
}