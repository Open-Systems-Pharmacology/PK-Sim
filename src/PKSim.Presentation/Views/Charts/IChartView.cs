using OSPSuite.Assets;
using PKSim.Presentation.Presenters.Charts;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Charts
{
   public interface ISimulationAnalysisView : IView
   {
      void SetChartView(IView view);
   }

   public interface IChartView<TPresenter> : IView<TPresenter>, ISimulationAnalysisView where TPresenter : IPresenter
   {
      void ShowChartView();
      void ShowPKAnalysisView();
      void SetPKAnalysisView(IView view);
   }

   public interface ISimulationTimeProfileChartView : IChartView<ISimulationTimeProfileChartPresenter>
   {
   }

   public interface ISimulationPredictedVsObservedChartView : IChartView<ISimulationPredictedVsObservedChartPresenter>
   {
   }

   public interface ISimulationAnalysisChartView : ISimulationAnalysisView, IView<IEditPopulationAnalysisChartPresenter>
   {
      void UpdateIcon(ApplicationIcon icon);
   }
}