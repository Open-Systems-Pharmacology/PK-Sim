using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OSPSuite.Core.Domain.UnitSystem;
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
   public class TimeProfileAnalysisChartTeXBuilder : AnalysisChartTEXBuilder<TimeProfileAnalysisChart,
      PopulationStatisticalAnalysis,
      TimeProfileXValue,
      TimeProfileYValue>
   {
      public TimeProfileAnalysisChartTeXBuilder(ITeXBuilderRepository builderRepository,  ITimeProfileChartDataCreator timeProfileChartDataCreator) :
         base(builderRepository, timeProfileChartDataCreator, AggregationFunctions.QuantityAggregation)
      {
      }

      protected override GroupPlot GetGroupPlot(ChartData<TimeProfileXValue, TimeProfileYValue> chartData, TimeProfileAnalysisChart analysisChart, Color[] colors, Text caption, GroupOptions groupOptions)
      {
         var xlabel = chartData.Axis.Caption;
         var axisOptions = GetAxisOptionsForGroup(xlabel);
         var groupedPlots = getGroupedPlots(chartData);
         return new GroupPlot(colors, axisOptions, groupOptions, groupedPlots, caption) {Position = FigureWriter.FigurePositions.H};
      }

      private IEnumerable<IBasePlot> getGroupedPlots(ChartData<TimeProfileXValue, TimeProfileYValue> chartData)
      {
         var groupedPlots = new List<IBasePlot>();

         foreach (var paneData in chartData.Panes)
         {
            var plots = new List<Plot>();
            var ydimension = paneData.Axis.Dimension;
            var yunit = paneData.Axis.DisplayUnit;
            var xdimension = chartData.Axis.Dimension;
            var xunit = chartData.Axis.DisplayUnit;

            addCurvePlots(paneData.Curves, xdimension, xunit, ydimension, yunit, plots);
            addObservedDataPlots(paneData.VisibleObservedCurveData, xdimension, xunit, ydimension, yunit, plots);
            var axisOptions = GetAxisOptionsForPlot(paneData);
            groupedPlots.Add(new BasePlot(axisOptions, plots));
         }
         return groupedPlots;
      }

      private bool isRange(CurveData<TimeProfileXValue, TimeProfileYValue> curveData)
      {
         return curveData.YValues.Any(x => x.IsRange);
      }

      private List<Coordinate> getCoordinatesFor(CurveData<TimeProfileXValue, TimeProfileYValue> curveData, IDimension xdimension, Unit xunit, IDimension ydimension, Unit yunit)
      {
         var coordinates = new List<Coordinate>();

         for (var i = 0; i < curveData.YValues.Count; i++)
         {
            var xValue = TEXHelper.ValueInDisplayUnit(xdimension, xunit, curveData.XValues[i].X);
            var yValue = TEXHelper.ValueInDisplayUnit(ydimension, yunit, curveData.YValues[i].Y);
            coordinates.Add(isRange(curveData)
                               ? getRangeCoordinate(xValue, curveData.YValues[i], ydimension, yunit)
                               : new Coordinate(xValue, yValue));
         }
         return coordinates;
      }

      private Coordinate getRangeCoordinate(float xValue, TimeProfileYValue timeProfileYValue, IDimension ydimension, Unit yunit)
      {
         var lowerValue = TEXHelper.ValueInDisplayUnit(ydimension, yunit, timeProfileYValue.LowerValue);
         var upperValue = TEXHelper.ValueInDisplayUnit(ydimension, yunit, timeProfileYValue.UpperValue);
         var yValue = lowerValue + (upperValue - lowerValue) / 2F;
         var errYValue = upperValue - yValue;
         return new Coordinate(xValue, yValue) {errY = errYValue};
      }

      private PlotOptions getPlotOptionsFor(CurveData<TimeProfileXValue, TimeProfileYValue> curveData)
      {
         return GetPlotOptions(color: curveData.Color.Name,
                                          shadedErrorBars: isRange(curveData),
                                          lineStyle: GetLineStyle(curveData.LineStyle));
      }

      private void addCurvePlots(IEnumerable<CurveData<TimeProfileXValue, TimeProfileYValue>> curves, IDimension xdimension, Unit xunit, IDimension ydimension, Unit yunit, List<Plot> plots)
      {
         foreach (var curve in curves)
            plots.Add(new Plot(getCoordinatesFor(curve, xdimension, xunit, ydimension, yunit),
                               getPlotOptionsFor(curve))
                         {
                            LegendEntry = GetLegendEntry(curve)
                         });
      }
      
      private List<Coordinate> getCoordinatesFor(ObservedCurveData observedCurveData, IDimension xdimension, Unit xunit, IDimension ydimension, Unit yunit)
      {
         var coordinates = new List<Coordinate>();
         for (var i = 0; i < observedCurveData.YValues.Count; i++)
         {
            var xValue = TEXHelper.ValueInDisplayUnit(xdimension, xunit, observedCurveData.XValues[i].X);
            var yValue = TEXHelper.ValueInDisplayUnit(ydimension, yunit, observedCurveData.YValues[i].Y);
            var error = TEXHelper.ValueInDisplayUnit(ydimension, yunit, observedCurveData.YValues[i].Error);
            coordinates.Add(new Coordinate(xValue, yValue) { errY = error });
         }
         return coordinates;
      }

      private PlotOptions getPlotOptionsFor(ObservedCurveData observedCurveData)
      {
         var plotOptions = GetPlotOptions(color: observedCurveData.Color.Name,
                                 shadedErrorBars: false,
                                 lineStyle: GetLineStyle(observedCurveData.LineStyle),
                                 marker: TEXHelper.GetMarker(observedCurveData.Symbol));

         plotOptions.ErrorType = TEXHelper.GetErrorType(observedCurveData.ErrorType);
         plotOptions.ErrorBars = true;
         plotOptions.ThicknessSize = Helper.Length(1, Helper.MeasurementUnits.pt);
         plotOptions.MarkSize = Helper.Length(1, Helper.MeasurementUnits.pt);
         return plotOptions;
      }

      private void addObservedDataPlots(IEnumerable<ObservedCurveData> observedDataCurves, IDimension xdimension, Unit xunit, IDimension ydimension, Unit yunit, List<Plot> plots)
      {
         foreach (var observedDataCurve in observedDataCurves)
            plots.Add(new Plot(getCoordinatesFor(observedDataCurve, xdimension, xunit, ydimension, yunit),
                               getPlotOptionsFor(observedDataCurve))
                         {
                            LegendEntry = GetLegendEntry(observedDataCurve)
                         });
      }

   }
}