using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface IPopulationAnalysisPKParameterSelectionView : IView<IPopulationAnalysisPKParameterSelectionPresenter>
   {
      void AddAllPKParametersView(IView view);
      void AddSelectedPKParametersView(IView view);
      void AddDistributionView(IView view);
   }
}