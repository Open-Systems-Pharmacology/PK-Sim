using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PKSim.Core.Model.PopulationAnalyses;
using ModelPopulationAnalysis = PKSim.Core.Model.PopulationAnalyses.PopulationAnalysis;
using SnapshotPopulationAnalysis = PKSim.Core.Snapshots.PopulationAnalysis;

namespace PKSim.Core.Snapshots.Mappers
{
   public class PopulationAnalysisMapper : SnapshotMapperBase<ModelPopulationAnalysis, SnapshotPopulationAnalysis>
   {
      private readonly PopulationAnalysisFieldMapper _populationAnalysisFieldMapper;

      public PopulationAnalysisMapper(PopulationAnalysisFieldMapper populationAnalysisFieldMapper)
      {
         _populationAnalysisFieldMapper = populationAnalysisFieldMapper;
      }

      public override async Task<SnapshotPopulationAnalysis> MapToSnapshot(ModelPopulationAnalysis populationAnalysis)
      {
         var snapshot = await SnapshotFrom(populationAnalysis);
         snapshot.Fields = await _populationAnalysisFieldMapper.MapToSnapshots(populationAnalysis.AllFields, populationAnalysis);
         mapStatisticalAnalysisProperties(snapshot, populationAnalysis as PopulationStatisticalAnalysis);
         mapBowWiskerAnalysisProperties(snapshot, populationAnalysis as PopulationBoxWhiskerAnalysis);
         return snapshot;
      }

      private void mapBowWiskerAnalysisProperties(SnapshotPopulationAnalysis snapshot, PopulationBoxWhiskerAnalysis populationAnalysis)
      {
         if (populationAnalysis == null)
            return;

         snapshot.ShowOutliers = populationAnalysis.ShowOutliers;
      }

      private void mapStatisticalAnalysisProperties(SnapshotPopulationAnalysis snapshot, PopulationStatisticalAnalysis populationAnalysis)
      {
         if (populationAnalysis == null)
            return;

         snapshot.Statistics = snapshotStatisticFrom(populationAnalysis.SelectedStatistics);
      }

      private StatisticalAggregation[] snapshotStatisticFrom(IEnumerable<Model.PopulationAnalyses.StatisticalAggregation> selectedStatistics)
      {
         return selectedStatistics.Select(x => new StatisticalAggregation
         {
            Id = x.Id,
            LineStyle = x.LineStyle
         }).ToArray();
      }

      public override Task<ModelPopulationAnalysis> MapToModel(SnapshotPopulationAnalysis snapshot)
      {
         throw new NotImplementedException();
      }
   }
}