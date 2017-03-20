using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;
using OSPSuite.TeXReporting.TeX.Converter;
using OSPSuite.TeXReporting.TeX.PGFPlots;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using Plot = OSPSuite.TeXReporting.TeX.PGFPlots.Plot;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class BoxWhiskerAnalysisChartTeXBuilder : 
      AnalysisChartTEXBuilder<BoxWhiskerAnalysisChart,
      PopulationBoxWhiskerAnalysis, 
      BoxWhiskerXValue, 
      BoxWhiskerYValue>
   {
      public BoxWhiskerAnalysisChartTeXBuilder(ITeXBuilderRepository builderRepository, IBoxWhiskerChartDataCreator boxWhiskerChartDataCreator) : 
         base(builderRepository,  boxWhiskerChartDataCreator, AggregationFunctions.BoxWhisker90Aggregation)
      {
      }
      
      protected override GroupPlot GetGroupPlot(ChartData<BoxWhiskerXValue, BoxWhiskerYValue> chartData, BoxWhiskerAnalysisChart analysisChart, Color[] colors, Text caption, GroupOptions groupOptions)
      {
         var xTicks = getXTicks(chartData);
         var xTickLabels = getXTickLabels(chartData);
         var groupLines = getGroupLines(chartData.AllXValues.ToList(), chartData.AllXValues.Min(x => x.Key.Count));
         var axisOptions = getAxisOptionsForGroup(xTicks, xTickLabels);
         var groupedPlots = getGroupedPlots(chartData, groupLines);
         return new GroupPlot(colors, axisOptions, groupOptions, groupedPlots, caption) { Position = FigureWriter.FigurePositions.H };
      }

      private float[] getXTicks(ChartData<BoxWhiskerXValue,BoxWhiskerYValue> chartData)
      {
         var xTicks = new List<float>();

         foreach (var xValue in chartData.AllXValues)
         {
            float value = (float) xValue.Value;
            if (xTicks.Contains(value)) continue;
            xTicks.Add(value);
         }
         return xTicks.ToArray();
      }

      private string[] getXTickLabels(ChartData<BoxWhiskerXValue, BoxWhiskerYValue> chartData)
      {
         var xTickLabels = new List<string>();

         foreach (var xValues in chartData.AllXValues)
         {
            var value = xValues.Key.LastOrDefault();
            if (value == null) value = string.Empty;
            xTickLabels.Add(value);
         }

         return xTickLabels.ToArray();
      }


      private List<AxisOptions.GroupLine> getGroupLines(IReadOnlyCollection<KeyValuePair<BoxWhiskerXValue, int>> xValues, int levels)
      {
         var groupLines = new List<AxisOptions.GroupLine>();
         for (var i = 0; i < levels-1; i++)
         {
            var lastXValue = xValues.FirstOrDefault().Key;
            var firstIndexOfLastXValue = lastXValue.X;
            var lastIndexOfLastXValue = firstIndexOfLastXValue;
            foreach (var xValueItem in xValues)
            {
               if (xValueItem.Key[i] == lastXValue[i])
                  lastIndexOfLastXValue = xValueItem.Key.X;
               else
               {
                  groupLines.Add(new AxisOptions.GroupLine(firstIndexOfLastXValue, lastIndexOfLastXValue, lastXValue[i], levels-(i+1)));
                  lastXValue = xValueItem.Key;
                  firstIndexOfLastXValue = lastXValue.X;
                  lastIndexOfLastXValue = firstIndexOfLastXValue;
               }
            }
            groupLines.Add(new AxisOptions.GroupLine(firstIndexOfLastXValue, lastIndexOfLastXValue, lastXValue[i], levels - (i + 1)));
         }
         return groupLines;
      }

      private static AxisOptions getAxisOptionsForGroup(float[] xTicks, string[] xTickLabels)
      {

         var axisOptions = new AxisOptions(DefaultConverter.Instance)
                              {
                                 IsUsedForBoxPlots = true,
                                 BoxPlotDrawDirection = AxisOptions.BoxPlotDrawDirections.y,
                                 YMode = AxisOptions.AxisMode.normal,
                                 XMin = -0.5F,
                                 XMax = xTickLabels.Count()-0.5F,
                                 XTicks = xTicks,
                                 XTickLabels = xTickLabels,
                                 XAxisPosition = AxisOptions.AxisXLine.bottom,
                                 YAxisPosition = AxisOptions.AxisYLine.left,
                                 YMajorGrid = true,
                                 YAxisArrow = true,
                                 EnlargeLimits = true
                              };
         if (xTickLabels.Sum(x => x.Length) >= 40)
         {
            axisOptions.XTickLabelsRotateBy = 45;
            axisOptions.XGroupLineTextOffset = Helper.Length(xTickLabels.Max(t => t.Length)*0.7, Helper.MeasurementUnits.ex);
         }

         return axisOptions;

      }

      private bool showInLegend(PaneData<BoxWhiskerXValue, BoxWhiskerYValue> pane, CurveData<BoxWhiskerXValue, BoxWhiskerYValue> curve, List<string> legendEntries)
      {
         var legendEntry = GetLegendEntry(curve);
         if (!legendEntries.Contains(legendEntry))
         {
            legendEntries.Add(legendEntry);
            return true;
         }
         return false;
      }

      private List<IBasePlot> getGroupedPlots(ChartData<BoxWhiskerXValue, BoxWhiskerYValue> chartData, List<AxisOptions.GroupLine> groupLines)
      {
         var groupedPlots = new List<IBasePlot>();

         foreach (var paneData in chartData.Panes)
         {
            var boxWhiskerPlots = new List<Plot>();
            var legendEntries = new List<string>();
            var dimension = paneData.Axis.Dimension;
            var unit = paneData.Axis.DisplayUnit;

            foreach (var box in paneData.Curves)
            {
               for (var i = 0; i < box.YValues.Count; i++)
               {
                  var color = box.Color;
                  var marker = PlotOptions.Markers.Circle;

                  var showInLegend = this.showInLegend(paneData, box, legendEntries);

                  var xValue = box.XValues[i].X;
                  var boxWhisker = box.YValues[i];
                  var lowerWhisker = TEXHelper.ValueInDisplayUnit(dimension, unit, boxWhisker.LowerWhisker);
                  var lowerBox = TEXHelper.ValueInDisplayUnit(dimension, unit, boxWhisker.LowerBox);
                  var median = TEXHelper.ValueInDisplayUnit(dimension, unit, boxWhisker.Median);
                  var upperBox = TEXHelper.ValueInDisplayUnit(dimension, unit, boxWhisker.UpperBox);
                  var upperWhisker = TEXHelper.ValueInDisplayUnit(dimension, unit, boxWhisker.UpperWhisker);
                  var outliers = TEXHelper.ValueInDisplayUnit(dimension, unit, boxWhisker.Outliers);

                  var coordinates = outliers.Select(outlier => new Coordinate(xValue, outlier)).ToList();

                  boxWhiskerPlots.Add(new Plot(coordinates, getBoxWhiskerPlotOptions(lowerWhisker,
                                                                           lowerBox,
                                                                           median,
                                                                           upperBox,
                                                                           upperWhisker,
                                                                           xValue,
                                                                           color.Name, 
                                                                           marker, showInLegend)) {LegendEntry = GetLegendEntry(box)});
               }
            }


            var axisOptions = GetAxisOptionsForPlot(paneData, paneData == chartData.Panes.Last() ? groupLines : null);
            groupedPlots.Add(new BasePlot(axisOptions, boxWhiskerPlots));
         }
         return groupedPlots;
      }

      private static PlotOptions getBoxWhiskerPlotOptions(float lowerWhisker, float lowerQuartile, float median, float upperQuartile, float upperWhisker, float drawPosition, string color, PlotOptions.Markers marker, bool showInLegend)
      {
         var plotOptions1 = new PlotOptions
         {
            LineStyle = PlotOptions.LineStyles.Solid,
            Marker = marker,
            MarkColor = color,
            Color = color,
            BoxPlotPrepared = new BoxPlotPrepared(lowerWhisker, lowerQuartile, median, upperQuartile, upperWhisker, drawPosition),
            ThicknessSize = Helper.Length(1, Helper.MeasurementUnits.pt),
            ShowInLegend = showInLegend
         };
         return plotOptions1;
      }

      protected override GroupOptions GetGroupOptions(int numberOfPanes, string title, string[] legendEntries, LegendOptions.LegendPositions legendPosition)
      {
         var groupOptions = base.GetGroupOptions(numberOfPanes, title, legendEntries, legendPosition);
         groupOptions.XLabelsAt = GroupOptions.GroupXPositions.EdgeBottom;
         groupOptions.XTickLabelsAt = GroupOptions.GroupXPositions.EdgeBottom;
         return groupOptions;
      }
   }
}
