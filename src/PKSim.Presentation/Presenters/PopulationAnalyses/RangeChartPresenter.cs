using PKSim.Core.Chart;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IRangeChartPresenter : IPresenter<IRangeChartView>, IPopulationAnalysisChartPresenter<RangeXValue, RangeYValue>
   {
   }

   public class RangeChartPresenter : PopulationAnalysisChartPresenter<IRangeChartView, IRangeChartPresenter, RangeXValue, RangeYValue>, IRangeChartPresenter
   {
      public RangeChartPresenter(IRangeChartView view, IPopulationAnalysisChartSettingsPresenter populationAnalysisChartSettingsPresenter) 
         : base(view,populationAnalysisChartSettingsPresenter)
      {
      }
   }
}