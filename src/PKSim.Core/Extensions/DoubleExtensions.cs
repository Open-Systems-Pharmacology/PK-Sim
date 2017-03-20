using OSPSuite.Core.Extensions;

namespace PKSim.Core.Extensions
{
   public static class DoubleExtensions
   {
      public static double CorrectedPercentileValue(this double percentile)
      {
         if (percentile.IsValidPercentile())
            return percentile;

         if (percentile >= 1)
            return CoreConstants.DEFAULT_MAX_PERCENTILE;

         return CoreConstants.DEFAULT_MIN_PERCENTILE;
      }
   }
}