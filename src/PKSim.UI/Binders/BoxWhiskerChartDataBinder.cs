using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OSPSuite.Utility.Extensions;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using PKSim.Core.Chart;
using PKSim.UI.Extensions;
using PKSim.UI.Views.PopulationAnalyses;

namespace PKSim.UI.Binders
{
   public class BoxWhiskerChartDataBinder : ChartDataBinder<BoxWhiskerXValue, BoxWhiskerYValue>
   {
      public BoxWhiskerChartDataBinder(BoxWhiskerChartView view)
         : base(view, ViewType.Line, allowZoom: false)
      {
      }

      protected override void AddYAxes(ChartData<BoxWhiskerXValue, BoxWhiskerYValue> chartData, XYDiagram diagram, bool showPaneCaptioninAxisCaption = true)
      {
         // do not show pane caption, because it is already contained in axisCaption
         base.AddYAxes(chartData, diagram, showPaneCaptioninAxisCaption: false);
      }

      protected override void AddXAxes(ChartData<BoxWhiskerXValue, BoxWhiskerYValue> chartData, XYDiagram diagram)
      {
         //max value must be greater than min value for axis! For no x axis values we need no x axis.
         if (!chartData.AllXValues.Any()) return;

         var axisX = diagram.AxisX;

         // because series/curves are not added yet, the AxisX.WholeRange.MaxValue must be modified explicitely
         axisX.WholeRange.MinValue = -0.5;
         axisX.WholeRange.MaxValue = chartData.AllXValues.Count() - 1 + 0.5;
         axisX.WholeRange.SideMarginsValue = 0;
         axisX.WholeRange.AutoSideMargins = false;
         axisX.Visibility = DefaultBoolean.False;

         int nXAxes = chartData.XFieldNames.Count;
         var xAxes = new SecondaryAxisX[nXAxes];

         for (int i = 0; i < nXAxes; i++)
         {
            xAxes[i] = new SecondaryAxisX(chartData.XFieldNames[i])
            {
               Visibility = DefaultBoolean.True,
               Alignment = AxisAlignment.Far,
               Tickmarks = {MinorVisible = false},
               Title =
               {
                  Text = chartData.XFieldNames[i],
                  Alignment = StringAlignment.Near,
                  Visibility = DefaultBoolean.False
               },
            };

            synchronizeAxis(xAxes[i], axisX);
         }

         setXAxesLabels(xAxes, chartData.AllXValues.ToList());

         diagram.SecondaryAxesX.Clear();

         //first XValues most bottom
         for (int i = nXAxes - 1; i >= 0; i--)
         {
            diagram.SecondaryAxesX.Add(xAxes[i]);
         }

       
      }

      private static void synchronizeAxis(SecondaryAxisX secondaryAxisX, AxisX axisX)
      {
         secondaryAxisX.WholeRange.MinValue = axisX.WholeRange.MinValue;
         secondaryAxisX.WholeRange.MaxValue = axisX.WholeRange.MaxValue;
         secondaryAxisX.WholeRange.AutoSideMargins = axisX.WholeRange.AutoSideMargins;
         secondaryAxisX.WholeRange.SideMarginsValue = axisX.WholeRange.SideMarginsValue;
      }

      private void setXAxesLabels(SecondaryAxisX[] xAxes, IReadOnlyCollection<KeyValuePair<BoxWhiskerXValue, int>> xValues)
      {
         for (int i = 0; i < xAxes.Length; i++)
         {
            BoxWhiskerXValue lastXValue = xValues.FirstOrDefault().Key;
            if (lastXValue == null)
               continue;

            float firstIndexOfLastXValue = lastXValue.X;
            float lastIndexOfLastXValue = firstIndexOfLastXValue;
            double pos = 0;

            foreach (var xValueItem in xValues)
            {
               bool xValueEqualsLastXValue = true;
               for (int j = 0; j <= i; j++)
               {
                  xValueEqualsLastXValue = xValueEqualsLastXValue && xValueItem.Key[j] == lastXValue[j];
               }

               if (xValueEqualsLastXValue) 
                  lastIndexOfLastXValue = xValueItem.Key.X;
               else
               {
                  pos = (lastIndexOfLastXValue + firstIndexOfLastXValue) / 2.0;
                  xAxes[i].CustomLabels.Add(new CustomAxisLabel(lastXValue[i], pos));
                  //add a custom label with empty name where axis value changes
                  xAxes[i].CustomLabels.Add(new CustomAxisLabel(string.Empty, lastIndexOfLastXValue + 0.5));
                  lastXValue = xValueItem.Key;
                  firstIndexOfLastXValue = lastXValue.X;
                  lastIndexOfLastXValue = firstIndexOfLastXValue;
               }
            }

            pos = (lastIndexOfLastXValue + firstIndexOfLastXValue) / 2.0;
            xAxes[i].CustomLabels.Add(new CustomAxisLabel(lastXValue[i], pos));
            //add a custom label with empty name at min and max of axis
            xAxes[i].CustomLabels.Add(new CustomAxisLabel(string.Empty, (double) xAxes[i].WholeRange.MinValue));
            xAxes[i].CustomLabels.Add(new CustomAxisLabel(string.Empty, (double) xAxes[i].WholeRange.MaxValue));
         }
      }

      protected override IReadOnlyList<Series> CreateCurveSeriesList(CurveData<BoxWhiskerXValue, BoxWhiskerYValue> curveData)
      {
         var boxSeries = createBoxSeries(curveData)
            .WithPoints(CreateSeriesPoints(curveData, y => y.LowerWhisker, y => y.UpperWhisker, y => y.LowerBox, y => y.UpperBox));

         var medianSeries = createBoxSeries(curveData)
            .WithPoints(CreateSeriesPoints(curveData, y => y.Median, y => y.Median, y => y.Median, y => y.Median));

         var lowerWhisker = createPointSeries(curveData, MarkerKind.Triangle)
            .WithPoints(CreateSeriesPoints(curveData, y => y.LowerWhisker));

         var upperWhisker = createPointSeries(curveData, MarkerKind.InvertedTriangle)
            .WithPoints(CreateSeriesPoints(curveData, y => y.UpperWhisker));

         var outliers = createPointSeries(curveData, MarkerKind.Circle)
            .WithPoints(CreateSeriesPoints(curveData, y => y.Outliers));


         return new[] {boxSeries, medianSeries, lowerWhisker, upperWhisker, outliers};
      }

      private Series createBoxSeries(ICurveDataSettings curve)
      {
         var series = CreateSeries(curve, ViewType.CandleStick);
         var fiancialSeriesView = series.View.DowncastTo<FinancialSeriesViewBase>();
         fiancialSeriesView.LineThickness = 1;
         fiancialSeriesView.ReductionOptions.Color = fiancialSeriesView.Color;
         return series;
      }

      private Series createPointSeries(ICurveDataSettings curve, MarkerKind markerKind)
      {
         var series = CreateSeries(curve, ViewType.Point);
         var pointView = series.View.DowncastTo<PointSeriesView>();
         pointView.PointMarkerOptions.Kind = markerKind;
         pointView.PointMarkerOptions.Size = 5;
         return series;
      }
   }
}