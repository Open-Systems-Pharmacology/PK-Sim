using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Extensions;
using OSPSuite.Core.Extensions;

namespace PKSim.Core
{
   public class DoubleExtensionsSpecs : StaticContextSpecification
   {
      [Observation]
      public void a_double_between_0_and_1_strict_should_be_a_valid_percentile()
      {
         0.5.IsValidPercentile().ShouldBeTrue();
      }

      [Observation]
      public void a_double_equal_to_0_or_1_should_not_be_a_valid_percentile()
      {
         0.0.IsValidPercentile().ShouldBeFalse();
         1.0.IsValidPercentile().ShouldBeFalse();
      }

      [Observation]
      public void a_double_strict_out_of_interval_0_and_1_should_not_be_a_valid_percentile()
      {
         (-3.0).IsValidPercentile().ShouldBeFalse();
         8.0.IsValidPercentile().ShouldBeFalse();
      }

      [Observation]
      public void the_corrected_value_of_a_double_bigger_than_1_should_be_the_max_percentile()
      {
         (1.0).CorrectedPercentileValue().ShouldBeEqualTo(CoreConstants.DEFAULT_MAX_PERCENTILE);
         (2.0).CorrectedPercentileValue().ShouldBeEqualTo(CoreConstants.DEFAULT_MAX_PERCENTILE);
      }

      [Observation]
      public void the_corrected_value_of_a_double_smaller_than_0_should_be_the_min_percentile()
      {
         (-2.0).CorrectedPercentileValue().ShouldBeEqualTo(CoreConstants.DEFAULT_MIN_PERCENTILE);
         (0d).CorrectedPercentileValue().ShouldBeEqualTo(CoreConstants.DEFAULT_MIN_PERCENTILE);
      }
   }
}