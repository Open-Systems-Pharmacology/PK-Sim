using PKSim.Core.Chart;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Presenters;
using PKSim.Core;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IRangeChartPresenter : IPresenter<IRangeChartView>, IPopulationAnalysisChartPresenter<RangeXValue, RangeYValue>
   {
   }

   public class RangeChartPresenter : PopulationAnalysisChartPresenter<IRangeChartView, IRangeChartPresenter, RangeXValue, RangeYValue>, IRangeChartPresenter
   {
      public RangeChartPresenter(IRangeChartView view, IPopulationAnalysisChartSettingsPresenter populationAnalysisChartSettingsPresenter, IApplicationSettings applicationSettings) 
         : base(view,populationAnalysisChartSettingsPresenter, applicationSettings)
      {
      }
   }
}