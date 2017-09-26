using System.Drawing;
using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_AxisMapper : ContextSpecificationAsync<AxisMapper>
   {
      protected Axis _axis;
      protected Snapshots.Axis _snapshot;
      private IDimension _dimension;

      protected override Task Context()
      {
         sut = new AxisMapper();
         _dimension = DomainHelperForSpecs.TimeDimensionForSpecs();

         _axis = new Axis(AxisTypes.Y)
         {
            Caption = "Axis Caption",
            GridLines = true,
            Dimension = _dimension,
            UnitName = _dimension.Unit("h").Name,
            Visible = true,
            Min = 60, //min
            Max = 120,
            DefaultColor = Color.AntiqueWhite,
            DefaultLineStyle = LineStyles.Solid
         };
         return _completed;
      }
   }

   public class When_mapping_an_axis_to_snapshot : concern_for_AxisMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_axis);
      }

      [Observation]
      public void should_save_the_axis_properties_to_snapshot()
      {
         _snapshot.Caption.ShouldBeEqualTo(_axis.Caption);
         _snapshot.GridLines.ShouldBeEqualTo(_axis.GridLines);
         _snapshot.Dimension.ShouldBeEqualTo(_axis.Dimension.Name);
         _snapshot.Unit.ShouldBeEqualTo(_axis.UnitName);
         _snapshot.Visible.ShouldBeEqualTo(_axis.Visible);
         _snapshot.Min.ShouldBeEqualTo(1);
         _snapshot.Max.ShouldBeEqualTo(2);
         _snapshot.DefaultColor.ShouldBeEqualTo(_axis.DefaultColor);
         _snapshot.DefaultLineStyle.ShouldBeEqualTo(_axis.DefaultLineStyle);
      }
   }

   public class When_mapping_an_axis_without_min_or_max_to_snapshot : concern_for_AxisMapper
   {

      protected override async Task Context()
      {
         await base.Context();
         _axis.Max = null;
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_axis);
      }

      [Observation]
      public void should_not_set_any_min_or_max_into_the_snapshot()
      {
         _snapshot.Min.ShouldBeEqualTo(1);
         _snapshot.Max.ShouldBeNull();
      }
   }
}