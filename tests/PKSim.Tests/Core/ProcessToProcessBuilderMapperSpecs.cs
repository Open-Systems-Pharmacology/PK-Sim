using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using static OSPSuite.Core.Domain.Constants;

namespace PKSim.Core
{
   public abstract class concern_for_ProcessToProcessBuilderMapper : ContextSpecification<IProcessToProcessBuilderMapper>
   {
      protected ICloner _cloneManager;
      protected ISimulationActiveProcessRepository _simulationActiveProcessRepository;
      protected IParameterSetUpdater _parameterSetUpdater;
      protected IObjectBaseFactory _objectBaseFactory;
      private IParameterContainerTask _parameterContainerTask;
      private ITransportTemplateRepository _transportTemplateRepository;

      protected override void Context()
      {
         _cloneManager = A.Fake<ICloner>();
         _simulationActiveProcessRepository = A.Fake<ISimulationActiveProcessRepository>();
         _parameterSetUpdater = A.Fake<IParameterSetUpdater>();
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _parameterContainerTask = A.Fake<IParameterContainerTask>();
         _transportTemplateRepository= A.Fake<ITransportTemplateRepository>();
         sut = new ProcessToProcessBuilderMapper(_cloneManager,_transportTemplateRepository, _simulationActiveProcessRepository, _parameterSetUpdater, _objectBaseFactory, _parameterContainerTask);
      }
   }

   public class when_mapping_template_reaction_builder_to_a_reaction_builder : concern_for_ProcessToProcessBuilderMapper
   {
      protected IReactionBuilder _reaction;
      protected string _compoundName = "Aspirin";
      protected IFormulaCache _formulaCache;

      protected override void Context()
      {
         base.Context();

         var formula = new ExplicitFormula().WithFormulaString("123").WithName("TOTO");
         formula.AddObjectPath(new FormulaUsablePath(new[] {"Organism", CoreConstants.KeyWords.Molecule}));
         formula.AddObjectPath(new FormulaUsablePath(new[] {"Organism", CoreConstants.Molecule.DrugFcRnComplexTemplate}));
         _reaction = new PKSimReaction().WithName("xx").WithId("xx").WithFormula(formula);
         _reaction.AddEduct(new ReactionPartnerBuilder(CoreConstants.KeyWords.Molecule, 1));
         _reaction.AddEduct(new ReactionPartnerBuilder("XY", 1));
         _reaction.AddProduct(new ReactionPartnerBuilder(CoreConstants.Molecule.DrugFcRnComplexTemplate, 1));
         _reaction.AddModifier(CoreConstants.KeyWords.Molecule);
         _formulaCache = new BuildingBlockFormulaCache();

         A.CallTo(() => _cloneManager.Clone(_reaction)).Returns(_reaction);
      }

      protected override void Because()
      {
         _reaction = sut.ReactionFrom(_reaction, _compoundName, new List<string>(), _formulaCache);
      }

      [Observation]
      public void should_have_replaced_the_keywords_defined_in_the_reaction_kinetic()
      {
         _reaction.Formula.ObjectPaths.ElementAt(0).ShouldOnlyContain("Organism", _compoundName);
         _reaction.Formula.ObjectPaths.ElementAt(1).ShouldOnlyContain("Organism", CoreConstants.Molecule.DrugFcRnComplexName(_compoundName));
      }

      [Observation]
      public void should_have_added_the_kinetic_in_the_formula_cache()
      {
         _formulaCache.Count().ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_have_left_the_formula_name_unchanged()
      {
         _reaction.Formula.Name.ShouldBeEqualTo("TOTO");
      }
   }

   public class When_mapping_a_process_to_a_reaction_builder : concern_for_ProcessToProcessBuilderMapper
   {
      private CompoundProcess _process;
      private IMoleculeBuilder _drug;
      private IMoleculeBuilder _metabolite;
      private string _enzymeName;
      private FormulaCache _formulaCache;
      private IReactionBuilder _reaction;
      private ExplicitFormula _parameterFormula;
      private PKSimParameter _parameterInReaction;
      private ExplicitFormula _kinetic;

