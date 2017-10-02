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
using CurveChart = PKSim.Core.Snapshots.CurveChart;

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
      protected CurveChart _snapshot;
      protected ChartMapper _chartMapper;

      protected override Task Context()
      {
         _axisMapper = A.Fake<AxisMapper>();
         _curveMapper = A.Fake<CurveMapper>();
         _chartMapper = A.Fake<ChartMapper>();
         sut = new SimulationTimeProfileChartMapper(_chartMapper, _axisMapper, _curveMapper);

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
         A.CallTo(() => _axisMapper.MapToSnapshots(_curveChart.Axes)).ReturnsAsync(new[] {_snapshotAxis});

         _snapshotCurve = new Snapshots.Curve();
         A.CallTo(() => _curveMapper.MapToSnapshots(_curveChart.Curves)).ReturnsAsync(new[] {_snapshotCurve});

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
      public void should_save_the_basic_chart_properties()
      {
         A.CallTo(() => _chartMapper.MapToSnapshot(_curveChart, _snapshot)).MustHaveHappened();
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

         A.CallTo(() => _axisMapper.MapToModels(_snapshot.Axes)).ReturnsAsync(new[] {_axis});
         A.CallTo(() => _curveMapper.MapToModels(_snapshot.Curves, _context)).ReturnsAsync(new[] {_curve});
      }

      protected override async Task Because()
      {
         _newChart = await sut.MapToModel(_snapshot, _context);
      }

      [Observation]
      public void should_update_the_basic_chart_properties()
      {
         A.CallTo(() => _chartMapper.MapToModel(_snapshot, _newChart)).MustHaveHappened();
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