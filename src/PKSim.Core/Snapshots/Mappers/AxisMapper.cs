using System;
using System.Threading.Tasks;
using ModelAxis = OSPSuite.Core.Chart.Axis;
using SnashotAxis = PKSim.Core.Snapshots.Axis;

namespace PKSim.Core.Snapshots.Mappers
{
   public class AxisMapper : SnapshotMapperBase<ModelAxis, SnashotAxis>
   {
      public override Task<SnashotAxis> MapToSnapshot(ModelAxis axis)
      {
         return SnapshotFrom(axis, x =>
         {
            x.Unit = UnitValueFor(axis.UnitName);
            x.Dimension = axis.Dimension.Name;
            x.Caption = SnapshotValueFor(axis.Caption);
            x.Type = axis.AxisType;
            x.GridLines = axis.GridLines;
            x.Visible = axis.Visible;
            x.DefaultColor = axis.DefaultColor;
            x.DefaultLineStyle = axis.DefaultLineStyle;
            x.Min = displayValueFor(axis, axis.Min);
            x.Max = displayValueFor(axis, axis.Max);
         });
      }

      private float? displayValueFor(ModelAxis axis, float? axisValue)
      {
         if (axisValue == null)
            return null;

         return (float) axis.Dimension.BaseUnitValueToUnitValue(axis.Unit, axisValue.Value);
      }

      public override Task<ModelAxis> MapToModel(SnashotAxis snapshot)
      {
         throw new NotImplementedException();
      }
   }
}