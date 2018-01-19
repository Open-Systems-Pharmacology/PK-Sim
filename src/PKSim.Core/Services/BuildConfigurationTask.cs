using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core.Services
{
   public interface IBuildConfigurationTask
   {
      /// <summary>
      ///    Create a build configuration based on the selected building blocks and model properties from the simulation
      /// </summary>
      /// <param name="simulation">Simulation used to create a building block configuration</param>
      /// <param name="shouldValidate">True if validation should be performed otherwise false</param>
      /// <param name="createAgingDataInSimulation">True if aging data should be created in the simulation. False if they should stay as is (typically for MoBi Export)</param>
      IBuildConfiguration CreateFor(Simulation simulation, bool shouldValidate, bool createAgingDataInSimulation);

   }

   public class BuildConfigurationTask : IBuildConfigurationTask
   {
      private readonly IEventBuildingBlockCreator _eventBuildingBlockCreator;
      private readonly IPKSimMoleculeStartValuesCreator _moleculeStartValuesCreator;
      private readonly IMoleculeCalculationRetriever _moleculeCalculationRetriever;
      private readonly IDistributedParameterToTableParameterConverter _distributedParameterToTableParameterConverter;
      private readonly IParameterDefaultStateUpdater _parameterDefaultStateUpdater;
      private readonly IModelObserverQuery _modelObserverQuery;
      private readonly IModelPassiveTransportQuery _modelPassiveTransportQuery;
      private readonly IMoleculesAndReactionsCreator _moleculesAndReactionsCreator;
      private readonly IPKSimParameterStartValuesCreator _parameterStartValuesCreator;
      private readonly IPKSimSpatialStructureFactory _spatialStructureFactory;

      public BuildConfigurationTask(IPKSimSpatialStructureFactory spatialStructureFactory, IModelObserverQuery modelObserverQuery,
                                    IModelPassiveTransportQuery modelPassiveTransportQuery, IPKSimParameterStartValuesCreator parameterStartValuesCreator,
                                    IMoleculesAndReactionsCreator moleculesAndReactionsCreator, IEventBuildingBlockCreator eventBuildingBlockCreator,
                                    IPKSimMoleculeStartValuesCreator moleculeStartValuesCreator, IMoleculeCalculationRetriever moleculeCalculationRetriever,
                                    IDistributedParameterToTableParameterConverter distributedParameterToTableParameterConverter,
                                    IParameterDefaultStateUpdater parameterDefaultStateUpdater)
      {
         _moleculeCalculationRetriever = moleculeCalculationRetriever;
         _distributedParameterToTableParameterConverter = distributedParameterToTableParameterConverter;
         _parameterDefaultStateUpdater = parameterDefaultStateUpdater;
         _spatialStructureFactory = spatialStructureFactory;
         _modelObserverQuery = modelObserverQuery;
         _modelPassiveTransportQuery = modelPassiveTransportQuery;
         _parameterStartValuesCreator = parameterStartValuesCreator;
         _moleculesAndReactionsCreator = moleculesAndReactionsCreator;
         _eventBuildingBlockCreator = eventBuildingBlockCreator;
         _moleculeStartValuesCreator = moleculeStartValuesCreator;
         _moleculeCalculationRetriever = moleculeCalculationRetriever;
      }

      public IBuildConfiguration CreateFor(Simulation simulation, bool shouldValidate, bool createAgingDataInSimulation)
      {
         var buildConfiguration = new BuildConfiguration {ShouldValidate = shouldValidate};
         var individual = simulation.Individual;

         //STEP1: Create spatial structure
         buildConfiguration.SpatialStructure = _spatialStructureFactory.CreateFor(individual, simulation);
         _parameterDefaultStateUpdater.UpdateDefaultFor(buildConfiguration.SpatialStructure);

         //STEP2: add used calculation method to the build configuration
         _moleculeCalculationRetriever.AllMoleculeCalculationMethodsUsedBy(simulation).Each(buildConfiguration.AddCalculationMethod);

         //STEP3: Add Passive Transports 
         buildConfiguration.PassiveTransports =_modelPassiveTransportQuery.AllPassiveTransportsFor(simulation)
            .WithName(simulation.Name);

         //STEP4 : Molecules, and Molecule Start and reactions are generated in one go from the molecule and reaction creator
         _moleculesAndReactionsCreator.CreateFor(buildConfiguration, simulation);

         //STEP5 Add Application, Formulation and events
         buildConfiguration.EventGroups = _eventBuildingBlockCreator.CreateFor(simulation);
         _parameterDefaultStateUpdater.UpdateDefaultFor(buildConfiguration.EventGroups);

         //STEP6: Add Observers
         buildConfiguration.Observers = _modelObserverQuery.AllObserversFor(buildConfiguration.Molecules, simulation);

         //STEP7 once all building blocks have been created, we need to create the default parameter and molecule values values 
         buildConfiguration.MoleculeStartValues = _moleculeStartValuesCreator.CreateFor(buildConfiguration, simulation);
         buildConfiguration.ParameterStartValues = _parameterStartValuesCreator.CreateFor(buildConfiguration, simulation);

         //STEP8 update simulation settings
         buildConfiguration.SimulationSettings = simulation.SimulationSettings.WithName(simulation.Name);

         //STEP9 Convert all parameters to table if required
         _distributedParameterToTableParameterConverter.UpdateBuildConfigurationForAging(buildConfiguration, simulation, createAgingDataInSimulation);

         return buildConfiguration;
      }
   }
}