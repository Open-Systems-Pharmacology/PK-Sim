using System.Collections.Generic;
using DevExpress.XtraCharts;
using PKSim.Core.Chart;
using PKSim.UI.Extensions;
using PKSim.UI.Views.PopulationAnalyses;

namespace PKSim.UI.Binders
{
   public class ScatterChartDataBinder : ChartDataBinder<ScatterXValue, ScatterYValue>
   {
      public ScatterChartDataBinder(ScatterChartView view) : base(view, ViewType.Point)
      {
      }

      protected override IReadOnlyList<Series> CreateCurveSeriesList(CurveData<ScatterXValue, ScatterYValue> curveData)
      {
         var series = CreateSeries(curveData, ViewType.Point)
            .WithPoints(CreateSeriesPoints(curveData, y => y.Value));

         return new[] {series};
      }
   }
}