using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Views;
using OSPSuite.Presentation.Views.Charts;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface IPopulationAnalysisChartSettingsView : IView<IPopulationAnalysisChartSettingsPresenter>
   {
      void SetChartSettingsView(IChartSettingsView view);
      bool AllowEditConfiguration { get; set; }
      void SetChartExportSettingsView(IChartExportSettingsView view);
   }
}