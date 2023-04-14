using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.Core
{
   public abstract class concern_for_CompetitiveInhibitionsKineticUpdaterSpecification : ContextSpecification<CompetitiveInhibitionsKineticUpdaterSpecification>
   {
      protected string _moleculeName = "ENZYME";
      protected string _drugName = "DRUG";
      protected Simulation _simulation;
      protected InteractionProperties _interactionProperties;
      protected Compound _compound1;
      protected Compound _compound2;
      protected InteractionProcess _competitiveInhibition1;
      protected InteractionProcess _competitiveInhibition2;
      protected InhibitionProcess _uncompetitiveInhibition;

      protected IObjectPathFactory _objectPathFactory;
      protected IDimensionRepository _dimensionRepository;
      private IInteractionTask _interactionTask;

      protected override void Context()
      {
         _simulation = A.Fake<Simulation>();
         _competitiveInhibition1 = new InhibitionProcess { InteractionType = InteractionType.CompetitiveInhibition }.WithName("CompetitiveInhibition1");
         _competitiveInhibition2 = new InhibitionProcess { InteractionType = InteractionType.CompetitiveInhibition }.WithName("CompetitiveInhibition2");
         _uncompetitiveInhibition = new InhibitionProcess { InteractionType = InteractionType.UncompetitiveInhibition }.WithName("UncompetitiveInhibition");
         _compound1 = new Compound().WithName("Compound1");
         _compound1.AddProcess(_competitiveInhibition1);
         _compound1.AddProcess(_uncompetitiveInhibition);

         _compound2 = new Compound().WithName("Compound2");
         _compound2.AddProcess(_competitiveInhibition2);

         _interactionProperties = new InteractionProperties();
         A.CallTo(() => _simulation.InteractionProperties).Returns(_interactionProperties);
         A.CallTo(() => _simulation.Compounds).Returns(new[] { _compound1, _compound2 });

         _objectPathFactory = new ObjectPathFactoryForSpecs();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _interactionTask = new InteractionTask();
         sut = new CompetitiveInhibitionsKineticUpdaterSpecification(_objectPathFactory, _dimensionRepository, _interactionTask);

         _interactionProperties.AddInteraction(new InteractionSelection { MoleculeName = _moleculeName, ProcessName = _competitiveInhibition1.Name, CompoundName = _compound1.Name });
         _interactionProperties.AddInteraction(new InteractionSelection { MoleculeName = _moleculeName, ProcessName = _competitiveInhibition2.Name, CompoundName = _compound2.Name });
         _interactionProperties.AddInteraction(new InteractionSelection { MoleculeName = _moleculeName, ProcessName = _uncompetitiveInhibition.Name, CompoundName = _compound2.Name });
      }
   }

   public class When_retrieving_the_km_numerator_factor_for_a_competitive_inhibition : concern_for_CompetitiveInhibitionsKineticUpdaterSpecification
   {
      [Observation]
      public void should_only_return_the_part_of_the_interaction_for_competitive_inhibition()
      {
         sut.KmNumeratorTerm(_moleculeName, _drugName, _simulation).ShouldBeEqualTo("K_water_c1*Ic1/KcI1 + K_water_c2*Ic2/KcI2");
      }
   }

   public class When_retrieving_the_km_denominator_factor_for_a_competitive_inhibition : concern_for_CompetitiveInhibitionsKineticUpdaterSpecification
   {
      [Observation]
      public void should_return_an_empty_string()
      {
         sut.KmDenominatorTerm(_moleculeName, _drugName, _simulation).ShouldBeEmpty();
      }
   }

   public class When_retrieving_the_vmax_denominator_factor_for_a_competitive_inhibition : concern_for_CompetitiveInhibitionsKineticUpdaterSpecification
   {
      [Observation]
      public void should_return_an_empty_string()
      {
         sut.KcatDenominatorTerm(_moleculeName, _drugName, _simulation).ShouldBeEmpty();
      }
   }

   public class When_retrieving_the_cl_spec_denominator_factor_for_a_competitive_inhibition : concern_for_CompetitiveInhibitionsKineticUpdaterSpecification
   {
      [Observation]
      public void should_only_return_the_part_of_the_interaction_for_competitive_inhibition()
      {
         sut.CLSpecDenominatorTerm(_moleculeName, _drugName, _simulation).ShouldBeEqualTo("K_water_c1*Ic1/KcI1 + K_water_c2*Ic2/KcI2");
      }
   }

   public class When_updating_the_modifiers_defined_in_a_reaction_process : concern_for_CompetitiveInhibitionsKineticUpdaterSpecification
   {
      private ReactionBuilder _reaction;

      protected override void Context()
      {
         base.Context();
         _reaction = new ReactionBuilder();
         _reaction.AddModifier(_compound1.Name);
      }

      protected override void Because()
      {
         sut.UpdateModifiers(_reaction, _moleculeName, _drugName, _simulation);
      }

      [Observation]
      public void should_only_add_the_name_of_inhibitors_if_not_already_defined_as_modifier()
      {
         _reaction.ModifierNames.ShouldOnlyContain(_compound1.Name, _compound2.Name);
      }
   }

   public class When_checking_if_an_update_is_required_based_on_a_given_interaction_configuration : concern_for_CompetitiveInhibitionsKineticUpdaterSpecification
   {
      protected override void Context()
      {
         base.Context();
         _interactionProperties.ClearInteractions();
      }

      [Observation]
      public void should_return_true_if_the_simulation_contains_any_competitive_inhibition_interaction()
      {
         _interactionProperties.AddInteraction(new InteractionSelection { MoleculeName = _moleculeName, ProcessName = _competitiveInhibition2.Name, CompoundName = _compound2.Name });
         sut.UpdateRequiredFor(_moleculeName, _drugName, _simulation).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_otherwise()
      {
         _interactionProperties.AddInteraction(new InteractionSelection { MoleculeName = _moleculeName, ProcessName = _uncompetitiveInhibition.Name, CompoundName = _compound2.Name });
         sut.UpdateRequiredFor(_moleculeName, _drugName, _simulation).ShouldBeFalse();
      }
   }

   public class When_updating_the_references_to_used_parameters_for_the_km_factor_formula_for_a_competitive_inhibition_interactions : concern_for_CompetitiveInhibitionsKineticUpdaterSpecification
   {
      private IParameter _kmFactor;
      private ReactionBuilder _reaction;

      protected override void Context()
      {
         base.Context();
         _kmFactor = new Parameter().WithFormula(new ExplicitFormula());
         _reaction = new ReactionBuilder();
         _interactionProperties.ClearInteractions();
         _interactionProperties.AddInteraction(new InteractionSelection { MoleculeName = _moleculeName, ProcessName = _competitiveInhibition2.Name, CompoundName = _compound2.Name });
      }

      protected override void Because()
      {
         sut.UpdateKmFactorReferences(_kmFactor, _moleculeName, _drugName, _simulation, _reaction);
      }

      [Observation]
      public void should_add_the_references_to_the_inhibitor_concentrations()
      {
         _kmFactor.Formula.FormulaUsablePathBy("Ic1").ShouldNotBeNull();
      }

      [Observation]
      public void should_have_created_the_expected_paths()
      {
         var inibitorPath = _kmFactor.Formula.FormulaUsablePathBy("Ic1");
         inibitorPath.ShouldOnlyContainInOrder(ObjectPath.PARENT_CONTAINER, ObjectPath.PARENT_CONTAINER, _compound2.Name, Constants.Parameters.CONCENTRATION);

         var kmPath = _kmFactor.Formula.FormulaUsablePathBy("KcI1");
         kmPath.ShouldOnlyContainInOrder(_compound2.Name, _competitiveInhibition2.Name, CoreConstants.Parameters.KI);
      }

      [Observation]
      public void should_add_references_to_the_kI_parameters()
      {
         _kmFactor.Formula.FormulaUsablePathBy("KcI1").ShouldNotBeNull();
      }
   }

   public class When_updating_the_references_to_used_parameters_for_the_cl_spec_factor_formula_for_a_competitive_inhibition_interactions : concern_for_CompetitiveInhibitionsKineticUpdaterSpecification
   {
      private IParameter _clSpecFactor;
      private ReactionBuilder _reaction;

      protected override void Context()
      {
         base.Context();
         _clSpecFactor = new Parameter().WithFormula(new ExplicitFormula());
         _reaction = new ReactionBuilder();
         _interactionProperties.ClearInteractions();
         _interactionProperties.AddInteraction(new InteractionSelection { MoleculeName = _moleculeName, ProcessName = _competitiveInhibition2.Name, CompoundName = _compound2.Name });
      }

      protected override void Because()
      {
         sut.UpdateCLSpecFactorReferences(_clSpecFactor, _moleculeName, _drugName, _simulation, _reaction);
      }

      [Observation]
      public void should_add_the_references_to_the_inhibitor_concentrations()
      {
         _clSpecFactor.Formula.FormulaUsablePathBy("Ic1").ShouldNotBeNull();
      }

      [Observation]
      public void should_have_created_the_expected_paths()
      {
         var inibitorPath = _clSpecFactor.Formula.FormulaUsablePathBy("Ic1");
         inibitorPath.ShouldOnlyContainInOrder(ObjectPath.PARENT_CONTAINER, ObjectPath.PARENT_CONTAINER, _compound2.Name, Constants.Parameters.CONCENTRATION);

         var kmPath = _clSpecFactor.Formula.FormulaUsablePathBy("KcI1");
         kmPath.ShouldOnlyContainInOrder(_compound2.Name, _competitiveInhibition2.Name, CoreConstants.Parameters.KI);
      }

      [Observation]
      public void should_add_references_to_the_kI_parameters()
      {
         _clSpecFactor.Formula.FormulaUsablePathBy("KcI1").ShouldNotBeNull();
      }
   }

   public class When_updating_the_references_to_used_parameters_for_the_cl_spec_factor_formula_for_a_competitive_inhibition_interactions_in_a_transport_process : concern_for_CompetitiveInhibitionsKineticUpdaterSpecification
   {
      private IParameter _clSpecFactor;
      private TransporterMoleculeContainer _transporterMoleculeContainer;

      protected override void Context()
      {
         base.Context();
         _clSpecFactor = new Parameter().WithFormula(new ExplicitFormula());
         _transporterMoleculeContainer = new TransporterMoleculeContainer();
         _interactionProperties.ClearInteractions();
         _interactionProperties.AddInteraction(new InteractionSelection { MoleculeName = _moleculeName, ProcessName = _competitiveInhibition2.Name, CompoundName = _compound2.Name });
      }

      protected override void Because()
      {
         sut.UpdateCLSpecFactorReferences(_clSpecFactor, _moleculeName, _drugName, _simulation, _transporterMoleculeContainer);
      }

      [Observation]
      public void should_add_the_references_to_the_inhibitor_concentrations()
      {
         _clSpecFactor.Formula.FormulaUsablePathBy("Ic1").ShouldNotBeNull();
      }

      [Observation]
      public void should_have_created_the_expected_paths()
      {
         var inibitorPath = _clSpecFactor.Formula.FormulaUsablePathBy("Ic1");
         inibitorPath.ShouldOnlyContainInOrder(ObjectPathKeywords.SOURCE, _compound2.Name, Constants.Parameters.CONCENTRATION);

         var kmPath = _clSpecFactor.Formula.FormulaUsablePathBy("KcI1");
         kmPath.ShouldOnlyContainInOrder(_compound2.Name, _competitiveInhibition2.Name, CoreConstants.Parameters.KI);
      }

      [Observation]
      public void should_add_references_to_the_kI_parameters()
      {
         _clSpecFactor.Formula.FormulaUsablePathBy("KcI1").ShouldNotBeNull();
      }
   }
}