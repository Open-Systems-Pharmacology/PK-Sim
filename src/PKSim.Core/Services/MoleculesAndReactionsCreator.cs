using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using IMoleculeBuilderFactory = PKSim.Core.Model.IMoleculeBuilderFactory;

namespace PKSim.Core.Services
{
   public interface IMoleculesAndReactionsCreator
   {
      /// <summary>
      ///    Create Molecule building block based on the selected individual and compound.
      ///    One molecule will be created for the compound.
      ///    One active transport for each active transport process taking place in the individual for the compound.
      ///    One enzyme/metabolite pair for each metabolization process taking place in the individual for the compound.
      ///    Once the molecules have been created the reaction taking place in the system will be added
      ///    Last, the molecule start values will be set
      /// </summary>
      void CreateFor(IBuildConfiguration buildConfiguration, Simulation simulation);
   }

   public class MoleculesAndReactionsCreator : IMoleculesAndReactionsCreator
   {
      private readonly IMoleculeBuilderFactory _moleculeBuilderFactory;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IParameterSetUpdater _parameterSetUpdater;
      private readonly IIndividualEnzymeFactory _individualEnzymeFactory;
      private readonly IIndividualTransporterFactory _individualTransporterFactory;
      private readonly IModelContainerMoleculeRepository _modelContainerMoleculeRepo;
      private readonly IStaticReactionRepository _staticReactionRepository;
      private readonly IParameterIdUpdater _parameterIdUpdater;
      private readonly IProcessToProcessBuilderMapper _processBuilderMapper;

      private Individual _individual;
      private IMoleculeBuildingBlock _moleculeBuildingBlock;
      private IReactionBuildingBlock _reactionBuildingBlock;
      private IBuildConfiguration _buildConfiguration;
      private readonly List<string> _allMoleculeNames;
      private readonly List<IMoleculeBuilder> _moleculeWithTurnoverReactions;
      private IPassiveTransportBuildingBlock _passiveTransports;
      private readonly IMoleculeCalculationRetriever _moleculeCalculationRetriever;
      private readonly IInteractionKineticUpdater _interactionKineticUpdater;
      private readonly IInteractionTask _interactionTask;
      private Simulation _simulation;

      public MoleculesAndReactionsCreator(IObjectBaseFactory objectBaseFactory,
         IMoleculeBuilderFactory moleculeBuilderFactory,
         IParameterIdUpdater parameterIdUpdater,
         IProcessToProcessBuilderMapper processBuilderMapper,
         IParameterSetUpdater parameterSetUpdater,
         IIndividualEnzymeFactory individualEnzymeFactory,
         IIndividualTransporterFactory individualTransporterFactory,
         IModelContainerMoleculeRepository modelContainerMoleculeRepo,
         IStaticReactionRepository staticReactionRepository,
         IMoleculeCalculationRetriever moleculeCalculationRetriever,
         IInteractionKineticUpdater interactionKineticUpdater,
         IInteractionTask interactionTask
         )

      {
         _objectBaseFactory = objectBaseFactory;
         _moleculeBuilderFactory = moleculeBuilderFactory;
         _parameterIdUpdater = parameterIdUpdater;
         _processBuilderMapper = processBuilderMapper;
         _parameterSetUpdater = parameterSetUpdater;
         _individualEnzymeFactory = individualEnzymeFactory;
         _individualTransporterFactory = individualTransporterFactory;
         _modelContainerMoleculeRepo = modelContainerMoleculeRepo;
         _staticReactionRepository = staticReactionRepository;
         _moleculeCalculationRetriever = moleculeCalculationRetriever;
         _interactionKineticUpdater = interactionKineticUpdater;
         _interactionTask = interactionTask;
         _allMoleculeNames = new List<string>();
         _moleculeWithTurnoverReactions = new List<IMoleculeBuilder>();
      }

