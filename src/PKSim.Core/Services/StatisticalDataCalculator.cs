using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Maths;

namespace PKSim.Core.Services
{
   public interface IStatisticalDataCalculator
   {
      IEnumerable<float[]> StatisticalDataFor(FloatMatrix sortedResults, StatisticalAggregation statisticalAggregation);
   }

   public class StatisticalDataCalculator : IStatisticalDataCalculator
   {
      public IEnumerable<float[]> StatisticalDataFor(FloatMatrix sortedResults, StatisticalAggregation statisticalAggregation)
      {
         var percentileSelection = statisticalAggregation as PercentileStatisticalAggregation;
         if (percentileSelection != null)
         {
            yield return calculatePercentileFor(percentileSelection.Percentile, sortedResults);
            yield break;
         }
         var methodSelection = statisticalAggregation.DowncastTo<PredefinedStatisticalAggregation>();

         switch (methodSelection.Method)
         {
            case StatisticalAggregationType.ArithmeticMean:
               yield return calculateArithmeticMeanFor(sortedResults);
               break;
            case StatisticalAggregationType.ArithmeticStandardDeviation:
               var aritMean = calculateArithmeticMeanFor(sortedResults);
               var aritStd = calculateArithmeticStandardDeviationFor(sortedResults);
               yield return calculateLowerArithmeticStandardDeviationFor(aritMean, aritStd);
               yield return calculateUpperArithmeticStandardDeviationFor(aritMean, aritStd);
               break;
            case StatisticalAggregationType.GeometricMean:
               yield return calculateGeometricMeanFor(sortedResults);
               break;
            case StatisticalAggregationType.Range90:
               yield return calculatePercentileFor(5f, sortedResults);
               yield return calculatePercentileFor(95f, sortedResults);
               break;
            case StatisticalAggregationType.Range95:
               yield return calculatePercentileFor(2.5f, sortedResults);
               yield return calculatePercentileFor(97.5f, sortedResults);
               break;
            case StatisticalAggregationType.GeometricStandardDeviation:
               var geoMean = calculateGeometricMeanFor(sortedResults);
               var geoStd = calculateGeometricStandardDeviationFor(sortedResults);
               yield return calculateLowerGeometricStandardDeviationFor(geoMean, geoStd);
               yield return calculateUpperGeometricStandardDeviationFor(geoMean, geoStd);
               break;
            case StatisticalAggregationType.Min:
               yield return calculateMinFor(sortedResults);
               break;
            case StatisticalAggregationType.Max:
               yield return calculateMaxFor(sortedResults);
               break;
            case StatisticalAggregationType.Median:
               yield return calculateMedianFor(sortedResults);
               break;
            default:
               break;
         }
      }

      private float[] calculateUpperGeometricStandardDeviationFor(float[] geoMean, float[] geoStd)
      {
         return deviationCalculation(geoMean, geoStd, (m, s) => m * s);
      }

      private float[] calculateLowerGeometricStandardDeviationFor(float[] geoMean, float[] geoStd)
      {
         return deviationCalculation(geoMean, geoStd, (m, s) => m / s);
      }

      private float[] calculateUpperArithmeticStandardDeviationFor(float[] aritMean, float[] aritStd)
      {
         return deviationCalculation(aritMean, aritStd, (m, s) => m + s);
      }

      private float[] calculateLowerArithmeticStandardDeviationFor(float[] aritMean, float[] aritStd)
      {
         return deviationCalculation(aritMean, aritStd, (m, s) => m - s);
      }

      private float[] deviationCalculation(float[] mean, float[] std, Func<float, float, float> operation)
      {
         return mean.Select((v, i) => operation(mean[i], std[i])).ToArray();
      }

      private float[] calculateGeometricMeanFor(FloatMatrix quantityResults)
      {
         return calculateValueFor(quantityResults, x => x.GeometricMean());
      }

      private float[] calculateGeometricStandardDeviationFor(FloatMatrix quantityResults)
      {
         return calculateValueFor(quantityResults, x => x.GeometricStandardDeviation());
      }

      private float[] calculateArithmeticStandardDeviationFor(FloatMatrix quantityResults)
      {
         return calculateValueFor(quantityResults, x => x.ArithmeticStandardDeviation());
      }

      private float[] calculateMaxFor(FloatMatrix quantityResults)
      {
         return calculateValueFor(quantityResults, x => x.Last());
      }

      private float[] calculateMinFor(FloatMatrix quantityResults)
      {
         return calculateValueFor(quantityResults, x => x.First());
      }

      private float[] calculateMedianFor(FloatMatrix quantityResults)
      {
         return calculateValueFor(quantityResults, x => new SortedFloatArray(x, true).Median());
      }

      private float[] calculateArithmeticMeanFor(FloatMatrix quantityResults)
      {
         return calculateValueFor(quantityResults, x => x.ArithmeticMean());
      }

      private float[] calculatePercentileFor(double percentile, FloatMatrix quantityResults)
      {
         return calculateValueFor(quantityResults, x => new SortedFloatArray(x, true).Percentile(percentile));
      }

      private float[] calculateValueFor(FloatMatrix quantityResults, Func<float[], float> calculationMethodForSortedArray)
      {
         var result = new float[quantityResults.NumberOfRows];

         for (int i = 0; i < result.Length; i++)
         {
            result[i] = calculationMethodForSortedArray(quantityResults.SortedValueAt(i));
         }

         return result;
      }
   }
}