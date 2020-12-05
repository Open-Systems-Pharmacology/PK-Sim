using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Descriptors;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

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
      IReactionBuilder MetabolismReactionFrom(CompoundProcess process, IMoleculeBuilder compoundBuilder, IMoleculeBuilder metabolite,
         string enzymeName, IReadOnlyCollection<string> forbiddenNames, IFormulaCache formulaCache);

      /// <summary>
      ///    Create a reaction based on the given <paramref name="process" /> in compound. The reaction will take place between
      ///    the the
      ///    <paramref name="compoundBuilder" />
      ///    and the enzyme <paramref name="enzymeName" /> and create an enzyme complex
      /// </summary>
      IReactionBuilder ComplexReactionFrom(CompoundProcess process, IMoleculeBuilder compoundBuilder, IMoleculeBuilder complex, string enzymeName,
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
      ITransportBuilder PassiveTransportProcessFrom(CompoundProcess compoundProcess, string compoundName, IFormulaCache formulaCache);

      /// <summary>
      ///    Creates general reaction builder from template
      /// </summary>
      IReactionBuilder ReactionFrom(IReactionBuilder templateReactionBuilder, string compoundName, IReadOnlyCollection<string> forbiddenNames,
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
      IReactionBuilder TurnoverReactionFrom(IReactionBuilder templateReaction, IMoleculeBuilder protein, IReadOnlyCollection<string> forbiddenNames,
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
      IReactionBuilder InactivationReactionFrom(InteractionProcess interactionProcess, IMoleculeBuilder protein,
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
      IReactionBuilder InductionReactionFrom(InteractionProcess interactionProcess, IMoleculeBuilder protein,
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

      public IReactionBuilder MetabolismReactionFrom(CompoundProcess process, IMoleculeBuilder compoundBuilder, IMoleculeBuilder metabolite,
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

      public IReactionBuilder ComplexReactionFrom(CompoundProcess process, IMoleculeBuilder compoundBuilder, IMoleculeBuilder complex,
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

      public IReactionBuilder TurnoverReactionFrom(IReactionBuilder templateReaction, IMoleculeBuilder protein,
         IReadOnlyCollection<string> forbiddenNames, IFormulaCache formulaCache)
      {
         var reaction = createReactionFromProcess(templateReaction, forbiddenNames);
         reaction.Name = reactionNameFor(reaction.Name, protein.Name);
         reaction.Formula.Name = CoreConstants.CompositeNameFor(reaction.Name, reaction.Formula.Name);

         replaceKeywordsInProcess(reaction, new[] {CoreConstants.KeyWords.Protein, CoreConstants.KeyWords.Reaction},
            new[] {protein.Name, reaction.Name});
         reaction.AddProduct(new ReactionPartnerBuilder(protein.Name, 1));

         formulaCache.Add(reaction.Formula);
         return reaction;
      }

      public IReactionBuilder InactivationReactionFrom(InteractionProcess interactionProcess, IMoleculeBuilder protein,
         IReadOnlyCollection<string> forbiddenNames, IFormulaCache formulaCache)
      {
         var reaction = interactionReactionFrom(interactionProcess, protein, forbiddenNames, formulaCache);
         reaction.AddEduct(new ReactionPartnerBuilder(protein.Name, 1));
         return reaction;
      }

      public IReactionBuilder InductionReactionFrom(InteractionProcess interactionProcess, IMoleculeBuilder protein,
         IReadOnlyCollection<string> forbiddenNames, IFormulaCache formulaCache)
      {
         var reaction = interactionReactionFrom(interactionProcess, protein, forbiddenNames, formulaCache);
         reaction.AddProduct(new ReactionPartnerBuilder(protein.Name, 1));
         return reaction;
      }

      private IReactionBuilder interactionReactionFrom(InteractionProcess interactionProcess, IMoleculeBuilder protein,
         IReadOnlyCollection<string> forbiddenNames, IFormulaCache formulaCache)
      {
         var compound = interactionProcess.ParentCompound;
         var reactionName = CoreConstants.CompositeNameFor(compound.Name, interactionProcess.Name);
         var reaction = createReactionFromProcess(interactionProcess, reactionName, forbiddenNames);

         //replace keywords 
         replaceKeywordsInProcess(reaction, new[] {CoreConstants.KeyWords.Molecule, CoreConstants.KeyWords.Protein, CoreConstants.KeyWords.Reaction},
            new[] {compound.Name, protein.Name, interactionProcess.Name});

         formulaCache.Add(reaction.Formula);
         return reaction;
      }

      public IReactionBuilder ReactionFrom(IReactionBuilder templateReactionBuilder, string compoundName, IReadOnlyCollection<string> forbiddenNames,
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

      private IReactionBuilder createReactionFromProcess(IReactionBuilder templateReactionBuilder, IReadOnlyCollection<string> forbiddenNames)
      {
         return createReactionFromProcess(templateReactionBuilder, templateReactionBuilder, templateReactionBuilder.Name, forbiddenNames);
      }

      private IReactionBuilder createReactionFromProcess(IContainer reactionParameterContainer, IReactionBuilder templateReactionBuilder,
         string newReactionName, IReadOnlyCollection<string> forbiddenNames)
      {
         //retrieve process for the simulation and create a clone
         var reaction = _cloner.Clone(templateReactionBuilder).WithName(newReactionName);

         while (forbiddenNames.Contains(reaction.Name))
            reaction.Name = $"@{reaction.Name}";

         _parameterSetUpdater.UpdateValuesByName(reactionParameterContainer.AllParameters(), reaction.Parameters);
         return reaction;
      }

      private IReactionBuilder createReactionFromProcess(IMoleculeBuilder moleculeBuilder, CompoundProcess process,
         IReadOnlyCollection<string> forbiddenNames)
      {
         var reactionName = CoreConstants.CompositeNameFor(moleculeBuilder.Name, process.Name);
         return createReactionFromProcess(process, reactionName, forbiddenNames);
      }

      private IReactionBuilder createReactionFromProcess(CompoundProcess process, string reactionName, IReadOnlyCollection<string> forbiddenNames)
      {
         //retrieve process for the simulation and create a clone
         var reaction = createReactionFromProcess(process, _simulationActiveProcessRepository.ProcessFor<PKSimReaction>(process.InternalName),
            reactionName, forbiddenNames);

         //make sure formula name is unique as it can be shared among processes
         reaction.Formula.Name = CoreConstants.CompositeNameFor(reaction.Name, reaction.Formula.Name);
         return reaction;
      }

      public ITransportBuilder PassiveTransportProcessFrom(CompoundProcess compoundProcess, string compoundName, IFormulaCache formulaCache)
      {
         var passiveProcess = _cloner.Clone(_simulationActiveProcessRepository.ProcessFor<PKSimTransport>(compoundProcess.InternalName));
         passiveProcess.Name = compoundProcess.Name;
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
            if (!string.IsNullOrEmpty(logicalContainer)){
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

               inducedProcess.AddSourceOrgan(transportTemplate.SourceOrgan);
               inducedProcess.AddTargetOrgan(transportTemplate.TargetOrgan);
            }
         }

         return inducedProcessCache;
      }

      private void updateTransporterTagsFor(ITransportBuilder transporterBuilder, InducedProcess inducedProcess)
      {
         var allSourceTags = transporterBuilder.SourceCriteria.OfType<TagCondition>()
            .Select(x => x.Tag).ToList();

         //More than one tag coming from the database => This is a specific transport and we do not need to do anything
         if (allSourceTags.Count > 1)
            return;

         foreach (var organName in inducedProcess.OrgansToExclude)
         {
            transporterBuilder.SourceCriteria.Add(new NotMatchTagCondition(organName));
         }
      }

      private ITransportBuilder activeTransportFrom(CompoundProcess process, InducedProcess inducedProcess, IFormulaCache formulaCache)
      {
         //retrieve process for the simulation and create a clone
         var activeTransport = _cloner.Clone(_simulationActiveProcessRepository.TransportFor(inducedProcess.Name, process.InternalName));
         updateTransporterFormulaFromCache(activeTransport, formulaCache);
         //remove all parameters defined in the builder
         var allParameters = activeTransport.Parameters.ToList();
         allParameters.Each(activeTransport.RemoveParameter);
         return activeTransport;
      }

      private void updateTransporterFormulaFromCache(ITransportBuilder transportBuilder, IFormulaCache formulaCache)
      {
         var formula = transportBuilder.Formula;
         //only add transporter formula if not already defined in the cache
         if (formulaCache.ExistsByName(formula.Name))
            transportBuilder.Formula = formulaCache.FindByName(formula.Name);
         else
            formulaCache.Add(formula);
      }

      private void replaceKeywordsInProcess(IReactionBuilder reactionBuilder, string[] keywords, string[] replacementValues)
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