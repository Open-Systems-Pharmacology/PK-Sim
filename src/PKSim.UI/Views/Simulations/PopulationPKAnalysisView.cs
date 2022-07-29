using System.Data;
using DevExpress.XtraLayout.Utils;
using OSPSuite.Assets;
using OSPSuite.DataBinding;
using OSPSuite.Presentation.Extensions;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Services;
using PKSim.Assets;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.UI.Views.Simulations
{
   public partial class PopulationPKAnalysisView : BaseUserControl, IPopulationPKAnalysisView
   {
      private readonly IPKAnalysisPivotView _populationAnalysisPivotViewOnCurve;
      private readonly IPKAnalysisPivotView _populationAnalysisPivotViewOnIndividuals;
      private IPopulationPKAnalysisPresenter _presenter;
      private readonly ScreenBinder<PKAnalysisDTO> _screenBinder;
      private readonly IImageListRetriever _imageListRetriever;

      public PopulationPKAnalysisView(IPKAnalysisPivotView populationAnalysisPivotView, IPKAnalysisPivotView populationAnalysisPivotViewOnIndividuals, IImageListRetriever imageListRetriever)
      {
         InitializeComponent();
         _imageListRetriever = imageListRetriever;
         _populationAnalysisPivotViewOnCurve = populationAnalysisPivotView;
         _populationAnalysisPivotViewOnIndividuals = populationAnalysisPivotViewOnIndividuals;
         addPopulationPKAnalysisView(_populationAnalysisPivotViewOnCurve, _populationAnalysisPivotViewOnIndividuals);
         _screenBinder = new ScreenBinder<PKAnalysisDTO>();
      }

      public void AddGlobalPKAnalysisView(IGlobalPKAnalysisView view)
      {
         globalPKParametersPanelControl.FillWith(view);
      }

      private void addPopulationPKAnalysisView(IPKAnalysisPivotView viewOnCurve, IPKAnalysisPivotView viewOnIndividuals)
      {
         populationPKAnalysisPanelOnCurve.FillWith(viewOnCurve);
         populationPKAnalysisPanelOnIndividuals.FillWith(viewOnIndividuals);
      }

      public void BindTo(IntegratedPKAnalysisDTO pkAnalysisDTO)
      {
         _screenBinder.BindToSource(pkAnalysisDTO.OnCurves);
         _populationAnalysisPivotViewOnCurve.BindTo(pkAnalysisDTO.OnCurves.DataTable);
         _populationAnalysisPivotViewOnIndividuals.BindTo(pkAnalysisDTO.OnIndividuals.DataTable);
      }

      public bool IsOnCurvesSelected => populationPKAnalysisXtraTabControl.SelectedTabPage == pageOnCurves;

      public void ShowPKAnalysisOnIndividuals(bool visible)
      {

         var visibility = LayoutVisibilityConvertor.FromBoolean(visible);
         pageOnIndividuals.PageVisible = visible;
         splitter.Visibility = visibility;
         layoutControlItemGlobalPKAnalysis.Visibility = visibility;
         layoutControlItemGlobalPKAnalysisDescription.Visibility = visibility;
      }

      public DataTable GetSummaryData()
      {
         return IsOnCurvesSelected
            ? _populationAnalysisPivotViewOnCurve.GetSummaryData()
            : _populationAnalysisPivotViewOnIndividuals.GetSummaryData();
      }

      public void AttachPresenter(IPopulationPKAnalysisPresenter presenter)
      {
         _presenter = presenter;
         _populationAnalysisPivotViewOnCurve.BindUnitsMenuToPresenter(presenter);
         _populationAnalysisPivotViewOnIndividuals.BindUnitsMenuToPresenter(presenter);
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
         populationPKAnalysisXtraTabControl.Images = _imageListRetriever.AllImages16x16;
         
         pageOnIndividuals.ImageIndex = _imageListRetriever.ImageIndex(ApplicationIcons.Population);
         pageOnIndividuals.Text = PKSimConstants.PKAnalysis.OnIndividuals;
         pageOnIndividuals.Tooltip = PKSimConstants.PKAnalysis.OnIndividualsTooltip;

         pageOnCurves.ImageIndex = _imageListRetriever.ImageIndex(ApplicationIcons.TimeProfileAnalysis);
         pageOnCurves.Text = PKSimConstants.PKAnalysis.OnCurves;
         pageOnCurves.Tooltip = PKSimConstants.PKAnalysis.OnCurvesTooltip;

         btnExportToExcel.InitWithImage(ApplicationIcons.Excel, text: PKSimConstants.UI.ExportPKAnalysesToExcel);
         layoutItemExportToExcel.AdjustLargeButtonSize();
         layoutControlItemGlobalPKAnalysis.TextVisible = false;
         populationPKAnalysisXtraTabControl.SelectedPageChanged += (o, e) => OnEvent(_presenter.HandleTabChanged);
         labelControlGlobalPKAnalysisDescription.Text = PKSimConstants.UI.GlobalPKAnalysisDescription.FormatForLabel();
      }
   }
}