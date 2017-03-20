using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IBoxWhiskerAnalysisResultsPresenter : IPopulationAnalysisResultsPresenter
   {
   }

   public class BoxWhiskerAnalysisResultsPresenter : PopulationAnalysisResultsPresenter<BoxWhiskerXValue, BoxWhiskerYValue, IBoxWhiskerAnalysisResultsView, IBoxWhiskerAnalysisResultsPresenter, IBoxWhiskerChartPresenter>, IBoxWhiskerAnalysisResultsPresenter
   {
      public BoxWhiskerAnalysisResultsPresenter(IBoxWhiskerAnalysisResultsView view, IBoxWhiskerFieldSelectionPresenter fieldSelectionPresenter, IBoxWhiskerChartPresenter boxWhiskerChartPresenter, IBoxWhiskerChartDataCreator boxWhiskerChartDataCreator,
         IPopulationAnalysisTask populationAnalysisTask)
         : base(view, fieldSelectionPresenter, boxWhiskerChartPresenter, boxWhiskerChartDataCreator, populationAnalysisTask)
      {
      }

      protected override ChartData<BoxWhiskerXValue, BoxWhiskerYValue> CreateChartData()
      {
         return _chartDataCreator.CreateFor(_populationPivotAnalysis, _populationDataCollector, Chart.ObservedDataCollection, AggregationFunctions.BoxWhisker90Aggregation);
      }
   }
}