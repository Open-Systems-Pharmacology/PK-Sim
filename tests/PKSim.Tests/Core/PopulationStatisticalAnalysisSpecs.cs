using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationStatisticalAnalysis : ContextSpecification<PopulationStatisticalAnalysis>
   {
      protected override void Context()
      {
         sut = new PopulationStatisticalAnalysis();
      }

      protected PredefinedStatisticalAggregation[] allPredefinedStatistics => sut.Statistics.OfType<PredefinedStatisticalAggregation>().ToArray();
   }

   public class When_adding_the_missing_predefined_statistics_to_an_analysis_saved_without_some_of_them : concern_for_PopulationStatisticalAnalysis
   {
      private PredefinedStatisticalAggregation _arithmeticMean;
      private PercentileStatisticalAggregation _percentile;

      protected override void Context()
      {
         base.Context();
         _arithmeticMean = new PredefinedStatisticalAggregation {Method = StatisticalAggregationType.ArithmeticMean, Selected = true};
         _percentile = new PercentileStatisticalAggregation {Percentile = 5, Selected = true};

         //analysis saved with a version that did not know about the 2 SD statistics
         sut.AddStatistic(_arithmeticMean);
         sut.AddStatistic(new PredefinedStatisticalAggregation {Method = StatisticalAggregationType.ArithmeticStandardDeviation});
         sut.AddStatistic(new PredefinedStatisticalAggregation {Method = StatisticalAggregationType.GeometricMean});
         sut.AddStatistic(new PredefinedStatisticalAggregation {Method = StatisticalAggregationType.GeometricStandardDeviation});
         sut.AddStatistic(new PredefinedStatisticalAggregation {Method = StatisticalAggregationType.Min});
         sut.AddStatistic(new PredefinedStatisticalAggregation {Method = StatisticalAggregationType.Max});
         sut.AddStatistic(new PredefinedStatisticalAggregation {Method = StatisticalAggregationType.Median});
         sut.AddStatistic(new PredefinedStatisticalAggregation {Method = StatisticalAggregationType.Range90});
         sut.AddStatistic(new PredefinedStatisticalAggregation {Method = StatisticalAggregationType.Range95});
         sut.AddStatistic(_percentile);
      }

      protected override void Because()
      {
         sut.AddMissingPredefinedStatistics();
      }

      [Observation]
      public void should_contain_each_predefined_statistic_exactly_once_in_the_predefined_order()
      {
         allPredefinedStatistics.Select(x => x.Method).ShouldOnlyContainInOrder(EnumHelper.AllValuesFor<StatisticalAggregationType>().ToArray());
      }

      [Observation]
      public void should_insert_the_missing_statistics_next_to_their_siblings()
      {
         var methods = allPredefinedStatistics.Select(x => x.Method).ToList();
         methods.IndexOf(StatisticalAggregationType.ArithmeticTwoStandardDeviations).ShouldBeEqualTo(methods.IndexOf(StatisticalAggregationType.ArithmeticStandardDeviation) + 1);
         methods.IndexOf(StatisticalAggregationType.GeometricTwoStandardDeviations).ShouldBeEqualTo(methods.IndexOf(StatisticalAggregationType.GeometricStandardDeviation) + 1);
      }

      [Observation]
      public void should_keep_the_percentile_statistics_after_the_predefined_ones()
      {
         sut.Statistics.Last().ShouldBeEqualTo(_percentile);
      }

      [Observation]
      public void should_preserve_the_existing_statistics_and_their_selection()
      {
         sut.Statistics.ShouldContain(_arithmeticMean);
         sut.SelectedStatistics.ShouldOnlyContain(_arithmeticMean, _percentile);
      }

      [Observation]
      public void should_not_select_the_added_statistics()
      {
         sut.Statistics.OfType<PredefinedStatisticalAggregation>()
            .Where(x => x.Method == StatisticalAggregationType.ArithmeticTwoStandardDeviations || x.Method == StatisticalAggregationType.GeometricTwoStandardDeviations)
            .Each(x => x.Selected.ShouldBeFalse());
      }
   }

   public class When_adding_the_missing_predefined_statistics_to_an_analysis_that_already_contains_all_of_them : concern_for_PopulationStatisticalAnalysis
   {
      private StatisticalAggregation[] _existingStatistics;

      protected override void Context()
      {
         base.Context();
         sut.AddMissingPredefinedStatistics();
         sut.AddStatistic(new PercentileStatisticalAggregation {Percentile = 50});
         _existingStatistics = sut.Statistics.ToArray();
      }

      protected override void Because()
      {
         sut.AddMissingPredefinedStatistics();
      }

      [Observation]
      public void should_not_change_the_statistics()
      {
         sut.Statistics.ShouldOnlyContainInOrder(_existingStatistics);
      }
   }

   public class When_adding_the_missing_predefined_statistics_to_an_empty_analysis : concern_for_PopulationStatisticalAnalysis
   {
      protected override void Because()
      {
         sut.AddMissingPredefinedStatistics();
      }

      [Observation]
      public void should_add_all_predefined_statistics_in_the_predefined_order()
      {
         allPredefinedStatistics.Select(x => x.Method).ShouldOnlyContainInOrder(EnumHelper.AllValuesFor<StatisticalAggregationType>().ToArray());
      }
   }
}
