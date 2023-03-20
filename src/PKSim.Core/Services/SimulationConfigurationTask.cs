using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Utility.Extensions;
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
      private readonly IParameterDefaultStateUpdater _parameterDefaultStateUpdater;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IModelObserverQuery _modelObserverQuery;
      private readonly IModelPassiveTransportQuery _modelPassiveTransportQuery;
      private readonly IMoleculesAndReactionsCreator _moleculesAndReactionsCreator;
      private readonly IPKSimParameterStartValuesCreator _parameterStartValuesCreator;
      private readonly IPKSimSpatialStructureFactory _spatialStructureFactory;

      public SimulationConfigurationTask(
         IPKSimSpatialStructureFactory spatialStructureFactory, IModelObserverQuery modelObserverQuery,
         IModelPassiveTransportQuery modelPassiveTransportQuery, IPKSimParameterStartValuesCreator parameterStartValuesCreator,
         IMoleculesAndReactionsCreator moleculesAndReactionsCreator, IEventBuildingBlockCreator eventBuildingBlockCreator,
         IPKSimMoleculeStartValuesCreator moleculeStartValuesCreator, IMoleculeCalculationRetriever moleculeCalculationRetriever,
         IDistributedParameterToTableParameterConverter distributedParameterToTableParameterConverter,
         IParameterDefaultStateUpdater parameterDefaultStateUpdater,
         IObjectBaseFactory objectBaseFactory)
      {
         _moleculeCalculationRetriever = moleculeCalculationRetriever;
         _distributedParameterToTableParameterConverter = distributedParameterToTableParameterConverter;
         _parameterDefaultStateUpdater = parameterDefaultStateUpdater;
         _objectBaseFactory = objectBaseFactory;
         _spatialStructureFactory = spatialStructureFactory;
         _modelObserverQuery = modelObserverQuery;
         _modelPassiveTransportQuery = modelPassiveTransportQuery;
         _parameterStartValuesCreator = parameterStartValuesCreator;
         _moleculesAndReactionsCreator = moleculesAndReactionsCreator;
         _eventBuildingBlockCreator = eventBuildingBlockCreator;
         _moleculeStartValuesCreator = moleculeStartValuesCreator;
         _moleculeCalculationRetriever = moleculeCalculationRetriever;
      }

      public SimulationConfiguration CreateFor(Simulation simulation, bool shouldValidate, bool createAgingDataInSimulation)
      {
         var module = _objectBaseFactory.Create<Module>().WithName(simulation.Name);
         var simulationConfiguration = new SimulationConfiguration
         {
            ShouldValidate = shouldValidate, 
            Module = module
         };
         var individual = simulation.Individual;

         //STEP1: Create spatial structure
         module.SpatialStructure = _spatialStructureFactory.CreateFor(individual, simulation);
         _parameterDefaultStateUpdater.UpdateDefaultFor(simulationConfiguration.SpatialStructure);

         //STEP2: add used calculation method to the build configuration
         _moleculeCalculationRetriever.AllMoleculeCalculationMethodsUsedBy(simulation).Each(simulationConfiguration.AddCalculationMethod);

         //STEP3: Add Passive Transports 
         module.PassiveTransport = _modelPassiveTransportQuery.AllPassiveTransportsFor(simulation)
            .WithName(simulation.Name);

         //STEP4 : Molecules, and Molecule Start and reactions are generated in one go from the molecule and reaction creator
         _moleculesAndReactionsCreator.CreateFor(simulationConfiguration, simulation);

         //STEP5 Add Application, Formulation and events
         module.EventGroup = _eventBuildingBlockCreator.CreateFor(simulation);
         _parameterDefaultStateUpdater.UpdateDefaultFor(simulationConfiguration.EventGroups);

         //STEP6: Add Observers
         module.Observer = _modelObserverQuery.AllObserversFor(module.Molecule, simulation);

         //STEP7 once all building blocks have been created, we need to create the default parameter and molecule values values 
         module.AddMoleculeStartValueBlock(_moleculeStartValuesCreator.CreateFor(simulationConfiguration, simulation));
         module.AddParameterStartValueBlock(_parameterStartValuesCreator.CreateFor(simulationConfiguration, simulation));

         //STEP8 update simulation settings
         simulationConfiguration.SimulationSettings = simulation.Settings.WithName(simulation.Name);

         //STEP9 Convert all parameters to table if required
         _distributedParameterToTableParameterConverter.UpdateSimulationConfigurationForAging(simulationConfiguration, simulation, createAgingDataInSimulation);

         return simulationConfiguration;
      }
   }
}