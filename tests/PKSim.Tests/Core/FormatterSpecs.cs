using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Assets;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_NullableBooleanFormatter : ContextSpecification<NullableBooleanFormatter>
   {
      protected override void Context()
      {
         sut = new NullableBooleanFormatter();
      }
   }

   public class When_using_the_nullable_boolean_formatter : concern_for_NullableBooleanFormatter
   {
      [Observation]
      public void should_return_an_empty_string_for_a_nullable_boolean()
      {
         sut.Format(null).ShouldBeNullOrEmpty();
      }

      [Observation]
      public void should_return_the_expected_constants_for_true_and_false()
      {
         sut.Format(true).ShouldBeEqualTo(PKSimConstants.UI.Yes);
         sut.Format(false).ShouldBeEqualTo(PKSimConstants.UI.No);
      }
   }
}