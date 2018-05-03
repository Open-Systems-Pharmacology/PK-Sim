using System.Threading.Tasks;
using PKSim.Core.Repositories;
using ModelAxis = OSPSuite.Core.Chart.Axis;
using SnashotAxis = PKSim.Core.Snapshots.Axis;

namespace PKSim.Core.Snapshots.Mappers
{
   public class AxisMapper : SnapshotMapperBase<ModelAxis, SnashotAxis>
   {
      private readonly IDimensionRepository _dimensionRepository;

      public AxisMapper(IDimensionRepository dimensionRepository)
      {
         _dimensionRepository = dimensionRepository;
      }

      public override Task<SnashotAxis> MapToSnapshot(ModelAxis axis)
      {
         return SnapshotFrom(axis, x =>
         {
            x.Dimension = axis.Dimension?.Name;
            x.Unit = ModelValueFor(axis.UnitName);
            x.Caption = SnapshotValueFor(axis.Caption);
            x.Type = axis.AxisType;
            x.GridLines = axis.GridLines;
            x.Visible = axis.Visible;
            x.Scaling = axis.Scaling;
            x.NumberMode = axis.NumberMode;
            x.DefaultColor = axis.DefaultColor;
            x.DefaultLineStyle = axis.DefaultLineStyle;
            x.Min = axis.Min;
            x.Max = axis.Max;
         });
      }

      public override Task<ModelAxis> MapToModel(SnashotAxis snapshot)
      {
         var axis = new ModelAxis(snapshot.Type)
         {
            Dimension = _dimensionRepository.DimensionByName(snapshot.Dimension),
            Caption = snapshot.Caption,
            GridLines = snapshot.GridLines,
            Visible = snapshot.Visible,
            DefaultColor = snapshot.DefaultColor,
            DefaultLineStyle = snapshot.DefaultLineStyle,
            Scaling = snapshot.Scaling,
            NumberMode = snapshot.NumberMode
         };

         axis.Dimension = _dimensionRepository.OptimalDimensionFor(axis.Dimension);
         axis.UnitName = ModelValueFor(snapshot.Unit);
         axis.Min = snapshot.Min;
         axis.Max = snapshot.Max;

         return Task.FromResult(axis);
      }
   }
}