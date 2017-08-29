using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Native;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Chart;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.UI.Mappers;
using PKSim.UI.Views.PopulationAnalyses;
using Axis = DevExpress.XtraCharts.Axis;
using Constants = OSPSuite.Core.Domain.Constants;

namespace PKSim.UI.Binders
{
   public abstract class ChartDataBinder<TX, TY> : IChartsDataBinder<TX, TY> where TX : IXValue where TY : IYValue
   {
      protected BaseChartView<TX, TY> _view;
      private readonly ViewType _viewType;
      private readonly bool _allowZoom;
      private readonly ISymbolToMarkerKindMapper _symbolMapper;
      private readonly ILineStyleToDashStyleMapper _lineStyleMapper;
      private ChartTitle _previewChartOrigin;
      private const int DEFAULT_TICKS_FOR_LOG_SCALE = 8;
      private const int DEFAULT_TICKS_FOR_LIN_SCALE = 2;

      protected ChartDataBinder(BaseChartView<TX, TY> view, ViewType viewType, bool allowZoom = true)
      {
         _view = view;
         _viewType = viewType;
         _allowZoom = allowZoom;
         _symbolMapper = new SymbolToMarkerKindMapper();
         _lineStyleMapper = new LineStyleToDashStyleMapper();
      }

      public void ClearPlot()
      {
         _view.ClearAllSeries();
      }

      private void doUpdateOf(ChartCollectionBase collection, Action actionToPerform)
      {
         collection.BeginUpdate();
         try
         {
            actionToPerform();
         }
         finally
         {
            collection.EndUpdate();
         }
      }

      public void Bind(ChartData<TX, TY> chartData, PopulationAnalysisChart analysisChart)
      {
         if (chartData == null)
         {
            ClearPlot();
            return;
         }

         //Required to add at least one series in order to create the Diagram
         if (_view.Diagram == null)
         {
            var dummySeries = new Series("dummy", _viewType);
            _view.Chart.Series.Add(dummySeries);

            //Zoom initialization should be performed with an existing Diagram
            if (_allowZoom)
               new DiagramZoomRectangleService(_view.Chart, zoomAction);
         }

         var diagram = _view.Diagram;
         if (diagram == null)
            return;

         //before AddSeries, because Series are assigned to panes
         doUpdateOf(diagram.Panes, () => addPanes(chartData, diagram));

         //before AddSeries, because Series are assigned to Secondary yAxes
         doUpdateOf(diagram.SecondaryAxesY, () => AddYAxes(chartData, diagram));

         doUpdateOf(_view.Chart.Series, () => AddSeries(chartData, diagram));

         // after AddSeries, because Axes are available after curves
         AddXAxes(chartData, diagram);
         doUpdateOf(diagram.Panes, () => SetVisibilityOfXAxisInPanes(chartData, diagram));

         UpdateSettings(analysisChart);

         // annotations must be placed at the end because otherwise they get removed for example when series are added.
         // this is a behavior of dev express.
         doUpdateOf(diagram.Panes, () => addPaneTitles(chartData, diagram));

         _view.Refresh();
      }

      private void zoomAction(Control control, Rectangle rectangle)
      {
         var cc = control as IChartContainer;
         if (cc != null && cc.Chart != null && !rectangle.IsEmpty)
            cc.Chart.PerformZoomIn(rectangle);
      }

      public void UpdateSettings(PopulationAnalysisChart populationAnalysisChart)
      {
         if (populationAnalysisChart == null) return;

         var diagram = _view.Diagram;
         if (diagram == null) return;

         _view.Text = populationAnalysisChart.Name;

         for (int index = 0; index < diagram.Panes.Count; index++)
         {
            var pane = diagram.Panes[index];
            pane.BackColor = populationAnalysisChart.ChartSettings.DiagramBackColor;
         }

         diagram.DefaultPane.BackColor = populationAnalysisChart.ChartSettings.DiagramBackColor;

         _view.Chart.BackColor = populationAnalysisChart.ChartSettings.BackColor;
         _view.Chart.Title = populationAnalysisChart.Title;
         _view.Chart.Description = populationAnalysisChart.Description;
         _view.Chart.Legend.LegendPosition(populationAnalysisChart.ChartSettings.LegendPosition);

         diagram.AxisX.VisualRange.SetSideMarginsEnabled(populationAnalysisChart.ChartSettings.SideMarginsEnabled);
         diagram.AxisY.VisualRange.SetSideMarginsEnabled(populationAnalysisChart.ChartSettings.SideMarginsEnabled);

         applyAxisZoom(populationAnalysisChart.PrimaryYAxisSettings, _view.Chart.AxisY);

         foreach (Axis axis in diagram.SecondaryAxesY)
         {
            var index = diagram.SecondaryAxesY.IndexOf(axis);
            applyAxisZoom(populationAnalysisChart.AxisSettingsForSecondaryYAxis(index), axis);
         }

         applyAxisZoom(populationAnalysisChart.PrimaryXAxisSettings, _view.Chart.AxisX);

         if (populationAnalysisChart.PreviewSettings)
            adjustDisplayForPreview(populationAnalysisChart);
         else
            adjustDisplayForApplication(populationAnalysisChart);
      }

