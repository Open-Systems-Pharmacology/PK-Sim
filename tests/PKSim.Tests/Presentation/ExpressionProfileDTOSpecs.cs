using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.ExpressionProfiles;
using static OSPSuite.Core.Domain.Constants;

namespace PKSim.Presentation
{
   public abstract class concern_for_ExpressionProfileDTO : ContextSpecification<ExpressionProfileDTO>
   {
      protected Species _human;
      protected Species _rat;

      protected override void Context()
      {
         sut = new ExpressionProfileDTO();
         _human = new Species {DisplayName = "Human"};
         _rat = new Species {DisplayName = "Rat"};
      }
   }

   public class When_checking_if_an_expression_profile_DTO_is_valid : concern_for_ExpressionProfileDTO
   {
      [Observation]
      public void should_return_false_if_the_molecule_name_is_not_set()
      {
         sut.Species = _human;
         sut.Category = "Category";
         sut.MoleculeName = "";
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_category_is_not_set()
      {
         sut.Species = _human;
         sut.Category = "";
         sut.MoleculeName = "MoleculeName";
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_category_species_and_molecule_name_combination_already_exists_in_the_project()
      {
         sut.AddExistingExpressionProfileNames(new[] {ContainerName.ExpressionProfileName("MOL", _human.DisplayName, "CAT")});
         sut.Category = "CAT";
         sut.MoleculeName = "MOL";
         sut.Species = _human;
         sut.IsValid().ShouldBeFalse();
      }


      [Observation]
      public void should_return_false_if_the_category_species_and_molecule_name_combination_already_exists_in_the_project_with_other_case()
      {
         sut.AddExistingExpressionProfileNames(new[] { ContainerName.ExpressionProfileName("MOL  ", _human.DisplayName, "cat") });
         sut.Category = "CAT";
         sut.MoleculeName = "MOL";
         sut.Species = _human;
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_if_the_category_species_and_molecule_name_combination_are_defined_and_the_combination_does_not_already_exist_in_the_project()
      {
         sut.AddExistingExpressionProfileNames(new[] {ContainerName.ExpressionProfileName("A", _rat.DisplayName, "C")});

         sut.Category = "CAT";
         sut.Species = _human;
         sut.MoleculeName = "MOL";
         sut.IsValid().ShouldBeTrue();
      }
   }
}