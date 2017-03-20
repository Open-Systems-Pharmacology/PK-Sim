using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.Charts;
using OSPSuite.Assets;

namespace PKSim.Presentation.Presenters.Charts
{
   public interface IEditRangeAnalysisChartPresenter : IEditPopulationAnalysisChartPresenter
   {
   }

   public class EditRangeAnalysisChartPresenter : EditPopulationAnalysisChartPresenter<RangeAnalysisChart, RangeXValue, RangeYValue>, IEditRangeAnalysisChartPresenter
   {
      public EditRangeAnalysisChartPresenter(ISimulationAnalysisChartView view, IRangeChartPresenter rangeChartPresenter, IRangeChartDataCreator rangeChartDataCreator, IPopulationSimulationAnalysisStarter populationSimulationAnalysisStarter, IPopulationAnalysisTask populationAnalysisTask) :
         base(view, rangeChartPresenter, rangeChartDataCreator, populationSimulationAnalysisStarter, populationAnalysisTask, ApplicationIcons.RangeAnalysis)
      {
      }

      protected override ChartData<RangeXValue, RangeYValue> CreateChartData()
      {
         return _chartDataCreator.CreateFor(PopulationAnalysisChart, AggregationFunctions.ValuesAggregation);
      }

      public override string PresentationKey => PresenterConstants.PresenterKeys.EditRangeAnalysisChartPresenter;
   }
}