using PKSim.Assets;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IBoxWhiskerFieldSelectionPresenter : IPopulationAnalysisFieldSelectionPresenter
   {
   }

   public class BoxWhiskerFieldSelectionPresenter : PopulationAnalysisFieldSelectionPresenter<IPopulationAnalysisFieldSelectionView>, IBoxWhiskerFieldSelectionPresenter
   {
      public BoxWhiskerFieldSelectionPresenter(IPopulationAnalysisFieldSelectionView view, IPopulationAnalysisFieldsArrangementPresenter fieldsArrangenentPresenter, IBoxWhiskerNumericFieldsPresenter boxWhiskerNumericFieldsPresenter) :
         base(view, fieldsArrangenentPresenter, boxWhiskerNumericFieldsPresenter)
      {
         fieldsArrangenentPresenter.UpdateDescription(PivotArea.RowArea, PKSimConstants.UI.XGrouping);
         fieldsArrangenentPresenter.UpdateAreaVisibility(PivotArea.SymbolArea, visible:false);
      }
   }
}