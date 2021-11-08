using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Core;
using PKSim.Presentation.DTO.ExpressionProfiles;

namespace PKSim.Presentation
{
   public abstract class concern_for_ExpressionProfileDTO : ContextSpecification<ExpressionProfileDTO>
   {
      protected override void Context()
      {
         sut = new ExpressionProfileDTO();
      }
   }

   public class When_checking_if_an_expression_profile_DTO_is_valid : concern_for_ExpressionProfileDTO
   {
      [Observation]
      public void should_return_false_if_the_molecule_name_is_not_set()
      {
         sut.Category = "Category";
         sut.MoleculeName = "";
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_category_is_not_set()
      {
         sut.Category = "";
         sut.MoleculeName = "MoleculeName";
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_category_and_molecule_name_combination_already_exists_in_the_project()
      {
         sut.AddExistingExpressionProfileNames(new[] {CoreConstants.ContainerName.ExpressionProfileName("MOL", "CAT")});
         sut.Category = "CAT";
         sut.MoleculeName = "MOL";
         sut.IsValid().ShouldBeFalse();
      }


      [Observation]
      public void should_return_true_if_the_category_and_molecule_name_combination_are_defined_and_the_combination_does_not_already_exist_in_the_project()
      {
         sut.AddExistingExpressionProfileNames(new[] { CoreConstants.ContainerName.ExpressionProfileName("A", "B") });
         sut.Category = "CAT";
         sut.MoleculeName = "MOL";
         sut.IsValid().ShouldBeTrue();
      }
   }
}