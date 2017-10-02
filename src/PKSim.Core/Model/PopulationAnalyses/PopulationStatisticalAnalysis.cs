using System.Collections.Generic;
using System.Linq;
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