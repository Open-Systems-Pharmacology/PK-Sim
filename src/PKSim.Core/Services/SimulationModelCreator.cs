using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Services
{
   public interface ISimulationModelCreator
   {
      /// <summary>
      ///    Creates and sets the model based on the simlation configuration defined in the simulaton
      /// </summary>
      void CreateModelFor(Simulation simulation, bool shouldValidate = true, bool shouldShowProgress = false);
   }

   public class SimulationModelCreator : ISimulationModelCreator
   {
      private readonly IBuildConfigurationTask _buildConfigurationTask;
      private readonly IModelConstructor _modelConstructor;
      private readonly IParameterIdUpdater _parameterIdUpdater;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IExpressionContainersRetriever _expressionContainersRetriever;
      private readonly ISimulationSettingsFactory _simulationSettingsFactory;
      private readonly ISimulationPersistableUpdater _simulationPersistableUpdater;

      public SimulationModelCreator(IBuildConfigurationTask buildConfigurationTask, IModelConstructor modelConstructor, IParameterIdUpdater parameterIdUpdater,
                                    IEntityPathResolver entityPathResolver, IExpressionContainersRetriever expressionContainersRetriever,
                                    ISimulationSettingsFactory  simulationSettingsFactory, ISimulationPersistableUpdater simulationPersistableUpdater)
      {
         _buildConfigurationTask = buildConfigurationTask;
         _modelConstructor = modelConstructor;
         _parameterIdUpdater = parameterIdUpdater;
         _entityPathResolver = entityPathResolver;
         _expressionContainersRetriever = expressionContainersRetriever;
         _simulationSettingsFactory = simulationSettingsFactory;
         _simulationPersistableUpdater = simulationPersistableUpdater;
      }

      public void CreateModelFor(Simulation simulation, bool shouldValidate = true, bool shouldShowProgress = false)
      {
         _buildConfigurationTask.ValidateConfigurationFor(simulation);

         simulation.SimulationSettings = _simulationSettingsFactory.CreateFor(simulation);
         var buildConfiguration = _buildConfigurationTask.CreateFor(simulation, shouldValidate, createAgingDataInSimulation:true);
         buildConfiguration.ShowProgress = shouldShowProgress;
         buildConfiguration.ShouldValidate =  shouldValidate;

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
         //in all paramater defined in the model
         _parameterIdUpdater.UpdateSimulationId(simulation);

         var allMoleculeAmounts = simulation.All<IMoleculeAmount>().ToList();

         //local protein parameters parameter id need to be updated as well
         var allIndividualMoleculeAmounts = allMoleculeAmounts.Where(m => m.IsIndividualMolecule());
         var individual = simulation.Individual;
         var allOrganismContainers = _expressionContainersRetriever.AllOrganismContainers(individual).ToList();

         hideAllUndefinedParameterForMolecules(simulation);

         foreach (var moleculeAmountGroup in allIndividualMoleculeAmounts.GroupBy(x => x.Name))
         {
            var molecule = individual.MoleculeByName<IndividualMolecule>(moleculeAmountGroup.Key);
            var moleculeAmountPath = new PathCache<IMoleculeAmount>(_entityPathResolver).For(moleculeAmountGroup);
            updateAllParametersFor(simulation, individual, allOrganismContainers, molecule, moleculeAmountPath);
         }

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

      private void hideAllUndefinedParameterForMolecules(Simulation simulation)
      {
         var allNaNParametersFromMolecules = simulation.Model.Root.GetAllChildren<IContainer>()
            .Where(c => c.ContainerType == ContainerType.Molecule)
            .SelectMany(c => c.AllParameters(p => double.IsNaN(p.Value)));

         allNaNParametersFromMolecules.Each(p => p.Visible = false);
      }

      private void updateAllParametersFor(Simulation simulation, Individual individual, IList<IContainer> allOrganismContainers, IndividualMolecule molecule, PathCache<IMoleculeAmount> moleculeAmountPath)
      {
         var globalMoleculeContainer = simulation.Model.Root
            .GetSingleChildByName<IContainer>(molecule.Name);

         globalMoleculeContainer.Parameter(CoreConstants.Parameter.ONTOGENY_FACTOR_GI).Visible = false;
         globalMoleculeContainer.Parameter(CoreConstants.Parameter.ONTOGENY_FACTOR).Visible = false;

         hideGlobalParametersForUndefinedMolecule(globalMoleculeContainer);

         foreach (var expressionContainer in molecule.AllExpressionsContainers())
         {
            var relExpNorm = expressionContainer.RelativeExpressionNormParameter;
            var relExp = expressionContainer.RelativeExpressionParameter;

            if (expressionContainer.IsSurrogate())
            {
               string relExpName;
               if (expressionContainer.IsBloodCell())
                  relExpName = CoreConstants.Parameter.RelExpBloodCell;
               else if (expressionContainer.IsPlasma())
                  relExpName = CoreConstants.Parameter.RelExpPlasma;
               else if (expressionContainer.IsVascularEndothelium())
                  relExpName = CoreConstants.Parameter.RelExpVascEndo;
               else
                  continue;

               string relExpNormName = CoreConstants.Parameter.NormParameterFor(relExpName);
               updateFromIndividualParameter(globalMoleculeContainer.Parameter(relExpName), relExp, individual, molecule);
               updateFromIndividualParameter(globalMoleculeContainer.Parameter(relExpNormName), relExpNorm, individual, molecule);
            }
            else
            {
               //not a global parameter simply update the norm  parameters
               var allContainers = _expressionContainersRetriever.AllContainersFor(individual.Organism, allOrganismContainers, molecule, expressionContainer);
               foreach (var container in allContainers)
               {
                  var amount = moleculeAmountPath[_entityPathResolver.ObjectPathFor(container).AndAdd(molecule.Name).ToString()];
                  if (amount == null) continue;

                  updateFromIndividualParameter(amount.Parameter(CoreConstants.Parameter.RelExpNorm), relExpNorm, individual, molecule);
                  updateFromIndividualParameter(amount.Parameter(CoreConstants.Parameter.RelExp), relExp, individual, molecule);
               }
            }
         }
      }

      private void hideGlobalParametersForUndefinedMolecule(IContainer globalMoleculeContainer)
      {
         if (!globalMoleculeContainer.IsUndefinedMolecule())
            return;

         globalMoleculeContainer.GetAllChildren<IParameter>().Each(x=>x.Visible=false);
      }

      private void updateFromIndividualParameter(IParameter parameterToUpdate, IParameter parameterInIndividual, Individual individual, IndividualMolecule molecule)
      {
         //undefined molecule parameters are always hidden
         updateFromIndividualParameter(parameterToUpdate, parameterInIndividual, individual, !molecule.IsUndefinedMolecule());
      }

      private void updateFromIndividualParameter(IParameter parameterToUpdate, IParameter parameterInIndividual, Individual individual, bool visible)
      {
         _parameterIdUpdater.UpdateParameterId(parameterInIndividual, parameterToUpdate);
         parameterToUpdate.BuildingBlockType = PKSimBuildingBlockType.Individual;
         parameterToUpdate.Origin.BuilingBlockId = individual.Id;
         parameterToUpdate.Visible = visible;
      }
   }
}