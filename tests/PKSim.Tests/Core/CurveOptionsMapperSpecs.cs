using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Chart;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_CurveOptionsMapper : ContextSpecificationAsync<CurveOptionsMapper>
   {
      protected CurveOptions _curveOption;
      protected Snapshots.CurveOptions _snapshot;

      protected override Task Context()
      {
         sut = new CurveOptionsMapper();
         _curveOption = new CurveOptions
         {
            LegendIndex = 4,
            LineThickness = 5,
            LineStyle = LineStyles.DashDot,
            VisibleInLegend = false
         };
         return _completed;
      }
   }

   public class When_mapping_a_curve_option_to_snapshot : concern_for_CurveOptionsMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_curveOption);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_values()
      {
         _snapshot.LineThickness.ShouldBeEqualTo(5);
         _snapshot.LegendIndex.ShouldBeEqualTo(4);
         _snapshot.LineStyle.ShouldBeEqualTo(LineStyles.DashDot);
         _snapshot.VisibleInLegend.ShouldBeEqualTo(false);
      }

      [Observation]
      public void should_set_all_default_properties_to_null()
      {
         _snapshot.Color.ShouldBeNull();
         _snapshot.InterpolationMode.ShouldBeNull();
         _snapshot.Symbol.ShouldBeNull();
         _snapshot.Visible.ShouldBeNull();
         _snapshot.ShouldShowLLOQ.ShouldBeNull();
      }
   }

   public class When_mapping_a_snapshot_curve_option_to_model : concern_for_CurveOptionsMapper
   {
      private CurveOptions _newCurveOptions;
      private CurveOptions _defaultCurveOptions;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_curveOption);
         _newCurveOptions = await sut.MapToModel(_snapshot, new SnapshotContext());
         _defaultCurveOptions  = new CurveOptions();
      }

      [Observation]
      public void should_map_the_snapshot_properties_to_the_model()
      {
         _newCurveOptions.LineThickness.ShouldBeEqualTo(5);
         _newCurveOptions.LegendIndex.ShouldBeEqualTo(4);
         _newCurveOptions.LineStyle.ShouldBeEqualTo(LineStyles.DashDot);
         _newCurveOptions.VisibleInLegend.ShouldBeEqualTo(false);
      }

      [Observation]
      public void should_map_other_properties_to_default_properties()
      {
         _newCurveOptions.Color.ShouldBeEqualTo(_defaultCurveOptions.Color);
         _newCurveOptions.Symbol.ShouldBeEqualTo(_defaultCurveOptions.Symbol);
      }
   }
}