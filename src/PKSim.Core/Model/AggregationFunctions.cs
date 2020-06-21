using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Data;
using PKSim.Core.Chart;

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
         const float outlierRange = 1.5F;

         return new Aggregate<double, BoxWhiskerYValue>
         {
            Aggregation = doubles =>
            {
               var orderedValues = doubles.ToFloatArray().OrderedAndPurified();

               var boxWhiskerYValue = new BoxWhiskerYValue
               {
                  LowerWhisker = valueWithIndex(orderedValues, percentile),
                  LowerBox = valueWithIndex(orderedValues,  25),
                  Median = valueWithIndex(orderedValues,  50),
                  UpperBox = valueWithIndex(orderedValues,  75),
                  UpperWhisker = valueWithIndex(orderedValues,  100 - percentile),
               };

               var range = outlierRange * (boxWhiskerYValue.UpperBox - boxWhiskerYValue.LowerBox);

               var lowerLimit = boxWhiskerYValue.LowerWhisker - range;
               var upperLimit = boxWhiskerYValue.UpperWhisker + range;

               var outliers = orderedValues.Where(f => f < lowerLimit || f > upperLimit)
                  .Select(f => new ValueWithIndividualId(f))
                  .ToArray();

               boxWhiskerYValue.Outliers = outliers;
               return boxWhiskerYValue;
            },
            Name = "BoxWhisker"
         };
      }

      private static ValueWithIndividualId valueWithIndex(float[] orderedValues,  double percentile)
      {
         var value = orderedValues.Percentile(percentile);
         return new ValueWithIndividualId(value);
      }
   }
}