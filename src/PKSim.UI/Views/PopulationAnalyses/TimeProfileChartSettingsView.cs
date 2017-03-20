using PKSim.Assets;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class TimeProfileChartSettingsView : BaseUserControl, ITimeProfileChartSettingsView
   {
      private ITimeProfileChartSettingsPresenter _presenter;

      public TimeProfileChartSettingsView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(ITimeProfileChartSettingsPresenter presenter)
      {
         _presenter = presenter;
      }

      public void AddChartSettingsView(IView chartSettingsView)
      {
         panelChartSettings.FillWith(chartSettingsView);
      }

      public void AddObservedDataSettingsView(IView observedDataSettingsView)
      {
         panelObservedDataSettings.FillWith(observedDataSettingsView);
      }

      public void AddChartExportSettingsView(IView exportView)
      {
         chartExportSettingsPanel.FillWith(exportView);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         observedDataLayoutGroup.Text = PKSimConstants.UI.ObservedDataSettings;
         chartSettingsLayoutGroup.Text = PKSimConstants.UI.ChartSettings;
         chartExportSettingsLayoutGroup.Text = PKSimConstants.UI.ExportSettings;
      }
   }
}
