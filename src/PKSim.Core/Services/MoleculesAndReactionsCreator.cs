using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using IMoleculeBuilderFactory = PKSim.Core.Model.IMoleculeBuilderFactory;
using ModelConfiguration = PKSim.Core.Model.ModelConfiguration;

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
      (MoleculeBuildingBlock moleculeBuildingBlock, ReactionBuildingBlock reactionBuildingBlock) CreateFor(Module module, Simulation simulation);
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
      private MoleculeBuildingBlock _moleculeBuildingBlock;
      private ReactionBuildingBlock _reactionBuildingBlock;
      private Module _module;
      private readonly List<string> _allMoleculeNames;
      private readonly List<MoleculeBuilder> _moleculeWithTurnoverReactions;
      private PassiveTransportBuildingBlock _passiveTransports;
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
         _moleculeWithTurnoverReactions = new List<MoleculeBuilder>();
      }

      public (MoleculeBuildingBlock moleculeBuildingBlock, ReactionBuildingBlock reactionBuildingBlock) CreateFor(Module module, Simulation simulation)
      {
         try
         {
            _simulation = simulation;
            _individual = simulation.Individual;
            _module = module;
            _passiveTransports = _module.PassiveTransports;

            _moleculeBuildingBlock = _objectBaseFactory.Create<MoleculeBuildingBlock>()
               .WithName(DefaultNames.MoleculeBuildingBlock);

            _reactionBuildingBlock = _objectBaseFactory.Create<ReactionBuildingBlock>()
               .WithName(DefaultNames.ReactionBuildingBlock);

            addIndividualMolecules(simulation.CompoundPropertiesList);

            addCompoundMolecules(simulation.CompoundPropertiesList, simulation.InteractionProperties);

            addTwoPoreModelsReactionAndMolecules(simulation.CompoundPropertiesList, simulation.ModelConfiguration);

            addPartialProcesses(simulation.CompoundPropertiesList);

            addInteractions(simulation);

            return (_moleculeBuildingBlock, _reactionBuildingBlock);
         }
         finally
         {
            _moleculeBuildingBlock = null;
            _reactionBuildingBlock = null;
            _individual = null;
            _module = null;
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

      private void addStaticReactions(MoleculeBuilder moleculeBuilder, string modelName)
      {
         foreach (var reaction in _staticReactionRepository.AllFor(modelName))
         {
            var reactionBuilder =
               _processBuilderMapper.ReactionFrom(reaction, moleculeBuilder.Name, _allMoleculeNames, _reactionBuildingBlock.FormulaCache);

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
         //first of all: add undefined proteins that are created internally and are necessary to use the systemic processes
         compoundPropertiesList.Select(x => x.Processes).Each(addUndefinedProteinForSystemicProcessesToIndividual);
         _individual.AllMolecules().Each(addMolecule);
      }

      private void addCompoundMolecules(IEnumerable<CompoundProperties> compoundPropertiesList, InteractionProperties interactionProperties)
      {
         compoundPropertiesList.Each(compoundProperties =>
         {
            var drug = _moleculeBuilderFactory.Create(compoundProperties.Compound, compoundProperties, interactionProperties,
               _moleculeBuildingBlock.FormulaCache);
            addMoleculeToBuildingBlock(drug, compoundProperties);
         });
      }

      private void addMolecule(IndividualMolecule individualMolecule)
      {
         var molecule = _moleculeBuilderFactory.Create(individualMolecule.MoleculeType, _moleculeBuildingBlock.FormulaCache)
            .WithName(individualMolecule.Name)
            .WithIcon(individualMolecule.Icon);

         addMoleculeToBuildingBlock(molecule, compoundProperties: null);

         //Update protein builder parameters with the parameter ids and values
         _parameterSetUpdater.UpdateValuesByName(individualMolecule, molecule.Parameters);

         //Update the building block ids
         _parameterIdUpdater.UpdateBuildingBlockId(molecule.Parameters, _individual);

         //since parameter value were updated from default, some IsDefault will be reset to false
         _parameterIdUpdater.ResetParameterIsDefaultState(molecule, individualMolecule.GetChildren<IParameter>());
      }

  

      private void addMoleculeToBuildingBlock(MoleculeBuilder moleculeBuilder, CompoundProperties compoundProperties)
      {
         if (_allMoleculeNames.Contains(moleculeBuilder.Name))
            throw new TwoMoleculesWithSameNameException(moleculeBuilder.Name);

         _allMoleculeNames.Add(moleculeBuilder.Name);

         //set the used calculation method according to current settings 
         if (compoundProperties != null)
            _moleculeCalculationRetriever.AllMoleculeCalculationMethodsUsedBy(compoundProperties).Each(moleculeBuilder.AddUsedCalculationMethod);

         _moleculeBuildingBlock.Add(moleculeBuilder);
      }

      private void addPartialProcesses(MoleculeBuilder compoundBuilder,
         Func<CompoundProcessesSelection, ProcessSelectionGroup> processSelectionGroup, CompoundProperties compoundProperties,
         Action<MoleculeBuilder, IReactionMapping, CompoundProperties> addPartialProcess)
      {
         foreach (var processSelection in processSelectionGroup(compoundProperties.Processes).AllEnabledProcesses())
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

         _individualTransporterFactory.AddUndefinedLiverTransporterTo(_individual);
      }

      private void addUndefinedLiver()
      {
         var undefinedLiver = _individual.MoleculeByName<IndividualEnzyme>(CoreConstants.Molecule.UndefinedLiver);
         //Already added
         if (undefinedLiver != null)
            return;

         _individualEnzymeFactory.AddUndefinedLiverTo(_individual);
      }

      private void addMetabolismProcess(MoleculeBuilder compoundBuilder, IReactionMapping compoundReactionMapping,
         CompoundProperties compoundProperties)
      {
         var compound = compoundProperties.Compound;
         var process = compound.ProcessByName(compoundReactionMapping.ProcessName);
         if (process == null)
            return;

         var metabolite = getOrCreateMetaboliteFor(compoundReactionMapping, compoundProperties);
         var reaction = _processBuilderMapper.MetabolismReactionFrom(process, compoundBuilder, metabolite, compoundReactionMapping.MoleculeName,
            _allMoleculeNames, _reactionBuildingBlock.FormulaCache);
         _interactionKineticUpdater.UpdateReaction(reaction, compoundReactionMapping.MoleculeName, compound.Name, _simulation,
            _reactionBuildingBlock.FormulaCache);
         addReactionToBuildingBlock(reaction, compound);
      }

      private MoleculeBuilder getOrCreateMetaboliteFor(IReactionMapping compoundReactionMapping, CompoundProperties compoundProperties)
      {
         MoleculeBuilder metabolite = null;
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

      private void addSpecificBindingProcess(MoleculeBuilder drug, IReactionMapping compoundReactionMapping, CompoundProperties compoundProperties)
      {
         var compound = compoundProperties.Compound;
         var process = compound.ProcessByName(compoundReactionMapping.ProcessName);
         var complex = _moleculeBuilderFactory.Create(QuantityType.Complex, _moleculeBuildingBlock.FormulaCache)
            .WithName(compoundReactionMapping.ProductName(CoreConstants.Molecule.Complex));

         addMoleculeToBuildingBlock(complex, compoundProperties);
         var complexReactionFrom = _processBuilderMapper.ComplexReactionFrom(process, drug, complex, compoundReactionMapping.MoleculeName,
            _allMoleculeNames, _reactionBuildingBlock.FormulaCache);
         addReactionToBuildingBlock(complexReactionFrom, compound);
      }

      private void addInteractions(Simulation simulation)
      {
         addInduction(simulation);
         addIrreversibleInhibition(simulation);
      }

      private void addInduction(Simulation simulation) => addInteraction(simulation, InteractionType.Induction, addInduction);

      private void addIrreversibleInhibition(Simulation simulation) =>
         addInteraction(simulation, InteractionType.IrreversibleInhibition, addIrreversibleBindings);

      private void addInteraction(Simulation simulation, InteractionType interactionType,
         Action<MoleculeBuilder, InteractionProcess> addInteractionAction)
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

      private void addIrreversibleBindings(MoleculeBuilder molecule, InteractionProcess irreversibleInteraction)
      {
         var irreversibleBindingReaction =
            _processBuilderMapper.InactivationReactionFrom(irreversibleInteraction, molecule, _allMoleculeNames, _reactionBuildingBlock.FormulaCache);
         _interactionKineticUpdater.UpdateReaction(irreversibleBindingReaction, molecule.Name, irreversibleInteraction.ParentCompound.Name,
            _simulation, _reactionBuildingBlock.FormulaCache);
         _reactionBuildingBlock.Add(irreversibleBindingReaction);
      }

      private void addInduction(MoleculeBuilder molecule, InteractionProcess inductionProcess)
      {
         var inductionReaction =
            _processBuilderMapper.InductionReactionFrom(inductionProcess, molecule, _allMoleculeNames, _reactionBuildingBlock.FormulaCache);
         _reactionBuildingBlock.Add(inductionReaction);
      }

      private void addTurnoverReactionIfNotDefinedAlreadyFor(MoleculeBuilder molecule)
      {
         if (_moleculeWithTurnoverReactions.Contains(molecule))
            return;

         var templateReaction = _staticReactionRepository.FindByName(CoreConstants.Reaction.TURNOVER);
         var reaction = _processBuilderMapper.TurnoverReactionFrom(templateReaction, molecule, _allMoleculeNames,
            _reactionBuildingBlock.FormulaCache);
         _reactionBuildingBlock.Add(reaction);
         _moleculeWithTurnoverReactions.Add(molecule);
      }

      private void addTransportProcess(MoleculeBuilder drug, IProcessMapping compoundProcessMapping, CompoundProperties compoundProperties)
      {
         var compound = compoundProperties.Compound;
         var process = compound.ProcessByName(compoundProcessMapping.ProcessName);

         //Passive transport added to the passive transport list
         if (isRenalClearance(compoundProcessMapping))
         {
            var renalPassiveProcess = _processBuilderMapper.PassiveTransportProcessFrom(process, drug.Name, _passiveTransports.FormulaCache);
            _passiveTransports.Add(renalPassiveProcess);
            _parameterIdUpdater.UpdateBuildingBlockId(renalPassiveProcess.Parameters, compound);
            return;
         }

         var transporter = _individual.MoleculeByName<IndividualTransporter>(compoundProcessMapping.MoleculeName);
         var transporterMoleculeContainer =
            _processBuilderMapper.ActiveTransportFrom(process, transporter, _individual, _moleculeBuildingBlock.FormulaCache);
         _interactionKineticUpdater.UpdateTransport(transporterMoleculeContainer, compoundProcessMapping.MoleculeName, compound.Name, _simulation,
            _moleculeBuildingBlock.FormulaCache);
         _parameterIdUpdater.UpdateBuildingBlockId(transporterMoleculeContainer.Parameters, compound);
         drug.AddTransporterMoleculeContainer(transporterMoleculeContainer);
      }

      private bool isRenalClearance(IProcessMapping compoundProcessMapping)
      {
         var systemicProcess = compoundProcessMapping as SystemicProcessSelection;
         if (systemicProcess == null) return false;
         return (systemicProcess.ProcessType == SystemicProcessTypes.Renal) ||
                (systemicProcess.ProcessType == SystemicProcessTypes.GFR);
      }

      private void addReactionToBuildingBlock(ReactionBuilder reaction, Compound compound)
      {
         _parameterIdUpdater.UpdateBuildingBlockId(reaction.Parameters, compound);
         _reactionBuildingBlock.Add(reaction);
      }
   }
}