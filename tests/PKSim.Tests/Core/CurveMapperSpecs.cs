using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_CurveMapper : ContextSpecificationAsync<CurveMapper>
   {
      protected Curve _curve;
      protected Snapshots.Curve _snapshot;
      protected IDimensionFactory _dimensionFactory;
      protected DataColumn _xColumn;
      protected DataColumn _yColumn;
      protected DataRepository _dataRepository;
      protected CurveOptionsMapper _curveOptionsMapper;
      protected Snapshots.CurveOptions _snapshotCurveOptions;
      private ILogger _logger;

      protected override Task Context()
      {
         _dimensionFactory = A.Fake<IDimensionFactory>();
         _curveOptionsMapper = A.Fake<CurveOptionsMapper>();
         _dataRepository = DomainHelperForSpecs.ObservedData();
         _logger= A.Fake<ILogger>();
         sut = new CurveMapper(_curveOptionsMapper, _dimensionFactory, _logger);

         _curve = new Curve
         {
            Name = "Hello",
         };

         _xColumn = _dataRepository.BaseGrid;
         _yColumn = _dataRepository.ObservationColumns().First();

         _xColumn.QuantityInfo.Path = new[] {"A", "B"};
         _yColumn.QuantityInfo.Path = new[] {"BaseGrid"};

         _curve.SetxData(_xColumn, _dimensionFactory);
         _curve.SetyData(_yColumn, _dimensionFactory);

         _snapshotCurveOptions = new Snapshots.CurveOptions();
         A.CallTo(() => _curveOptionsMapper.MapToSnapshot(_curve.CurveOptions)).Returns(_snapshotCurveOptions);
         return _completed;
      }
   }

   public class When_mapping_a_chart_curve_to_snapshot : concern_for_CurveMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_curve);
      }

      [Observation]
      public void should_have_saved_all_curve_properties_in_the_snapshot()
      {
         _snapshot.CurveOptions.ShouldBeEqualTo(_snapshotCurveOptions);
         _snapshot.Name.ShouldBeEqualTo(_curve.Name);
         _snapshot.X.ShouldBeEqualTo(_curve.xData.PathAsString);
         _snapshot.Y.ShouldBeEqualTo(_curve.yData.PathAsString);
      }
   }

   public class When_mapping_a_chart_curve_with_undefined_x_or_y_data_to_snapshot : concern_for_CurveMapper
   {
      protected override async Task Context()
      {
         await base.Context();
         _curve.SetxData(null, _dimensionFactory);
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_curve);
      }

      [Observation]
      public void should_return_a_snapshot_with_undefined_x_or_y()
      {
         _snapshot.X.ShouldBeNull();
      }
   }

   public class When_mapping_a_snapshot_curve_to_curve_using_base_grid_as_x_axis : concern_for_CurveMapper
   {
      private SimulationAnalysisContext _context;
      private Curve _newCurve;
      private CurveOptions _newModelCurveOptions;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_curve);
         _context = new SimulationAnalysisContext(new[] {_dataRepository});
         _newModelCurveOptions = new CurveOptions {Color = Color.Aqua};
         A.CallTo(() => _curveOptionsMapper.MapToModel(_snapshot.CurveOptions)).Returns(_newModelCurveOptions);
      }

      protected override async Task Because()
      {
         _newCurve = await sut.MapToModel(_snapshot, _context);
      }

      [Observation]
      public void should_return_a_curve_having_the_expected_properties()
      {
         _newCurve.Name.ShouldBeEqualTo(_snapshot.Name);
         _newCurve.xData.ShouldBeEqualTo(_xColumn);
         _newCurve.yData.ShouldBeEqualTo(_yColumn);
      }

      [Observation]
      public void should_update_curve_option_properties()
      {
         _newCurve.Color.ShouldBeEqualTo(_newModelCurveOptions.Color);
      }
   }

   public class When_mapping_a_snapshot_curve_to_curve_using_another_column_as_x_axis : concern_for_CurveMapper
   {
      private SimulationAnalysisContext _context;
      private Curve _newCurve;
      private DataRepository _anotherRepository;
      private DataColumn _y2Column;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_curve);
         _anotherRepository = DomainHelperForSpecs.ObservedData();
         _y2Column = _anotherRepository.AllButBaseGrid().First();
         _y2Column.QuantityInfo.Path = new[] {"D", "E", "F"};
         _context = new SimulationAnalysisContext(new[] {_dataRepository, _anotherRepository,});
         _snapshot.CurveOptions.Color = Color.Aqua;
         _snapshot.X = _y2Column.PathAsString;
      }

      protected override async Task Because()
      {
         _newCurve = await sut.MapToModel(_snapshot, _context);
      }

      [Observation]
      public void should_map_the_expected_column_for_x_data()
      {
         _newCurve.xData.ShouldBeEqualTo(_y2Column);
         _newCurve.yData.ShouldBeEqualTo(_yColumn);
      }
   }
}