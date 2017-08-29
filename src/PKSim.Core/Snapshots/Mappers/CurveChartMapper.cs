using System.Collections.Generic;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Chart;
using ModelCurveChart = OSPSuite.Core.Chart.CurveChart;
using SnapshotCurveChart = PKSim.Core.Snapshots.CurveChart;

namespace PKSim.Core.Snapshots.Mappers
{
   public abstract class CurveChartMapper<TCurveChart> : ObjectBaseSnapshotMapperBase<TCurveChart, SnapshotCurveChart, SimulationAnalysisContext> where TCurveChart : ModelCurveChart, new()
   {
      private readonly ChartMapper _chartMapper;
      private readonly AxisMapper _axisMapper;
      private readonly CurveMapper _curveMapper;
      private readonly IIdGenerator _idGenerator;

      protected CurveChartMapper(ChartMapper chartMapper, AxisMapper axisMapper, CurveMapper curveMapper, IIdGenerator idGenerator)
      {
         _chartMapper = chartMapper;
         _axisMapper = axisMapper;
         _curveMapper = curveMapper;
         _idGenerator = idGenerator;
      }

      public override async Task<SnapshotCurveChart> MapToSnapshot(TCurveChart curveChart)
      {
         var snapshot = await SnapshotFrom(curveChart);
         await _chartMapper.MapToSnapshot(curveChart, snapshot);
         snapshot.Axes = await _axisMapper.MapToSnapshots(curveChart.Axes);
         snapshot.Curves = await _curveMapper.MapToSnapshots(curveChart.Curves);
         return snapshot;
      }

      public override async Task<TCurveChart> MapToModel(SnapshotCurveChart snapshot, SimulationAnalysisContext simulationAnalysisContext)
      {
         var curveChart = new TCurveChart().WithId(_idGenerator.NewId());
         MapSnapshotPropertiesToModel(snapshot, curveChart);
         await _chartMapper.MapToModel(snapshot, curveChart);
         await updateChartAxes(curveChart, snapshot.Axes);
         await updateChartCurves(curveChart, snapshot.Curves, simulationAnalysisContext);
         return curveChart;
      }

      private async Task updateChartAxes(ModelCurveChart curveChart, IReadOnlyList<Axis> snapshotAxes)
      {
         var axes = await _axisMapper.MapToModels(snapshotAxes);
         axes?.Each(curveChart.AddAxis);
      }

      private async Task updateChartCurves(ModelCurveChart curveChart, IReadOnlyList<Curve> snapshotCurves, SimulationAnalysisContext simulationAnalysisContext)
      {
         var curves = await _curveMapper.MapToModels(snapshotCurves, simulationAnalysisContext);
         curves?.Each(x => curveChart.AddCurve(x, useAxisDefault: false));
      }
   }

   public class SimulationTimeProfileChartMapper : CurveChartMapper<SimulationTimeProfileChart>
   {
      public SimulationTimeProfileChartMapper(ChartMapper chartMapper, AxisMapper axisMapper, CurveMapper curveMapper, IIdGenerator idGenerator) : base(chartMapper, axisMapper, curveMapper, idGenerator)
      {
      }
   }

   public class IndividualSimulationComparisonMapper : CurveChartMapper<IndividualSimulationComparison>
   {
      public IndividualSimulationComparisonMapper(ChartMapper chartMapper, AxisMapper axisMapper, CurveMapper curveMapper, IIdGenerator idGenerator) : base(chartMapper, axisMapper, curveMapper, idGenerator)
      {
      }
   }
}