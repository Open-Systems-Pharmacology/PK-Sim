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

      /// <summary>
      ///    Validate the configuration defined in the simulation.
      /// </summary>
      /// <param name="simulation">Simulation to validate</param>
      /// <exception cref="InvalidSimulationConfigurationException">is thrown if the configuration is not valid</exception>
      void ValidateConfigurationFor(Simulation simulation);
   }

   public class BuildConfigurationTask : IBuildConfigurationTask
   {
      private readonly IEventBuildingBlockCreator _eventBuildingBlockCreator;
      private readonly IPKSimMoleculeStartValuesCreator _moleculeStartValuesCreator;
      private readonly IMoleculeCalculationRetriever _moleculeCalculationRetriever;
      private readonly IDistributedParameterToTableParameterConverter _distributedParameterToTableParameterConverter;
      private readonly IModelObserverQuery _modelObserverQuery;
      private readonly IModelPassiveTransportQuery _modelPassiveTransportQuery;
      private readonly IMoleculesAndReactionsCreator _moleculesAndReactionsCreator;
      private readonly IPKSimParameterStartValuesCreator _parameterStartValuesCreator;
      private readonly IPKSimSpatialStructureFactory _spatialStructureFactory;

      public BuildConfigurationTask(IPKSimSpatialStructureFactory spatialStructureFactory, IModelObserverQuery modelObserverQuery,
                                    IModelPassiveTransportQuery modelPassiveTransportQuery, IPKSimParameterStartValuesCreator parameterStartValuesCreator,
                                    IMoleculesAndReactionsCreator moleculesAndReactionsCreator, IEventBuildingBlockCreator eventBuildingBlockCreator,
                                    IPKSimMoleculeStartValuesCreator moleculeStartValuesCreator, IMoleculeCalculationRetriever moleculeCalculationRetriever,
                                    IDistributedParameterToTableParameterConverter distributedParameterToTableParameterConverter)
      {
         _moleculeCalculationRetriever = moleculeCalculationRetriever;
         _distributedParameterToTableParameterConverter = distributedParameterToTableParameterConverter;
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

         //STEP2: add used calculation method to the build configuration
         _moleculeCalculationRetriever.AllMoleculeCalculationMethodsUsedBy(simulation).Each(buildConfiguration.AddCalculationMethod);

         //STEP3: Add Passive Transports 
         buildConfiguration.PassiveTransports =_modelPassiveTransportQuery.AllPassiveTransportsFor(simulation)
            .WithName(simulation.Name);

         //STEP4 : Molecules, and Molecule Start and reactions are generated in one go from the molecule and reaction creator
         _moleculesAndReactionsCreator.CreateFor(buildConfiguration, simulation);

         //STEP5 Add Application, Formulation and events
         buildConfiguration.EventGroups = _eventBuildingBlockCreator.CreateFor(simulation);

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

      public void ValidateConfigurationFor(Simulation simulation)
      {
         var compounds = simulation.Compounds;
         var individual = simulation.Individual;
         var modelConfiguration = simulation.ModelConfiguration;

         //ForComp model can only be used with small molecules
         if (string.Equals(modelConfiguration.ModelName ,CoreConstants.Model.FourComp) && compounds.Any(x=>!x.IsSmallMolecule))
            throw new InvalidSimulationConfigurationException(PKSimConstants.Error.FourCompModelCannotBeUsedWithLargeMolecule);

         if (simulation.NameIsOneOf(compounds.AllNames()))
            throw new InvalidSimulationConfigurationException(PKSimConstants.Error.CompoundAndSimulationCannotShareTheSameName);

         if(simulation.NameIsOneOf(individual.AllMolecules().AllNames()))
            throw new InvalidSimulationConfigurationException(PKSimConstants.Error.IndividualMoleculesAnSimulationCannotShareTheSameName);
      }
   }
}