using System;
using System.Linq;
using System.Threading.Tasks;
using ModelPopulationAnalysisChart = PKSim.Core.Model.PopulationAnalyses.PopulationAnalysisChart;
using SnapshotPopulationAnalysisChart = PKSim.Core.Snapshots.PopulationAnalysisChart;

namespace PKSim.Core.Snapshots.Mappers
{
   public class PopulationAnalysisChartMapper : ObjectBaseSnapshotMapperBase<ModelPopulationAnalysisChart, SnapshotPopulationAnalysisChart>
   {
      private readonly ChartMapper _chartMapper;
      private readonly PopulationAnalysisMapper _populationAnalysisMapper;

      public PopulationAnalysisChartMapper(ChartMapper chartMapper, PopulationAnalysisMapper populationAnalysisMapper)
      {
         _chartMapper = chartMapper;
         _populationAnalysisMapper = populationAnalysisMapper;
      }

      public override async Task<SnapshotPopulationAnalysisChart> MapToSnapshot(ModelPopulationAnalysisChart populationAnalysisChart)
      {
         var snapshot = await SnapshotFrom(populationAnalysisChart, x =>
         {
            x.Type = populationAnalysisChart.AnalysisType;
            x.XAxisSettings = populationAnalysisChart.PrimaryXAxisSettings;
            x.YAxisSettings = populationAnalysisChart.PrimaryYAxisSettings;
            x.SecondaryYAxisSettings = populationAnalysisChart.SecondaryYAxisSettings.ToArray();
         });

         await _chartMapper.MapToSnapshot(populationAnalysisChart, snapshot);
         snapshot.Analysis = await _populationAnalysisMapper.MapToSnapshot(populationAnalysisChart.BasePopulationAnalysis);

         return snapshot;
      }

      public override Task<ModelPopulationAnalysisChart> MapToModel(SnapshotPopulationAnalysisChart snapshot)
      {
         throw new NotImplementedException();
      }
   }
}