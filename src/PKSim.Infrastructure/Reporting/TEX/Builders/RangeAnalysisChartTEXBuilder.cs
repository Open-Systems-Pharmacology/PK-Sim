using System.Collections.Generic;
using System.Drawing;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;
using OSPSuite.TeXReporting.TeX.PGFPlots;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using Plot = OSPSuite.TeXReporting.TeX.PGFPlots.Plot;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class RangeAnalysisChartTeXBuilder : AnalysisChartTEXBuilder<RangeAnalysisChart,
      PopulationPivotAnalysis,
      RangeXValue,
      RangeYValue>
   {

      public RangeAnalysisChartTeXBuilder(ITeXBuilderRepository builderRepository, IRangeChartDataCreator rangeChartDataCreator) :          
         base(builderRepository,  rangeChartDataCreator, AggregationFunctions.ValuesAggregation)
      {
      }

      protected override GroupPlot GetGroupPlot(ChartData<RangeXValue, RangeYValue> chartData, RangeAnalysisChart analysisChart, Color[] colors, Text caption, GroupOptions groupOptions)
      {
         var xlabel = chartData.Axis.Caption;
         var axisOptions = GetAxisOptionsForGroup(xlabel);
         var groupedPlots = getGroupedPlots(chartData);
         return new GroupPlot(colors, axisOptions, groupOptions, groupedPlots, caption) { Position = FigureWriter.FigurePositions.H };
      }

      private IEnumerable<IBasePlot> getGroupedPlots(ChartData<RangeXValue, RangeYValue> chartData)
      {
         var groupedPlots = new List<IBasePlot>();

         foreach (var paneData in chartData.Panes)
         {
            var plots = new List<Plot>();
            var ydimension = paneData.Axis.Dimension;
            var yunit = paneData.Axis.DisplayUnit;
            var xdimension = chartData.Axis.Dimension;
            var xunit = chartData.Axis.DisplayUnit;

            foreach (var curve in paneData.Curves)
            {
               var coordinates = new List<Coordinate>();
               var color = curve.Color;
 
               for (var i = 0; i < curve.YValues.Count; i++)
               {
                  var xValue = TEXHelper.ValueInDisplayUnit(xdimension, xunit, curve.XValues[i].X);
                  var yvalue = TEXHelper.ValueInDisplayUnit(ydimension, yunit, curve.YValues[i].Median);
                  var errYValue = yvalue - TEXHelper.ValueInDisplayUnit(ydimension, yunit, curve.YValues[i].LowerPercentile);
                  var errY2Value = TEXHelper.ValueInDisplayUnit(ydimension, yunit, curve.YValues[i].UpperPercentile) - yvalue;

                  coordinates.Add(new Coordinate(xValue, yvalue) {errY = errYValue, errY2 = errY2Value});
               }
               //plot range
               plots.Add(new Plot(coordinates, GetPlotOptions(color: color.Name, showInLegend:false,
                                                              shadedErrorBars: false)));
               //plot median curve
               plots.Add(new Plot(coordinates, GetPlotOptions(color: color.Name,
                                                              shadedErrorBars: true)) {LegendEntry = GetLegendEntry(curve)});
            }
            var axisOptions = GetAxisOptionsForPlot(paneData);
            groupedPlots.Add(new BasePlot(axisOptions, plots));
         }
         return groupedPlots;
      }


   }
}