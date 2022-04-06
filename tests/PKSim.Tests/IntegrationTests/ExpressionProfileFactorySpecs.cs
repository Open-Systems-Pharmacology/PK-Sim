using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ExpressionProfileFactory : ContextForIntegration<IExpressionProfileFactory>
   {
      protected ISpeciesRepository _speciesRepository;
      protected IUsedExpressionProfileCategoryRepository _usedExpressionProfileCategoryRepository;

      protected override void Context()
      {
         sut = IoC.Resolve<IExpressionProfileFactory>();
         _speciesRepository = IoC.Resolve<ISpeciesRepository>();
         _usedExpressionProfileCategoryRepository = IoC.Resolve<IUsedExpressionProfileCategoryRepository>();
      }
   }

   public class When_creating_en_expression_profile_for_a_given_molecule_type : concern_for_ExpressionProfileFactory
   {
      private ExpressionProfile _expressionProfile;

      protected override void Because()
      {
         _expressionProfile = sut.Create<IndividualEnzyme>();
      }

      [Observation]
      public void should_return_an_expression_profile_created_for_the_default_species()
      {
         _expressionProfile.Individual.ShouldNotBeNull();
         _expressionProfile.Species.ShouldBeEqualTo(_speciesRepository.DefaultSpecies);
      }

      [Observation]
      public void should_set_the_default_category_to_the_first_entry_available_in_the_used_category_repository()
      {
         _expressionProfile.Category.ShouldBeEqualTo(_usedExpressionProfileCategoryRepository.All().FirstOrDefault());
      }


      [Observation]
      public void should_have_created_a_molecule_for_the_given_molecule_type()
      {
         _expressionProfile.Molecule.ShouldBeAnInstanceOf<IndividualEnzyme>();
      }

      [Observation]
      public void should_not_have_added_the_ontogeny_parameter_into_the_individual_for_the_enzyme()
      {
         _expressionProfile.Molecule.AllOntogenyParameters.Where(x => x != null).ShouldBeEmpty();
      }

      [Observation]
      public void should_return_a_loaded_building_block()
      {
         _expressionProfile.IsLoaded.ShouldBeTrue();
      }
   }

   public class When_updating_the_species_for_an_expression_profile : concern_for_ExpressionProfileFactory
   {
      private ExpressionProfile _expressionProfile;

      protected override void Context()
      {
         base.Context();
         _expressionProfile = sut.Create<IndividualTransporter>();
      }

      protected override void Because()
      {
         sut.UpdateSpecies(_expressionProfile, _speciesRepository.FindByName(CoreConstants.Species.MOUSE));
      }

      [Observation]
      public void should_have_updated_the_species_in_the_underlying_profile()
      {
         _expressionProfile.Individual.ShouldNotBeNull();
         _expressionProfile.Species.ShouldBeEqualTo(_speciesRepository.FindByName(CoreConstants.Species.MOUSE));
      }

      [Observation]
      public void should_have_created_a_molecule_for_the_given_molecule_type()
      {
         _expressionProfile.Molecule.ShouldBeAnInstanceOf<IndividualTransporter>();
      }
   }
}