      private static void applyAxisZoom(AxisSettings axisSettings, Axis axis)
      {
         if (axisSettings.AutoRange)
         {
            axis.VisualRange.Auto = true;
            return;
         }

         if (!axisSettings.HasRange) return;

         axis.VisualRange.Auto = false;
         axis.VisualRange.SetMinMaxValues(axisSettings.Min, axisSettings.Max);
      }

      private void adjustDisplayForApplication(PopulationAnalysisChart populationAnalysisChart)
      {
         _view.SetDockStyle(DockStyle.Fill);
         _view.Chart.SetFontAndSizeSettings(new ChartFontAndSizeSettings(), _view.Chart.Size);
         clearOriginText();
         updateWatermark(populationAnalysisChart, showWatermark:false);
      }

      private void updateWatermark(PopulationAnalysisChart populationAnalysisChart, bool showWatermark)
      {
         _view.UpdateWatermark(populationAnalysisChart, showWatermark);
      }

      private void clearOriginText()
      {
         if (_view.Chart.Titles.Contains(_previewChartOrigin))
            _view.Chart.Titles.Remove(_previewChartOrigin);
      }

      private void adjustDisplayForPreview(PopulationAnalysisChart populationAnalysisChart)
      {
         _view.SetDockStyle(hasHeightAndWidth(populationAnalysisChart) ? DockStyle.None : DockStyle.Fill);
         _view.Chart.SetFontAndSizeSettings(populationAnalysisChart.FontAndSize, _view.Chart.Size);
         previewOriginText(populationAnalysisChart);
         updateWatermark(populationAnalysisChart, showWatermark: true);
      }

      private void previewOriginText(PopulationAnalysisChart populationAnalysisChart)
      {
         clearOriginText();
         if (populationAnalysisChart.IncludeOriginData)
            _previewChartOrigin = _view.Chart.AddOriginData(populationAnalysisChart);
      }

      private bool hasHeightAndWidth(PopulationAnalysisChart chart)
      {
         var chartHeight = chart.FontAndSize.ChartHeight;
         var chartWidth = chart.FontAndSize.ChartWidth;

         return chartHeight.HasValue && chartWidth.HasValue;
      }

      // How to display SecondaryXAxes at bottom pane (pane order seems not to be editable, SecondaryXAxes are not used by Series), see end of AddSeries
      // all curves are displayed in additional panes and at secondary y Axes
      private void addPanes(ChartData<TX, TY> chartData, XYDiagram diagram)
      {
         diagram.PaneDistance = 10;
         diagram.PaneLayoutDirection = PaneLayoutDirection.Vertical;

         diagram.Panes.Clear();
         foreach (var paneData in chartData.Panes)
         {
            var newPane = new XYDiagramPane(paneData.Id);
            diagram.Panes.Add(newPane);
         }

         if (!chartData.Panes.Any())
            return;

         diagram.Panes[0].Weight = 1;
         diagram.DefaultPane.Visible = false;
      }

      private bool isNoTitleNeededFor(PaneData<TX, TY> paneData)
      {
         return (paneData.Axis.Caption.Equals(Constants.NameWithUnitFor(paneData.Caption, paneData.Axis.DisplayUnit)));
      }

      private void addPaneTitles(ChartData<TX, TY> chartData, XYDiagram diagram)
      {
         //only add pane titles when at least 2 panes occur
         if (diagram.Panes.Count < 2) return;

         foreach (var paneData in chartData.Panes)
         {
            var pane = getPaneFor(paneData.Id, diagram);
            if (isNoTitleNeededFor(paneData))
               continue;

            doUpdateOf(pane.Annotations, () => _view.AddTitleTo(pane, paneData.Caption));
         }
      }

