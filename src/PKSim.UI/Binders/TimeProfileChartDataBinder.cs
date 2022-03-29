using System;
using System.Collections.Generic;
using OSPSuite.Utility.Extensions;
using DevExpress.XtraCharts;
using PKSim.Core.Chart;
using PKSim.UI.Extensions;
using PKSim.UI.Views.PopulationAnalyses;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain.Data;

namespace PKSim.UI.Binders
{
   public class TimeProfileChartDataBinder : ChartDataBinder<TimeProfileXValue, TimeProfileYValue>
   {
      public TimeProfileChartDataBinder(TimeProfileChartView view) : base(view, ViewType.Line)
      {
      }

      protected override IReadOnlyList<Series> CreateCurveSeriesList(CurveData<TimeProfileXValue, TimeProfileYValue> curveData)
      {
         Series series;
         if (curveData.IsRange())
         {
            series = CreateRangeAreaSeries(curveData)
               .WithPoints(CreateSeriesPoints(curveData, y => y.LowerValue, y => y.UpperValue));
         }
         else
         {
            series = CreateLineSeries(curveData)
               .WithPoints(CreateSeriesPoints(curveData, y => y.Y));
         }

         return new[] {series};
      }

      protected override IReadOnlyList<Series> CreateObservedDataSeriesList(ObservedCurveData observedCurveData)
      {
         Series meanPoints = null, meanLine = null;
         var series = new List<Series>();
         if (!observedCurveData.Visible)
            return series;

         if (observedCurveData.Symbol != Symbols.None)
         {
            meanPoints = CreateSeries(observedCurveData, ViewType.Point)
               .WithPoints(CreateSeriesPoints(observedCurveData, y => y.Mean));
         }

         if (observedCurveData.LineStyle != LineStyles.None)
         {
            meanLine = CreateLineSeries(observedCurveData)
               .WithPoints(CreateSeriesPoints(observedCurveData, y => y.Mean));
         }

         series.AddRange(new[]{meanPoints, meanLine});
         if (observedCurveData.ErrorType == AuxiliaryType.Undefined)
            return series;

         var verticalLine = CreateFinancialSeries(observedCurveData)
            .WithPoints(CreateSeriesPoints(observedCurveData, y => y.LowerValue, y => y.UpperValue, y => y.Mean, y => y.Mean));

         var upperPoint = createExtremSeries(observedCurveData, y => y.UpperValue, MarkerKind.InvertedTriangle);
         var lowerPoint = createExtremSeries(observedCurveData, y => y.LowerValue, MarkerKind.Triangle);

         series.AddRange(new[] {upperPoint, lowerPoint, verticalLine });
      
         return series;
      }

      private Series createExtremSeries(ObservedCurveData observedCurveData, Func<ObservedDataYValue, float> extremValue, MarkerKind markerKind)
      {
         var series = CreateSeries(observedCurveData, ViewType.Point)
            .WithPoints(CreateSeriesPoints(observedCurveData, extremValue));

         var pointView = series.View.DowncastTo<PointSeriesView>();
         pointView.PointMarkerOptions.Kind = markerKind;
         pointView.PointMarkerOptions.Size = 4;
         return series;
      }
   }
}