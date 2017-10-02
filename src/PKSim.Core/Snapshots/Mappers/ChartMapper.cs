using System.Threading.Tasks;
using OSPSuite.Core.Chart;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ChartMapper : ObjectBaseSnapshotMapperBase<IChart, Chart, IChart, Chart>
   {
      public override Task<Chart> MapToSnapshot(IChart chart, Chart snapshot)
      {
         MapModelPropertiesToSnapshot(chart, snapshot);
         snapshot.Settings = settingsIfChanged(chart.ChartSettings);
         snapshot.FontAndSize = fontsAndSizeIfChanged(chart.FontAndSize);
         snapshot.IncludeOriginData = SnapshotValueFor(chart.IncludeOriginData);
         snapshot.OriginText = SnapshotValueFor(chart.OriginText);
         snapshot.PreviewSettings = SnapshotValueFor(chart.PreviewSettings);
         snapshot.Title = SnapshotValueFor(chart.Title);
         return Task.FromResult(snapshot);
      }

      private ChartSettings settingsIfChanged(ChartSettings chartSettings) => isDefault(chartSettings) ? null : chartSettings;
      private ChartFontAndSizeSettings fontsAndSizeIfChanged(ChartFontAndSizeSettings fontAndSizeSettings) => isDefault(fontAndSizeSettings) ? null : fontAndSizeSettings;

      public override Task<IChart> MapToModel(Chart snapshot, IChart chart)
      {
         MapSnapshotPropertiesToModel(snapshot, chart);
         chart.ChartSettings.UpdatePropertiesFrom(snapshot.Settings);
         chart.FontAndSize.UpdatePropertiesFrom(snapshot.FontAndSize);
         chart.IncludeOriginData = snapshot.IncludeOriginData.GetValueOrDefault(chart.IncludeOriginData);
         chart.OriginText = snapshot.OriginText;
         chart.PreviewSettings = snapshot.PreviewSettings.GetValueOrDefault(chart.PreviewSettings);
         chart.Title = ModelValueFor(snapshot.Title);
         return Task.FromResult(chart);
      }

      private bool isDefault(ChartSettings chartSettings)
      {
         var defaultChartSettings = new ChartSettings();
         return
            chartSettings.SideMarginsEnabled == defaultChartSettings.SideMarginsEnabled &&
            chartSettings.LegendPosition == defaultChartSettings.LegendPosition &&
            chartSettings.BackColor == defaultChartSettings.BackColor &&
            chartSettings.DiagramBackColor == defaultChartSettings.DiagramBackColor;
      }

      private bool isDefault(ChartFontAndSizeSettings fontAndSizeSettings)
      {
         var defaultFontAndSizeSettings = new ChartFontAndSizeSettings();
         return
            fontAndSizeSettings.ChartWidth == defaultFontAndSizeSettings.ChartWidth &&
            fontAndSizeSettings.ChartHeight == defaultFontAndSizeSettings.ChartHeight &&
            isDefault(fontAndSizeSettings.Fonts);
      }

      private bool isDefault(ChartFonts fonts)
      {
         var defaultFonts = new ChartFonts();
         return
            fonts.AxisSize == defaultFonts.AxisSize &&
            fonts.DescriptionSize == defaultFonts.DescriptionSize &&
            fonts.LegendSize == defaultFonts.LegendSize &&
            fonts.OriginSize == defaultFonts.OriginSize &&
            fonts.TitleSize == defaultFonts.TitleSize &&
            fonts.FontFamilyName == defaultFonts.FontFamilyName;
      }
   }
}