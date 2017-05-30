using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IRangeAnalysisResultsPresenter : IPopulationAnalysisResultsPresenter
   {
   }

   public class RangeAnalysisResultsPresenter : PopulationAnalysisResultsPresenter<RangeXValue, RangeYValue, IRangeAnalysisResultsView, IRangeAnalysisResultsPresenter, IRangeChartPresenter>, IRangeAnalysisResultsPresenter
   {
      public RangeAnalysisResultsPresenter(IRangeAnalysisResultsView view, IRangeFieldSelectionPresenter fieldSelectionPresenter, IRangeChartPresenter rangeChartPresenter, IRangeChartDataCreator chartDataCreator,
         IPopulationAnalysisTask populationAnalysisTask)
         : base(view, fieldSelectionPresenter, rangeChartPresenter, chartDataCreator, populationAnalysisTask)
      {
      }


      protected override ChartData<RangeXValue, RangeYValue> CreateChartData()
      {
         return _chartDataCreator.CreateFor(_populationPivotAnalysis, _populationDataCollector, Chart.ObservedDataCollection, AggregationFunctions.ValuesAggregation);
      }
   }
}