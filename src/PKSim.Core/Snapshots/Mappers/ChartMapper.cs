using System.Threading.Tasks;
using OSPSuite.Core.Chart;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ChartSnapshotContext : SnapshotContext
   {
      public IChart Chart { get; }

      public ChartSnapshotContext(IChart chart, SnapshotContext baseContext) : base(baseContext)
      {
         Chart = chart;
      }
   }

   public class ChartMapper : ObjectBaseSnapshotMapperBase<IChart, Chart, ChartSnapshotContext, Chart>
   {
      public override Task<Chart> MapToSnapshot(IChart chart, Chart snapshot)
      {
         MapModelPropertiesToSnapshot(chart, snapshot);
         snapshot.Settings = chart.ChartSettings;
         snapshot.FontAndSize = chart.FontAndSize;
         snapshot.IncludeOriginData = SnapshotValueFor(chart.IncludeOriginData);
         snapshot.OriginText = SnapshotValueFor(chart.OriginText);
         snapshot.PreviewSettings = SnapshotValueFor(chart.PreviewSettings);
         snapshot.Title = SnapshotValueFor(chart.Title);
         return Task.FromResult(snapshot);
      }

      public override Task<IChart> MapToModel(Chart snapshot, ChartSnapshotContext snapshotContext)
      {
         var chart = snapshotContext.Chart;
         MapSnapshotPropertiesToModel(snapshot, chart);
         chart.ChartSettings.UpdatePropertiesFrom(snapshot.Settings);
         chart.FontAndSize.UpdatePropertiesFrom(snapshot.FontAndSize);
         chart.IncludeOriginData = snapshot.IncludeOriginData.GetValueOrDefault(chart.IncludeOriginData);
         chart.OriginText = snapshot.OriginText;
         chart.PreviewSettings = snapshot.PreviewSettings.GetValueOrDefault(chart.PreviewSettings);
         chart.Title = ModelValueFor(snapshot.Title);
         return Task.FromResult(chart);
      }
   }
}