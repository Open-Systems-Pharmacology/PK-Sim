using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_CurveChartMapper : ContextSpecificationAsync<CurveChartMapper>
   {
      protected AxisMapper _axisMapper;
      protected CurveMapper _curveMapper;
      protected CurveChart _curveChart;
      protected Axis _axis;
      protected Curve _curve;
      protected Snapshots.Axis _snapshotAxis;
      protected Snapshots.Curve _snapshotCurve;
      protected Snapshots.CurveChart _snapshot;

      protected override Task Context()
      {
         _axisMapper = A.Fake<AxisMapper>();
         _curveMapper = A.Fake<CurveMapper>();
         sut = new CurveChartMapper(_axisMapper, _curveMapper);

         var dimensionFactory = A.Fake<IDimensionFactory>();

         _curveChart = new CurveChart
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
}