using PKSim.Core.Chart;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IScatterChartPresenter : IPresenter<IScatterChartView>, IPopulationAnalysisChartPresenter<ScatterXValue, ScatterYValue>
   {
   }

   public class ScatterChartPresenter : PopulationAnalysisChartPresenter<IScatterChartView, IScatterChartPresenter, ScatterXValue, ScatterYValue>, IScatterChartPresenter
   {
      public ScatterChartPresenter(IScatterChartView view, IPopulationAnalysisChartSettingsPresenter populationAnalysisChartSettingsPresenter)
         : base(view, populationAnalysisChartSettingsPresenter)
      {
      }
   }
}