using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface IPopulationAnalysisResultsView : IView
   {
      void SetChartView(IView view);
      void SetFieldSelectionView(IView view);
   }

   public interface IBoxWhiskerAnalysisResultsView : IView<IBoxWhiskerAnalysisResultsPresenter>, IPopulationAnalysisResultsView
   {
   }

   public interface IScatterAnalysisResultsView : IView<IScatterAnalysisResultsPresenter>, IPopulationAnalysisResultsView
   {
   }

   public interface IRangeAnalysisResultsView : IView<IRangeAnalysisResultsPresenter>, IPopulationAnalysisResultsView
   {
   }

   public interface ITimeProfileAnalysisResultsView : IView<ITimeProfileAnalysisResultsPresenter>, IPopulationAnalysisResultsView
   {
   }
}