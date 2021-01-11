using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationModelCreator
   {
      /// <summary>
      ///    Creates and sets the model based on the simulation configuration defined in the simulation
      /// </summary>
      void CreateModelFor(Simulation simulation, bool shouldValidate = true, bool shouldShowProgress = false);
   }

   public class SimulationModelCreator : ISimulationModelCreator
   {
      private readonly IBuildConfigurationTask _buildConfigurationTask;
      private readonly IModelConstructor _modelConstructor;
      private readonly IParameterIdUpdater _parameterIdUpdater;
      private readonly ISimulationSettingsFactory _simulationSettingsFactory;
      private readonly ISimulationPersistableUpdater _simulationPersistableUpdater;
      private readonly ISimulationConfigurationValidator _simulationConfigurationValidator;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IContainerTask _containerTask;

      public SimulationModelCreator(IBuildConfigurationTask buildConfigurationTask, 
         IModelConstructor modelConstructor, 
         IParameterIdUpdater parameterIdUpdater,
         ISimulationSettingsFactory simulationSettingsFactory, 
         ISimulationPersistableUpdater simulationPersistableUpdater, 
         ISimulationConfigurationValidator simulationConfigurationValidator,
         IEntityPathResolver entityPathResolver,
         IContainerTask containerTask)
      {
         _buildConfigurationTask = buildConfigurationTask;
         _modelConstructor = modelConstructor;
         _parameterIdUpdater = parameterIdUpdater;
         _simulationSettingsFactory = simulationSettingsFactory;
         _simulationPersistableUpdater = simulationPersistableUpdater;
         _simulationConfigurationValidator = simulationConfigurationValidator;
         _entityPathResolver = entityPathResolver;
         _containerTask = containerTask;
      }

      public void CreateModelFor(Simulation simulation, bool shouldValidate = true, bool shouldShowProgress = false)
      {
         _simulationConfigurationValidator.ValidateConfigurationFor(simulation);

         simulation.SimulationSettings = _simulationSettingsFactory.CreateFor(simulation);
         var buildConfiguration = _buildConfigurationTask.CreateFor(simulation, shouldValidate, createAgingDataInSimulation: true);
         buildConfiguration.ShowProgress = shouldShowProgress;
         buildConfiguration.ShouldValidate = shouldValidate;

         var creationResult = _modelConstructor.CreateModelFrom(buildConfiguration, simulation.Name);

         if (creationResult.IsInvalid)
            throw new CannotCreateSimulationException(creationResult.ValidationResult);

         simulation.Model = creationResult.Model;
         simulation.Reactions = buildConfiguration.Reactions;

         updateSimulationAfterModelCreation(simulation);
      }

      private void updateSimulationAfterModelCreation(Simulation simulation)
      {
         //last step. Once the model has been created, it is necessary to set the id of the simulation 
         //in all parameter defined in the model
         _parameterIdUpdater.UpdateSimulationId(simulation);

         //local molecule parameters parameter id need to be updated as well
         var allParameters = _containerTask.CacheAllChildren<IParameter>(simulation.Model.Root);
         var individual = simulation.Individual;

         individual.AllMolecules().Each(m =>
         {
            var allMoleculeParameters = individual.AllMoleculeParametersFor(m);
            allMoleculeParameters.Each(p =>
            {
               var simulationParameter = allParameters[_entityPathResolver.PathFor(p)];
               if (simulationParameter != null)
                  updateFromIndividualParameter(simulationParameter, p, individual);
            });
         });

         //we need to update the observer types according to their location (container observer are always of type drug, amount observer are depending on parent)
         var allObservers = simulation.All<IObserver>();
         foreach (var observer in allObservers)
         {
            var quantity = observer.ParentContainer as IQuantity;
            if (quantity != null)
               observer.QuantityType = QuantityType.Observer | quantity.QuantityType;
            else
               observer.QuantityType = QuantityType.Observer | QuantityType.Drug;
         }

         _simulationPersistableUpdater.ResetPersistable(simulation);
      }


      private void updateFromIndividualParameter(IParameter parameterToUpdate, IParameter parameterInIndividual, Individual individual)
      {
         _parameterIdUpdater.UpdateParameterId(parameterInIndividual, parameterToUpdate);
         parameterToUpdate.BuildingBlockType = PKSimBuildingBlockType.Individual;
         parameterToUpdate.Origin.BuilingBlockId = individual.Id;
         parameterToUpdate.Visible = parameterInIndividual.Visible;
         //This ensures that default value set to NaN (for example relative expression localized) have the expected default value from the individual
         parameterToUpdate.DefaultValue = parameterInIndividual.DefaultValue;
      }
   }
}