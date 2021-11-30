using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_Individual : ContextSpecification<Individual>
   {
      protected OriginData _originData;
      protected Organism _organism;

      protected override void Context()
      {
         _originData = new OriginData();
         sut = new Individual {OriginData = _originData};
         _organism = A.Fake<Organism>();
         sut.Add(_organism);
      }
   }

   public class When_retrieving_the_organism : concern_for_Individual
   {
      [Observation]
      public void should_return_the_organism_it_was_initialized_with()
      {
         sut.Organism.ShouldBeEqualTo(_organism);
      }
   }

   public class When_retrieving_the_list_of_all_available_enzyme_expression : concern_for_Individual
   {
      private IndividualProtein _enzyme1;
      private IndividualProtein _enzyme2;

      protected override void Context()
      {
         base.Context();
         _enzyme1 = new IndividualEnzyme().WithName("_enzyme1");
         _enzyme2 = new IndividualOtherProtein().WithName("_enzyme2");
         sut.AddMolecule(_enzyme1);
         sut.AddMolecule(_enzyme2);
      }

      [Observation]
      public void should_return_all_defined_enzyme_expressions_defined()
      {
         sut.AllMolecules().ShouldOnlyContain(_enzyme1, _enzyme2);
      }
   }

   public class When_retrieving_the_list_of_all_available_enzyme_expression_of_a_specific_type : concern_for_Individual
   {
      private IndividualProtein _enzyme1;
      private IndividualEnzyme _enzyme2;

      protected override void Context()
      {
         base.Context();
         _enzyme1 = new IndividualOtherProtein().WithName("_enzyme1");
         _enzyme2 = new IndividualEnzyme().WithName("_enzyme2");
         sut.AddMolecule(_enzyme1);
         sut.AddMolecule(_enzyme2);
      }

      [Observation]
      public void should_return_all_enzyme_expressions_defined_with_given_type()
      {
         sut.AllMolecules<IndividualEnzyme>().ShouldOnlyContain(_enzyme2);
      }
   }


   public class When_asked_if_the_individual_represents_the_human_species : concern_for_Individual
   {
      private Species _human;
      private Species _notHuman;

      protected override void Context()
      {
         base.Context();
         _human = A.Fake<Species>();
         A.CallTo(() => _human.IsHuman).Returns(true);
         _notHuman = A.Fake<Species>();
         A.CallTo(() => _notHuman.IsHuman).Returns(false);
      }

      [Observation]
      public void should_return_true_if_the_underlying_species_is_human()
      {
         _originData.Species = _human;
         sut.IsHuman.ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_the_underlying_species_is_not_human()
      {
         _originData.Species = _notHuman;
         sut.IsHuman.ShouldBeFalse();
      }
   }

   public class When_asked_if_the_individual_represents_an_age_dependent_species : concern_for_Individual
   {
      protected override void Context()
      {
         base.Context();
         _originData = new OriginData
         {
            SpeciesPopulation = new SpeciesPopulation()
         };
         sut.OriginData  =_originData;
      }

      [Observation]
      public void should_return_true_if_the_underying_population_is_age_dependent()
      {
         _originData.SpeciesPopulation.IsAgeDependent = true;
         sut.IsAgeDependent.ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_the_underying_population_is_not_age_dependent()
      {
         _originData.SpeciesPopulation.IsAgeDependent = false;
         sut.IsAgeDependent.ShouldBeFalse();
      }
   }

   public class When_checking_if_an_individual_created_with_the_preterm_population_and_a_gestational_age_string_smaller_than_40_is_preterm : concern_for_Individual
   {
      protected override void Context()
      {
         base.Context();
         _originData.GestationalAge = 25;
         var preterm = A.Fake<SpeciesPopulation>();
         A.CallTo(() => preterm.IsPreterm).Returns(true);
         _originData.SpeciesPopulation = preterm;
      }

      [Observation]
      public void should_return_if_the_gestional_age_was_defined_and_is_less_than_40_week()
      {
         sut.IsPreterm.ShouldBeTrue();
      }
   }

   public class When_checking_if_an_individual_created_with_a_non_preterm_population_and_a_gestational_age_equal_to_40_is_preterm : concern_for_Individual
   {
      protected override void Context()
      {
         base.Context();
         _originData.GestationalAge = 40;
         var preterm = A.Fake<SpeciesPopulation>();
         A.CallTo(() => preterm.IsPreterm).Returns(false);
         _originData.SpeciesPopulation = preterm;
      }

      [Observation]
      public void should_return_if_the_gestional_age_was_defined_and_is_less_than_40_week()
      {
         sut.IsPreterm.ShouldBeFalse();
      }
   }

   public class When_checking_if_an_individual_created_with_the_a_non_preterm_population : concern_for_Individual
   {
      protected override void Context()
      {
         base.Context();
         _originData.GestationalAge = 40;
         var preterm = A.Fake<SpeciesPopulation>();
         A.CallTo(() => preterm.IsPreterm).Returns(true);
         _originData.SpeciesPopulation = preterm;
      }

      [Observation]
      public void should_return_if_the_gestional_age_was_defined_and_is_less_than_40_week()
      {
         sut.IsPreterm.ShouldBeTrue();
      }
   }
}