      public void CreateFor(IBuildConfiguration buildConfiguration, Simulation simulation)
      {
         try
         {
            _simulation = simulation;
            _individual = simulation.Individual;
            _buildConfiguration = buildConfiguration;
            _passiveTransports = _buildConfiguration.PassiveTransports;

            _moleculeBuildingBlock = _objectBaseFactory.Create<IMoleculeBuildingBlock>()
               .WithName(simulation.Name);

            _reactionBuildingBlock = _objectBaseFactory.Create<IReactionBuildingBlock>()
               .WithName(simulation.Name);

            addIndividualMolecules(simulation.CompoundPropertiesList);

            addCompoundMolecules(simulation.CompoundPropertiesList, simulation.InteractionProperties);

            addTwoPoreModelsReactionAndMolecules(simulation.CompoundPropertiesList, simulation.ModelConfiguration);

            addPartialProcesses(simulation.CompoundPropertiesList);

            addInteractions(simulation);

            _buildConfiguration.Molecules = _moleculeBuildingBlock;
            _buildConfiguration.Reactions = _reactionBuildingBlock;
         }
         finally
         {
            _moleculeBuildingBlock = null;
            _reactionBuildingBlock = null;
            _individual = null;
            _buildConfiguration = null;
            _passiveTransports = null;
            _simulation = null;
            _allMoleculeNames.Clear();
            _moleculeWithTurnoverReactions.Clear();
         }
      }

      private void addPartialProcesses(IEnumerable<CompoundProperties> compoundPropertiesList)
      {
         foreach (var compoundProperties in compoundPropertiesList)
         {
            var compoundBuilder = _moleculeBuildingBlock[compoundProperties.Compound.Name];
            addPartialProcesses(compoundBuilder, x => x.MetabolizationSelection, compoundProperties, addMetabolismProcess);
            addPartialProcesses(compoundBuilder, x => x.SpecificBindingSelection, compoundProperties, addSpecificBindingProcess);
            addPartialProcesses(compoundBuilder, x => x.TransportAndExcretionSelection, compoundProperties, addTransportProcess);
         }
      }

      private void addTwoPoreModelsReactionAndMolecules(IReadOnlyList<CompoundProperties> compoundPropertiesList, ModelConfiguration modelConfiguration)
      {
         if (modelConfiguration == null)
            return;

         foreach (var compoundProperties in compoundPropertiesList)
         {
            var compoundBuilder = _moleculeBuildingBlock[compoundProperties.Compound.Name];

            addAdditionalStaticMolecules(modelConfiguration.ModelName, compoundProperties);
            addStaticReactions(compoundBuilder, modelConfiguration.ModelName);
         }
      }

      private void addStaticReactions(IMoleculeBuilder moleculeBuilder, string modelName)
      {
         foreach (var reaction in _staticReactionRepository.AllFor(modelName))
         {
            var reactionBuilder = _processBuilderMapper.ReactionFrom(reaction, moleculeBuilder.Name, _allMoleculeNames, _reactionBuildingBlock.FormulaCache);

            //some reactions are compound-independent (e.g. "FcRn binding endogenous Igg")
            //if we have >1 compounds, we must check that reaction was not added previously
            if (_reactionBuildingBlock.ExistsByName(reactionBuilder.Name))
               continue;

            _reactionBuildingBlock.Add(reactionBuilder);
         }
      }

      private void addAdditionalStaticMolecules(string modelName, CompoundProperties compoundProperties)
      {
         var templateMoleculeNames = _modelContainerMoleculeRepo.MoleculeNamesWithoutDrug(modelName);
         var compoundName = compoundProperties.Compound.Name;
         foreach (var templateMoleculeName in templateMoleculeNames)
         {
            var moleculeName = moleculeNameFor(templateMoleculeName, compoundName);

            // templateMoleculeNames contains both compound-dependent molecule names (e.g. Drug_FcRn_Complex)
            // and compound-independent molecule names (e.g. FcRn)
            // Compound-independent molecules must be added only once.
            // Thus if we have >1 compound, we have to check if molecule already exists and skip adding it if so.
            if (_allMoleculeNames.Contains(moleculeName))
               continue;

            var molecule = _moleculeBuilderFactory.Create(QuantityType.Undefined, _moleculeBuildingBlock.FormulaCache)
               .WithName(moleculeName);

            addMoleculeToBuildingBlock(molecule, compoundProperties);
         }
      }

