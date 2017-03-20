using System.Linq;

namespace PKSim.Core.Chart
{
   public static class CurveDataExtensions
   {
      public static bool IsRange(this CurveData<TimeProfileXValue, TimeProfileYValue> timeProfileCurveData)
      {
         return timeProfileCurveData.YValues.Any(x => x.IsRange);
      }
   }
}