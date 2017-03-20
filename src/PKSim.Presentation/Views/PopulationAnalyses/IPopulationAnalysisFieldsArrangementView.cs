using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface IPopulationAnalysisFieldsArrangementView : IView<IPopulationAnalysisFieldsArrangementPresenter>
   {
      void SetAreaView(PivotArea area, IView view);
      void SetAreaVisibility(PivotArea area, bool visible);
   }
}