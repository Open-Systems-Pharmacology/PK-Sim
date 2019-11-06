using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraCharts;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Core;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Services;
using PKSim.Assets;
using PKSim.Core.Chart;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class BaseChartView<TX, TY> : BaseUserControl, IChartWithSettings
      where TX : IXValue
      where TY : IYValue
   {
      public event EventHandler<IDragEvent> OnDragOverEvent = delegate { };
      public event EventHandler<IDragEvent> OnDragDropEvent = delegate { };


      private readonly IToolTipCreator _toolTipCreator;
      protected IPopulationAnalysisChartPresenter<TX, TY> _presenter;
      protected CurveData<TX, TY> _latestTrackedCurvedData;
      protected SeriesPoint _latestSeriesPoint;
      public bool DragDropEnabled { get; set; }

      public XYDiagram Diagram => Chart.XYDiagram;

      public UxChartControl Chart { get; }

      public IChartsDataBinder<TX, TY> ChartsDataBinder { get; protected set; }

      public BaseChartView(IImageListRetriever imageListRetriever, IToolTipCreator toolTipCreator)
      {
         InitializeComponent();

         _toolTipCreator = toolTipCreator;
         Chart = new UxChartControl(useDefaultPopupMechanism: true)
         {
            Images = imageListRetriever.AllImages16x16,
            CrosshairEnabled = DefaultBoolean.False
         };
         _pnlChart.FillWith(Chart);
         Chart.RefreshDataOnRepaint = false;
         Chart.CacheToMemory = true;

         DragDropEnabled = false;
         initializeChart(imageListRetriever);
      }

      private void initializeChart(IImageListRetriever imageListRetriever)
      {
         Chart.ToolTipController = new ToolTipController {ToolTipType = ToolTipType.SuperTip};
         Chart.ToolTipController.Initialize(imageListRetriever);
         Chart.ObjectHotTracked += (o, e) => OnEvent(onObjectHotTracked, e);
         Chart.ObjectSelected += (o, e) => OnEvent(onObjectSelected, e);
         Chart.SelectionMode = ElementSelectionMode.Single;
         Chart.SeriesSelectionMode = SeriesSelectionMode.Point;
         Chart.AllowDrop = true;
         Chart.DragOver += (o, e) => OnEvent(OnDragOver, e);
         Chart.DragDrop += (o, e) => OnEvent(OnDragDrop, e);
         Chart.Zoom += (o, e) => OnEvent(() => ChartsDataBinder.UpdateAxesSettings(_presenter.AnalysisChart));
         Chart.PopupMenu.BeforePopup += (o, e) => OnEvent(() => ConfigurePopup(e));
      }

      protected virtual void ConfigurePopup(CancelEventArgs cancelEventArgs)
      {
      }

      public override void InitializeResources()
      {
         base.InitializeResources();

         _pnlChartSettings.Text = PKSimConstants.UI.ChartSettings;
         _pnlChartSettings.Anchor = AnchorStyles.None;
         _pnlChartSettings.Dock = DockingStyle.Right;
         _pnlChartSettings.Options.AllowFloating = false;
         _pnlChartSettings.Options.ShowCloseButton = false;
         _pnlChartSettings.OriginalSize = new Size(411, 200);

         _dockManager.DockingOptions.ShowCaptionImage = true;
         _dockManager.DockingOptions.HideImmediatelyOnAutoHide = true;
         _dockManager.AutoHideSpeed = 100;
      }

      protected override void OnDragOver(DragEventArgs e)
      {
         if (!DragDropEnabled) return;
         base.OnDragOver(e);
         OnDragOverEvent(this, new DragEvent(e));
      }

      protected override void OnDragDrop(DragEventArgs e)
      {
         if (!DragDropEnabled) return;
         base.OnDragDrop(e);
         OnDragDropEvent(this, new DragEvent(e));
      }

      private void onObjectHotTracked(HotTrackEventArgs e)
      {
         var series = e.Series();
         _latestTrackedCurvedData = null;
         _latestSeriesPoint = null;
         if (series == null)
         {
            hideToolTip(e);
            return;
         }

         var diagramCoordinates = Chart.DiagramCoordinatesAt(e);
         _latestSeriesPoint = e.HitInfo.SeriesPoint;
         SuperToolTip superToolTip = null;

         var observedCurveData = _presenter.ObservedCurveDataFor(diagramCoordinates.Pane.Name, series.Name);
         if (observedCurveData != null)
         {
            superToolTip = getSuperToolTipFor(observedCurveData, _latestSeriesPoint, diagramCoordinates);
         }

         if (superToolTip == null)
         {
            _latestTrackedCurvedData = _presenter.CurveDataFor(diagramCoordinates.Pane.Name, series.Name);
            if (_latestTrackedCurvedData == null)
            {
               hideToolTip(e);
               return;
            }

            superToolTip = getSuperToolTipFor(_latestTrackedCurvedData, _latestSeriesPoint, diagramCoordinates);
         }
         if (superToolTip == null)
         {
            hideToolTip(e);
            return;
         }

         var args = new ToolTipControllerShowEventArgs {SuperTip = superToolTip};
         Chart.ToolTipController.ShowHint(args);
      }

      private SuperToolTip getSuperToolTipFor<TXValue, TYValue>(CurveData<TXValue, TYValue> curveData, SeriesPoint point, DiagramCoordinates diagramCoordinates)
         where TXValue : IXValue
         where TYValue : IYValue
      {
         if (point != null)
            return _toolTipCreator.ToolTipFor(curveData, point.NumericalArgument, point.Values[0]);

         var x = diagramCoordinates.NumericalArgument;
         return _toolTipCreator.ToolTipFor(curveData, x, double.NaN);
      }

      private void onObjectSelected(HotTrackEventArgs e)
      {
         e.Cancel = true;
      }

      private void hideToolTip(HotTrackEventArgs e)
      {
         e.Cancel = true;
         Chart.ToolTipController.HideHint();
      }

      public virtual void AddTitleTo(XYDiagramPane pane, string title)
      {
         var textAnnotation = pane.Annotations.AddTextAnnotation(Guid.NewGuid().ToString(), title);
         textAnnotation.Visible = true;
         textAnnotation.ShapePosition = new FreePosition {DockTarget = pane, DockCorner = DockCorner.LeftTop};
         textAnnotation.BackColor = Color.FromArgb(80, Color.White);
         textAnnotation.ShapeKind = ShapeKind.Rectangle;
         textAnnotation.ConnectorStyle = AnnotationConnectorStyle.None;
         textAnnotation.Font = new Font(textAnnotation.Font, FontStyle.Bold);
      }

      public virtual void ClearAllSeries()
      {
         Chart.Series.Clear();
      }

      public virtual void AddDynamicMenus(bool allowEdit = true)
      {
         Chart.AddPopupMenu(MenuNames.ResetZoom, ResetZoom, ApplicationIcons.Reset);
         Chart.AddPopupMenu(MenuNames.CopyToClipboard, copyToClipboard, ApplicationIcons.Copy, beginGroup: true);

         if (allowEdit)
            Chart.AddPopupMenu(MenuNames.Edit, _presenter.Edit, ApplicationIcons.Edit);

         Chart.AddPopupMenu(MenuNames.ExportToExcel, _presenter.ExportDataToExcel, ApplicationIcons.Excel);
         Chart.AddPopupMenu(MenuNames.ExportToPDF, _presenter.ExportToPDF, ApplicationIcons.PDF);
      }

      private void copyToClipboard()
      {
         Chart.CopyToClipboard(_presenter.AnalysisChart, _presenter.Watermark);
      }

      protected virtual void ResetZoom()
      {
         if (Diagram == null) return;
         resetAxis(Diagram.AxisX);
         resetAxis(Diagram.AxisY);
         foreach (Axis yAxis2 in Diagram.SecondaryAxesY)
         {
            resetAxis(yAxis2);
         }

         ChartsDataBinder.UpdateAxesSettings(_presenter.AnalysisChart);
      }

      private void resetAxis(Axis axis)
      {
         axis.WholeRange.Auto = true;
         axis.VisualRange.Auto = true;
      }

      public virtual void Bind(IEnumerable<Series> seriesList)
      {
         Chart.Series.AddRange(seriesList.ToArray());
      }

      public virtual void SetChartSettingsEditor(IView view)
      {
         _pnlChartSettings.FillWith(view);
      }

      public void SetDockStyle(DockStyle dockStyle)
      {
         Chart.Dock = dockStyle;
      }

      public void UpdateWatermark(PopulationAnalysisChart populationAnalysisChart, bool showWatermark)
      {
         Chart.AddWatermark(showWatermark? _presenter.Watermark : null, populationAnalysisChart);
      }
   }  
}