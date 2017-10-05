using System.Threading.Tasks;
using ModelCurveOptions = OSPSuite.Core.Chart.CurveOptions;
using SnapshotCurveOptions = PKSim.Core.Snapshots.CurveOptions;

namespace PKSim.Core.Snapshots.Mappers
{
   public class CurveOptionsMapper : SnapshotMapperBase<ModelCurveOptions, SnapshotCurveOptions>
   {
      private readonly ModelCurveOptions _defaultCurveOption;

      public CurveOptionsMapper()
      {
         _defaultCurveOption = new ModelCurveOptions();
      }

      public override Task<SnapshotCurveOptions> MapToSnapshot(ModelCurveOptions curveOptions)
      {
         return SnapshotFrom(curveOptions, x =>
         {
            x.Visible = SnapshotValueFor(curveOptions.Visible, _defaultCurveOption.Visible);
            x.ShouldShowLLOQ = SnapshotValueFor(curveOptions.ShouldShowLLOQ, _defaultCurveOption.ShouldShowLLOQ);
            x.VisibleInLegend = SnapshotValueFor(curveOptions.VisibleInLegend, _defaultCurveOption.VisibleInLegend);
            x.InterpolationMode = SnapshotValueFor(curveOptions.InterpolationMode, _defaultCurveOption.InterpolationMode);
            x.Color = SnapshotValueFor(curveOptions.Color, _defaultCurveOption.Color);
            x.LegendIndex = curveOptions.LegendIndex;
            x.LineStyle = SnapshotValueFor(curveOptions.LineStyle, _defaultCurveOption.LineStyle);
            x.Symbol = SnapshotValueFor(curveOptions.Symbol, _defaultCurveOption.Symbol);
            x.yAxisType = SnapshotValueFor(curveOptions.yAxisType, _defaultCurveOption.yAxisType);
         });
      }

      public override Task<ModelCurveOptions> MapToModel(SnapshotCurveOptions snapshot)
      {
         return  Task.FromResult(new ModelCurveOptions
         {
            Visible = ModelValueFor(snapshot.Visible, _defaultCurveOption.Visible),
            ShouldShowLLOQ = ModelValueFor(snapshot.ShouldShowLLOQ, _defaultCurveOption.ShouldShowLLOQ),
            VisibleInLegend = ModelValueFor(snapshot.VisibleInLegend, _defaultCurveOption.VisibleInLegend),
            InterpolationMode = ModelValueFor(snapshot.InterpolationMode, _defaultCurveOption.InterpolationMode),
            Color = ModelValueFor(snapshot.Color, _defaultCurveOption.Color),
            LegendIndex = snapshot.LegendIndex,
            LineStyle = ModelValueFor(snapshot.LineStyle, _defaultCurveOption.LineStyle),
            Symbol = ModelValueFor(snapshot.Symbol, _defaultCurveOption.Symbol),
            yAxisType = ModelValueFor(snapshot.yAxisType, _defaultCurveOption.yAxisType),
         });
      }
   }
}