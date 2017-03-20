using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Chart;

namespace PKSim.Core
{
   public abstract class concern_for_CurveDataExtensions : StaticContextSpecification
   {
      protected CurveData<TimeProfileXValue, TimeProfileYValue> _timeProfileCurveData;

      protected override void Context()
      {
         _timeProfileCurveData = new CurveData<TimeProfileXValue, TimeProfileYValue>();
      }
   }

   public class When_checking_if_a_time_profile_curve_data_is_a_range_curve : concern_for_CurveDataExtensions
   {
      [Observation]
      public void should_return_true_if_at_least_one_point_represents_a_range_point()
      {
         _timeProfileCurveData.Add(new TimeProfileXValue(1), new TimeProfileYValue{LowerValue = 1, Y = 2, UpperValue = 3});
         _timeProfileCurveData.Add(new TimeProfileXValue(1), new TimeProfileYValue{Y = 2});
         _timeProfileCurveData.IsRange().ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_the_curve_data_is_empty()
      {
         _timeProfileCurveData.IsRange().ShouldBeFalse();
      }


      [Observation]
      public void should_return_false_if_not_one_point_represents_a_range_plot()
      {
         _timeProfileCurveData.Add(new TimeProfileXValue(1), new TimeProfileYValue { Y = 2 });
         _timeProfileCurveData.Add(new TimeProfileXValue(2), new TimeProfileYValue { Y = 3 });
         _timeProfileCurveData.IsRange().ShouldBeFalse();
      }
   }
}	