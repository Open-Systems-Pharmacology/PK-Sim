using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface ITimeProfileFieldSelectionPresenter : IPopulationAnalysisFieldSelectionPresenter
   {
   }

   public class TimeProfileFieldSelectionPresenter : PopulationAnalysisFieldSelectionPresenter<IPopulationAnalysisFieldSelectionView>,  ITimeProfileFieldSelectionPresenter
   {
      public TimeProfileFieldSelectionPresenter(IPopulationAnalysisFieldSelectionView view, IPopulationAnalysisFieldsArrangementPresenter fieldsArrangenentPresenter, IMultipleNumericFieldsPresenter multipleNumericFieldsPresenter) :
         base(view, fieldsArrangenentPresenter, multipleNumericFieldsPresenter)
      {
         fieldsArrangenentPresenter.UpdateAreaVisibility(PivotArea.SymbolArea, visible: false); 
         multipleNumericFieldsPresenter.AllowedType = typeof(PopulationAnalysisOutputField);
      }
   }
}