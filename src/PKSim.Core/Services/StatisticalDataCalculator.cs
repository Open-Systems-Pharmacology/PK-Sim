using OSPSuite.Core.Extensions;
using OSPSuite.Core.Maths;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using System;
using System.Collections.Generic;
using System.Linq;
using static PKSim.Core.Services.StatisticalDataCalculator;

namespace PKSim.Core.Services
{
   public interface IStatisticalDataCalculator
   {
      IEnumerable<float[]> StatisticalDataFor(FloatMatrix sortedResults, StatisticalAggregation statisticalAggregation, DeviationModes deviationMode);
   }

   public class StatisticalDataCalculator : IStatisticalDataCalculator
   {
      public enum DeviationModes
      {
         /// <summary>
         /// The deviation will be represented by two values as a range around the mean
         /// </summary>
         Range,
         /// <summary>
         /// The deviation will be represented by a single value
         /// </summary>
         Value
      }

      public IEnumerable<float[]> StatisticalDataFor(FloatMatrix sortedResults, StatisticalAggregation statisticalAggregation, DeviationModes deviationMode)
      {
         if (statisticalAggregation is PercentileStatisticalAggregation percentileSelection)
            return new[] {calculatePercentileFor(percentileSelection.Percentile, sortedResults)};

         var methodSelection = statisticalAggregation.DowncastTo<PredefinedStatisticalAggregation>();

         switch (methodSelection.Method)
         {
            case StatisticalAggregationType.ArithmeticMean:
               return new[] {calculateArithmeticMeanFor(sortedResults)};
            case StatisticalAggregationType.ArithmeticStandardDeviation:
               return arithmeticStandardDeviationsFor(sortedResults, deviationMode, numberOfStandardDeviations: 1);
            case StatisticalAggregationType.ArithmeticTwoStandardDeviations:
               return arithmeticStandardDeviationsFor(sortedResults, deviationMode, numberOfStandardDeviations: 2);
            case StatisticalAggregationType.GeometricMean:
               return new[] {calculateGeometricMeanFor(sortedResults)};
            case StatisticalAggregationType.GeometricStandardDeviation:
               return geometricStandardDeviationsFor(sortedResults, deviationMode, numberOfStandardDeviations: 1);
            case StatisticalAggregationType.GeometricTwoStandardDeviations:
               return geometricStandardDeviationsFor(sortedResults, deviationMode, numberOfStandardDeviations: 2);
            case StatisticalAggregationType.Range90:
               return new[] {calculatePercentileFor(5f, sortedResults), calculatePercentileFor(95f, sortedResults)};
            case StatisticalAggregationType.Range95:
               return new[] {calculatePercentileFor(2.5f, sortedResults), calculatePercentileFor(97.5f, sortedResults)};
            case StatisticalAggregationType.Min:
               return new[] {calculateMinFor(sortedResults)};
            case StatisticalAggregationType.Max:
               return new[] {calculateMaxFor(sortedResults)};
            case StatisticalAggregationType.Median:
               return new[] {calculateMedianFor(sortedResults)};
            default:
               return Enumerable.Empty<float[]>();
         }
      }

      /// <summary>
      ///    Returns the arithmetic deviation scaled by <paramref name="numberOfStandardDeviations" /> (SD * n). In range mode, the
      ///    lower and upper bounds (mean - n * SD, mean + n * SD) are returned instead
      /// </summary>
      private IEnumerable<float[]> arithmeticStandardDeviationsFor(FloatMatrix sortedResults, DeviationModes deviationMode, int numberOfStandardDeviations)
      {
         var arithmeticStd = calculateArithmeticStandardDeviationFor(sortedResults).Select(std => std * numberOfStandardDeviations).ToArray();
         if (deviationMode == DeviationModes.Value)
            return new[] {arithmeticStd};

         var aritMean = calculateArithmeticMeanFor(sortedResults);
         return new[]
         {
            calculateLowerArithmeticStandardDeviationFor(aritMean, arithmeticStd),
            calculateUpperArithmeticStandardDeviationFor(aritMean, arithmeticStd)
         };
      }

      /// <summary>
      ///    Returns the geometric deviation raised to the power of <paramref name="numberOfStandardDeviations" /> (GSD^n). In range mode, the
      ///    lower and upper bounds (mean / GSD^n, mean * GSD^n) are returned instead
      /// </summary>
      private IEnumerable<float[]> geometricStandardDeviationsFor(FloatMatrix sortedResults, DeviationModes deviationMode, int numberOfStandardDeviations)
      {
         var geoStd = calculateGeometricStandardDeviationFor(sortedResults).Select(std => (float) Math.Pow(std, numberOfStandardDeviations)).ToArray();
         if (deviationMode == DeviationModes.Value)
            return new[] {geoStd};

         var geoMean = calculateGeometricMeanFor(sortedResults);
         return new[]
         {
            calculateLowerGeometricStandardDeviationFor(geoMean, geoStd),
            calculateUpperGeometricStandardDeviationFor(geoMean, geoStd)
         };
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
         return calculateValueFor(quantityResults, x => x.Length > 0 ? x.Last() : float.NaN);
      }

      private float[] calculateMinFor(FloatMatrix quantityResults)
      {
         return calculateValueFor(quantityResults, x => x.Length > 0 ? x.First() : float.NaN);
      }

      private float[] calculateMedianFor(FloatMatrix quantityResults)
      {
         return calculateValueFor(quantityResults, x => new SortedFloatArray(x, alreadySorted: true).Median());
      }

      private float[] calculateArithmeticMeanFor(FloatMatrix quantityResults)
      {
         return calculateValueFor(quantityResults, x => x.ArithmeticMean());
      }

      private float[] calculatePercentileFor(double percentile, FloatMatrix quantityResults)
      {
         return calculateValueFor(quantityResults, x => new SortedFloatArray(x, alreadySorted: true).Percentile(percentile));
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