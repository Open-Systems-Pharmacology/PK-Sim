using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface IPopulationAnalysisOutputSelectionView : IView<IPopulationAnalysisOutputSelectionPresenter>
   {
      void AddPopulationOutputsView(IView view);
      void AddSelectedOutputsView(IView view);
      void AddStatisticsSelectionView(IView view);
      void BindTo(PopulationStatisticalAnalysis populationAnalysis);
   }
}