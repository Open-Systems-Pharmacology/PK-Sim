using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Extensions
{
   public static class CurveExtensions
   {
      /// <summary>
      /// Returns <c>true</c> if the <paramref name="curve"/> represents a Concentration Time profile curve otherwise <c>false</c>
      /// </summary>
      public static bool IsConcentrationTimeProfile(this Curve curve)
      {
         if (curve == null)
            return false;

         var xData = curve.xData;
         var yData = curve.yData;
         
         if (xData == null || yData == null)
            return false;

         if (!string.Equals(xData.Dimension.Name, Constants.Dimension.TIME))
            return false;

         return yData.IsConcentration();
      }

      /// <summary>
      /// Returns the subset of <see cref="ICurve"/> defined in <paramref name="curves"/> that can be used for PK-Analysis calculations
      /// </summary>
      public static IReadOnlyList<Curve> ForPKAnalysis(this IEnumerable<Curve> curves)
      {
         return curves.Where(c => c.Visible).Where(c => c.IsConcentrationTimeProfile()).ToList();
      }
   }
}