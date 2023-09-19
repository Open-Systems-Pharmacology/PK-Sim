using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Descriptors;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using static OSPSuite.Core.Domain.Constants;
using static PKSim.Core.CoreConstants.Compartment;
using static PKSim.Core.CoreConstants.Organ;

namespace PKSim.Core.Mappers
{
   public interface IProcessToProcessBuilderMapper
   {
      /// <summary>
      ///    Create a reaction based on the given <paramref name="process" /> in compound. The reaction will take place between
      ///    the
      ///    <paramref name="compoundBuilder" />
      ///    and the enzyme <paramref name="enzymeName" /> and create the <paramref name="metabolite" />
      /// </summary>
      ReactionBuilder MetabolismReactionFrom(CompoundProcess process, MoleculeBuilder compoundBuilder, MoleculeBuilder metabolite,
         string enzymeName, IReadOnlyCollection<string> forbiddenNames, IFormulaCache formulaCache);

      /// <summary>
      ///    Create a reaction based on the given <paramref name="process" /> in compound. The reaction will take place between
      ///    the the
      ///    <paramref name="compoundBuilder" />
      ///    and the enzyme <paramref name="enzymeName" /> and create an enzyme complex
      /// </summary>
      ReactionBuilder ComplexReactionFrom(CompoundProcess process, MoleculeBuilder compoundBuilder, MoleculeBuilder complex, string enzymeName,
         IReadOnlyCollection<string> forbiddenNames, IFormulaCache formulaCache);

      /// <summary>
      ///    Create a transport builder container for the given process and transporter
      /// </summary>
      TransporterMoleculeContainer ActiveTransportFrom(CompoundProcess compoundProcess, IndividualTransporter transporter, Individual individual,
         IFormulaCache formulaCache);

      /// <summary>
      ///    Creates a passive transport process based on the given compound process that will be applied to the molecule
      ///    <paramref name="compoundName" />
      /// </summary>
      TransportBuilder PassiveTransportProcessFrom(CompoundProcess compoundProcess, string compoundName, IFormulaCache formulaCache);

      /// <summary>
      ///    Creates general reaction builder from template
      /// </summary>
      ReactionBuilder ReactionFrom(ReactionBuilder templateReactionBuilder, string compoundName, IReadOnlyCollection<string> forbiddenNames,
         IFormulaCache formulaCache);

      /// <summary>
      ///    Creates a reaction modeling the creation of the <paramref name="protein" />
      /// </summary>
      /// <param name="templateReaction">Template reaction for protein creation</param>
      /// <param name="protein">Molecule for which the reaction will be created</param>
      /// <param name="forbiddenNames">
      ///    List of all names that are already existing in the system and cannot be used as reaction
      ///    name
      /// </param>
      /// <param name="formulaCache">Formula cache where the kinetic will be saved</param>
      ReactionBuilder TurnoverReactionFrom(ReactionBuilder templateReaction, MoleculeBuilder protein, IReadOnlyCollection<string> forbiddenNames,
         IFormulaCache formulaCache);

      /// <summary>
      ///    Creates a reaction modeling the irreversible binding of the <paramref name="protein" /> with a
      ///    <see cref="Compound" />
      /// </summary>
      /// <param name="interactionProcess">Interaction process defined in compound</param>
      /// <param name="protein">Molecule for which the reaction will be created</param>
      /// <param name="forbiddenNames">
      ///    List of all names that are already existing in the system and cannot be used as reaction
      ///    name
      /// </param>
      /// <param name="formulaCache">Formula cache where the kinetic will be saved</param>
      ReactionBuilder InactivationReactionFrom(InteractionProcess interactionProcess, MoleculeBuilder protein,
         IReadOnlyCollection<string> forbiddenNames, IFormulaCache formulaCache);

