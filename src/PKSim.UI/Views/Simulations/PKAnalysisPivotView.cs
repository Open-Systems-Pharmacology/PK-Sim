using System.Data;
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraPivotGrid;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Binders;
using OSPSuite.Assets;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Exceptions;

namespace PKSim.UI.Views.Simulations
{
   public partial class PKAnalysisPivotView : BaseUserControl, IPKAnalysisPivotView
   {
      private readonly IPKAnalysisToolTipManager _pkAnalysisToolTipManager;
      protected IToolTipCreator _toolTipCreator;
      protected PivotGridUnitsMenuBinder _columUnitsMenuBinder;
      private IPKAnalysisPresenter _presenter;
      private readonly PivotGridField _valueField;
      private readonly RepositoryItemIconTextEdit _warningRepositoryEdit;

      public PKAnalysisPivotView(IPKAnalysisToolTipManager pkAnalysisToolTipManager, IExceptionManager exceptionManager, IImageListRetriever imageListRetriever)
      {
         _pkAnalysisToolTipManager = pkAnalysisToolTipManager;
         InitializeComponent();
         pivotGrid.ToolTipController = new ToolTipController();
         pivotGrid.ExceptionManager = exceptionManager;

         var compoundName = new PivotGridField(PKSimConstants.PKAnalysis.Compound, PivotArea.ColumnArea);
         compoundName.Options.AllowExpand = DefaultBoolean.False;
         var curveField = new PivotGridField(PKSimConstants.PKAnalysis.CurveName, PivotArea.ColumnArea);
         curveField.Options.AllowExpand = DefaultBoolean.False;
         var parameterField = new PivotGridField(PKSimConstants.PKAnalysis.ParameterName, PivotArea.RowArea);
         _valueField = new PivotGridField(PKSimConstants.PKAnalysis.Value, PivotArea.DataArea) {SummaryType = PivotSummaryType.Custom};
         pivotGrid.AddParameterField(parameterField);
         pivotGrid.AddValueField(_valueField);
         pivotGrid.AddField(compoundName);
         pivotGrid.AddField(curveField);
         pivotGrid.SetParameterDisplay(s => _presenter.DisplayNameFor(s));
         pivotGrid.ValueImages = imageListRetriever.AllImages16x16;
         _columUnitsMenuBinder = new PivotGridUnitsMenuBinder(pivotGrid, parameterField);
         pivotGrid.PopupMenuShowing += (o, e) => OnEvent(() => onPopupMenuShowing(e));
         pivotGrid.CustomCellEdit += (o, e) => OnEvent(onCustomCellEdit, e);
         _warningRepositoryEdit = new RepositoryItemIconTextEdit
         {
            ImageList = pivotGrid.ValueImages,
            ImageIndex = ApplicationIcons.Warning.Index
         };
      }

      private void onCustomCellEdit(PivotCustomCellEditEventArgs e)
      {
         if (e.DataField != _valueField) return;
         var ds = e.CreateDrillDownDataSource();
         if (!hasWarning(ds))
            return;

         e.RepositoryItem = _warningRepositoryEdit;
      }

      private void onPopupMenuShowing(PopupMenuShowingEventArgs e)
      {
         if (e.MenuType != PivotGridMenuType.FieldValue && e.MenuType != PivotGridMenuType.HeaderArea)
         {
            e.Allow = false;
         }
      }

      public void BindTo(DataTable dataTable)
      {
         pivotGrid.DataSource = dataTable;
         if (dataTable.Rows.Count == 0)
         {
            layoutItemPivot.Visibility = LayoutVisibilityConvertor.FromBoolean(false);
            return;
         }
         layoutItemPivot.Visibility = LayoutVisibilityConvertor.FromBoolean(true);
         _pkAnalysisToolTipManager.CreateForPivotGrid(pivotGrid);

         pivotGrid.BestFitRowArea();
      }

      public DataTable GetSummaryData()
      {
         var dataTable = pivotGrid.GetCellsSummary();
         dataTable.TableName = PKSimConstants.UI.PKAnalyses;
         return dataTable;
      }

      public void BindUnitsMenuToPresenter(IPKAnalysisPresenter presenter)
      {
         _presenter = presenter;
         _columUnitsMenuBinder.BindTo(presenter);
      }

      private bool hasWarning(PivotDrillDownDataSource ds)
      {
         var warning = ds.StringValue(PKSimConstants.PKAnalysis.Warning);
         return !string.IsNullOrEmpty(warning);
      }
   }
}