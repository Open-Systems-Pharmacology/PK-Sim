using System.Collections.Generic;
using DevExpress.XtraCharts;
using PKSim.Core.Chart;
using PKSim.UI.Extensions;
using PKSim.UI.Views.PopulationAnalyses;

namespace PKSim.UI.Binders
{
   public class RangeChartDataBinder : ChartDataBinder<RangeXValue, RangeYValue>
   {
      public RangeChartDataBinder(RangeChartView view) : base(view, ViewType.RangeArea)
      {
      }

      protected override IReadOnlyList<Series> CreateCurveSeriesList(CurveData<RangeXValue, RangeYValue> curveData)
      {
         var rangeAreaSeries = CreateRangeAreaSeries(curveData)
            .WithPoints(CreateSeriesPoints(curveData, y => y.LowerPercentile, y => y.UpperPercentile));

         var medianAreaSeries = CreateLineSeries(curveData)
            .WithPoints(CreateSeriesPoints(curveData, y => y.Median));

         return new[] {rangeAreaSeries, medianAreaSeries};
      }
   }
}