using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model.PopulationAnalyses;
using ModelPopulationAnalysis = PKSim.Core.Model.PopulationAnalyses.PopulationAnalysis;
using SnapshotPopulationAnalysis = PKSim.Core.Snapshots.PopulationAnalysis;

namespace PKSim.Core.Snapshots.Mappers
{
   public class PopulationAnalysisMapper : SnapshotMapperBase<ModelPopulationAnalysis, SnapshotPopulationAnalysis, ModelPopulationAnalysis>
   {
      private readonly PopulationAnalysisFieldMapper _populationAnalysisFieldMapper;

      public PopulationAnalysisMapper(PopulationAnalysisFieldMapper populationAnalysisFieldMapper)
      {
         _populationAnalysisFieldMapper = populationAnalysisFieldMapper;
      }

      public override async Task<SnapshotPopulationAnalysis> MapToSnapshot(ModelPopulationAnalysis populationAnalysis)
      {
         var snapshot = await SnapshotFrom(populationAnalysis);
         snapshot.Fields = await _populationAnalysisFieldMapper.MapToSnapshots(populationAnalysis.AllFields);
         updateSnapshotFieldPositions(snapshot.Fields, populationAnalysis);
         mapIf<PopulationStatisticalAnalysis>(snapshot, populationAnalysis, mapStatisticalAnalysisToSnapshot);
         mapIf<PopulationBoxWhiskerAnalysis>(snapshot, populationAnalysis, mapBowWiskerAnalysisToSnapshot);
         return snapshot;
      }

      private void updateSnapshotFieldPositions(PopulationAnalysisField[] fields, ModelPopulationAnalysis populationAnalysis)
      {
         var pivotPopulatonAnalysis = populationAnalysis as PopulationPivotAnalysis;
         if (pivotPopulatonAnalysis == null)
            return;

         fields?.Each(x =>
         {
            var position = pivotPopulatonAnalysis.GetPosition(x.Name);
            x.Area = position.Area;
            x.Index = SnapshotValueFor(position.Index);
         });
      }

      private void mapBowWiskerAnalysisToSnapshot(SnapshotPopulationAnalysis snapshot, PopulationBoxWhiskerAnalysis populationAnalysis)
      {
         snapshot.ShowOutliers = populationAnalysis.ShowOutliers;
      }

      private void mapBowWiskerAnalysisToModel(SnapshotPopulationAnalysis snapshot, PopulationBoxWhiskerAnalysis populationAnalysis)
      {
         populationAnalysis.ShowOutliers = ModelValueFor(snapshot.ShowOutliers, populationAnalysis.ShowOutliers);
      }

      private void mapStatisticalAnalysisToSnapshot(SnapshotPopulationAnalysis snapshot, PopulationStatisticalAnalysis populationAnalysis)
      {
         snapshot.Statistics = snapshotStatisticFrom(populationAnalysis.SelectedStatistics);
      }

      private void mapStatisticalAnalysisToModel(SnapshotPopulationAnalysis snapshot, PopulationStatisticalAnalysis populationAnalysis)
      {
         snapshot.Statistics.Each(x =>
         {
            var statistic = populationAnalysis.Statistics.Find(s => string.Equals(s.Id, x.Id));
            if (statistic == null) return;
            statistic.Selected = true;
            statistic.LineStyle = x.LineStyle;
         });
      }

      private void mapIf<TPopulationAnalysis>(SnapshotPopulationAnalysis snapshot, ModelPopulationAnalysis modelPopulationAnalysis, Action<SnapshotPopulationAnalysis, TPopulationAnalysis> mapAction) where TPopulationAnalysis : ModelPopulationAnalysis
      {
         var populationAnalysis = modelPopulationAnalysis as TPopulationAnalysis;
         if (populationAnalysis == null)
            return;

         mapAction(snapshot, populationAnalysis);
      }

      private StatisticalAggregation[] snapshotStatisticFrom(IEnumerable<Model.PopulationAnalyses.StatisticalAggregation> selectedStatistics)
      {
         return selectedStatistics.Select(x => new StatisticalAggregation
         {
            Id = x.Id,
            LineStyle = x.LineStyle
         }).ToArray();
      }

      public override async Task<ModelPopulationAnalysis> MapToModel(SnapshotPopulationAnalysis snapshot, ModelPopulationAnalysis populationAnalysis)
      {
         var fields = await _populationAnalysisFieldMapper.MapToModels(snapshot.Fields);
         fields?.Each(populationAnalysis.Add);
         updateModelFieldPositions(populationAnalysis, snapshot.Fields);
         mapIf<PopulationBoxWhiskerAnalysis>(snapshot, populationAnalysis, mapBowWiskerAnalysisToModel);
         mapIf<PopulationStatisticalAnalysis>(snapshot, populationAnalysis, mapStatisticalAnalysisToModel);
         return populationAnalysis;
      }

      private void updateModelFieldPositions(ModelPopulationAnalysis populationAnalysis, PopulationAnalysisField[] fields)
      {
         var pivotPopulatonAnalysis = populationAnalysis as PopulationPivotAnalysis;
         if (pivotPopulatonAnalysis == null)
            return;

         fields?.Each(x => pivotPopulatonAnalysis.SetPosition(x.Name, x.Area, x.Index));
      }
   }
}