using OSPSuite.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.Presenters.Charts;
using PKSim.Presentation.Views.Charts;
using OSPSuite.Presentation;

namespace PKSim.UI.Views.Charts
{
   public class SimulationTimeProfileChartView : BasePKAnalysisWithChartView, ISimulationTimeProfileChartView
   {      
      public override ApplicationIcon ApplicationIcon => ApplicationIcons.TimeProfileAnalysis;

      public void AttachPresenter(Presentation.Presenters.Charts.ISimulationTimeProfileChartPresenter presenter)
      {
         AttachPresenter(presenter.DowncastTo<IPKAnalysisWithChartPresenter>());
      }

      protected override int TopicId => HelpId.PKSim_Simulations_DisplayResultsIndividuals;
   }
}