      /// <summary>
      ///    Creates a reaction modeling the induction the <paramref name="protein" /> with a
      ///    <see cref="Compound" />
      /// </summary>
      /// <param name="interactionProcess">Induction process defined in compound</param>
      /// <param name="protein">Molecule for which the reaction will be created</param>
      /// <param name="forbiddenNames">
      ///    List of all names that are already existing in the system and cannot be used as reaction
      ///    name
      /// </param>
      /// <param name="formulaCache">Formula cache where the kinetic will be saved</param>
      ReactionBuilder InductionReactionFrom(InteractionProcess interactionProcess, MoleculeBuilder protein,
         IReadOnlyCollection<string> forbiddenNames, IFormulaCache formulaCache);
   }

   public class ProcessToProcessBuilderMapper : IProcessToProcessBuilderMapper
   {
      private readonly ICloner _cloner;
      private readonly ITransportTemplateRepository _transportTemplateRepository;
      private readonly IParameterSetUpdater _parameterSetUpdater;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IParameterContainerTask _parameterContainerTask;
      private readonly ISimulationActiveProcessRepository _simulationActiveProcessRepository;

      public ProcessToProcessBuilderMapper(
         ICloner cloner,
         ITransportTemplateRepository transportTemplateRepository,
         ISimulationActiveProcessRepository simulationActiveProcessRepository,
         IParameterSetUpdater parameterSetUpdater,
         IObjectBaseFactory objectBaseFactory,
         IParameterContainerTask parameterContainerTask)
      {
         _cloner = cloner;
         _transportTemplateRepository = transportTemplateRepository;
         _simulationActiveProcessRepository = simulationActiveProcessRepository;
         _parameterSetUpdater = parameterSetUpdater;
         _objectBaseFactory = objectBaseFactory;
         _parameterContainerTask = parameterContainerTask;
      }

      public ReactionBuilder MetabolismReactionFrom(CompoundProcess process, MoleculeBuilder compoundBuilder, MoleculeBuilder metabolite,
         string enzymeName, IReadOnlyCollection<string> forbiddenNames, IFormulaCache formulaCache)
      {
         //retrieve process for the simulation and create a clone
         var reaction = createReactionFromProcess(compoundBuilder, process, forbiddenNames);

         //replace keywords 
         replaceKeywordsInProcess(reaction, new[] {CoreConstants.KeyWords.Molecule, CoreConstants.KeyWords.Protein, CoreConstants.KeyWords.Reaction},
            new[] {compoundBuilder.Name, enzymeName, reaction.Name});

         reaction.AddEduct(new ReactionPartnerBuilder(compoundBuilder.Name, 1));
         reaction.AddProduct(new ReactionPartnerBuilder(metabolite.Name, 1));
         reaction.AddModifier(enzymeName);

         formulaCache.Add(reaction.Formula);
         return reaction;
      }

      public ReactionBuilder ComplexReactionFrom(CompoundProcess process, MoleculeBuilder compoundBuilder, MoleculeBuilder complex,
         string enzymeName, IReadOnlyCollection<string> forbiddenNames, IFormulaCache formulaCache)
      {
         //retrieve process for the simulation and create a clone
         var reaction = createReactionFromProcess(compoundBuilder, process, forbiddenNames);

         //replace keywords 
         replaceKeywordsInProcess(reaction,
            new[] {CoreConstants.KeyWords.Molecule, CoreConstants.KeyWords.Protein, CoreConstants.KeyWords.Complex, CoreConstants.KeyWords.Reaction},
            new[] {compoundBuilder.Name, enzymeName, complex.Name, reaction.Name});

         reaction.AddEduct(new ReactionPartnerBuilder(compoundBuilder.Name, 1));
         reaction.AddEduct(new ReactionPartnerBuilder(enzymeName, 1));
         reaction.AddProduct(new ReactionPartnerBuilder(complex.Name, 1));

         formulaCache.Add(reaction.Formula);
         return reaction;
      }

