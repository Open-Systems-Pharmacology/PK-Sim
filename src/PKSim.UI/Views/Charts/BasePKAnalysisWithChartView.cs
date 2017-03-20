using DevExpress.XtraEditors;
using DevExpress.XtraLayout.Utils;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Charts;
using OSPSuite.Presentation.Views;
using OSPSuite.Assets;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.Charts
{
   public abstract partial class BasePKAnalysisWithChartView : BaseUserControl, IView<IPKAnalysisWithChartPresenter>
   {
      protected IPKAnalysisWithChartPresenter _presenter;

      protected BasePKAnalysisWithChartView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IPKAnalysisWithChartPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         btnSwithPKAnalysisPlot.Click += (o, e) => OnEvent(switchPKAnalysis);
      }

      private void switchPKAnalysis()
      {
         _presenter.SwitchPKAnalysisPlot();
      }

      private void setAnalysisVisibility(bool visible)
      {
         layoutControlGroup.Visibility = LayoutVisibilityConvertor.FromBoolean(false);
         analysisControlItem.Visibility = LayoutVisibilityConvertor.FromBoolean(visible);
         viewSplitter.Visibility = analysisControlItem.Visibility;
         layoutControlGroup.Visibility = LayoutVisibilityConvertor.FromBoolean(true);
      }

      public void ShowPKAnalysisView()
      {
         setAnalysisVisibility(true);
         btnSwithPKAnalysisPlot.InitWithImage(ApplicationIcons.TimeProfileAnalysis, text: PKSimConstants.UI.HidePKAnalysis, imageLocation: ImageLocation.TopCenter);
      }

      public void ShowChartView()
      {
         setAnalysisVisibility(false);
         btnSwithPKAnalysisPlot.InitWithImage(ApplicationIcons.PKAnalysis, text: PKSimConstants.UI.ShowPKAnalysis, imageLocation: ImageLocation.TopCenter);
      }

      public void SetChartView(IView view)
      {
         chartPanel.FillWith(view);
      }

      public void SetPKAnalysisView(IView view)
      {
         analysisPanel.FillWith(view);
      }
   }
}