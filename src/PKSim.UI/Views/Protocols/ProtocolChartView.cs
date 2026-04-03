using DevExpress.Utils;
using DevExpress.XtraCharts;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Services;
using PKSim.Presentation.Presenters.Protocols;
using PKSim.Presentation.Views.Protocols;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace PKSim.UI.Views.Protocols
{
   public partial class ProtocolChartView : BaseUserControl, IProtocolChartView
   {
      private readonly IImageListRetriever _imageListRetriever;
      private IProtocolChartPresenter _presenter;

      public string XAxisTitle { get; set; }
      public string YAxisTitle { get; set; }
      public string Y2AxisTitle { get; set; }
      public double BarWidth { get; set; }

      public ProtocolChartView(IImageListRetriever imageListRetriever)
      {
         _imageListRetriever = imageListRetriever;
         InitializeComponent();
         chart.ObjectHotTracked += chartObjectHotTracked;
         chart.Images = _imageListRetriever.AllImages16x16;
         chart.CrosshairEnabled = DefaultBoolean.False;
         _toolTipController.Initialize(imageListRetriever);
      }

      public void AttachPresenter(IProtocolChartPresenter presenter)
      {
         _presenter = presenter;
         chart.AddCopyToClipboardPopupMenu(presenter);
         chart.AddExportToPngPopupMenu(presenter);
      }

      public void Clear()
      {
         chart.Series.Clear();
         chart.SeriesDataMember = null;
         chart.DataSource = null;
         chart.SeriesTemplate.ArgumentDataMember = null;
      }

      public void Plot(IProtocolChartData dataToPlot)
      {
         Clear();
         chart.DataSource = dataToPlot.DataTable;
         chart.SeriesDataMember = dataToPlot.GroupingName;
         chart.InitializeColor();

         // Specify the date-time argument scale type for the series,
         // as it is qualitative, by default.
         chart.SeriesTemplate.ArgumentScaleType = ScaleType.Numerical;
         chart.SeriesTemplate.ArgumentDataMember = dataToPlot.XValue;
         chart.SeriesTemplate.ValueScaleType = ScaleType.Numerical;
         chart.SeriesTemplate.ValueDataMembers.AddRange(dataToPlot.YValue);
         chart.SeriesTemplate.LabelsVisibility = DefaultBoolean.False;
         var stackedBarSeriesView = new StackedBarSeriesView {BarWidth = BarWidth};
         chart.SeriesTemplate.View = stackedBarSeriesView;
         var diagram = (XYDiagram) chart.Diagram;
         ////// Access the type-specific options of the diagram.
         diagram.AxisX.Title.Text = XAxisTitle;
         diagram.AxisX.Title.Visibility = DefaultBoolean.True;
         diagram.AxisY.Title.Text = YAxisTitle;
         diagram.AxisY.Title.Visibility = DefaultBoolean.True;
         diagram.AxisX.WholeRange.Auto = true;
         diagram.AxisX.WholeRange.SetMinMaxValues(dataToPlot.XMin - BarWidth, dataToPlot.XMax + BarWidth);
         chart.Legend.LegendPosition(LegendPositions.RightInside);
         diagram.SecondaryAxesY.Clear();

         SecondaryAxisY axisY2 = null;
         if (dataToPlot.NeedsMultipleAxis)
         {
            axisY2 = new SecondaryAxisY("Y2");
            diagram.SecondaryAxesY.Add(axisY2);
            axisY2.Title.Text = Y2AxisTitle;
            axisY2.Title.Visibility = DefaultBoolean.True;
         }

         foreach (Series series in chart.Series)
         {
            var view = (BarSeriesView) series.View;
            if (dataToPlot.NeedsMultipleAxis)
            {
               if (dataToPlot.SeriesShouldBeOnSecondAxis(series.Name))
                  view.AxisY = axisY2;
            }
            series.Visible = true;
         }

         addEventSeries(dataToPlot);
      }

      private void addEventSeries(IProtocolChartData dataToPlot)
      {
         if (!dataToPlot.EventPoints.Any())
            return;

         // Position markers slightly above the X-axis so they aren't clipped
         double maxDose = 0;
         foreach (System.Data.DataRow row in dataToPlot.DataTable.Rows)
         {
            var dose = (double) row[dataToPlot.YValue];
            if (dose > maxDose) maxDose = dose;
         }

         if (maxDose == 0) maxDose = 1;
         var eventY = maxDose * 0.03;

         foreach (var eventGroup in dataToPlot.EventPoints.GroupBy(p => p.GroupingName))
         {
            var series = new Series(eventGroup.Key, ViewType.Point);
            var pointView = (PointSeriesView) series.View;
            pointView.PointMarkerOptions.Kind = MarkerKind.Diamond;
            pointView.PointMarkerOptions.Size = 10;
            pointView.PointMarkerOptions.BorderVisible = true;
            pointView.PointMarkerOptions.BorderColor = Color.Black;

            foreach (var point in eventGroup)
            {
               var sp = new SeriesPoint(point.Time, eventY);
               sp.Tag = point.SchemaItem;
               series.Points.Add(sp);
            }

            series.LabelsVisibility = DefaultBoolean.False;
            series.ArgumentScaleType = ScaleType.Numerical;
            series.ValueScaleType = ScaleType.Numerical;
            chart.Series.Add(series);
         }
      }

      private void chartObjectHotTracked(object sender, HotTrackEventArgs e)
      {
         var point = e.AdditionalObject as SeriesPoint;
         if (point == null)
         {
            hideToolTip(e);
            return;
         }

         var schemaItemDTO = _presenter.SchemaItemDTOFrom(point.Tag);
         var message = _presenter.DescriptionFor(schemaItemDTO);
         if (string.IsNullOrEmpty(message) || schemaItemDTO == null)
         {
            hideToolTip(e);
            return;
         }

         var imageIndex = _imageListRetriever.ImageIndex(schemaItemDTO.IconName);
         if (imageIndex != _toolTipController.ImageIndex)
            hideToolTip(e);

         _toolTipController.ImageIndex = _imageListRetriever.ImageIndex(schemaItemDTO.IconName);
         _toolTipController.ShowHint(message);
      }

      private void hideToolTip(HotTrackEventArgs e)
      {
         e.Cancel = true;
         _toolTipController.HideHint();
      }

      public void CopyToClipboard(string watermark) => chart.CopyToClipboard(watermark);

      public void ExportToPng(string filePath, string watermark) => 
         chart.ExportChartToImageFile(watermark, filePath, ImageFormat.Png);
   }
}