using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface ICreatePopulationAnalysisGroupingFieldView : IModalView<ICreatePopulationAnalysisGroupingFieldPresenter>
   {
      void SetGroupingView(IView view);
      void BindTo(GroupingFieldDTO groupingFieldDTO);
   }
}