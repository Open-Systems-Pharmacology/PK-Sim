using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public class PopulationStatisticalAnalysis : PopulationPivotAnalysis
   {
      /// <summary>
      /// Unit in which the statistical analysis will be displayed
      /// </summary>
      public Unit TimeUnit { get; set; }
      private readonly List<StatisticalAggregation> _statistics;

      public PopulationStatisticalAnalysis()
      {
         _statistics = new List<StatisticalAggregation>();
      }

      public IReadOnlyList<StatisticalAggregation> Statistics => _statistics;

      public void AddStatistic(StatisticalAggregation statisticalAggregation)
      {
         _statistics.Add(statisticalAggregation);
      }

      public IEnumerable<StatisticalAggregation> SelectedStatistics
      {
         get { return Statistics.Where(x => x.Selected); }
      }

      /// <summary>
      ///    Adds the predefined statistics that are not part of the analysis yet (e.g. statistics introduced after the analysis
      ///    was saved). Existing statistics and their selection are preserved. Missing statistics are inserted following the
      ///    order of <see cref="StatisticalAggregationType" />
      /// </summary>
      public void AddMissingPredefinedStatistics()
      {
         var existingMethods = predefinedStatistics.Select(x => x.Method).ToList();
         EnumHelper.AllValuesFor<StatisticalAggregationType>()
            .Where(method => !existingMethods.Contains(method))
            .Each(insertPredefinedStatistic);
      }

      private IEnumerable<PredefinedStatisticalAggregation> predefinedStatistics => _statistics.OfType<PredefinedStatisticalAggregation>();

      private void insertPredefinedStatistic(StatisticalAggregationType method) =>
         _statistics.Insert(insertIndexFor(method), new PredefinedStatisticalAggregation {Method = method});

      /// <summary>
      ///    Index before the first predefined statistic following <paramref name="method" />, or right after the last predefined
      ///    statistic if there is none (keeps the percentile statistics at the end)
      /// </summary>
      private int insertIndexFor(StatisticalAggregationType method)
      {
         var nextStatistic = predefinedStatistics.FirstOrDefault(x => x.Method > method);
         if (nextStatistic != null)
            return _statistics.IndexOf(nextStatistic);

         var lastStatistic = predefinedStatistics.LastOrDefault();
         return lastStatistic != null ? _statistics.IndexOf(lastStatistic) + 1 : _statistics.Count;
      }

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);
         var statisticalAnalysis = source as PopulationStatisticalAnalysis;
         if (statisticalAnalysis == null) return;
         statisticalAnalysis.Statistics.Each(s => AddStatistic(cloneManager.Clone(s)));
         TimeUnit = statisticalAnalysis.TimeUnit;
      }
   }
}