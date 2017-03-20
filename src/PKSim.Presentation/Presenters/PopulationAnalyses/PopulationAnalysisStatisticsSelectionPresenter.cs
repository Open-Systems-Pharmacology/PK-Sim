using System;
using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IPopulationAnalysisStatisticsSelectionPresenter : IPresenter<IPopulationAnalysisStatisticsSelectionView>, IPopulationAnalysisPresenter
   {
      string DisplayNameFor(StatisticalAggregation statisticalAggregation);
      event EventHandler SelectionChanged;
   }

   public class PopulationAnalysisStatisticsSelectionPresenter : AbstractSubPresenter<IPopulationAnalysisStatisticsSelectionView, IPopulationAnalysisStatisticsSelectionPresenter>, IPopulationAnalysisStatisticsSelectionPresenter
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private PopulationStatisticalAnalysis _statisicalAnalysis;
      public event EventHandler SelectionChanged = delegate { };

      public PopulationAnalysisStatisticsSelectionPresenter(IPopulationAnalysisStatisticsSelectionView view, IRepresentationInfoRepository representationInfoRepository) : base(view)
      {
         _representationInfoRepository = representationInfoRepository;
      }

      public void StartAnalysis(IPopulationDataCollector populationDataCollector, PopulationAnalysis populationAnalysis)
      {
         _statisicalAnalysis = populationAnalysis.DowncastTo<PopulationStatisticalAnalysis>();
         addDefaultCurves();
         _view.BindTo(_statisicalAnalysis.Statistics);
      }

      public string DisplayNameFor(StatisticalAggregation statisticalAggregation)
      {
         return _representationInfoRepository.DisplayNameFor(statisticalAggregation);
      }

      public override void ViewChanged()
      {
         base.ViewChanged();
         SelectionChanged(this, EventArgs.Empty);
      }

      private void addDefaultCurves()
      {
         var allSelected = _statisicalAnalysis.Statistics.Where(x => x.Selected);
         if (allSelected.Any()) return;

         var first = _statisicalAnalysis.Statistics.FirstOrDefault();
         if (first == null) return;
         first.Selected = true;
      }
   }
}