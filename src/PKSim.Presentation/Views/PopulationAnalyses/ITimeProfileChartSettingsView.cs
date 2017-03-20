using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface ITimeProfileChartSettingsView:IView<ITimeProfileChartSettingsPresenter>
   {
      void AddChartSettingsView(IView chartSettingsView);
      void AddObservedDataSettingsView(IView observedDataSettingsView);
      void AddChartExportSettingsView(IView exportView);
   }
}