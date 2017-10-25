using OSPSuite.Presentation.Presenters;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IScatterChartPresenter : IPresenter<IScatterChartView>, IPopulationAnalysisChartPresenter<ScatterXValue, ScatterYValue>
   {
   }

   public class ScatterChartPresenter : PopulationAnalysisChartPresenter<IScatterChartView, IScatterChartPresenter, ScatterXValue, ScatterYValue>, IScatterChartPresenter
   {
      public ScatterChartPresenter(IScatterChartView view, IPopulationAnalysisChartSettingsPresenter populationAnalysisChartSettingsPresenter, IApplicationSettings applicationSettings)
         : base(view, populationAnalysisChartSettingsPresenter, applicationSettings)
      {
      }
   }
}