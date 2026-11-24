using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Extensions;
using static OSPSuite.Core.Domain.Constants;

namespace PKSim.Core
{
   public abstract class concern_for_StringExtensions : StaticContextSpecification
   {
   }

   public class When_checking_if_a_string_contains_illegal_characters : concern_for_StringExtensions
   {
      [Observation]
      public void should_return_false_for_a_null_or_empty_string()
      {
         ((string) null).ContainsIllegalCharacters().ShouldBeFalse();
         string.Empty.ContainsIllegalCharacters().ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_for_a_string_without_illegal_characters()
      {
         "CYP3A4 - Lab (2023)".ContainsIllegalCharacters().ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_for_a_string_containing_any_of_the_illegal_characters()
      {
         foreach (var illegalCharacter in ILLEGAL_CHARACTERS)
         {
            $"this{illegalCharacter}that".ContainsIllegalCharacters().ShouldBeTrue($"'{illegalCharacter}' was not detected");
         }
      }
   }
}
