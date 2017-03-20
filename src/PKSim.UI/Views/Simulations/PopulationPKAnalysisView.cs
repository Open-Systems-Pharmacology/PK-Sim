using System.Data;
using PKSim.Assets;
using OSPSuite.DataBinding;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Assets;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.Simulations
{
   public partial class PopulationPKAnalysisView : BaseUserControl, IPopulationPKAnalysisView
   {
      private readonly IPKAnalysisPivotView _populationAnalysisPivotView;
      private IPopulationPKAnalysisPresenter _presenter;
      private readonly ScreenBinder<PKAnalysisDTO> _screenBinder;

      public PopulationPKAnalysisView(IPKAnalysisPivotView populationAnalysisPivotView)
      {
         _populationAnalysisPivotView = populationAnalysisPivotView;
         InitializeComponent();
         addPopulationPKAnalysisView(_populationAnalysisPivotView);
         _screenBinder = new ScreenBinder<PKAnalysisDTO>();
      }

      private void addPopulationPKAnalysisView(IPKAnalysisPivotView view)
      {
         populationPKAnalysisPanel.FillWith(view);
      }

      public void BindTo(PKAnalysisDTO pkAnalysisDTO)
      {
         _screenBinder.BindToSource(pkAnalysisDTO);
         _populationAnalysisPivotView.BindTo(pkAnalysisDTO.DataTable);
      }

      public DataTable GetSummaryData()
      {
         return _populationAnalysisPivotView.GetSummaryData();
      }

      public void AttachPresenter(IPopulationPKAnalysisPresenter presenter)
      {
         _presenter = presenter;
         _populationAnalysisPivotView.BindUnitsMenuToPresenter(presenter);
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         btnExportToExcel.Click += (o, e) => OnEvent(_presenter.ExportToExcel);
         _screenBinder.Bind(x => x.HasRows).ToEnableOf(btnExportToExcel);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         btnExportToExcel.InitWithImage(ApplicationIcons.Excel, text: PKSimConstants.UI.ExportPKAnalysesToExcel);
         layoutItemExportToExcel.AdjustLargeButtonSize();
         populationPKAnalysisItem.TextVisible = false;
      }
   }
}