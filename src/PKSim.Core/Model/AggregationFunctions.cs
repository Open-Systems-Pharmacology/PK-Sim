using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Data;
using PKSim.Core.Chart;
using PKSim.Core.Extensions;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Model
{
   public static class AggregationFunctions
   {
      public static Aggregate<BoxWhiskerYValue> BoxWhisker95Aggregation = boxWhiskerAggregation(2.5);

      public static Aggregate<BoxWhiskerYValue> BoxWhisker90Aggregation = boxWhiskerAggregation(5);

      public static Aggregate<float[]> ValuesAggregation = new Aggregate<double, float[]>
      {
         Aggregation = doubles => doubles.ToFloatArray(),
         Name = "Values"
      };

      public static Aggregate<IEnumerable<QuantityValues>> QuantityAggregation = new Aggregate<QuantityValues, IEnumerable<QuantityValues>>
      {
         Aggregation = values => values,
         Name = "Values"
      };

      private static Aggregate<BoxWhiskerYValue> boxWhiskerAggregation(double percentile)
      {
         return new Aggregate<double, BoxWhiskerYValue>
         {
            Aggregation = doubles =>
            {
               var floats = doubles.ToFloatArray().OrderedAndPurified();
               var lowerWhisker = floats.Percentile(percentile);
               var upperWhisker = floats.Percentile(100 - percentile);

               return getBoxWhiskerYValue(lowerWhisker, floats, upperWhisker);
            },
            Name = "BoxWhisker"
         };
      }

      private static BoxWhiskerYValue getBoxWhiskerYValue(float lowerWhisker, float[] orderedArray, float upperWhisker)
      {
         const float outlierRange = 1.5F;

         var lowerBox = orderedArray.Percentile(25);
         var upperBox = orderedArray.Percentile(75);
         var median = orderedArray.Median();
         var lowerLimit = lowerWhisker - outlierRange * (upperBox - lowerBox);
         var upperLimit = upperWhisker + outlierRange * (upperBox - lowerBox);
         var outliers = orderedArray.Where(f => f < lowerLimit || f > upperLimit).ToArray();

         return new BoxWhiskerYValue
         {
            LowerWhisker = lowerWhisker,
            LowerBox = lowerBox,
            Median = median,
            UpperBox = upperBox,
            UpperWhisker = upperWhisker,
            Outliers = outliers
         };
      }
   }
}