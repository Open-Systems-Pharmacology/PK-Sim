using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface IEditPopulationAnalysisGroupingFieldView : IModalView<IEditPopulationAnalysisGroupingFieldPresenter>
   {
      void SetGroupingView(IView view);
   }
}