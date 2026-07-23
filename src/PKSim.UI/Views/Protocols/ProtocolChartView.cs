using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using OSPSuite.Core.Chart;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.Presenters.Protocols;
using PKSim.Presentation.Views.Protocols;

namespace PKSim.UI.Views.Protocols
{
   public partial class ProtocolChartView : BaseUserControl, IProtocolChartView
   {
      private readonly IImageListRetriever _imageListRetriever;
      private IProtocolChartPresenter _presenter;

      //moderate translucency so overlapping infusion rectangles remain readable
      private const int INFUSION_AREA_TRANSPARENCY = 100;

      public string XAxisTitle { get; set; }
      public string YAxisTitle { get; set; }
      public string Y2AxisTitle { get; set; }
      public double BarWidth { get; set; }

      public ProtocolChartView(IImageListRetriever imageListRetriever)
      {
         _imageListRetriever = imageListRetriever;
         InitializeComponent();
         chart.ObjectHotTracked += chartObjectHotTracked;
         chart.CustomDrawSeries += chartCustomDrawSeries;
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

         if (dataToPlot.DataTable.Rows.Count == 0)
            return;

         chart.InitializeColor();
         addSeries(dataToPlot);

         var diagram = (XYDiagram)chart.Diagram;
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
            if (dataToPlot.NeedsMultipleAxis && dataToPlot.SeriesShouldBeOnSecondAxis(series.Name))
               ((XYDiagramSeriesViewBase)series.View).AxisY = axisY2;
         }
      }

      private void addSeries(IProtocolChartData dataToPlot)
      {
         //one administration grouping (= one application type) maps to one color and one legend entry
         var groups = dataToPlot.DataTable.Rows.Cast<DataRow>()
            .GroupBy(row => row[dataToPlot.GroupingName].ToString())
            .ToList();

         var paletteColors = chart.GetPaletteEntries(groups.Count);

         groups.Each((group, i) =>
            createSeriesFor(group.Key, group.ToList(), dataToPlot, paletteColors[i].Color).Each(series => chart.Series.Add(series)));
      }

      private IEnumerable<Series> createSeriesFor(string applicationName, IReadOnlyList<DataRow> rows, IProtocolChartData dataToPlot, Color color)
      {
         var isInfusion = rows.Any(row => endTimeFor(row, dataToPlot) > startTimeFor(row, dataToPlot));
         if (!isInfusion)
            return [createApplicationSeries(applicationName, rows, dataToPlot, color)];

         //each infusion is its own area series so overlapping infusions render as independent translucent rectangles
         //(a single area series cannot represent two different heights over the same time span)
         return rows.Select((row, index) => createInfusionSeries(applicationName, row, dataToPlot, color, showInLegend: index == 0));
      }

      /// <summary>
      ///    Builds an <see cref="ViewType.Area" /> series for a single infusion, drawn as a rectangle that spans
      ///    [start, start + infusion time] with a height equal to the dose. One series is created per infusion so
      ///    that overlapping infusions render as independent (translucent) rectangles instead of a single merged
      ///    shape (an area series can only have one height at any given time).
      /// </summary>
      private Series createInfusionSeries(string applicationName, DataRow row, IProtocolChartData dataToPlot, Color color, bool showInLegend)
      {
         var series = newSeries(applicationName, ViewType.Area);
         //only one legend entry per grouping; keep the rectangle corner points in the order added
         series.ShowInLegend = showInLegend;
         series.SeriesPointsSorting = SortingMode.None;

         var start = startTimeFor(row, dataToPlot);
         var end = endTimeFor(row, dataToPlot);
         var dose = doseFor(row, dataToPlot);
         var tag = tagFor(row, dataToPlot);

         //trace the rectangle [start, end] x [0, dose]
         series.Points.Add(taggedPoint(start, 0, tag));
         series.Points.Add(taggedPoint(start, dose, tag));
         series.Points.Add(taggedPoint(end, dose, tag));
         series.Points.Add(taggedPoint(end, 0, tag));

         var view = (AreaSeriesView)series.View;
         view.Color = color;
         view.Transparency = INFUSION_AREA_TRANSPARENCY;
         return series;
      }

      /// <summary>
      ///    Builds a single <see cref="ViewType.StackedBar" /> series for instantaneous administrations (e.g. IV
      ///    bolus, oral): one thin bar per administration, centered on its start time with a height equal to the
      ///    dose. Bars belonging to different application types that share a start time stack on top of each other.
      /// </summary>
      private Series createApplicationSeries(string applicationName, IReadOnlyList<DataRow> rows, IProtocolChartData dataToPlot, Color color)
      {
         var series = newSeries(applicationName, ViewType.StackedBar);
         rows.Each(row => series.Points.Add(taggedPoint(startTimeFor(row, dataToPlot), doseFor(row, dataToPlot), tagFor(row, dataToPlot))));

         var view = (BarSeriesView)series.View;
         view.Color = color;
         view.BarWidth = BarWidth;
         return series;
      }

      private static Series newSeries(string name, ViewType viewType)
      {
         return new Series(name, viewType)
         {
            ArgumentScaleType = ScaleType.Numerical,
            ValueScaleType = ScaleType.Numerical,
            LabelsVisibility = DefaultBoolean.False
         };
      }

      private static SeriesPoint taggedPoint(double argument, double value, object tag) => new SeriesPoint(argument, value) { Tag = tag };

      private static double startTimeFor(DataRow row, IProtocolChartData dataToPlot) => (double)row[dataToPlot.XValue];
      private static double endTimeFor(DataRow row, IProtocolChartData dataToPlot) => (double)row[dataToPlot.XValue2];
      private static double doseFor(DataRow row, IProtocolChartData dataToPlot) => (double)row[dataToPlot.YValue];
      private static object tagFor(DataRow row, IProtocolChartData dataToPlot) => row[dataToPlot.SchemaItemName];

      /// <summary>
      ///    The default legend glyph for an area series is a triangle shaped miniature of the area; draw a square
      ///    filled with the series color instead.
      /// </summary>
      private void chartCustomDrawSeries(object sender, CustomDrawSeriesEventArgs e)
      {
         if (!(e.Series.View is AreaSeriesView areaView))
            return;

         var marker = new Bitmap(e.LegendMarkerSize.Width, e.LegendMarkerSize.Height);
         using (var g = Graphics.FromImage(marker))
         using (var brush = new SolidBrush(Color.FromArgb(byte.MaxValue - areaView.Transparency, areaView.Color)))
         {
            g.FillRectangle(brush, 0, 0, marker.Width, marker.Height);
         }

         e.LegendMarkerImage = marker;
         e.DisposeLegendMarkerImage = true;
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