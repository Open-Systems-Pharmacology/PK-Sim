using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
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
      private readonly ISimulationConfigurationTask _simulationConfigurationTask;
      private readonly IModelConstructor _modelConstructor;
      private readonly IParameterIdUpdater _parameterIdUpdater;
      private readonly ISimulationSettingsFactory _simulationSettingsFactory;
      private readonly ISimulationPersistableUpdater _simulationPersistableUpdater;
      private readonly ISimulationConfigurationValidator _simulationConfigurationValidator;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IContainerTask _containerTask;

      public SimulationModelCreator(ISimulationConfigurationTask simulationConfigurationTask,
         IModelConstructor modelConstructor,
         IParameterIdUpdater parameterIdUpdater,
         ISimulationSettingsFactory simulationSettingsFactory,
         ISimulationPersistableUpdater simulationPersistableUpdater,
         ISimulationConfigurationValidator simulationConfigurationValidator,
         IEntityPathResolver entityPathResolver,
         IContainerTask containerTask)
      {
         _simulationConfigurationTask = simulationConfigurationTask;
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

         simulation.Settings = _simulationSettingsFactory.CreateFor(simulation);
         var simulationConfiguration = _simulationConfigurationTask.CreateFor(simulation, shouldValidate, createAgingDataInSimulation: true);
         simulationConfiguration.ShowProgress = shouldShowProgress;
         simulationConfiguration.ShouldValidate = shouldValidate;

         var creationResult = _modelConstructor.CreateModelFrom(simulationConfiguration, simulation.Name);

         if (creationResult.IsInvalid)
            throw new CannotCreateSimulationException(creationResult.ValidationResult);

         simulation.Model = creationResult.Model;
         simulation.UpdateReactions(simulationConfiguration.All<ReactionBuildingBlock>());

         updateSimulationAfterModelCreation(simulation);
      }

      private void updateSimulationAfterModelCreation(Simulation simulation)
      {
         
         var individual = simulation.Individual;

         var allIndividualParameters = _containerTask.CacheAllChildren<IParameter>(individual);
         var allSimulationParameters = _containerTask.CacheAllChildren<IParameter>(simulation.Model.Root);

         //Once the model has been created, it is necessary to update origin to ensure that references are set properly
         //Simulation id for all parameters
         _parameterIdUpdater.UpdateSimulationId(simulation);
         //Parameter Ids for all parameters coming from individuals (as some where created in the spatial structure independently from the individual)
         _parameterIdUpdater.UpdateParameterIds(allIndividualParameters, allSimulationParameters);
         //Building block id for the same parameters
         _parameterIdUpdater.UpdateBuildingBlockId(allSimulationParameters, individual);
         
         //local molecule parameters parameter id need to be updated as well
         individual.AllMolecules().Each(m =>
         {
            var allMoleculeParameters = individual.AllMoleculeParametersFor(m);
            allMoleculeParameters.Each(p =>
            {
               var simulationParameter = allSimulationParameters[_entityPathResolver.PathFor(p)];
               if (simulationParameter != null)
                  updateFromIndividualParameter(simulationParameter, p, individual);
            });
         });

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