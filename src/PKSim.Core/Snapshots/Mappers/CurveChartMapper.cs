using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Chart;
using ModelCurveChart = OSPSuite.Core.Chart.CurveChart;
using SnapshotCurveChart = PKSim.Core.Snapshots.CurveChart;
using ModelDataRepository = OSPSuite.Core.Domain.Data.DataRepository;

namespace PKSim.Core.Snapshots.Mappers
{
   public class CurveChartContext
   {
      private readonly List<ModelDataRepository> _dataRepositories = new List<ModelDataRepository>();

      public IReadOnlyList<ModelDataRepository> DataRepositories => _dataRepositories;

      public CurveChartContext()
      {
      }

      public CurveChartContext(IEnumerable<ModelDataRepository> dataRepositories)
      {
         _dataRepositories.AddRange(dataRepositories);
      }

      public void AddDataRepository(ModelDataRepository dataRepository)
      {
         _dataRepositories.Add(dataRepository);
      }
   }

   public abstract class CurveChartMapper<TCurveChart> : ObjectBaseSnapshotMapperBase<TCurveChart, SnapshotCurveChart, CurveChartContext> where TCurveChart: ModelCurveChart, new() 
   {
      private readonly AxisMapper _axisMapper;
      private readonly CurveMapper _curveMapper;

      protected CurveChartMapper(AxisMapper axisMapper, CurveMapper curveMapper)
      {
         _axisMapper = axisMapper;
         _curveMapper = curveMapper;
      }

      public override async Task<SnapshotCurveChart> MapToSnapshot(TCurveChart curveChart)
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

      public override async Task<TCurveChart> MapToModel(SnapshotCurveChart snapshot, CurveChartContext curveChartContext)
      {
         var curveChart = new TCurveChart();
         MapSnapshotPropertiesToModel(snapshot, curveChart);
         curveChart.Title = snapshot.Title;
         curveChart.OriginText = snapshot.OriginText;
         curveChart.IncludeOriginData = snapshot.IncludeOriginData;
         curveChart.PreviewSettings = snapshot.PreviewSettings;
         curveChart.ChartSettings.UpdatePropertiesFrom(snapshot.Settings);
         curveChart.FontAndSize.UpdatePropertiesFrom(snapshot.FontAndSize);
         await updateChartAxes(curveChart, snapshot.Axes);
         await updateChartCurves(curveChart, snapshot.Curves, curveChartContext);
         return curveChart;
      }

      private async Task updateChartAxes(ModelCurveChart curveChart, IReadOnlyList<Axis> snapshotAxes)
      {
         var tasks = snapshotAxes.Select(x => _axisMapper.MapToModel(x));
         var axes = await Task.WhenAll(tasks);
         axes.Each(curveChart.AddAxis);
      }

      private async Task updateChartCurves(ModelCurveChart curveChart, IReadOnlyList<Curve> snapshotCurves, CurveChartContext curveChartContext)
      {
         var tasks = snapshotCurves.Select(x => _curveMapper.MapToModel(x, curveChartContext));
         var curves = await Task.WhenAll(tasks);
         curves.Each(x => curveChart.AddCurve(x, useAxisDefault: false));
      }
   }

   public class SimulationTimeProfileChartMapper : CurveChartMapper<SimulationTimeProfileChart>
   {
      public SimulationTimeProfileChartMapper(AxisMapper axisMapper, CurveMapper curveMapper) : base(axisMapper, curveMapper)
      {
      }
   }
}