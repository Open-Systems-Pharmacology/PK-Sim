using DevExpress.Utils;
using DevExpress.XtraLayout.Utils;
using PKSim.Assets;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Assets;
using OSPSuite.UI.Controls;
using OSPSuite.Presentation.Views.Charts;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class PopulationAnalysisChartSettingsView : BaseUserControl, IPopulationAnalysisChartSettingsView
   {
      private IPopulationAnalysisChartSettingsPresenter _presenter;

      public PopulationAnalysisChartSettingsView()
      {
         InitializeComponent();
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         _layoutControl.AllowCustomization = false;
         _layoutControlItemChartSettings.TextVisible = false;
         _layoutControlItemChartSettings.TextLocation = Locations.Top;
         _btnEdit.InitWithImage(ApplicationIcons.Edit, text: PKSimConstants.UI.EditPopulationAnalysisConfiguration);
         _layoutControlItemEdit.AdjustLargeButtonSize(_layoutControl);

         chartSettingsLayoutGroup.Text = PKSimConstants.UI.ChartSettings;
         chartExportSettingsLayoutGroup.Text = PKSimConstants.UI.ExportSettings;
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         _btnEdit.Click += (o, e) => OnEvent(_presenter.EditConfiguration);
      }

      public void AttachPresenter(IPopulationAnalysisChartSettingsPresenter presenter)
      {
         _presenter = presenter;
      }

      public void SetChartExportSettingsView(IChartExportSettingsView view)
      {
         _pnlChartExportSettings.FillWith(view);
      }

      public void SetChartSettingsView(IChartSettingsView view)
      {
         _pnlChartSettings.FillWith(view);
      }

      public bool AllowEditConfiguration
      {
         get { return _layoutControlItemEdit.Visible; }
         set { _layoutControlItemEdit.Visibility = LayoutVisibilityConvertor.FromBoolean(value); }
      }
   }
}