      public ReactionBuilder TurnoverReactionFrom(ReactionBuilder templateReaction, MoleculeBuilder protein,
         IReadOnlyCollection<string> forbiddenNames, IFormulaCache formulaCache)
      {
         var reaction = createReactionFromProcess(templateReaction, forbiddenNames);
         reaction.Name = reactionNameFor(reaction.Name, protein.Name);
         reaction.Formula.Name = CompositeNameFor(reaction.Name, reaction.Formula.Name);

         replaceKeywordsInProcess(reaction, new[] {CoreConstants.KeyWords.Protein, CoreConstants.KeyWords.Reaction},
            new[] {protein.Name, reaction.Name});
         reaction.AddProduct(new ReactionPartnerBuilder(protein.Name, 1));

         formulaCache.Add(reaction.Formula);
         return reaction;
      }

      public ReactionBuilder InactivationReactionFrom(InteractionProcess interactionProcess, MoleculeBuilder protein,
         IReadOnlyCollection<string> forbiddenNames, IFormulaCache formulaCache)
      {
         var reaction = interactionReactionFrom(interactionProcess, protein, forbiddenNames, formulaCache);
         reaction.AddEduct(new ReactionPartnerBuilder(protein.Name, 1));
         return reaction;
      }

      public ReactionBuilder InductionReactionFrom(InteractionProcess interactionProcess, MoleculeBuilder protein,
         IReadOnlyCollection<string> forbiddenNames, IFormulaCache formulaCache)
      {
         var reaction = interactionReactionFrom(interactionProcess, protein, forbiddenNames, formulaCache);
         reaction.AddProduct(new ReactionPartnerBuilder(protein.Name, 1));

         //Induction occurs only in Intracellular and Interstitial
         reaction.ContainerCriteria.Add(new InContainerCondition(TISSUE_ORGAN));
         reaction.ContainerCriteria.Add(new NotMatchTagCondition(PLASMA));
         reaction.ContainerCriteria.Add(new NotMatchTagCondition(BLOOD_CELLS));
         reaction.ContainerCriteria.Add(new NotMatchTagCondition(ENDOSOME));
         return reaction;
      }

      private ReactionBuilder interactionReactionFrom(InteractionProcess interactionProcess, MoleculeBuilder protein,
         IReadOnlyCollection<string> forbiddenNames, IFormulaCache formulaCache)
      {
         var compound = interactionProcess.ParentCompound;
         var reactionName = CompositeNameFor(compound.Name, interactionProcess.Name);
         var reaction = createReactionFromProcess(interactionProcess, reactionName, forbiddenNames);

         //replace keywords 
         replaceKeywordsInProcess(reaction, new[] {CoreConstants.KeyWords.Molecule, CoreConstants.KeyWords.Protein, CoreConstants.KeyWords.Reaction},
            new[] {compound.Name, protein.Name, interactionProcess.Name});

         formulaCache.Add(reaction.Formula);
         return reaction;
      }

      public ReactionBuilder ReactionFrom(ReactionBuilder templateReactionBuilder, string compoundName, IReadOnlyCollection<string> forbiddenNames,
         IFormulaCache formulaCache)
      {
         //retrieve process for the simulation and create a clone
         var reaction = createReactionFromProcess(templateReactionBuilder, forbiddenNames);

         //adjust reaction name (e.g. replace "drug" placeholder with compound name if needed
         reaction.Name = reactionNameFor(reaction.Name, compoundName);

         //replace keywords 
         replaceKeywordsInProcess(reaction,
            new[]
            {
               CoreConstants.Molecule.Drug, CoreConstants.KeyWords.Molecule, CoreConstants.KeyWords.Reaction,
               CoreConstants.Molecule.DrugFcRnComplexTemplate
            },
            new[] {compoundName, compoundName, reaction.Name, CoreConstants.Molecule.DrugFcRnComplexName(compoundName)});

         formulaCache.Add(reaction.Formula);
         return reaction;
      }