      protected override void Context()
      {
         base.Context();
         _process = new EnzymaticProcess().WithName("tralala");
         _process.InternalName = "internalNAme";
         _drug = new MoleculeBuilder().WithName("drug");
         _metabolite = new MoleculeBuilder().WithName("metabolite");
         _enzymeName = "enzyme";
         _formulaCache = new FormulaCache();
         _kinetic = new ExplicitFormula().WithId("trala").WithName("KINETIC");
         _kinetic.AddObjectPath(new FormulaUsablePath(new[] {"Organism", CoreConstants.KeyWords.Molecule}));
         _kinetic.AddObjectPath(new FormulaUsablePath(new[] {"Organism", CoreConstants.KeyWords.Protein}));
         _parameterFormula = new ExplicitFormula();
         _parameterFormula.AddObjectPath(new FormulaUsablePath(new[] {"Liver", CoreConstants.KeyWords.Molecule}));

         _parameterInReaction = new PKSimParameter().WithName("p").WithFormula(_parameterFormula);
         var pkSimReaction = new PKSimReaction().WithName("template");
         pkSimReaction.AddParameter(_parameterInReaction);

         A.CallTo(() => _simulationActiveProcessRepository.ProcessFor<PKSimReaction>(_process.InternalName)).Returns(pkSimReaction);
         pkSimReaction.Formula = _kinetic;
         A.CallTo(() => _cloneManager.Clone<IReactionBuilder>(pkSimReaction)).Returns(pkSimReaction);
      }

      protected override void Because()
      {
         _reaction = sut.MetabolismReactionFrom(_process, _drug, _metabolite, _enzymeName, new List<string>(), _formulaCache);
      }

      [Observation]
      public void should_retrieve_the_template_reaction_based_on_the_process_internal_name()
      {
         A.CallTo(() => _simulationActiveProcessRepository.ProcessFor<PKSimReaction>(_process.InternalName)).MustHaveHappened();
      }

      [Observation]
      public void should_have_replaced_the_keywords_defined_in_the_reacion_kinetic_by_the_actual_protein_and_molecule_name()
      {
         _kinetic.ObjectPaths.ElementAt(0).ShouldOnlyContain("Organism", _drug.Name);
         _kinetic.ObjectPaths.ElementAt(1).ShouldOnlyContain("Organism", _enzymeName);
      }

      [Observation]
      public void should_have_replaced_the_keywords_defined_in_the_reacion_parameters_by_the_actual_protein_and_molecule_name()
      {
         _parameterFormula.ObjectPaths.ElementAt(0).ShouldOnlyContain("Liver", _drug.Name);
      }

      [Observation]
      public void should_have_added_the_kinetic_in_the_formula_cache()
      {
         _formulaCache.Contains(_kinetic).ShouldBeTrue();
      }

      [Observation]
      public void should_have_set_the_name_of_the_reaction_to_the_name_of_the_incoming_process_combine_with_the_name_of_the_drug()
      {
         _reaction.Name.ShouldBeEqualTo(CompositeNameFor(_drug.Name, _process.Name));
      }

      [Observation]
      public void should_have_set_the_name_of_the_kinetic_to_the_name_of_the_incoming_process_combine_with_the_name_of_the_formula()
      {
         _reaction.Formula.Name.ShouldBeEqualTo(CompositeNameFor(_reaction.Name, "KINETIC"));
      }
   }

   public class When_mapping_a_process_to_a_reaciton_builder_where_one_of_the_educt_or_product_has_the_same_name_as_the_process : concern_for_ProcessToProcessBuilderMapper
   {
      private EnzymaticProcess _process;
      private IMoleculeBuilder _drug;
      private IMoleculeBuilder _metabolite;
      private string _enzymeName;
      private FormulaCache _formulaCache;
      private ExplicitFormula _kinetic;
      private ExplicitFormula _parameterFormula;
      private IReactionBuilder _reaction;

