using System.Threading.Tasks;
using OSPSuite.Core.Domain.UnitSystem;
using ModelAxis = OSPSuite.Core.Chart.Axis;
using SnashotAxis = PKSim.Core.Snapshots.Axis;

namespace PKSim.Core.Snapshots.Mappers
{
   public class AxisMapper : SnapshotMapperBase<ModelAxis, SnashotAxis>
   {
      private readonly IDimensionFactory _dimensionFactory;

      public AxisMapper(IDimensionFactory dimensionFactory)
      {
         _dimensionFactory = dimensionFactory;
      }

      public override Task<SnashotAxis> MapToSnapshot(ModelAxis axis)
      {
         return SnapshotFrom(axis, x =>
         {
            x.Dimension = axis.Dimension.Name;
            x.Unit = ModelValueFor(axis.UnitName);
            x.Caption = SnapshotValueFor(axis.Caption);
            x.Type = axis.AxisType;
            x.GridLines = axis.GridLines;
            x.Visible = axis.Visible;
            x.Scaling = axis.Scaling;
            x.NumberMode = axis.NumberMode;
            x.DefaultColor = axis.DefaultColor;
            x.DefaultLineStyle = axis.DefaultLineStyle;
            x.Min = displayValueFor(axis, axis.Min);
            x.Max = displayValueFor(axis, axis.Max);
         });
      }

      private float? displayValueFor(ModelAxis axis, float? baseValue)
      {
         if (baseValue == null)
            return null;

         return (float) axis.Dimension.BaseUnitValueToUnitValue(axis.Unit, baseValue.Value);
      }

      private float? baseValueFor(ModelAxis axis, float? displayValue)
      {
         if (displayValue == null)
            return null;

         return (float) axis.Dimension.UnitValueToBaseUnitValue(axis.Unit, displayValue.Value);
      }

      public override Task<ModelAxis> MapToModel(SnashotAxis snapshot)
      {
         var axis = new ModelAxis(snapshot.Type)
         {
            Dimension = _dimensionFactory.Dimension(snapshot.Dimension),
            UnitName = ModelValueFor(snapshot.Unit),
            Caption = snapshot.Caption,
            GridLines = snapshot.GridLines,
            Visible = snapshot.Visible,
            DefaultColor = snapshot.DefaultColor,
            DefaultLineStyle = snapshot.DefaultLineStyle,
            Scaling = snapshot.Scaling,
            NumberMode = snapshot.NumberMode
         };

         axis.Dimension = _dimensionFactory.OptimalDimension(axis.Dimension);
         axis.Min = baseValueFor(axis, snapshot.Min);
         axis.Max = baseValueFor(axis, snapshot.Max);

         return Task.FromResult(axis);
      }
   }
}