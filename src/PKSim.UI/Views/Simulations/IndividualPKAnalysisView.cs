using System.Data;
using OSPSuite.DataBinding;
using DevExpress.XtraLayout.Utils;
using PKSim.Assets;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Assets;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.Simulations
{
   public partial class IndividualPKAnalysisView : BaseUserControl, IIndividualPKAnalysisView
   {
      private IIndividualPKAnalysisPresenter _presenter;
      private readonly IPKAnalysisPivotView _individualPKAnalysisPivotView;
      private readonly ScreenBinder<PKAnalysisDTO> _binder;

      public IndividualPKAnalysisView(IPKAnalysisPivotView individualPKAnalysisPivotView)
      {
         InitializeComponent();
         _individualPKAnalysisPivotView = individualPKAnalysisPivotView;
         addIndividualPKAnalysisView(individualPKAnalysisPivotView);
         _binder = new ScreenBinder<PKAnalysisDTO>();
      }

      public void AddGlobalPKAnalysisView(IGlobalPKAnalysisView view)
      {
         panelControlGlobalAnalysis.FillWith(view);
      }

      private void addIndividualPKAnalysisView(IPKAnalysisPivotView view)
      {
         panelControlIndividualAnalysis.FillWith(view);
      }

      public void BindTo(PKAnalysisDTO pkAnalysisDTO)
      {
         _binder.BindToSource(pkAnalysisDTO);
         _individualPKAnalysisPivotView.BindTo(pkAnalysisDTO.DataTable);
      }

      public DataTable GetSummaryData()
      {
         return _individualPKAnalysisPivotView.GetSummaryData();
      }

      public bool GlobalPKVisible
      {
         set { setGlobalAnalysisViewVisibility(value); }
      }

      private void setGlobalAnalysisViewVisibility(bool showing)
      {
         layoutItemGlobalPKAnalysis.Visibility = LayoutVisibilityConvertor.FromBoolean(showing);
         splitterItem.Visibility = layoutItemGlobalPKAnalysis.Visibility;
      }

      public bool ShowControls
      {
         set { layoutControl.Visible = value; }
      }

      public void AttachPresenter(IIndividualPKAnalysisPresenter presenter)
      {
         _presenter = presenter;
         _individualPKAnalysisPivotView.BindUnitsMenuToPresenter(presenter);
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         btnExportToExcel.Click += (o, e) => OnEvent(_presenter.ExportToExcel);
         _binder.Bind(x => x.HasRows).ToEnableOf(btnExportToExcel);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         btnExportToExcel.InitWithImage(ApplicationIcons.Excel, text: PKSimConstants.UI.ExportPKAnalysesToExcel);
         layoutItemExportToExcel.AdjustLargeButtonSize(layoutControl);

         layoutItemGlobalPKAnalysis.TextVisible = false;
         layoutItemIndividualPKAnalysis.TextVisible = false;
      }
   }
}