      private string reactionNameFor(string templateReactionName, string compoundName)
      {
         var reactionName = templateReactionName.ReplaceKeywords(new[]
            {
               CoreConstants.Reaction.FC_RN_BINDING_DRUG_ENDOSOME,
               CoreConstants.Reaction.FC_RN_BINDING_DRUG_INTERSTITIAL,
               CoreConstants.Reaction.FC_RN_BINDING_DRUG_PLASMA,
               CoreConstants.Reaction.FC_RN_BINDING_TISSUE,
               CoreConstants.Reaction.TURNOVER,
            },
            new[]
            {
               CoreConstants.Reaction.FcRnBindingDrugEndosomeNameFrom(compoundName),
               CoreConstants.Reaction.FcRnBindingDrugInterstitialNameFrom(compoundName),
               CoreConstants.Reaction.FcRnBindingDrugPlasmaNameFrom(compoundName),
               CoreConstants.Reaction.FcRnBindingTissueNameFrom(compoundName),
               CoreConstants.Reaction.ProteinTurnoverNameFor(compoundName)
            });

         return reactionName;
      }

      private ReactionBuilder createReactionFromProcess(ReactionBuilder templateReactionBuilder, IReadOnlyCollection<string> forbiddenNames)
      {
         return createReactionFromProcess(templateReactionBuilder, templateReactionBuilder, templateReactionBuilder.Name, forbiddenNames);
      }

      private ReactionBuilder createReactionFromProcess(IContainer reactionParameterContainer, ReactionBuilder templateReactionBuilder,
         string newReactionName, IReadOnlyCollection<string> forbiddenNames)
      {
         //retrieve process for the simulation and create a clone
         var reaction = _cloner.Clone(templateReactionBuilder).WithName(newReactionName);

         while (forbiddenNames.Contains(reaction.Name))
            reaction.Name = $"@{reaction.Name}";

         _parameterSetUpdater.UpdateValuesByName(reactionParameterContainer.AllParameters(), reaction.Parameters);
         return reaction;
      }

      private ReactionBuilder createReactionFromProcess(MoleculeBuilder moleculeBuilder, CompoundProcess process,
         IReadOnlyCollection<string> forbiddenNames)
      {
         var reactionName = CompositeNameFor(moleculeBuilder.Name, process.Name);
         return createReactionFromProcess(process, reactionName, forbiddenNames);
      }

      private ReactionBuilder createReactionFromProcess(CompoundProcess process, string reactionName, IReadOnlyCollection<string> forbiddenNames)
      {
         //retrieve process for the simulation and create a clone
         var reaction = createReactionFromProcess(process, _simulationActiveProcessRepository.ProcessFor<PKSimReaction>(process.InternalName),
            reactionName, forbiddenNames);

         //make sure formula name is unique as it can be shared among processes
         reaction.Formula.Name = CompositeNameFor(reaction.Name, reaction.Formula.Name);
         return reaction;
      }

      public TransportBuilder PassiveTransportProcessFrom(CompoundProcess compoundProcess, string compoundName, IFormulaCache formulaCache)
      {
         var passiveProcess = _cloner.Clone(_simulationActiveProcessRepository.ProcessFor<PKSimTransport>(compoundProcess.InternalName));
         passiveProcess.Name = CompositeNameFor(compoundProcess.Name, compoundName);
         passiveProcess.ForAll = false;
         passiveProcess.AddMoleculeName(compoundName);
         updateTransporterFormulaFromCache(passiveProcess, formulaCache);
         passiveProcess.Parameters.Where(x => !x.Formula.IsConstant()).Each(p => formulaCache.Add(p.Formula));
         _parameterSetUpdater.UpdateValuesByName(compoundProcess.AllParameters(), passiveProcess.Parameters);
         return passiveProcess;
      }

