using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;
using OSPSuite.TeXReporting.TeX.Converter;
using OSPSuite.TeXReporting.TeX.PGFPlots;
using OSPSuite.Utility.Data;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Infrastructure.Reporting.TeX.Items;
using TEXPlot = OSPSuite.TeXReporting.TeX.PGFPlots.Plot;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public abstract class AnalysisChartTEXBuilder<TPopulationAnalysisChart, TPopulationAnalysis, TXValue, TYValue> : OSPSuiteTeXBuilder<TPopulationAnalysisChart>
      where TPopulationAnalysisChart : PopulationAnalysisChart<TPopulationAnalysis>
      where TPopulationAnalysis : PopulationPivotAnalysis
      where TXValue : IXValue
      where TYValue : IYValue
   {
      private readonly ITeXBuilderRepository _builderRepository;
      private readonly IChartDataCreator<TXValue, TYValue> _chartDataCreator;
      private readonly Aggregate _aggregate;
      private Color _diagramBackColor;

      protected AnalysisChartTEXBuilder(ITeXBuilderRepository builderRepository, IChartDataCreator<TXValue, TYValue> chartDataCreator, Aggregate aggregate)
      {
         _builderRepository = builderRepository;
         _chartDataCreator = chartDataCreator;
         _aggregate = aggregate;
      }

      public override void Build(TPopulationAnalysisChart analysisChart, OSPSuiteTracker buildTracker)
      {
         var groupPlots = GetGroupPlots(analysisChart);

         var listToReport = new List<object>();

         foreach (var groupPlot in groupPlots)
         {
            listToReport.Add(groupPlot);
            buildTracker.AddReference(analysisChart, groupPlot);
         }

         if (analysisChart.AnalysisType == PopulationAnalysisType.TimeProfile)
         {
            var chartData = _chartDataCreator.CreateFor(analysisChart, _aggregate) as ChartData<TimeProfileXValue, TimeProfileYValue>;
            if (chartData != null)
            {
               var populationPKAnalyses = new PopulationPKAnalyses(analysisChart.Analysable.DowncastTo<IPopulationDataCollector>(), chartData, analysisChart);
               listToReport.Add(populationPKAnalyses);
            }
         }

         _builderRepository.Report(listToReport, buildTracker);
      }

      protected IEnumerable<GroupPlot> GetGroupPlots(TPopulationAnalysisChart analysisChart)
      {
         var chartData = _chartDataCreator.CreateFor(analysisChart, _aggregate);
         var chartSettings = analysisChart.ChartSettings;

         if (chartData == null)
            return Enumerable.Empty<GroupPlot>();

         var groupPlots = new List<GroupPlot>();

         var colors = getUsedColors(chartSettings, chartData);
         var legendPosition = getLegendPositionFor(chartSettings.LegendPosition);
         _diagramBackColor = chartSettings.DiagramBackColor;
         var caption = new Text(analysisChart.Description ?? String.Empty);
         var title = analysisChart.Title;
         var legendEntries = getLegendEntries(chartData);
         var groupOptions = GetGroupOptions(getNumberOfPanes(chartData), title, legendEntries, legendPosition);
         var groupPlot = GetGroupPlot(chartData, analysisChart, colors, caption, groupOptions);

         purgeMultipleSameAxisTitles(groupPlot);
         updateLegendPlots(groupPlot, legendEntries);

         groupPlots.Add(groupPlot);
         return groupPlots;
      }

      /// <summary>
      ///    This method is needed for grouped plot to get a legend with all lengend entries once for the whole group.
      /// </summary>
      /// <remarks>
      ///    In PGFPlots a legend can be placed outside of a plot with the legend to name feature.
      ///    When grouped plots do not all have the same number of plots used for legends only first plots are used for the
      ///    legend.
      ///    Therefor all plots get marked as not to be used for the legend.
      ///    For each pane dummy plots are generated which just are used for the legend creation.
      /// </remarks>
      private void updateLegendPlots(GroupPlot groupPlot, string[] legendEntries)
      {
         var legendPlots = getLegendPlots(groupPlot, legendEntries);
         foreach (var pane in groupPlot.GroupedPlots)
         {
            foreach (var plot in pane.Plots)
            {
               plot.LegendEntry = string.Empty;
               plot.Options.ShowInLegend = false;
            }

            pane.InsertPlots(0, legendPlots);
         }
      }

      private static IEnumerable<TEXPlot> getLegendPlots(GroupPlot groupPlot, string[] legendEntries)
      {
         var legendPlots = new Dictionary<string, TEXPlot>();
         foreach (var entry in legendEntries)
         {
            foreach (var pane in groupPlot.GroupedPlots)
            {
               var plot = pane.Plots.FirstOrDefault(x => x.LegendEntry == entry && x.Options.ShowInLegend);
               if (plot == null) continue;
               if (!legendPlots.Keys.Contains(entry))
                  legendPlots.Add(entry, createLegendPlotBasedOn(plot));
            }
         }
         return legendPlots.Values;
      }

      private static TEXPlot createLegendPlotBasedOn(TEXPlot plot)
      {
         var dummyOptions = plot.Options.Clone();
         dummyOptions.IsLegendPlot = true;
         dummyOptions.ShowInLegend = true;
         return new TEXPlot(new List<Coordinate>(), dummyOptions) {LegendEntry = plot.LegendEntry};
      }

      private string[] getLegendEntries(ChartData<TXValue, TYValue> chartData)
      {
         var legendEntries = new List<string>();
         foreach (var plot in chartData.Panes)
         {
            foreach (var curveData in plot.Curves)
               addLegendEntry(curveData, legendEntries);
            foreach (var observedCurveData in plot.VisibleObservedCurveData)
               addLegendEntry(observedCurveData, legendEntries);
         }
         return legendEntries.ToArray();
      }

      protected static string GetLegendEntry(CurveData<TXValue, TYValue> curve)
      {
         return curve.Caption;
      }

      protected static string GetLegendEntry(ObservedCurveData curve)
      {
         return curve.Caption;
      }

      private void addLegendEntry(ObservedCurveData curve, List<string> legendEntries)
      {
         var legendEntry = GetLegendEntry(curve);
         addLegendEntry(legendEntry, legendEntries);
      }

      private void addLegendEntry(CurveData<TXValue, TYValue> curve, List<string> legendEntries)
      {
         var legendEntry = GetLegendEntry(curve);
         addLegendEntry(legendEntry, legendEntries);
      }

      private void addLegendEntry(string legendEntry, List<string> legendEntries)
      {
         if (legendEntries.Contains(legendEntry)) 
            return;
         legendEntries.Add(legendEntry);
      }

      protected abstract GroupPlot GetGroupPlot(ChartData<TXValue, TYValue> chartData, TPopulationAnalysisChart analysisChart, Color[] colors, Text caption, GroupOptions groupOptions);

      private LegendOptions.LegendPositions getLegendPositionFor(LegendPositions legendPosition)
      {
         switch (legendPosition)
         {
            case LegendPositions.None:
               return LegendOptions.LegendPositions.OuterNorthEast;
            case LegendPositions.Right:
               return LegendOptions.LegendPositions.OuterNorthEast;
            case LegendPositions.RightInside:
               return LegendOptions.LegendPositions.NorthEast;
            case LegendPositions.Bottom:
               return LegendOptions.LegendPositions.OuterSouthEast;
            case LegendPositions.BottomInside:
               return LegendOptions.LegendPositions.SouthEast;
            default:
               throw new ArgumentOutOfRangeException("legendPosition");
         }
      }

      private int getLegendColumnsFor(int numberOfLegendEntries, LegendOptions.LegendPositions legendPosition)
      {
         switch (legendPosition)
         {
            case LegendOptions.LegendPositions.SouthWest:
            case LegendOptions.LegendPositions.SouthEast:
            case LegendOptions.LegendPositions.NorthWest:
            case LegendOptions.LegendPositions.NorthEast:
            case LegendOptions.LegendPositions.OuterNorthEast:
            case LegendOptions.LegendPositions.OuterNorthWest:
            case LegendOptions.LegendPositions.OuterSouthEast:
            case LegendOptions.LegendPositions.OuterSouthWest:
               return 1;
            case LegendOptions.LegendPositions.OuterSouth:
            case LegendOptions.LegendPositions.South:
            case LegendOptions.LegendPositions.OuterNorth:
            case LegendOptions.LegendPositions.North:
               return numberOfLegendEntries;
            default:
               throw new ArgumentOutOfRangeException("legendPosition");
         }
      }

      protected virtual GroupOptions GetGroupOptions(int numberOfPanes, string title, string[] legendEntries, LegendOptions.LegendPositions legendPosition)
      {
         var groupOptions = new GroupOptions
         {
            Title = title,
            Columns = 1,
            Rows = numberOfPanes,
            HorizontalSep = Helper.Length(2, Helper.MeasurementUnits.em),
            VerticalSep = Helper.Length(8, Helper.MeasurementUnits.ex),
            XLabelsAt = GroupOptions.GroupXPositions.EdgeBottom,
            XTickLabelsAt = GroupOptions.GroupXPositions.All,
            YLabelsAt = GroupOptions.GroupYPositions.All,
            YTickLabelsAt = GroupOptions.GroupYPositions.All,
            GroupLegendOptions =
            {
               LegendEntries = legendEntries,
               LegendOptions = new LegendOptions
               {
                  Columns = getLegendColumnsFor(legendEntries.Count(), legendPosition),
                  LegendPosition = legendPosition,
                  FontSize = LegendOptions.FontSizes.scriptsize
               }
            }
         };
         return groupOptions;
      }

      protected static AxisOptions GetAxisOptionsForGroup(string xlabel)
      {
         var axisOptions = new AxisOptions(DefaultConverter.Instance)
         {
            YMode = AxisOptions.AxisMode.normal,
            XLabel = xlabel,
            XAxisPosition = AxisOptions.AxisXLine.bottom,
            YAxisPosition = AxisOptions.AxisYLine.left,
            YMajorGrid = true,
            YAxisArrow = true,
            EnlargeLimits = true
         };

         return axisOptions;
      }

      private int getNumberOfPanes(ChartData<TXValue, TYValue> chartData)
      {
         return chartData.Panes.Count();
      }

      private Color[] getUsedColors(ChartSettings chartSettings, ChartData<TXValue, TYValue> chartData)
      {
         var colors = new HashSet<Color>();
         colors.Add(chartSettings.BackColor);
         colors.Add(chartSettings.DiagramBackColor);

         foreach (var curve in chartData.Panes.SelectMany(pane => pane.Curves))
            colors.Add(curve.Color);
         foreach (var observedCurve in chartData.Panes.SelectMany(pane => pane.ObservedCurveData.Where(c => c.Visible)))
            colors.Add(observedCurve.Color);

         return colors.ToArray();
      }

      protected AxisOptions GetAxisOptionsForPlot(PaneData<TXValue, TYValue> plot, List<AxisOptions.GroupLine> groupLines = null)
      {
         var axisOptions = new AxisOptions(DefaultConverter.Instance)
         {
            Title = plot.Caption,
            YLabel = plot.Axis.Caption,
            BackgroundColor = _diagramBackColor.Name,
            YMode = plot.Axis.Scaling == Scalings.Log ? AxisOptions.AxisMode.log : AxisOptions.AxisMode.normal, 
            XMode = plot.ChartAxis.Scaling == Scalings.Log ? AxisOptions.AxisMode.log : AxisOptions.AxisMode.normal,
         };
         if (groupLines != null)
            axisOptions.GroupLines = groupLines;

         return axisOptions;
      }

      private void purgeMultipleSameAxisTitles(GroupPlot groupPlot)
      {
         var groupedPlots = groupPlot.GroupedPlots.ToList();
         if (!groupedPlots.Any()) 
            return;
         var title = groupedPlots.First().AxisOptions.YLabel;
         if (groupedPlots.Any(a => a.AxisOptions.YLabel != title))
            return;
         groupedPlots.Each(a => a.AxisOptions.YLabel = String.Empty);
         var index = (int)((groupedPlots.Count - 0.5) / 2);
         groupedPlots[index].AxisOptions.YLabel = title;
      }

      protected PlotOptions GetPlotOptions(string color, bool shadedErrorBars, bool showInLegend = true, PlotOptions.LineStyles lineStyle = PlotOptions.LineStyles.Solid, PlotOptions.Markers marker = PlotOptions.Markers.None)
      {
         return new PlotOptions
         {
            ShadedErrorBars = shadedErrorBars,
            Opacity = TEXHelper.GetOpacityFor(Constants.Population.STD_DEV_CURVE_TRANSPARENCY),
            LineStyle = lineStyle,
            Marker = marker,
            MarkColor = color,
            Color = color,
            ShowInLegend = showInLegend,
            ThicknessSize = Helper.Length(1, Helper.MeasurementUnits.pt)
         };
      }

      
      protected PlotOptions.LineStyles GetLineStyle(LineStyles lineStyle)
      {
         switch (lineStyle)
         {
            case LineStyles.None:
               return PlotOptions.LineStyles.None;
            case LineStyles.Solid:
               return PlotOptions.LineStyles.Solid;
            case LineStyles.Dash:
               return PlotOptions.LineStyles.Dashed;
            case LineStyles.Dot:
               return PlotOptions.LineStyles.Dotted;
            case LineStyles.DashDot:
               return PlotOptions.LineStyles.DashDotted;
            default:
               throw new ArgumentOutOfRangeException("lineStyle");
         }
      }
   }
}