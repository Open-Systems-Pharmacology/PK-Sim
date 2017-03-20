using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface IPopulationAnalysisFieldSelectionView : IView<IPopulationAnalysisFieldSelectionPresenter>
   {
      void SetArrangementFieldView(IView view);
      void SetDataFieldView(IView view);
   }
}