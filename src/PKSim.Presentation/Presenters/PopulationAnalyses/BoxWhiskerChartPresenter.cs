using PKSim.Core.Chart;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IBoxWhiskerChartPresenter : IPresenter<IBoxWhiskerChartView>, IPopulationAnalysisChartPresenter<BoxWhiskerXValue, BoxWhiskerYValue>
   {
   }

   public class BoxWhiskerChartPresenter : PopulationAnalysisChartPresenter<IBoxWhiskerChartView, IBoxWhiskerChartPresenter, BoxWhiskerXValue, BoxWhiskerYValue>, IBoxWhiskerChartPresenter
   {
      public BoxWhiskerChartPresenter(IBoxWhiskerChartView view,IPopulationAnalysisChartSettingsPresenter populationAnalysisChartSettingsPresenter) : base(view,populationAnalysisChartSettingsPresenter)
      {
      }
   }
}