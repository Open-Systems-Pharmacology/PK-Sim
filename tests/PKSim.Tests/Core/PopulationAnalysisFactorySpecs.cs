using System.Linq;
using OSPSuite.Utility;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationAnalysisFactory : ContextSpecification<IPopulationAnalysisFactory>
   {
      private IDimensionRepository _dimensionRepository;
      protected Unit _defaultUnit;
      private IDisplayUnitRetriever _displayUnitRetriever;

      protected override void Context()
      {
         _dimensionRepository= A.Fake<IDimensionRepository>();
         _displayUnitRetriever= A.Fake<IDisplayUnitRetriever>();
         sut = new PopulationAnalysisFactory(_dimensionRepository,_displayUnitRetriever);

         var timeDimension= A.Fake<IDimension>();
         _defaultUnit = A.Fake<Unit>();
         A.CallTo(() => _displayUnitRetriever.PreferredUnitFor(timeDimension)).Returns(_defaultUnit);
         A.CallTo(() => _dimensionRepository.Time).Returns(timeDimension);

      }
   }

   public class When_creating_a_pivot_analysis : concern_for_PopulationAnalysisFactory
   {
      [Observation]
      public void should_return_a_valid_pivot_analysis()
      {
         sut.Create<PopulationPivotAnalysis>().ShouldNotBeNull();
      }
   }

   public class When_creating_a_box_whisker_analysis : concern_for_PopulationAnalysisFactory
   {
      [Observation]
      public void should_return_a_valid_box_whisker_analysis()
      {
         sut.Create<PopulationBoxWhiskerAnalysis>().ShouldNotBeNull();
      }
   }

   public class When_creating_a_statistical_analysis : concern_for_PopulationAnalysisFactory
   {
      private PopulationStatisticalAnalysis _statAnalysis;

      protected override void Because()
      {
         _statAnalysis = sut.Create<PopulationStatisticalAnalysis>();
      }

      [Observation]
      public void should_set_the_default_time_unit()
      {
         _statAnalysis.TimeUnit.ShouldBeEqualTo(_defaultUnit);
      }

      [Observation]
      public void should_return_a_valid_statistic_analysis()
      {
         _statAnalysis.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_added_the_default_percentiles()
      {
         var allPercentiles = _statAnalysis.Statistics.OfType<PercentileStatisticalAggregation>().Select(x => x.Percentile).ToList();
         allPercentiles.ShouldOnlyContain(CoreConstants.DEFAULT_STATISTIC_PERCENTILES);
      }


      [Observation]
      public void should_have_added_the_predefined_curves()
      {
         var allPredefined = _statAnalysis.Statistics.OfType<PredefinedStatisticalAggregation>();
         allPredefined.Count().ShouldBeEqualTo(EnumHelper.AllValuesFor<StatisticalAggregationType>().Count());
      }
   }
}	