using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface ITimeProfileAnalysisResultsPresenter : IPopulationAnalysisResultsPresenter
   {
   }

   public class TimeProfileAnalysisResultsPresenter : PopulationAnalysisResultsPresenter<TimeProfileXValue, TimeProfileYValue, ITimeProfileAnalysisResultsView, ITimeProfileAnalysisResultsPresenter, ITimeProfileChartPresenter>, ITimeProfileAnalysisResultsPresenter
   {
      public TimeProfileAnalysisResultsPresenter(ITimeProfileAnalysisResultsView view, ITimeProfileFieldSelectionPresenter fieldSelectionPresenter, ITimeProfileChartPresenter timeProfileChartPresenter,
         ITimeProfileChartDataCreator timeProfileChartDataCreator, IPopulationAnalysisTask populationAnalysisTask)
         : base(view, fieldSelectionPresenter, timeProfileChartPresenter, timeProfileChartDataCreator, populationAnalysisTask)
      {
      }

      protected override ChartData<TimeProfileXValue, TimeProfileYValue> CreateChartData()
      {
         return _chartDataCreator.CreateFor(_populationPivotAnalysis, _populationDataCollector, Chart.ObservedDataCollection, AggregationFunctions.QuantityAggregation);
      }
   }
}