      public TransporterMoleculeContainer ActiveTransportFrom(CompoundProcess compoundProcess, IndividualTransporter transporter,
         Individual individual, IFormulaCache formulaCache)
      {
         var transporterMoleculeContainer = _objectBaseFactory.Create<TransporterMoleculeContainer>().WithName(compoundProcess.InternalName);
         transporterMoleculeContainer.Icon = compoundProcess.Icon;
         transporterMoleculeContainer.TransportName = compoundProcess.Name;

         foreach (var inducedProcess in allInducedProcessesFor(transporter, individual))
         {
            var activeTransporterBuilder = activeTransportFrom(compoundProcess, inducedProcess, formulaCache);
            transporterMoleculeContainer.Icon = activeTransporterBuilder.Icon;
            activeTransporterBuilder.TransportType = transporter.TransportType;
            transporterMoleculeContainer.AddActiveTransportRealization(activeTransporterBuilder);
            updateTransporterTagsFor(activeTransporterBuilder, inducedProcess);
         }

         _parameterContainerTask.AddProcessBuilderParametersTo(transporterMoleculeContainer);
         _parameterSetUpdater.UpdateValuesByName(compoundProcess.AllParameters(), transporterMoleculeContainer.Parameters);

         transporterMoleculeContainer.Name = transporter.Name;
         return transporterMoleculeContainer;
      }

      private IReadOnlyCollection<InducedProcess> allInducedProcessesFor(IndividualTransporter transporter, Individual individual)
      {
         var inducedProcessCache = new Cache<string, InducedProcess>(x => x.Name, x => null);
         var allTransporterContainers = individual.AllMoleculeContainersFor<TransporterExpressionContainer>(transporter)
            .Where(x => x.TransportDirection != TransportDirectionId.None).ToList();

         //Global containers surrogate do not have a logical container associated
         var allOrganNames = new HashSet<string>(allTransporterContainers.Select(x => x.LogicalContainerName).Where(x => !string.IsNullOrEmpty(x)));

         foreach (var transporterContainer in allTransporterContainers)
         {
            //Logical containerName is empty for surrogate containers
            var logicalContainer = transporterContainer.LogicalContainerName;

            var allTransportTemplates = _transportTemplateRepository.All()
               .Where(x => x.TransportDirection == transporterContainer.TransportDirection);

            //a logical container is defined. We are dealing with a localized transporter
            if (!string.IsNullOrEmpty(logicalContainer))
            {
               //For apical transport for Lumen => Mucosa, the compartment is the name of our mapping
               allTransportTemplates = allTransportTemplates
                  .Where(x => (x.SourceOrgan == logicalContainer || x.SourceCompartment == logicalContainer));
            }

            foreach (var transportTemplate in allTransportTemplates)
            {
               var inducedProcess = inducedProcessCache[transportTemplate.ProcessName];
               if (inducedProcess == null)
               {
                  inducedProcess = new InducedProcess {Name = transportTemplate.ProcessName};
                  inducedProcess.AddOrgansToExclude(allOrganNames);
                  inducedProcessCache.Add(inducedProcess);
               }

               // For lumen transport. The source organ or target organ is in fact defined as a compartment
               inducedProcess.AddSourceOrgan(transportTemplate.SourceOrgan.IsLumen() ? transportTemplate.SourceCompartment : transportTemplate.SourceOrgan);
               inducedProcess.AddTargetOrgan(transportTemplate.TargetOrgan.IsLumen() ? transportTemplate.TargetCompartment : transportTemplate.TargetOrgan);
            }
         }

         return inducedProcessCache;
      }

      private void updateTransporterTagsFor(TransportBuilder transporterBuilder, InducedProcess inducedProcess)
      {
         var sourceCriteria = transporterBuilder.SourceCriteria;
         var allSourceTags = sourceCriteria.OfType<MatchTagCondition>().Select(x => x.Tag).ToList();
         var (sourceIsLumen, sourceIsMucosa) = (allSourceTags.Contains(LUMEN), allSourceTags.Contains(MUCOSA));

         var targetCriteria = transporterBuilder.TargetCriteria;
         var allTargetTags = targetCriteria.OfType<MatchTagCondition>().Select(x => x.Tag).ToList();
         var (targetIsLumen, targetIsMucosa) = (allTargetTags.Contains(LUMEN), allTargetTags.Contains(MUCOSA));

         var isLumen = sourceIsLumen || targetIsLumen;
         var isMucosa = sourceIsMucosa || targetIsMucosa;

         if (isLumen && isMucosa)
         {
            //We just want to remove GI segments from the include list as we know, that the transport is between Lumen and Mucosa in this case
            var segmentToDeletes = inducedProcess.OrgansToExclude.Intersect(Constants.Compartment.LumenSegmentsDuodenumToRectum).ToList();
            var criteria = sourceIsLumen ? sourceCriteria : targetCriteria;
            addAsNotMatchToCriteria(criteria, segmentToDeletes);
            return;
         }

         // This is a specific transport and we do not need to do anything
         if (allSourceTags.Count > 1)
            return;

         addAsNotMatchToCriteria(sourceCriteria, inducedProcess.OrgansToExclude);
      }

