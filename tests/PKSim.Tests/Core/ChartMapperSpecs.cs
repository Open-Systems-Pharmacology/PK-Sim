using System.Drawing;
using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Chart;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_ChartMapper : ContextSpecificationAsync<ChartMapper>
   {
      protected IChart _chart;
      protected Snapshots.Chart _snapshot;

      protected override Task Context()
      {
         _chart = new CurveChart
         {
            Name = "Hello",
            Description = "Tralala"
         };

         _chart.ChartSettings.BackColor = Color.AntiqueWhite;
         _chart.FontAndSize.ChartHeight = 150;
         _chart.Title = "A great title";
         _chart.IncludeOriginData = true;

         _snapshot = new Snapshots.CurveChart();

         sut = new ChartMapper();
         return _completed;
      }
   }

   public class When_upading_a_snapshot_chart_from_a_chart : concern_for_ChartMapper
   {
      protected override async Task Because()
      {
         await sut.MapToSnapshot(_chart, _snapshot);
      }

      [Observation]
      public void should_save_the_expected_snapshot_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_chart.Name);
         _snapshot.Description.ShouldBeEqualTo(_chart.Description);
      }

      [Observation]
      public void should_copy_chart_settings()
      {
         _snapshot.Settings.ShouldBeEqualTo(_chart.ChartSettings);
      }

      [Observation]
      public void should_copy_font_and_size_properties()
      {
         _snapshot.FontAndSize.ShouldBeEqualTo(_chart.FontAndSize);
      }
   }

   public class When_updating_a_snapshot_from_a_chart_for_which_settings_and_font_have_not_changed : concern_for_ChartMapper
   {
      protected override async Task Context()
      {
         await base.Context();
         _chart = new CurveChart
         {
            Name = "Hello",
            Description = "Tralala"
         };
      }

      protected override async Task Because()
      {
         await sut.MapToSnapshot(_chart, _snapshot);
      }

      [Observation]
      public void should_save_the_expected_snapshot_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_chart.Name);
         _snapshot.Description.ShouldBeEqualTo(_chart.Description);
      }

      [Observation]
      public void should_set_settings_to_null()
      {
         _snapshot.Settings.ShouldBeNull();
      }

      [Observation]
      public void should_set_font_to_null()
      {
         _snapshot.FontAndSize.ShouldBeNull();
      }
   }


   public class When_upading_a_chart_from_a_snapshot_chart : concern_for_ChartMapper
   {
      private CurveChart _newChart;

      protected override async Task Context()
      {
         await base.Context();
         await sut.MapToSnapshot(_chart, _snapshot);

         _snapshot.Settings.LegendPosition = LegendPositions.Bottom;
         _snapshot.FontAndSize.ChartWidth = 100;
         _newChart = new CurveChart();
      }

      protected override async Task Because()
      {
         await sut.MapToModel(_snapshot, _newChart);
      }

      [Observation]
      public void should_save_the_expected_snapshot_properties()
      {
         _newChart.Name.ShouldBeEqualTo(_snapshot.Name);
         _newChart.Description.ShouldBeEqualTo(_snapshot.Description);
         _newChart.Title.ShouldBeEqualTo(_snapshot.Title);
         _newChart.OriginText.ShouldBeEqualTo(_snapshot.OriginText);
         _newChart.IncludeOriginData.ShouldBeEqualTo(_chart.IncludeOriginData);
         _newChart.PreviewSettings.ShouldBeEqualTo(_chart.PreviewSettings);
      }

      [Observation]
      public void should_copy_chart_settings()
      {
         _newChart.ChartSettings.LegendPosition.ShouldBeEqualTo(_snapshot.Settings.LegendPosition);
      }

      [Observation]
      public void should_copy_font_and_size_properties()
      {
         _newChart.FontAndSize.ChartWidth.ShouldBeEqualTo(_snapshot.FontAndSize.ChartWidth);
      }
   }
}