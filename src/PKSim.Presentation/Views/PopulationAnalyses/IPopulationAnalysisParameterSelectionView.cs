using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface IPopulationAnalysisParameterSelectionView : IView<IPopulationAnalysisParameterSelectionPresenter>
   {
      void AddPopulationParametersView(IView view);
      void AddSelectedParametersView(IView view);
      void AddDistributionView(IView view);
   }
}