      private void addAsNotMatchToCriteria(DescriptorCriteria criteria, IEnumerable<string> list)
         => list.Each(x => criteria.Add(new NotMatchTagCondition(x)));

      private TransportBuilder activeTransportFrom(CompoundProcess process, InducedProcess inducedProcess, IFormulaCache formulaCache)
      {
         //retrieve process for the simulation and create a clone
         var simulationProcess = _simulationActiveProcessRepository.TransportFor(inducedProcess.Name, process.InternalName);
         if (simulationProcess == null)
            throw new PKSimException(PKSimConstants.Error.CannotCreateTransportProcessWithKinetic(inducedProcess.Name, process.Name));

         var activeTransport = _cloner.Clone(simulationProcess);
         updateTransporterFormulaFromCache(activeTransport, formulaCache);
         //remove all parameters defined in the builder
         var allParameters = activeTransport.Parameters.ToList();
         allParameters.Each(activeTransport.RemoveParameter);
         return activeTransport;
      }

      private void updateTransporterFormulaFromCache(TransportBuilder transportBuilder, IFormulaCache formulaCache)
      {
         var formula = transportBuilder.Formula;
         //only add transporter formula if not already defined in the cache
         if (formulaCache.ExistsByName(formula.Name))
            transportBuilder.Formula = formulaCache.FindByName(formula.Name);
         else
            formulaCache.Add(formula);
      }

      private void replaceKeywordsInProcess(ReactionBuilder reactionBuilder, string[] keywords, string[] replacementValues)
      {
         replaceFormulaKeywords(reactionBuilder, keywords, replacementValues);
         reactionBuilder.Parameters.Each(p => replaceFormulaKeywords(p, keywords, replacementValues));

         foreach (var reactionPartner in reactionBuilder.Educts.Union(reactionBuilder.Products))
         {
            reactionPartner.MoleculeName = reactionPartner.MoleculeName.ReplaceKeywords(keywords, replacementValues);
         }

         var replacedModifiers = new List<string>();
         foreach (var modifier in reactionBuilder.ModifierNames)
         {
            replacedModifiers.Add(modifier.ReplaceKeywords(keywords, replacementValues));
         }

         reactionBuilder.ClearModifiers();
         foreach (var replacedModifier in replacedModifiers)
         {
            reactionBuilder.AddModifier(replacedModifier);
         }
      }

      private void replaceFormulaKeywords(IUsingFormula usingFormula, string[] keywords, string[] replacementValues)
      {
         usingFormula.Formula.ReplaceKeywordsInObjectPaths(keywords, replacementValues);
      }

      private class InducedProcess : IWithName
      {
         public string Name { get; set; }
         private readonly HashSet<string> _sourceOrgans = new HashSet<string>();
         private readonly HashSet<string> _targetOrgans = new HashSet<string>();
         private readonly List<string> _organsToExclude = new List<string>();
         public IReadOnlyCollection<string> SourceOrgans => _sourceOrgans;
         public IReadOnlyCollection<string> TargetOrgans => _targetOrgans;
         public IReadOnlyList<string> OrgansToExclude => _organsToExclude;

         public void AddOrgansToExclude(IEnumerable<string> organsToExclude) => _organsToExclude.AddRange(organsToExclude);

         public void AddSourceOrgan(string organName)
         {
            _sourceOrgans.Add(organName);
            _organsToExclude.Remove(organName);
         }

         public void AddTargetOrgan(string organName) => _targetOrgans.Add(organName);
      }
   }
}