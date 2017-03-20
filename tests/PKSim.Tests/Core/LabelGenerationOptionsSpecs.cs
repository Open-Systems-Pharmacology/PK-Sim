using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using NUnit.Framework;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_LabelGenerationOptions : ContextSpecification<LabelGenerationOptions>
   {
      protected override void Context()
      {
         sut = new LabelGenerationOptions();
      }
   }

   public class When_checking_if_a_pattern_is_using_intervals : concern_for_LabelGenerationOptions
   {
      [Observation]
      public void should_return_false_if_the_pattern_is_null()
      {
         sut.Pattern = null;
         sut.HasIntervalPattern.ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_pattern_is_empty()
      {
         sut.Pattern = string.Empty;
         sut.HasIntervalPattern.ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_if_the_pattern_using_the_interval_without_digits()
      {
         sut.Pattern = "{start}";
         sut.HasIntervalPattern.ShouldBeTrue();
      }

      [Observation]
      public void should_return_true_if_the_pattern_using_the_interval_with_digits()
      {
         sut.Pattern = "{start:2}";
         sut.HasIntervalPattern.ShouldBeTrue();
      }
   }

   public class When_replacing_a_pattern_with_a_given_value : concern_for_LabelGenerationOptions
   {
      [TestCase("A {start}", 5d, "A 5.00")]
      [TestCase("A {start:1}", 5d, "A 5.0")]
      [TestCase("A {start:4}", 5d, "A 5.0000")]
      public void should_return_the_expected_value(string pattern, double value, string expectation)
      {
         sut.Pattern = pattern;
         sut.ReplaceStartIntervalIn(pattern, value).ShouldBeEqualTo(expectation);
      }
   }
}