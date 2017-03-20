using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IRangeFieldSelectionPresenter : IPopulationAnalysisFieldSelectionPresenter
   {
   }

   public class RangeFieldSelectionPresenter : PopulationAnalysisFieldSelectionPresenter<IPopulationAnalysisFieldSelectionView>, IRangeFieldSelectionPresenter
   {
      public RangeFieldSelectionPresenter(IPopulationAnalysisFieldSelectionView view, IPopulationAnalysisFieldsArrangementPresenter fieldsArrangenentPresenter, IXAndYNumericFieldsPresenter numericFieldsPresenter) :
         base(view, fieldsArrangenentPresenter, numericFieldsPresenter)
      {
         fieldsArrangenentPresenter.UpdateAreaVisibility(PivotArea.SymbolArea, visible: false);
      }
   }
}