using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IScatterAnalysisResultsPresenter : IPopulationAnalysisResultsPresenter
   {
   }

   public class ScatterAnalysisResultsPresenter : PopulationAnalysisResultsPresenter<ScatterXValue, ScatterYValue, IScatterAnalysisResultsView, IScatterAnalysisResultsPresenter, IScatterChartPresenter>, IScatterAnalysisResultsPresenter
   {
      public ScatterAnalysisResultsPresenter(IScatterAnalysisResultsView view, IScatterFieldSelectionPresenter fieldSelectionPresenter, IScatterChartPresenter scatterChartPresenter, IScatterChartDataCreator scatterDataCreator, IPopulationAnalysisTask populationAnalysisTask)
         : base(view, fieldSelectionPresenter, scatterChartPresenter, scatterDataCreator, populationAnalysisTask)
      {
      }

      protected override ChartData<ScatterXValue, ScatterYValue> CreateChartData()
      {
         return _chartDataCreator.CreateFor(_populationPivotAnalysis, _populationDataCollector, Chart.ObservedDataCollection, AggregationFunctions.ValuesAggregation);
      }
   }
}