      private static XYDiagramPane getPaneFor(string id, XYDiagram diagram)
      {
         return diagram.Panes.Cast<XYDiagramPane>().First(p => p.Name == id);
      }

      protected virtual void AddYAxes(ChartData<TX, TY> chartData, XYDiagram diagram, bool showPaneCaptioninAxisCaption = true)
      {
         diagram.SecondaryAxesY.Clear();

         if (!chartData.Panes.Any())
            return;

         //only one pane
         if (chartData.Panes.Count == 1)
         {
            diagram.AxisY.Visibility = DefaultBoolean.True;
            diagram.AxisY.GridLines.Visible = true;
            initializeAxis(diagram.AxisY, chartData.Panes.First().Axis);
         }
         else
         {
            foreach (var paneData in chartData.Panes)
            {
               var yAxis = new SecondaryAxisY(paneData.Id);
               initializeAxis(yAxis, paneData.Axis);
               diagram.SecondaryAxesY.Add(yAxis);
            }
            purgeMultipleSameAxisTitles(diagram);

            diagram.AxisY.Visibility = DefaultBoolean.False;
            diagram.AxisY.GridLines.Visible = false;
         }
      }

      private static void purgeMultipleSameAxisTitles(XYDiagram diagram)
      {
         var allTitles = diagram.SecondaryAxesY.Cast<Axis2D>().Select(x => x.Title).ToList();
         if (!allTitles.Any()) return;

         var firstTitle = allTitles.First();
         //all titles but the first one
         allTitles.Remove(firstTitle);
         if (allTitles.Any(a => !string.Equals(a.Text, firstTitle.Text)))
            return;

         allTitles.Each(a => a.Text = String.Empty);
      }

      private void initializeAxis(Axis2D axis, AxisData axisData)
      {
         axis.Title.Text = axisData.Caption;
         axis.Title.Alignment = StringAlignment.Center;
         axis.Title.Visibility = DefaultBoolean.True;
         axis.Alignment = AxisAlignment.Near;
         axis.GridLines.Visible = true;
         axis.WholeRange.Auto = true;
         axis.WholeRange.AlwaysShowZeroLevel = false;
         axis.Logarithmic = (axisData.Scaling == Scalings.Log);
         axis.MinorCount = axis.Logarithmic ? DEFAULT_TICKS_FOR_LOG_SCALE : DEFAULT_TICKS_FOR_LIN_SCALE;
      }

      protected virtual void SetVisibilityOfXAxisInPanes(ChartData<TX, TY> chartData, XYDiagram diagram)
      {
         // show AxisX at bottom pane, only if AxisX is Visible 
         for (var i = 0; i < diagram.Panes.Count - 1; i++)
            diagram.AxisX.SetVisibilityInPane(false, diagram.Panes[i]);

         if (!chartData.Panes.Any())
            return;

         // axis must be available, therefore series for this axis must be displayed
         diagram.AxisX.SetVisibilityInPane(true, diagram.Panes[diagram.Panes.Count - 1]);
      }

      protected virtual void AddXAxes(ChartData<TX, TY> chartData, XYDiagram diagram)
      {
         initializeAxis(diagram.AxisX, chartData.Axis);
      }

      protected void AddSeries(ChartData<TX, TY> chartData, XYDiagram diagram)
      {
         _view.ClearAllSeries();
         var allSeriesList = new List<Series>();

         foreach (var paneData in chartData.Panes)
         {
            allSeriesList.AddRange(addCurveAsSeries(paneData, diagram, x => x.Curves, CreateCurveSeriesList));
            allSeriesList.AddRange(addCurveAsSeries(paneData, diagram, x => x.ObservedCurveData, CreateObservedDataSeriesList));
         }

         //display legend for each seriesName != "" once
         var allSeriesGroupByName = allSeriesList
            .Where(s => !string.IsNullOrEmpty(s.Name))
            .GroupBy(s => s.Name);

         foreach (var seriesGroup in allSeriesGroupByName)
            seriesGroup.First().ShowInLegend = true;

         _view.Bind(reorderedSeries(allSeriesList));
      }

