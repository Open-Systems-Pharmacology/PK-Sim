using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OSPSuite.UI.Services;
using OSPSuite.UI.Extensions;
using DevExpress.Utils;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraCharts;
using PKSim.Assets;
using PKSim.Core.Chart;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Views;
using OSPSuite.Assets;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class BaseChartView<TX, TY> : BaseUserControl, IChartWithSettings
      where TX : IXValue
      where TY : IYValue
   {
      private readonly IToolTipCreator _toolTipCreator;
      private readonly UxChartControl _chartControl;
      protected IPopulationAnalysisChartPresenter<TX, TY> _presenter;
      protected CurveData<TX, TY> _latestTrackedCurvedData;
      protected SeriesPoint _latestSeriesPoint;
      public bool DragDropEnabled { get; set; }

      public XYDiagram Diagram => _chartControl.XYDiagram;

      public UxChartControl Chart => _chartControl;

      public IChartsDataBinder<TX, TY> ChartsDataBinder { get; protected set; }

      public BaseChartView(IImageListRetriever imageListRetriever, IToolTipCreator toolTipCreator)
      {
         InitializeComponent();

         _toolTipCreator = toolTipCreator;
         _chartControl = new UxChartControl(useDefaultPopupMechanism: true,addCopyToClipboardMenu:false)
         {
            Images = imageListRetriever.AllImages16x16,
            CrosshairEnabled = DefaultBoolean.False
         };
         _pnlChart.FillWith(_chartControl);
         _chartControl.RefreshDataOnRepaint = false;
         _chartControl.CacheToMemory = true;

         DragDropEnabled = false;
         initializeChart(imageListRetriever);
      }

      private void initializeChart(IImageListRetriever imageListRetriever)
      {
         _chartControl.ToolTipController = new ToolTipController { ToolTipType = ToolTipType.SuperTip };
         _chartControl.ToolTipController.Initialize(imageListRetriever);
         _chartControl.ObjectHotTracked += (o, e) => OnEvent(onObjectHotTracked, e);
         _chartControl.ObjectSelected += (o, e) => OnEvent(onObjectSelected, e);
         _chartControl.SelectionMode = ElementSelectionMode.Single;
         _chartControl.SeriesSelectionMode = SeriesSelectionMode.Point;
         _chartControl.AllowDrop = true;
         _chartControl.DragOver += (o, e) => OnEvent(OnDragOver, e);
         _chartControl.DragDrop += (o, e) => OnEvent(OnDragDrop, e);
         _chartControl.Zoom += (o, e) => OnEvent(() => ChartsDataBinder.UpdateAxesSettings(_presenter.AnalysisChart));
         _chartControl.PopupMenu.BeforePopup += (o, e) => OnEvent(() => ConfigurePopup(e));
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
      }

      protected override void OnDragDrop(DragEventArgs e)
      {
         if (!DragDropEnabled) return;
         base.OnDragDrop(e);
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

         var diagramCoordinates = _chartControl.DiagramCoordinatesAt(e);
         _latestSeriesPoint  = e.HitInfo.SeriesPoint;
         SuperToolTip superToolTip = null;

         var observedCurveData = _presenter.ObservedCurveDataFor(diagramCoordinates.Pane.Name, series.Name);
         if (observedCurveData != null)
         {
            superToolTip = getSuperToolTipFor(observedCurveData, _latestSeriesPoint, diagramCoordinates);
         }

         if (superToolTip == null)
         {
            _latestTrackedCurvedData  = _presenter.CurveDataFor(diagramCoordinates.Pane.Name, series.Name);
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

         var args = new ToolTipControllerShowEventArgs { SuperTip = superToolTip };
         _chartControl.ToolTipController.ShowHint(args);
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
         _chartControl.ToolTipController.HideHint();
      }

      public virtual void AddTitleTo(XYDiagramPane pane, string title)
      {
         _chartControl.DoUpdateOf(pane.Annotations, () =>
         {
            var textAnnotation = pane.Annotations.AddTextAnnotation(Guid.NewGuid().ToString(), title);
            textAnnotation.Visible = true;
            textAnnotation.ShapePosition = new FreePosition { DockTarget = pane, DockCorner = DockCorner.LeftTop };
            textAnnotation.BackColor = Color.FromArgb(80, Color.White);
            textAnnotation.ShapeKind = ShapeKind.Rectangle;
            textAnnotation.ConnectorStyle = AnnotationConnectorStyle.None;
            textAnnotation.Font = new Font(textAnnotation.Font, FontStyle.Bold);
         });
      }

      public virtual void ClearAllSeries()
      {
         _chartControl.Series.Clear();
      }

      public virtual void AddDynamicMenus(bool allowEdit = true)
      {
         _chartControl.AddPopupMenu(MenuNames.ResetZoom, ResetZoom, ApplicationIcons.Reset);
         _chartControl.AddPopupMenu(MenuNames.CopyToClipboard, () => _presenter.AnalysisChart.CopyToClipboard(_chartControl), ApplicationIcons.Copy, beginGroup: true);

         if (allowEdit)
            _chartControl.AddPopupMenu(MenuNames.Edit, _presenter.Edit, ApplicationIcons.Edit);

         _chartControl.AddPopupMenu(MenuNames.ExportToExcel, _presenter.ExportDataToExcel, ApplicationIcons.Excel);
         _chartControl.AddPopupMenu(MenuNames.ExportToPDF, _presenter.ExportToPDF, ApplicationIcons.PDF);
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
         _chartControl.Series.AddRange(seriesList.ToArray());
      }

      public virtual void SetChartSettingsEditor(IView view)
      {
         _pnlChartSettings.FillWith(view);
      }

      public void SetDockStyle(DockStyle dockStyle)
      {
         _chartControl.Dock = dockStyle;
      }
   }
}