      protected override void Context()
      {
         base.Context();
         _process = new EnzymaticProcess().WithName("P1");
         _process.InternalName = "internalNAme";
         _drug = new MoleculeBuilder().WithName("drug");
         _metabolite = new MoleculeBuilder().WithName("metabolite");
         _enzymeName = "P1";
         _formulaCache = new FormulaCache();
         _kinetic = new ExplicitFormula().WithId("trala");
         _kinetic.AddObjectPath(new FormulaUsablePath("Organism", CoreConstants.KeyWords.Molecule));
         _kinetic.AddObjectPath(new FormulaUsablePath(new[] {"Organism", CoreConstants.KeyWords.Protein}));
         _parameterFormula = new ExplicitFormula();
         _parameterFormula.AddObjectPath(new FormulaUsablePath(new[] {"Liver", CoreConstants.KeyWords.Molecule}));

         var pkSimReaction = new PKSimReaction().WithName("template");

         A.CallTo(() => _simulationActiveProcessRepository.ProcessFor<PKSimReaction>(_process.InternalName)).Returns(pkSimReaction);
         pkSimReaction.Formula = _kinetic;
         A.CallTo(() => _cloneManager.Clone(pkSimReaction)).Returns(pkSimReaction);
      }

      protected override void Because()
      {
         _reaction = sut.MetabolismReactionFrom(_process, _drug, _metabolite, _enzymeName, new List<string> {"drug-P1"}, _formulaCache);
      }

      [Observation]
      public void should_rename_the_process_to_avoid_the_conflict_of_reaction_having_the_same_name_as_a_molecule()
      {
         _reaction.Name.ShouldBeEqualTo("@drug-P1");
      }
   }

   public class When_creating_a_passive_transport_process_for_a_given_molecule : concern_for_ProcessToProcessBuilderMapper
   {
      private IFormulaCache _formulaCache;
      private CompoundProcess _process;
      private ITransportBuilder _transport;
      private PKSimTransport _pkSimTransport;

      protected override void Context()
      {
         base.Context();
         _formulaCache = A.Fake<IFormulaCache>();
         _pkSimTransport = new PKSimTransport {Formula = A.Fake<IFormula>()};
         _process = new SystemicProcess {InternalName = "Proc", Name = "BLA BLA"};
         A.CallTo(() => _simulationActiveProcessRepository.ProcessFor<PKSimTransport>(_process.InternalName)).Returns(_pkSimTransport);
         A.CallTo(() => _cloneManager.Clone(_pkSimTransport)).Returns(_pkSimTransport);
      }

      protected override void Because()
      {
         _transport = sut.PassiveTransportProcessFrom(_process, "DRUG", _formulaCache);
      }

      [Observation]
      public void should_return_a_passive_transport_which_only_applied_for_one_molecule()
      {
         _transport.ForAll.ShouldBeFalse();
         _transport.MoleculeNames().ShouldOnlyContain("DRUG");
      }

      [Observation]
      public void should_return_a_passive_transport_named_after_the_process_name()
      {
         _transport.Name.ShouldBeEqualTo(_process.Name);
      }
   }

   public class When_creating_a_protein_turnover_reaction_for_a_given_protein : concern_for_ProcessToProcessBuilderMapper
   {
      private IFormulaCache _formulaCache;
      private IReactionBuilder _template;
      private IMoleculeBuilder _protein;
      private IReactionBuilder _reaction;

      protected override void Context()
      {
         base.Context();
         _formulaCache = new FormulaCache();
         _protein = A.Fake<IMoleculeBuilder>().WithName("CYP3A4");
         var reaction = new ReactionBuilder {Formula = new ExplicitFormula().WithName("ABC")};
         reaction.Formula.AddObjectPath(new FormulaUsablePath(CoreConstants.KeyWords.Protein, "P1").WithAlias("P1"));
         reaction.Formula.AddObjectPath(new FormulaUsablePath(CoreConstants.KeyWords.Reaction, "R1").WithAlias("R1"));
         _template = A.Fake<IReactionBuilder>().WithName(CoreConstants.Reaction.TURNOVER);
         A.CallTo(() => _cloneManager.Clone(_template)).Returns(reaction);
      }