      private IEnumerable<Series> addCurveAsSeries<TCurve>(PaneData<TX, TY> paneData, XYDiagram diagram, Func<PaneData<TX, TY>, IEnumerable<TCurve>> curveRetrieverFunc,
         Func<TCurve, IReadOnlyList<Series>> seriesMapFunc)
      {
         var allSeriesList = new List<Series>();

         foreach (var curveData in curveRetrieverFunc(paneData))
         {
            var curveSeriesList = seriesMapFunc(curveData).Where(s => s != null).ToList();
            AssignCurveSeriesList(curveSeriesList, diagram.Panes.GetPaneByName(paneData.Id), diagram.SecondaryAxesY.GetAxisByName(paneData.Id));
            allSeriesList.AddRange(curveSeriesList);
         }

         return allSeriesList;
      }

      protected virtual IReadOnlyList<Series> CreateObservedDataSeriesList(ObservedCurveData observedCurveData)
      {
         return new List<Series>();
      }

      private IEnumerable<Series> reorderedSeries(List<Series> allSeriesList)
      {
         //it is required to add range series first for tool tips to click in
         var series = new List<Series>();
         series.AddRange(allSeriesList.Where(x => x.View.IsAnImplementationOf<RangeAreaSeriesView>()));
         series.AddRange(allSeriesList.Where(x => !x.View.IsAnImplementationOf<RangeAreaSeriesView>()));
         return series;
      }

      protected abstract IReadOnlyList<Series> CreateCurveSeriesList(CurveData<TX, TY> curveData);

      protected Series CreateSeries(ICurveDataSettings curve, ViewType viewType, ScaleType argumentScaleType = ScaleType.Numerical)
      {
         return CreateSeries<SeriesViewBase>(curve, viewType, null, argumentScaleType);
      }

      protected Series CreateSeries<TView>(ICurveDataSettings curve, ViewType viewType, Action<TView> viewConfiguration, ScaleType argumentScaleType = ScaleType.Numerical)
      {
         var series = new Series(curve.Caption, viewType)
         {
            ShowInLegend = false,
            ArgumentScaleType = argumentScaleType,
            ValueScaleType = ScaleType.Numerical,
            LabelsVisibility = DefaultBoolean.False,
            Visible = true,
            View = {Color = curve.Color},
         };


         var pointSeriesView = series.View as PointSeriesView;
         if (pointSeriesView != null)
            pointSeriesView.PointMarkerOptions.Kind = _symbolMapper.MapFrom(curve.Symbol);

         if (viewConfiguration != null)
         {
            var view = series.View.DowncastTo<TView>();
            viewConfiguration(view);
         }

         return series;
      }

      protected void AssignCurveSeriesList(IEnumerable<Series> seriesList, XYDiagramPane pane, SecondaryAxisY axisY)
      {
         foreach (var series in seriesList)
         {
            var view = series.View.DowncastTo<XYDiagramSeriesViewBase>();
            view.Pane = pane;
            view.AxisY = axisY;
         }
      }

      protected IReadOnlyList<SeriesPoint> CreateSeriesPoints<TXValue, TYValue>(CurveData<TXValue, TYValue> curveData, params Func<TYValue, float>[] yProperties)
         where TXValue : IXValue
         where TYValue : IYValue
      {
         return createSeriesPoints(curveData, y => yProperties.Select(prop => prop(y)), addYValuesAsSeparateSeriesPoint: false);
      }

      protected IReadOnlyList<SeriesPoint> CreateSeriesPoints<TXValue, TYValue>(CurveData<TXValue, TYValue> curveData, Func<TYValue, IEnumerable<float>> yProperty)
         where TXValue : IXValue
         where TYValue : IYValue
      {
         return createSeriesPoints(curveData, yProperty, addYValuesAsSeparateSeriesPoint: true);
      }

