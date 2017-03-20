using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationAnalysisStatisticsSelectionPresenter : ContextSpecification<IPopulationAnalysisStatisticsSelectionPresenter>
   {
      private IRepresentationInfoRepository _representationInfoRepository;
      protected IPopulationAnalysisStatisticsSelectionView _view;

      protected override void Context()
      {
         _representationInfoRepository= A.Fake<IRepresentationInfoRepository>();
         _view= A.Fake<IPopulationAnalysisStatisticsSelectionView>();
         sut = new PopulationAnalysisStatisticsSelectionPresenter(_view,_representationInfoRepository);
      }
   }

   public class When_editing_the_statistics_defined_in_a_population_statistic_analysis : concern_for_PopulationAnalysisStatisticsSelectionPresenter
   {
      private PopulationStatisticalAnalysis _statisticalAnalysis;
      private StatisticalAggregation _statistic;

      protected override void Context()
      {
         base.Context();
         _statistic=new PercentileStatisticalAggregation();
         _statisticalAnalysis=new PopulationStatisticalAnalysis();
         _statisticalAnalysis.AddStatistic(_statistic);
      }

      protected override void Because()
      {
         sut.StartAnalysis(A.Fake<IPopulationDataCollector>(),_statisticalAnalysis);
      }

      [Observation]
      public void should_select_the_first_availalbe_statistic_if_none_was_selected()
      {
         _statistic.Selected.ShouldBeTrue();
      }

      [Observation]
      public void should_bind_the_available_statistics_to_the_view()
      {
         A.CallTo(() => _view.BindTo(_statisticalAnalysis.Statistics)).MustHaveHappened();
      }
   }
}	