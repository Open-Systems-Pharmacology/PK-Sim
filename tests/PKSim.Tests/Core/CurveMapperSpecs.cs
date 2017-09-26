using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_CurveMapper : ContextSpecificationAsync<CurveMapper>
   {
      protected Curve _curve;
      protected Snapshots.Curve _snapshot;
      protected IDimensionFactory _dimensionFactory;
      private DataColumn _xColumn;
      private DataColumn _yColumn;

      protected override Task Context()
      {
         _dimensionFactory= A.Fake<IDimensionFactory>();
         var dataRepository = DomainHelperForSpecs.ObservedData();
         sut = new CurveMapper();

         _curve = new Curve
         {
            Name = "Hello",
         };

         _xColumn = dataRepository.BaseGrid;
         _yColumn = dataRepository.ObservationColumns().First();

         _curve.SetxData(_xColumn,_dimensionFactory);
         _curve.SetyData(_yColumn, _dimensionFactory);

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
         _snapshot.CurveOptions.ShouldBeEqualTo(_curve.CurveOptions);
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
}