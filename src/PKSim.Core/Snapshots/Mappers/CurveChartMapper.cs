using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModelCurveChart = OSPSuite.Core.Chart.CurveChart;
using SnapshotCurveChart = PKSim.Core.Snapshots.CurveChart;

namespace PKSim.Core.Snapshots.Mappers
{
   public class CurveChartMapper : ObjectBaseSnapshotMapperBase<ModelCurveChart, SnapshotCurveChart>
   {
      private readonly AxisMapper _axisMapper;
      private readonly CurveMapper _curveMapper;

      public CurveChartMapper(AxisMapper axisMapper, CurveMapper curveMapper)
      {
         _axisMapper = axisMapper;
         _curveMapper = curveMapper;
      }

      public override async Task<SnapshotCurveChart> MapToSnapshot(ModelCurveChart curveChart)
      {
         var snapshot = await SnapshotFrom(curveChart, x =>
         {
            x.Title = SnapshotValueFor(curveChart.Title);
            x.OriginText = SnapshotValueFor(curveChart.OriginText);
            x.IncludeOriginData = curveChart.IncludeOriginData;
            x.PreviewSettings = curveChart.PreviewSettings;
            x.Settings = curveChart.ChartSettings;
            x.FontAndSize = curveChart.FontAndSize;
         });

         snapshot.Axes = await snapshotAxisFrom(curveChart.Axes);
         snapshot.Curves = await snapshotCurvesFrom(curveChart.Curves);
         return snapshot;
      }

      private Task<Curve[]> snapshotCurvesFrom(IReadOnlyCollection<OSPSuite.Core.Chart.Curve> curves)
      {
         return Task.WhenAll(curves.Select(_curveMapper.MapToSnapshot));
      }

      private Task<Axis[]> snapshotAxisFrom(IReadOnlyCollection<OSPSuite.Core.Chart.Axis> axis)
      {
         return Task.WhenAll(axis.Select(_axisMapper.MapToSnapshot));
      }

      public override Task<ModelCurveChart> MapToModel(SnapshotCurveChart snapshot)
      {
         throw new NotImplementedException();
      }
   }
}