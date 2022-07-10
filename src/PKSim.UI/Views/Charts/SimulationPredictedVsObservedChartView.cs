using OSPSuite.Utility.Extensions;
using PKSim.Presentation.Presenters.Charts;
using PKSim.Presentation.Views.Charts;

namespace PKSim.UI.Views.Charts
{
   public class SimulationPredictedVsObservedChartView : BasePKAnalysisWithChartView, ISimulationPredictedVsObservedChartView
   {
      public void AttachPresenter(ISimulationPredictedVsObservedChartPresenter presenter)
      {
         AttachPresenter(presenter.DowncastTo<IPKAnalysisWithChartPresenter>());
      }
   }
}