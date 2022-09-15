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
      private readonly IPKAnalysisPivotView _populationAnalysisPivotViewAggregatedPKValues;
      private readonly IPKAnalysisPivotView _populationAnalysisPivotViewIndividualPKValues;
      private IPopulationPKAnalysisPresenter _presenter;
      private readonly ScreenBinder<PKAnalysisDTO> _screenBinder;
      private readonly IImageListRetriever _imageListRetriever;

      public PopulationPKAnalysisView(IPKAnalysisPivotView populationAnalysisPivotView, IPKAnalysisPivotView populationAnalysisPivotViewIndividualPKValues, IImageListRetriever imageListRetriever)
      {
         InitializeComponent();
         _imageListRetriever = imageListRetriever;
         _populationAnalysisPivotViewAggregatedPKValues = populationAnalysisPivotView;
         _populationAnalysisPivotViewIndividualPKValues = populationAnalysisPivotViewIndividualPKValues;
         addPopulationPKAnalysisView(_populationAnalysisPivotViewAggregatedPKValues, _populationAnalysisPivotViewIndividualPKValues);
         _screenBinder = new ScreenBinder<PKAnalysisDTO>();
      }

      public void AddGlobalPKAnalysisView(IGlobalPKAnalysisView view)
      {
         globalPKParametersPanelControl.FillWith(view);
      }

      private void addPopulationPKAnalysisView(IPKAnalysisPivotView viewAggregatedPKValues, IPKAnalysisPivotView viewIndividualPKValues)
      {
         populationPKAnalysisPanelAggregatedPKValues.FillWith(viewAggregatedPKValues);
         populationPKAnalysisPanelIndividualPKValues.FillWith(viewIndividualPKValues);
      }

      public void BindTo(IntegratedPKAnalysisDTO pkAnalysisDTO)
      {
         _screenBinder.BindToSource(pkAnalysisDTO.AggregatedPKValues);
         _populationAnalysisPivotViewAggregatedPKValues.BindTo(pkAnalysisDTO.AggregatedPKValues.DataTable);
         _populationAnalysisPivotViewIndividualPKValues.BindTo(pkAnalysisDTO.IndividualPKValues.DataTable);
      }

      public bool IsAggregatedPKValuesSelected => populationPKAnalysisTabControl.SelectedTabPage == pageAggregatedPKValues;

      public void ShowPKAnalysisIndividualPKValues(bool visible)
      {
         var visibility = LayoutVisibilityConvertor.FromBoolean(visible);
         pageIndividualPKValues.PageVisible = visible;
         splitter.Visibility = visibility;
         layoutControlItemGlobalPKAnalysis.Visibility = visibility;
         layoutControlItemGlobalPKAnalysisDescription.Visibility = visibility;
      }

      public DataTable GetSummaryData()
      {
         return IsAggregatedPKValuesSelected
            ? _populationAnalysisPivotViewAggregatedPKValues.GetSummaryData()
            : _populationAnalysisPivotViewIndividualPKValues.GetSummaryData();
      }

      public void AttachPresenter(IPopulationPKAnalysisPresenter presenter)
      {
         _presenter = presenter;
         _populationAnalysisPivotViewAggregatedPKValues.BindUnitsMenuToPresenter(presenter);
         _populationAnalysisPivotViewIndividualPKValues.BindUnitsMenuToPresenter(presenter);
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
         populationPKAnalysisTabControl.Images = _imageListRetriever.AllImages16x16;
         
         pageIndividualPKValues.ImageIndex = _imageListRetriever.ImageIndex(ApplicationIcons.Population);
         pageIndividualPKValues.Text = PKSimConstants.PKAnalysis.IndividualPKValues;
         pageIndividualPKValues.Tooltip = PKSimConstants.PKAnalysis.IndivdualPKValuesTooltip;

         pageAggregatedPKValues.ImageIndex = _imageListRetriever.ImageIndex(ApplicationIcons.TimeProfileAnalysis);
         pageAggregatedPKValues.Text = PKSimConstants.PKAnalysis.AggregatedPKValues;
         pageAggregatedPKValues.Tooltip = PKSimConstants.PKAnalysis.AggregatedPKValuesTooltip;

         btnExportToExcel.InitWithImage(ApplicationIcons.Excel, text: PKSimConstants.UI.ExportPKAnalysesToExcel);
         layoutItemExportToExcel.AdjustLargeButtonSize(layoutControl);
         layoutControlItemGlobalPKAnalysis.TextVisible = false;
         populationPKAnalysisTabControl.SelectedPageChanged += (o, e) => OnEvent(_presenter.HandleTabChanged);
         labelControlGlobalPKAnalysisDescription.Text = PKSimConstants.UI.GlobalPKAnalysisDescription.FormatForLabel();
      }
   }
}