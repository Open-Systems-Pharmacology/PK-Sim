using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IScatterFieldSelectionPresenter : IPopulationAnalysisFieldSelectionPresenter
   {
   }

   public class ScatterFieldSelectionPresenter : PopulationAnalysisFieldSelectionPresenter<IPopulationAnalysisFieldSelectionView>, IScatterFieldSelectionPresenter
   {
      public ScatterFieldSelectionPresenter(IPopulationAnalysisFieldSelectionView view, IPopulationAnalysisFieldsArrangementPresenter fieldsArrangenentPresenter, IXAndYNumericFieldsPresenter numericFieldsPresenter) :
         base(view, fieldsArrangenentPresenter, numericFieldsPresenter)
      {
      }
   }
}