      protected override void Because()
      {
         _reaction = sut.TurnoverReactionFrom(_template, _protein, new List<string>(), _formulaCache);
      }

      [Observation]
      public void should_return_a_reaction_having_the_expected_name()
      {
         _reaction.Name.ShouldBeEqualTo(CoreConstants.Reaction.ProteinTurnoverNameFor(_protein.Name));
      }

      [Observation]
      public void should_have_replaced_the_protein_keyword()
      {
         _reaction.Formula.FormulaUsablePathBy("P1")[0].ShouldBeEqualTo(_protein.Name);
      }

      [Observation]
      public void should_have_added_the_protein_as_product_of_the_reaction()
      {
         _reaction.Products.Select(x => x.MoleculeName).ShouldOnlyContain(_protein.Name);
      }

      [Observation]
      public void should_have_no_educt_for_the_reaction()
      {
         _reaction.Educts.ShouldBeEmpty();
      }

      [Observation]
      public void should_have_replaced_the_reaction_keyword()
      {
         _reaction.Formula.FormulaUsablePathBy("R1")[0].ShouldBeEqualTo(_reaction.Name);
      }

      [Observation]
      public void should_have_added_the_reaction_formula_to_the_formula_cache()
      {
         _formulaCache.Contains(_reaction.Formula).ShouldBeTrue();
      }
   }

   public class When_creating_an_irreversible_binding_reaction_for_a_given_interaction_process_and_protein : concern_for_ProcessToProcessBuilderMapper
   {
      private InteractionProcess _interactionProcess;
      private IMoleculeBuilder _protein;
      private IFormulaCache _formulaCache;
      private Compound _compound;
      private IReactionBuilder _reaction;
      private PKSimReaction _template;

      protected override void Context()
      {
         base.Context();
         _formulaCache = new FormulaCache();
         _protein = new MoleculeBuilder().WithName("CYP3A4");

         _interactionProcess = new InhibitionProcess().WithName("Interaction");
         _interactionProcess.InternalName = "AA";

         _compound = new Compound().WithName("Inhibitor");
         _compound.AddProcess(_interactionProcess);

         _template = new PKSimReaction();
         A.CallTo(() => _simulationActiveProcessRepository.ProcessFor<PKSimReaction>(_interactionProcess.InternalName)).Returns(_template);

         var reaction = new PKSimReaction {Formula = new ExplicitFormula().WithName("ABC")};
         reaction.Formula.AddObjectPath(new FormulaUsablePath(CoreConstants.KeyWords.Protein, "P1").WithAlias("P1"));
         reaction.Formula.AddObjectPath(new FormulaUsablePath(CoreConstants.KeyWords.Reaction, "R1").WithAlias("R1"));
         reaction.Formula.AddObjectPath(new FormulaUsablePath(CoreConstants.KeyWords.Molecule, "M11").WithAlias("M1"));
         A.CallTo(() => _cloneManager.Clone<IReactionBuilder>(_template)).Returns(reaction);
      }

      protected override void Because()
      {
         _reaction = sut.InactivationReactionFrom(_interactionProcess, _protein, new List<string>(), _formulaCache);
      }

      [Observation]
      public void should_return_a_reaction_having_the_expected_name()
      {
         _reaction.Name.ShouldBeEqualTo(CompositeNameFor(_compound.Name, _interactionProcess.Name));      }

      [Observation]
      public void should_have_replaced_the_protein_keyword()
      {
         _reaction.Formula.FormulaUsablePathBy("P1")[0].ShouldBeEqualTo(_protein.Name);
      }

      [Observation]
      public void should_have_replaced_the_molecule_keyword()
      {
         _reaction.Formula.FormulaUsablePathBy("M1")[0].ShouldBeEqualTo(_compound.Name);
      }

      [Observation]
      public void should_have_replaced_the_reaction_keyword_with_the_name_of_the_process()
      {
         _reaction.Formula.FormulaUsablePathBy("R1")[0].ShouldBeEqualTo(_interactionProcess.Name);
      }

