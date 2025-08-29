using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IRangeChartPresenter : IPresenter<IRangeChartView>, IPopulationAnalysisChartPresenter<RangeXValue, RangeYValue>
   {
   }

   public class RangeChartPresenter : PopulationAnalysisChartPresenter<IRangeChartView, IRangeChartPresenter, RangeXValue, RangeYValue>, IRangeChartPresenter
   {
      public RangeChartPresenter(IRangeChartView view,
         IPopulationAnalysisChartSettingsPresenter populationAnalysisChartSettingsPresenter,
         IApplicationSettings applicationSettings,
         IDialogCreator dialogCreator)
         : base(view, populationAnalysisChartSettingsPresenter, applicationSettings, dialogCreator)
      {
      }
   }
}