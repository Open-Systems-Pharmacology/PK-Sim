using OSPSuite.Assets;
using OSPSuite.Presentation;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.Presenters.Charts;
using PKSim.Presentation.Views.Charts;

namespace PKSim.UI.Views.Charts
{
   public class SimulationTimeProfileChartView : BasePKAnalysisWithChartView, ISimulationTimeProfileChartView
   {
      public override ApplicationIcon ApplicationIcon => ApplicationIcons.TimeProfileAnalysis;

      public void AttachPresenter(ISimulationTimeProfileChartPresenter presenter)
      {
         AttachPresenter(presenter.DowncastTo<IPKAnalysisWithChartPresenter>());
      }
   }
}