      [Observation]
      public void should_have_added_the_protein_as_educt_of_the_reaction()
      {
         _reaction.Educts.Select(x => x.MoleculeName).ShouldContain(_protein.Name);
      }

      [Observation]
      public void should_not_added_the_compound_as_educt_of_the_reaction()
      {
         _reaction.Educts.Select(x => x.MoleculeName).ShouldNotContain(_compound.Name);
      }

      [Observation]
      public void should_have_no_product_for_the_reaction()
      {
         _reaction.Products.ShouldBeEmpty();
      }

      [Observation]
      public void should_have_added_the_reaction_formula_to_the_formula_cache()
      {
         _formulaCache.Contains(_reaction.Formula).ShouldBeTrue();
      }
   }

   public class When_creating_an_induction_reaction_for_a_given_interaction_process_and_protein : concern_for_ProcessToProcessBuilderMapper
   {
      private InteractionProcess _interactionProcess;
      private IMoleculeBuilder _protein;
      private IFormulaCache _formulaCache;
      private Compound _compound;
      private IReactionBuilder _reaction;
      private PKSimReaction _template;

      protected override void Context()
      {
         base.Context();
         _formulaCache = new FormulaCache();
         _protein = new MoleculeBuilder().WithName("CYP3A4");

         _interactionProcess = new InductionProcess().WithName("Induction");
         _interactionProcess.InternalName = "AA";

         _compound = new Compound().WithName("Inhibitor");
         _compound.AddProcess(_interactionProcess);

         _template = new PKSimReaction();
         A.CallTo(() => _simulationActiveProcessRepository.ProcessFor<PKSimReaction>(_interactionProcess.InternalName)).Returns(_template);

         var reaction = new PKSimReaction { Formula = new ExplicitFormula().WithName("ABC") };
         reaction.Formula.AddObjectPath(new FormulaUsablePath(CoreConstants.KeyWords.Protein, "P1").WithAlias("P1"));
         reaction.Formula.AddObjectPath(new FormulaUsablePath(CoreConstants.KeyWords.Reaction, "R1").WithAlias("R1"));
         reaction.Formula.AddObjectPath(new FormulaUsablePath(CoreConstants.KeyWords.Molecule, "M11").WithAlias("M1"));
         A.CallTo(() => _cloneManager.Clone<IReactionBuilder>(_template)).Returns(reaction);
      }

      protected override void Because()
      {
         _reaction = sut.InductionReactionFrom(_interactionProcess, _protein, new List<string>(), _formulaCache);
      }

      [Observation]
      public void should_return_a_reaction_having_the_expected_name()
      {
         _reaction.Name.ShouldBeEqualTo(CompositeNameFor(_compound.Name, _interactionProcess.Name));
      }

      [Observation]
      public void should_have_replaced_the_protein_keyword()
      {
         _reaction.Formula.FormulaUsablePathBy("P1")[0].ShouldBeEqualTo(_protein.Name);
      }

      [Observation]
      public void should_have_replaced_the_molecule_keyword()
      {
         _reaction.Formula.FormulaUsablePathBy("M1")[0].ShouldBeEqualTo(_compound.Name);
      }

      [Observation]
      public void should_have_replaced_the_reaction_keyword_with_the_name_of_the_process()
      {
         _reaction.Formula.FormulaUsablePathBy("R1")[0].ShouldBeEqualTo(_interactionProcess.Name);
      }

      [Observation]
      public void should_have_added_the_protein_as_product_of_the_reaction()
      {
         _reaction.Products.Select(x => x.MoleculeName).ShouldContain(_protein.Name);
      }

      [Observation]
      public void should_not_added_the_compound_as_educt_of_the_reaction()
      {
         _reaction.Educts.Select(x => x.MoleculeName).ShouldNotContain(_compound.Name);
      }

      [Observation]
      public void should_have_no_educt_for_the_reaction()
      {
         _reaction.Educts.ShouldBeEmpty();
      }

      [Observation]
      public void should_have_added_the_reaction_formula_to_the_formula_cache()
      {
         _formulaCache.Contains(_reaction.Formula).ShouldBeTrue();
      }
   }
}