      private string moleculeNameFor(string moleculeName, string compoundName)
      {
         return moleculeName.ReplaceKeywords(new[] {CoreConstants.Molecule.Drug, CoreConstants.Molecule.DrugFcRnComplexTemplate},
            new[] {compoundName, CoreConstants.Molecule.DrugFcRnComplexName(compoundName)});
      }

      private void addIndividualMolecules(IEnumerable<CompoundProperties> compoundPropertiesList)
      {
         //first of all: add undefined proteins that are created internaly and are necessary to use the systemic processes
         compoundPropertiesList.Select(x => x.Processes).Each(addUndefinedProteinForSystemicProcessesToIndividual);
         _individual.AllMolecules().Each(addMolecule);
      }

      private void addCompoundMolecules(IEnumerable<CompoundProperties> compoundPropertiesList, InteractionProperties interactionProperties)
      {
         compoundPropertiesList.Each(compoundProperties =>
         {
            var drug = _moleculeBuilderFactory.Create(compoundProperties.Compound, compoundProperties, interactionProperties, _moleculeBuildingBlock.FormulaCache);
            addMoleculeToBuildingBlock(drug, compoundProperties);
         });
      }

      private void addMolecule(IndividualMolecule individualMolecule)
      {
         var molecule = _moleculeBuilderFactory.Create(individualMolecule.MoleculeType, _moleculeBuildingBlock.FormulaCache)
            .WithName(individualMolecule.Name)
            .WithIcon(individualMolecule.Icon);

         addMoleculeToBuildingBlock(molecule, null);

         //Update protein builder parameters with the parameter ids
         _parameterSetUpdater.UpdateValuesByName(individualMolecule, molecule.Parameters);

         //Update the building block ids
         _parameterIdUpdater.UpdateBuildingBlockId(molecule.Parameters, _individual);
      }

      private void addMoleculeToBuildingBlock(IMoleculeBuilder moleculeBuilder, CompoundProperties compoundProperties)
      {
         if (_allMoleculeNames.Contains(moleculeBuilder.Name))
            throw new TwoMoleculesWithSameNameException(moleculeBuilder.Name);

         _allMoleculeNames.Add(moleculeBuilder.Name);

         //set the used calculation method according to current settings 
         if (compoundProperties != null)
            _moleculeCalculationRetriever.AllMoleculeCalculationMethodsUsedBy(compoundProperties).Each(moleculeBuilder.AddUsedCalculationMethod);

         _moleculeBuildingBlock.Add(moleculeBuilder);
      }

      private void addPartialProcesses(IMoleculeBuilder compoundBuilder, Func<CompoundProcessesSelection, ProcessSelectionGroup> processSelectionGroup, CompoundProperties compoundProperties, Action<IMoleculeBuilder, IReactionMapping, CompoundProperties> addPartialProcess)
      {
         foreach (var processSelection in  processSelectionGroup(compoundProperties.Processes).AllEnabledProcesses())
         {
            addPartialProcess(compoundBuilder, processSelection, compoundProperties);
         }
      }

      private void addUndefinedProteinForSystemicProcessesToIndividual(CompoundProcessesSelection compoundProcessesSelection)
      {
         var hepaticPlasmaClearance = compoundProcessesSelection.MetabolizationSelection.ProcessSelectionFor(SystemicProcessTypes.Hepatic);
         if (hepaticPlasmaClearance != null)
            addUndefinedLiver();

         var biliaryClearance = compoundProcessesSelection.TransportAndExcretionSelection.ProcessSelectionFor(SystemicProcessTypes.Biliary);
         if (biliaryClearance != null)
            addUndefinedLiverTransporter();
      }