      private IReadOnlyList<SeriesPoint> createSeriesPoints<TXValue, TYValue>(CurveData<TXValue, TYValue> curveData, Func<TYValue, IEnumerable<float>> yProperty, bool addYValuesAsSeparateSeriesPoint)
         where TYValue : IYValue
         where TXValue : IXValue
      {
         var yAxis = curveData.YAxis;
         var xAxis = curveData.XAxis;
         var seriesPoints = new List<SeriesPoint>();

         for (int i = 0; i < curveData.XValues.Count; i++)
         {
            var xValue = xAxis.ConvertToDisplayUnit(curveData.XValues[i].X);
            var yValues = yProperty(curveData.YValues[i]);

            double[] yConvertedValues = yValues.Select(y => yAxis.ConvertToDisplayUnit(y)).ToArray();

            if (!valuesAreValidForAxes(xAxis, xValue, yAxis, yConvertedValues))
               continue;

            if (addYValuesAsSeparateSeriesPoint)
               seriesPoints.AddRange(yConvertedValues.Select(yValue => new SeriesPoint(xValue, yValue)));
            else
               seriesPoints.Add(new SeriesPoint(xValue, yConvertedValues));
         }
         return seriesPoints;
      }

      private static bool valuesAreValidForAxes(AxisData xAxis, double xValue, AxisData yAxis, double[] yValues)
      {
         return valuesAreValidForAxis(xAxis, xValue) && valuesAreValidForAxis(yAxis, yValues);
      }

      private static bool valuesAreValidForAxis(AxisData axis, params double[] values)
      {
         // avoid series points with invalid values. On log scale, all values <= 0 are invalid
         return axis.Scaling != Scalings.Log || values.All(y => y > 0);
      }

      protected Series CreateRangeAreaSeries(ICurveDataSettings curve)
      {
         return CreateSeries<RangeAreaSeriesView>(curve, ViewType.RangeArea, view =>
         {
            view.Marker1Visibility = DefaultBoolean.False;
            view.Marker2Visibility = DefaultBoolean.False;
            view.FillStyle.FillMode = FillMode.Solid;
            view.Transparency = Constants.RANGE_AREA_TRANSPARENCY;
         });
      }

      protected Series CreateLineSeries(ICurveDataSettings curve)
      {
         return CreateSeries<LineSeriesView>(curve, ViewType.Line, view => { view.LineStyle.DashStyle = _lineStyleMapper.MapFrom(curve.LineStyle); });
      }

      protected Series CreateFinancialSeries(ICurveDataSettings curve)
      {
         return CreateSeries<FinancialSeriesViewBase>(curve, ViewType.CandleStick, view =>
         {
            view.ReductionOptions.Visible = false;
            view.LevelLineLength = 0;
            view.LineThickness = 1;
         });
      }

      public void UpdateAxesSettings(PopulationAnalysisChart analysisChart)
      {
         updatePrimaryXAxisSettings(analysisChart);

         updatePrimaryYAxisSettings(analysisChart);

         updateSecondaryYAxisSettings(analysisChart);
      }

      private void updateSecondaryYAxisSettings(PopulationAnalysisChart analysisChart)
      {
         foreach (Axis axis in _view.Chart.XYDiagram.SecondaryAxesY)
         {
            var secondaryAxisIndex = _view.Chart.XYDiagram.SecondaryAxesY.IndexOf(axis);
            var axisSettings = analysisChart.AxisSettingsForSecondaryYAxis(secondaryAxisIndex);
            updateAxisSettingsAndRanges(axis, axisSettings);
         }
      }

      private void updatePrimaryYAxisSettings(PopulationAnalysisChart analysisChart)
      {
         var axis = _view.Chart.AxisY;
         var axisSettings = analysisChart.PrimaryYAxisSettings;

         updateAxisSettingsAndRanges(axis, axisSettings);
      }

      private void updatePrimaryXAxisSettings(PopulationAnalysisChart analysisChart)
      {
         var axis = _view.Chart.AxisX;
         var axisSettings = analysisChart.PrimaryXAxisSettings;

         updateAxisSettingsAndRanges(axis, axisSettings);
      }

      private void updateAxisSettingsAndRanges(Axis axis, AxisSettings axisSettings)
      {
         var minValue = axis.GetMinForVisualRange();
         var maxValue = axis.GetMaxForVisualRange();


         axisSettings.Max = maxValue;
         axisSettings.Min = minValue;
         axisSettings.AutoRange = axis.VisualRange.Auto;

         // After zoom, there is some strange behaviour around the side margins. When the range is re-applied
         // using SetMinMaxValues, the result is not the same as when first zoom occurred.
         // Easiest way to make the appearance of the chart the same before and after is to apply the zoom range
         // using SetMinMaxValues in the zoom event handler
         axis.VisualRange.SetMinMaxValues(minValue, maxValue);
      }
   }
}