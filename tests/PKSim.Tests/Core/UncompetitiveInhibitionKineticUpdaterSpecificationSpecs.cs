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
   public abstract class concern_for_UncompetitiveInhibitionKineticUpdaterSpecification : ContextSpecification<UncompetitiveInhibitionKineticUpdaterSpecification>
   {
      private IDimensionRepository _dimensionRepository;
      private IObjectPathFactory _objectPathFactory;
      private InhibitionProcess _competitiveInhibition;
      protected InhibitionProcess _uncompetitiveInhibition2;
      protected InhibitionProcess _uncompetitiveInhibition1;
      protected Compound _compound1;
      protected Compound _compound2;
      protected InteractionProperties _interactionProperties;
      protected Simulation _simulation;
      protected string _moleculeName = "ENZYME";
      protected string _drugName = "DRUG";
      private IInteractionTask _interactionTask;

      protected override void Context()
      {
         _objectPathFactory = new ObjectPathFactoryForSpecs();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _interactionTask =new InteractionTask();
         sut = new UncompetitiveInhibitionKineticUpdaterSpecification(_objectPathFactory, _dimensionRepository, _interactionTask);

         _simulation = A.Fake<Simulation>();
         _competitiveInhibition = new InhibitionProcess {InteractionType = InteractionType.CompetitiveInhibition}.WithName("CompetitiveInhibition");
         _uncompetitiveInhibition1 = new InhibitionProcess {InteractionType = InteractionType.UncompetitiveInhibition}.WithName("UncompetitiveInhibition1");
         _uncompetitiveInhibition2 = new InhibitionProcess {InteractionType = InteractionType.UncompetitiveInhibition}.WithName("UncompetitiveInhibition2");

         _interactionProperties = new InteractionProperties();
         A.CallTo(() => _simulation.InteractionProperties).Returns(_interactionProperties);

         _compound1 = new Compound().WithName("Compound1");
         _compound1.AddProcess(_competitiveInhibition);

         _compound2 = new Compound().WithName("Compound2");
         _compound2.AddProcess(_uncompetitiveInhibition1);
         _compound2.AddProcess(_uncompetitiveInhibition2);


         A.CallTo(() => _simulation.Compounds).Returns(new[] {_compound1, _compound2});

         _interactionProperties.AddInteraction(new InteractionSelection {MoleculeName = _moleculeName, ProcessName = _competitiveInhibition.Name, CompoundName = _compound1.Name});
         _interactionProperties.AddInteraction(new InteractionSelection {MoleculeName = _moleculeName, ProcessName = _uncompetitiveInhibition1.Name, CompoundName = _compound2.Name});
         _interactionProperties.AddInteraction(new InteractionSelection {MoleculeName = _moleculeName, ProcessName = _uncompetitiveInhibition2.Name, CompoundName = _compound2.Name});
      }
   }

   public class When_retrieving_the_km_numerator_factor_for_an_uncompetitive_inhibition : concern_for_UncompetitiveInhibitionKineticUpdaterSpecification
   {
      [Observation]
      public void should_return_an_empty_string()
      {
         sut.KmNumeratorTerm(_moleculeName, _drugName, _simulation).ShouldBeEmpty();
      }
   }

   public class When_retrieving_the_km_denominator_factor_for_an_uncompetitive_inhibition : concern_for_UncompetitiveInhibitionKineticUpdaterSpecification
   {
      [Observation]
      public void should_only_return_the_part_of_the_interaction_for_uncompetitive_inhibition()
      {
         sut.KmDenominatorTerm(_moleculeName, _drugName, _simulation).ShouldBeEqualTo("K_water1*Iu1/KuI1 + K_water2*Iu2/KuI2");
      }
   }

   public class When_retrieving_the_vmax_denominator_factor_for_an_uncompetitive_inhibition : concern_for_UncompetitiveInhibitionKineticUpdaterSpecification
   {
      [Observation]
      public void should_only_return_the_part_of_the_interaction_for_uncompetitive_inhibition()
      {
         sut.KcatDenominatorTerm(_moleculeName, _drugName, _simulation).ShouldBeEqualTo("K_water1*Iu1/KuI1 + K_water2*Iu2/KuI2");
      }
   }

   public class When_retrieving_the_cl_denominator_factor_for_an_uncompetitive_inhibition : concern_for_UncompetitiveInhibitionKineticUpdaterSpecification
   {
      [Observation]
      public void should_only_return_the_part_of_the_interaction_for_uncompetitive_inhibition()
      {
         sut.KmNumeratorTerm(_moleculeName, _drugName, _simulation).ShouldBeEmpty();
      }
   }

   public class When_retrieving_the_km_denominator_factor_for_an_uncompetitive_inhibition_taking_for_drug_being_also_the_inhibitor : concern_for_UncompetitiveInhibitionKineticUpdaterSpecification
   {
      protected override void Context()
      {
         base.Context();
         _compound1 = new Compound().WithName(_drugName);
         _compound1.AddProcess(_uncompetitiveInhibition1);

         _compound2 = new Compound().WithName("Compound2");
         _compound2.AddProcess(_uncompetitiveInhibition2);

         A.CallTo(() => _simulation.Compounds).Returns(new[] {_compound1, _compound2});

         _interactionProperties.ClearInteractions();

         _interactionProperties.AddInteraction(new InteractionSelection {MoleculeName = _moleculeName, ProcessName = _uncompetitiveInhibition1.Name, CompoundName = _compound1.Name});
         _interactionProperties.AddInteraction(new InteractionSelection {MoleculeName = _moleculeName, ProcessName = _uncompetitiveInhibition2.Name, CompoundName = _compound2.Name});
      }

      [Observation]
      public void should_have_removed_the_terms_in_the_km_denominator_due_to_the_inhibitor_itself()
      {
         sut.KmDenominatorTerm(_moleculeName, _drugName, _simulation).ShouldBeEqualTo("K_water1*Iu1/KuI1");
      }

      [Observation]
      public void should_have_removed_the_terms_in_the_vmax_denominator_due_to_the_inhibitor_itself()
      {
         sut.KcatDenominatorTerm(_moleculeName, _drugName, _simulation).ShouldBeEqualTo("K_water1*Iu1/KuI1");
      }
   }

   public class When_updating_the_references_to_used_parameters_for_the_km_factor_formula_for_an_uncompetitive_inhibition_interactions_with_autoinhibition : concern_for_UncompetitiveInhibitionKineticUpdaterSpecification
   {
      private IParameter _kmFactor;
      private IReactionBuilder _reaction;

      protected override void Context()
      {
         base.Context();
         _compound1 = new Compound().WithName(_drugName);
         _compound1.AddProcess(_uncompetitiveInhibition1);

         _compound2 = new Compound().WithName("Compound2");
         _compound2.AddProcess(_uncompetitiveInhibition2);

         A.CallTo(() => _simulation.Compounds).Returns(new[] {_compound1, _compound2});

         _kmFactor = new Parameter().WithFormula(new ExplicitFormula());
         _reaction = new ReactionBuilder();
         _interactionProperties.ClearInteractions();
         _interactionProperties.AddInteraction(new InteractionSelection {MoleculeName = _moleculeName, ProcessName = _uncompetitiveInhibition1.Name, CompoundName = _compound1.Name});
         _interactionProperties.AddInteraction(new InteractionSelection {MoleculeName = _moleculeName, ProcessName = _uncompetitiveInhibition2.Name, CompoundName = _compound2.Name});
      }

      protected override void Because()
      {
         sut.UpdateKmFactorReferences(_kmFactor, _moleculeName, _drugName, _simulation, _reaction);
      }

      [Observation]
      public void should_have_created_the_expected_paths()
      {
         var inibitorPath = _kmFactor.Formula.FormulaUsablePathBy("Iu1");
         inibitorPath.ShouldOnlyContainInOrder(ObjectPath.PARENT_CONTAINER, ObjectPath.PARENT_CONTAINER, _compound2.Name, Constants.Parameters.CONCENTRATION);
      }

      [Observation]
      public void should_not_create_a_reference_to_the_auto_inhibited_inhibitor()
      {
         _kmFactor.Formula.FormulaUsablePathBy("Iu2").ShouldBeNull();
      }
   }
}