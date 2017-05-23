using OSPSuite.Assets;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.Charts;

namespace PKSim.Presentation.Presenters.Charts
{
   public interface IEditBoxWhiskerAnalysisChartPresenter : IEditPopulationAnalysisChartPresenter
   {
   }

   public class EditBoxWhiskerAnalysisChartPresenter : EditPopulationAnalysisChartPresenter<BoxWhiskerAnalysisChart, BoxWhiskerXValue, BoxWhiskerYValue>, IEditBoxWhiskerAnalysisChartPresenter
   {
      public override string PresentationKey => PresenterConstants.PresenterKeys.EditBoxWhiskerAnalysisChartPresenter;

      public EditBoxWhiskerAnalysisChartPresenter(ISimulationAnalysisChartView view, IBoxWhiskerChartPresenter boxWhiskerChartPresenter, IBoxWhiskerChartDataCreator boxWhiskerChartDataCreator,
         IPopulationSimulationAnalysisStarter populationSimulationAnalysisStarter, IPopulationAnalysisTask populationAnalysisTask)
         : base(view, boxWhiskerChartPresenter, boxWhiskerChartDataCreator, populationSimulationAnalysisStarter, populationAnalysisTask, ApplicationIcons.BoxWhiskerAnalysis)
      {
      }

      protected override ChartData<BoxWhiskerXValue, BoxWhiskerYValue> CreateChartData()
      {
         return _chartDataCreator.CreateFor(PopulationAnalysisChart, AggregationFunctions.BoxWhisker90Aggregation);
      }
   }
}