      private void addUndefinedLiverTransporter()
      {
         var undefinedLiverTransporter = _individual.MoleculeByName<IndividualTransporter>(CoreConstants.Molecule.UndefinedLiverTransporter);
         //Already added
         if (undefinedLiverTransporter != null)
            return;

         undefinedLiverTransporter = _individualTransporterFactory.UndefinedLiverTransporterFor(_individual);
         _individual.AddMolecule(undefinedLiverTransporter);
      }

      private void addUndefinedLiver()
      {
         var undefinedLiver = _individual.MoleculeByName<IndividualEnzyme>(CoreConstants.Molecule.UndefinedLiver);
         //Already added
         if (undefinedLiver != null)
            return;

         undefinedLiver = _individualEnzymeFactory.UndefinedLiverFor(_individual);
         _individual.AddMolecule(undefinedLiver);
      }

      private void addMetabolismProcess(IMoleculeBuilder compoundBuilder, IReactionMapping compoundReactionMapping, CompoundProperties compoundProperties)
      {
         var compound = compoundProperties.Compound;
         var process = compound.ProcessByName(compoundReactionMapping.ProcessName);
         if (process == null)
            return;

         var metabolite = getOrCreateMetaboliteFor(compoundReactionMapping, compoundProperties);
         var reaction = _processBuilderMapper.MetabolismReactionFrom(process, compoundBuilder, metabolite, compoundReactionMapping.MoleculeName, _allMoleculeNames, _reactionBuildingBlock.FormulaCache);
         _interactionKineticUpdater.UpdateReaction(reaction, compoundReactionMapping.MoleculeName, compound.Name, _simulation, _reactionBuildingBlock.FormulaCache);
         addReactionToBuildingBlock(reaction, compound);
      }

      private IMoleculeBuilder getOrCreateMetaboliteFor(IReactionMapping compoundReactionMapping, CompoundProperties compoundProperties)
      {
         IMoleculeBuilder metabolite = null;
         var enzymaticProcess = compoundReactionMapping as EnzymaticProcessSelection;
         if (enzymaticProcess != null)
            metabolite = _moleculeBuildingBlock[enzymaticProcess.MetaboliteName];

         if (metabolite != null)
            return metabolite;

         metabolite = _moleculeBuilderFactory.Create(QuantityType.Metabolite, _moleculeBuildingBlock.FormulaCache)
            .WithName(compoundReactionMapping.ProductName(CoreConstants.Molecule.Metabolite));

         addMoleculeToBuildingBlock(metabolite, compoundProperties);
         return metabolite;
      }

      private void addSpecificBindingProcess(IMoleculeBuilder drug, IReactionMapping compoundReactionMapping, CompoundProperties compoundProperties)
      {
         var compound = compoundProperties.Compound;
         var process = compound.ProcessByName(compoundReactionMapping.ProcessName);
         var complex = _moleculeBuilderFactory.Create(QuantityType.Complex, _moleculeBuildingBlock.FormulaCache)
            .WithName(compoundReactionMapping.ProductName(CoreConstants.Molecule.Complex));

         addMoleculeToBuildingBlock(complex, compoundProperties);
         var complexReactionFrom = _processBuilderMapper.ComplexReactionFrom(process, drug, complex, compoundReactionMapping.MoleculeName, _allMoleculeNames, _reactionBuildingBlock.FormulaCache);
         addReactionToBuildingBlock(complexReactionFrom, compound);
      }

      private void addInteractions(Simulation simulation)
      {
         addInduction(simulation);
         addIrreversibleInhibition(simulation);
      }

      private void addInduction(Simulation simulation)
      {
         addInteraction(simulation, InteractionType.Induction, addInduction);
      }

      private void addIrreversibleInhibition(Simulation simulation)
      {
         addInteraction(simulation, InteractionType.IrreversibleInhibition, addIrreversibleBindings);
      }

