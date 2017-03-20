using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.Charts;
using OSPSuite.Assets;

namespace PKSim.Presentation.Presenters.Charts
{
   public interface IEditScatterAnalysisChartPresenter : IEditPopulationAnalysisChartPresenter
   {
   }

   public class EditScatterAnalysisChartPresenter : EditPopulationAnalysisChartPresenter<ScatterAnalysisChart, ScatterXValue, ScatterYValue>, IEditScatterAnalysisChartPresenter
   {
      public EditScatterAnalysisChartPresenter(ISimulationAnalysisChartView view, IScatterChartPresenter scatterChartPresenter, IScatterChartDataCreator scatterChartDataCreator, IPopulationSimulationAnalysisStarter populationSimulationAnalysisStarter, IPopulationAnalysisTask populationAnalysisTask)
         : base(view, scatterChartPresenter, scatterChartDataCreator, populationSimulationAnalysisStarter, populationAnalysisTask, ApplicationIcons.ScatterAnalysis)
      {
      }

      protected override ChartData<ScatterXValue, ScatterYValue> CreateChartData()
      {
         return _chartDataCreator.CreateFor(PopulationAnalysisChart, AggregationFunctions.ValuesAggregation);
      }

      public override string PresentationKey => PresenterConstants.PresenterKeys.EditScatterAnalysisChartPresenter;
   }
}