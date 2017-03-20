using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraCharts;

namespace PKSim.UI.Extensions
{
   public static class SeriesExtensions
   {
      public static TSeries WithPoints<TSeries>(this TSeries series, IReadOnlyList<SeriesPoint> seriesPoints) where TSeries : Series
      {
         series.Points.Clear();
         series.Points.AddRange(seriesPoints.ToArray());
         return series;
      }
   }
}