using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Chart;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_CurveChartMapper : ContextSpecificationAsync<SimulationTimeProfileChartMapper>
   {
      protected AxisMapper _axisMapper;
      protected CurveMapper _curveMapper;
      protected SimulationTimeProfileChart _curveChart;
      protected Axis _axis;
      protected Curve _curve;
      protected Snapshots.Axis _snapshotAxis;
      protected Snapshots.Curve _snapshotCurve;
      protected Snapshots.CurveChart _snapshot;

      protected override Task Context()
      {
         _axisMapper = A.Fake<AxisMapper>();
         _curveMapper = A.Fake<CurveMapper>();
         sut = new SimulationTimeProfileChartMapper(_axisMapper, _curveMapper);

         var dimensionFactory = A.Fake<IDimensionFactory>();

         _curveChart = new SimulationTimeProfileChart
         {
            Name = "Chart",
            Description = "ChartDescription",
            Title = "Chart Title",
            OriginText = "Chart Origin Text",
            IncludeOriginData = true,
            PreviewSettings = true,
         };

         _axis = new Axis(AxisTypes.X);
         _curveChart.AddAxis(_axis);

         _curve = new Curve();

         var dataRepository = DomainHelperForSpecs.ObservedData();
         var xColumn = dataRepository.BaseGrid;
         var yColumn = dataRepository.ObservationColumns().First();

         _curve.SetxData(xColumn, dimensionFactory);
         _curve.SetyData(yColumn, dimensionFactory);


         _curveChart.AddCurve(_curve);

         _snapshotAxis = new Snapshots.Axis();
         A.CallTo(() => _axisMapper.MapToSnapshot(_axis)).ReturnsAsync(_snapshotAxis);

         _snapshotCurve = new Snapshots.Curve();
         A.CallTo(() => _curveMapper.MapToSnapshot(_curve)).ReturnsAsync(_snapshotCurve);

         return Task.FromResult(true);
      }
   }

   public class When_exporting_a_curve_chart_to_snapshot : concern_for_CurveChartMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_curveChart);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_chart_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_curveChart.Name);
         _snapshot.Description.ShouldBeEqualTo(_curveChart.Description);
      }

      [Observation]
      public void should_save_curves_to_snapshot()
      {
         _snapshot.Axes.ShouldContain(_snapshotAxis);
      }

      [Observation]
      public void should_save_axes_to_snapshot()
      {
         _snapshot.Curves.ShouldOnlyContain(_snapshotCurve);
      }

      [Observation]
      public void should_copy_chart_settings()
      {
         _snapshot.Settings.ShouldBeEqualTo(_curveChart.ChartSettings);
      }

      [Observation]
      public void should_copy_font_and_size_properties()
      {
         _snapshot.FontAndSize.ShouldBeEqualTo(_curveChart.FontAndSize);
      }
   }

   public class When_mapping_a_curve_chart_snapshot_to_curve_chart : concern_for_CurveChartMapper
   {
      private CurveChartContext _context;
      private SimulationTimeProfileChart _newChart;

      protected override async Task Context()
      {
         await base.Context();
         _context = new CurveChartContext();
         _snapshot = await sut.MapToSnapshot(_curveChart);
         _snapshot.Axes = new[] {_snapshotAxis,};
         _snapshot.Settings.LegendPosition = LegendPositions.Right;
         _snapshot.FontAndSize.ChartHeight = 150;

         A.CallTo(() => _axisMapper.MapToModel(_snapshotAxis)).ReturnsAsync(_axis);
         A.CallTo(() => _curveMapper.MapToModel(_snapshotCurve, _context)).ReturnsAsync(_curve);
      }

      protected override async Task Because()
      {
         _newChart = await sut.MapToModel(_snapshot, _context);
      }

      [Observation]
      public void should_return_a_chart_having_the_expected_properties()
      {
         _newChart.Name.ShouldBeEqualTo(_snapshot.Name);
         _newChart.Description.ShouldBeEqualTo(_snapshot.Description);
         _newChart.Title.ShouldBeEqualTo(_snapshot.Title);
         _newChart.OriginText.ShouldBeEqualTo(_snapshot.OriginText);
         _newChart.IncludeOriginData.ShouldBeEqualTo(_snapshot.IncludeOriginData);
         _newChart.PreviewSettings.ShouldBeEqualTo(_snapshot.PreviewSettings);
      }

      [Observation]
      public void should_have_updated_chart_settings_properties()
      {
         _newChart.ChartSettings.LegendPosition.ShouldBeEqualTo(_snapshot.Settings.LegendPosition);
      }

      [Observation]
      public void should_have_updated_size_and_fonts_properties()
      {
         _newChart.FontAndSize.ChartHeight.ShouldBeEqualTo(_snapshot.FontAndSize.ChartHeight);
      }

      [Observation]
      public void should_have_added_the_axis_from_snapshot()
      {
         _newChart.Axes.ShouldContain(_axis);  
      }
      
      [Observation]
      public void should_have_added_the_curve_from_snapshot()
      {
         _newChart.Curves.ShouldContain(_curve);
      }
   }
}