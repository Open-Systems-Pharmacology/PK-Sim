using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;

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
      private readonly ISimulationConfigurationValidator _simulationConfigurationValidator;

      public SimulationModelCreator(IBuildConfigurationTask buildConfigurationTask, 
         IModelConstructor modelConstructor, 
         IParameterIdUpdater parameterIdUpdater,
         IEntityPathResolver entityPathResolver, 
         IExpressionContainersRetriever expressionContainersRetriever,
         ISimulationSettingsFactory simulationSettingsFactory, 
         ISimulationPersistableUpdater simulationPersistableUpdater, 
         ISimulationConfigurationValidator simulationConfigurationValidator)
      {
         _buildConfigurationTask = buildConfigurationTask;
         _modelConstructor = modelConstructor;
         _parameterIdUpdater = parameterIdUpdater;
         _entityPathResolver = entityPathResolver;
         _expressionContainersRetriever = expressionContainersRetriever;
         _simulationSettingsFactory = simulationSettingsFactory;
         _simulationPersistableUpdater = simulationPersistableUpdater;
         _simulationConfigurationValidator = simulationConfigurationValidator;
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
         //in all paramater defined in the model
         _parameterIdUpdater.UpdateSimulationId(simulation);

         var allMoleculeAmounts = simulation.All<IMoleculeAmount>().ToList();

         //local molecule parameters parameter id need to be updated as well
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

         allNaNParametersFromMolecules.Each(hideParameter);
      }

      private void updateAllParametersFor(Simulation simulation, Individual individual, IList<IContainer> allOrganismContainers, IndividualMolecule molecule, PathCache<IMoleculeAmount> moleculeAmountPath)
      {
         var globalMoleculeContainer = simulation.Model.Root
            .GetSingleChildByName<IContainer>(molecule.Name);

         hideParameter(globalMoleculeContainer.Parameter(CoreConstants.Parameter.ONTOGENY_FACTOR_GI));
         hideParameter(globalMoleculeContainer.Parameter(CoreConstants.Parameter.ONTOGENY_FACTOR));

         hideGlobalParametersForUndefinedMolecule(globalMoleculeContainer);

         hideInterstialParametersForIntracellularLocalizationInTissue(individual,  molecule, allOrganismContainers, moleculeAmountPath);

         foreach (var expressionContainer in molecule.AllExpressionsContainers())
         {
            if (expressionContainer.IsSurrogate())
            {
               updateReferenceToIndividualParametersForSurrogateContainer(individual, molecule, expressionContainer, globalMoleculeContainer);
            }
            else
            {
               updateReferenceToIndividualParametersForStandardContainer(individual, molecule, expressionContainer, allOrganismContainers, moleculeAmountPath);
            }
         }
      }

      private void hideInterstialParametersForIntracellularLocalizationInTissue(Individual individual, IndividualMolecule molecule, IList<IContainer> allOrganismContainers, PathCache<IMoleculeAmount> moleculeAmountPath)
      {
         var protein = molecule as IndividualProtein;

         if (protein?.TissueLocation != TissueLocation.Intracellular || protein.IntracellularVascularEndoLocation != IntracellularVascularEndoLocation.Interstitial)
            return;

         var vascularEndothelium = protein.ExpressionContainer(CoreConstants.Compartment.VascularEndothelium);
         if (vascularEndothelium == null)
            return;

         var allInterstitialContainer = _expressionContainersRetriever.AllContainersFor(individual.Organism, allOrganismContainers, protein, vascularEndothelium);
      
         foreach (var interstitial in allInterstitialContainer)
         {
            var amount = amountFor(interstitial, molecule, moleculeAmountPath);
            if (amount == null) continue;

            hideParameter(amount.Parameter(CoreConstants.Parameter.REL_EXP));
            hideParameter(amount.Parameter(CoreConstants.Parameter.REL_EXP_NORM));
         }
      }

      private void hideParameter(IParameter parameter)
      {
         if (parameter == null)
            return;

         parameter.Visible = false;
      }

      private void updateReferenceToIndividualParametersForStandardContainer(Individual individual, IndividualMolecule molecule, MoleculeExpressionContainer expressionContainer, IList<IContainer> allOrganismContainers, PathCache<IMoleculeAmount> moleculeAmountPath)
      {
         var allContainers = _expressionContainersRetriever.AllContainersFor(individual.Organism, allOrganismContainers, molecule, expressionContainer);

         foreach (var container in allContainers)
         {
            var amount = amountFor(container, molecule, moleculeAmountPath);
            if (amount == null) continue;

            updateFromIndividualParameter(amount.Parameter(CoreConstants.Parameter.REL_EXP), expressionContainer.RelativeExpressionParameter, individual, molecule);
            updateFromIndividualParameter(amount.Parameter(CoreConstants.Parameter.REL_EXP_NORM), expressionContainer.RelativeExpressionNormParameter, individual, molecule);
         }
      }

      private IMoleculeAmount amountFor(IContainer container, IndividualMolecule molecule, PathCache<IMoleculeAmount> moleculeAmountPath)
      {
         var amount = moleculeAmountPath[_entityPathResolver.ObjectPathFor(container).AndAdd(molecule.Name).ToString()];
         return amount;
      }

      private void updateReferenceToIndividualParametersForSurrogateContainer(Individual individual, IndividualMolecule molecule, MoleculeExpressionContainer expressionContainer, IContainer globalMoleculeContainer)
      {
         string relExpName;
         if (expressionContainer.IsBloodCell())
            relExpName = CoreConstants.Parameter.REL_EXP_BLOOD_CELL;
         else if (expressionContainer.IsPlasma())
            relExpName = CoreConstants.Parameter.REL_EXP_PLASMA;
         else if (expressionContainer.IsVascularEndothelium())
            relExpName = CoreConstants.Parameter.REL_EXP_VASC_ENDO;
         else
            return;

         var relExpNormName = CoreConstants.Parameter.NormParameterFor(relExpName);
         updateFromIndividualParameter(globalMoleculeContainer.Parameter(relExpName), expressionContainer.RelativeExpressionParameter, individual, molecule);
         updateFromIndividualParameter(globalMoleculeContainer.Parameter(relExpNormName), expressionContainer.RelativeExpressionNormParameter, individual, molecule);
      }

      private void hideGlobalParametersForUndefinedMolecule(IContainer globalMoleculeContainer)
      {
         if (!globalMoleculeContainer.IsUndefinedMolecule())
            return;

         globalMoleculeContainer.GetAllChildren<IParameter>().Each(hideParameter);
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