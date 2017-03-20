using System.Collections.Generic;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface IPopulationAnalysisStatisticsSelectionView : IView<IPopulationAnalysisStatisticsSelectionPresenter>
   {
      void BindTo(IEnumerable<StatisticalAggregation> selection);
   }
}