using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation
{
   public abstract class concern_for_CompoundDTO : ContextSpecification<CompoundDTO>
   {
      protected override void Context()
      {
         sut = new CompoundDTO();
         sut.AddUsedNames(new[] {"Caffeine"});
         sut.AddForbiddenNames(new[] {"Volume", "Liver", CoreConstants.Molecule.UndefinedLiver});
      }
   }

   public class When_validating_the_name_of_a_new_compound : concern_for_CompoundDTO
   {
      [Observation]
      public void should_be_valid_for_a_name_that_is_neither_used_nor_forbidden()
      {
         sut.Name = "Theophylline";
         sut.IsValid().ShouldBeTrue();
      }

      [Observation]
      public void should_be_invalid_for_a_name_already_used_by_another_compound()
      {
         sut.Name = "Caffeine";
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_be_invalid_for_the_name_of_a_model_element()
      {
         sut.Name = "Volume";
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_be_invalid_for_the_name_of_a_model_element_regardless_of_case_and_whitespaces()
      {
         sut.Name = " liver ";
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_be_invalid_for_the_name_of_a_molecule_created_at_runtime()
      {
         sut.Name = CoreConstants.Molecule.UndefinedLiver;
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_explain_why_the_name_of_a_model_element_cannot_be_used()
      {
         sut.Name = "Volume";
         sut.Validate().Message.ShouldBeEqualTo(PKSimConstants.Error.CompoundNameIsReservedForModelElement("Volume"));
      }
   }
}