      private void addInteraction(Simulation simulation, InteractionType interactionType, Action<IMoleculeBuilder, InteractionProcess> addInteractionAction)
      {
         if (!simulation.InteractionProperties.Any())
            return;

         foreach (var moleculeName in _allMoleculeNames)
         {
            var allInteractions = _interactionTask.AllInteractionProcessesFor(moleculeName, interactionType, simulation);
            if (!allInteractions.Any())
               continue;

            var molecule = _moleculeBuildingBlock[moleculeName];
            addTurnoverReactionIfNotDefinedAlreadyFor(molecule);
            allInteractions.Each(x => addInteractionAction(molecule, x));
         }
      }

      private void addIrreversibleBindings(IMoleculeBuilder molecule, InteractionProcess irreversibleInteraction)
      {
         var irreversibleBindingReaction = _processBuilderMapper.InactivationReactionFrom(irreversibleInteraction, molecule, _allMoleculeNames, _reactionBuildingBlock.FormulaCache);
         _interactionKineticUpdater.UpdateReaction(irreversibleBindingReaction, molecule.Name, irreversibleInteraction.ParentCompound.Name, _simulation, _reactionBuildingBlock.FormulaCache);
         _reactionBuildingBlock.Add(irreversibleBindingReaction);
      }

      private void addInduction(IMoleculeBuilder molecule, InteractionProcess inductionProcess)
      {
         var inductionReaction = _processBuilderMapper.InductionReactionFrom(inductionProcess, molecule, _allMoleculeNames, _reactionBuildingBlock.FormulaCache);
         _reactionBuildingBlock.Add(inductionReaction);
      }

      private void addTurnoverReactionIfNotDefinedAlreadyFor(IMoleculeBuilder molecule)
      {
         if (_moleculeWithTurnoverReactions.Contains(molecule))
            return;

         var templateReaction = _staticReactionRepository.FindByName(CoreConstants.Reaction.TURNOVER);
         var reaction = _processBuilderMapper.TurnoverReactionFrom(templateReaction, molecule, _allMoleculeNames, _reactionBuildingBlock.FormulaCache);
         _reactionBuildingBlock.Add(reaction);
         _moleculeWithTurnoverReactions.Add(molecule);
      }

      private void addTransportProcess(IMoleculeBuilder drug, IProcessMapping compoundProcessMapping, CompoundProperties compoundProperties)
      {
         var compound = compoundProperties.Compound;
         var process = compound.ProcessByName(compoundProcessMapping.ProcessName);

         if (isRenalClearance(compoundProcessMapping))
         {
            var renalPassiveProcess = _processBuilderMapper.PassiveTransportProcessFrom(process, drug.Name, _passiveTransports.FormulaCache);
            _passiveTransports.Add(renalPassiveProcess);
            _parameterIdUpdater.UpdateBuildingBlockId(renalPassiveProcess.Parameters, compound);
            return;
         }

         var transporter = _individual.MoleculeByName<IndividualTransporter>(compoundProcessMapping.MoleculeName);
         var transporterMoleculeContainer = _processBuilderMapper.ActiveTransportFrom(process, transporter, _moleculeBuildingBlock.FormulaCache);
         _interactionKineticUpdater.UpdateTransport(transporterMoleculeContainer, compoundProcessMapping.MoleculeName, compound.Name, _simulation, _moleculeBuildingBlock.FormulaCache);
         _parameterIdUpdater.UpdateBuildingBlockId(transporterMoleculeContainer.Parameters, compound);
         drug.AddTransporterMoleculeContainer(transporterMoleculeContainer);
      }

      private bool isRenalClearance(IProcessMapping compoundProcessMapping)
      {
         var systemicPocess = compoundProcessMapping as SystemicProcessSelection;
         if (systemicPocess == null) return false;
         return (systemicPocess.ProcessType == SystemicProcessTypes.Renal) ||
                (systemicPocess.ProcessType == SystemicProcessTypes.GFR);
      }

      private void addReactionToBuildingBlock(IReactionBuilder reaction, Compound compound)
      {
         _parameterIdUpdater.UpdateBuildingBlockId(reaction.Parameters, compound);
         _reactionBuildingBlock.Add(reaction);
      }
   }
}