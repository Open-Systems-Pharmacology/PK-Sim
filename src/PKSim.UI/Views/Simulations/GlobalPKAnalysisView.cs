using OSPSuite.UI.Services;
using OSPSuite.Utility.Exceptions;
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraPivotGrid;
using PKSim.Assets;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Binders;
using OSPSuite.Assets;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;

namespace PKSim.UI.Views.Simulations
{
   public partial class GlobalPKAnalysisView : BaseUserControl, IGlobalPKAnalysisView
   {
      private IGlobalPKAnalysisPresenter _presenter;
      private readonly PivotGridField _compoundField;
      private readonly UxRepositoryItemButtonEdit _calculateBioAvailabilityRepository;
      private readonly UxRepositoryItemButtonEdit _calculateDDIRatioRepository;
      private readonly PivotGridUnitsMenuBinder _columUnitsMenuBinder;
      private readonly PivotGridField _parameterField;
      private readonly PivotGridField _valueField;
      private readonly IPKAnalysisToolTipManager _pkAnalysisToolTipManager;
      private RepositoryItemIconTextEdit _warningRepositoryEdit;

      public GlobalPKAnalysisView(IPKAnalysisToolTipManager pkAnalysisToolTipManager, IExceptionManager exceptionManager, IImageListRetriever imageListRetriever)
      {
         InitializeComponent();

         pivotGrid.OptionsBehavior.EditorShowMode = EditorShowMode.MouseDown;
         pivotGrid.ExceptionManager = exceptionManager;

         _calculateBioAvailabilityRepository = new UxRepositoryItemButtonImage(ApplicationIcons.Run, PKSimConstants.UI.CalculateBioavailability);
         _calculateDDIRatioRepository = new UxRepositoryItemButtonImage(ApplicationIcons.Run, PKSimConstants.UI.CalculateDDIRatio);

         _compoundField = new PivotGridField(PKSimConstants.PKAnalysis.Compound, PivotArea.ColumnArea);
         _parameterField = new PivotGridField(PKSimConstants.PKAnalysis.ParameterName, PivotArea.RowArea);
         _valueField = new PivotGridField(PKSimConstants.PKAnalysis.Value, PivotArea.DataArea) {SummaryType = PivotSummaryType.Custom};

         pivotGrid.AddParameterField(_parameterField);
         pivotGrid.AddValueField(_valueField);
         pivotGrid.AddField(_compoundField);
         pivotGrid.SetParameterDisplay(s => _presenter.DisplayNameFor(s));
         pivotGrid.ValueImages = imageListRetriever.AllImages16x16;
         _columUnitsMenuBinder = new PivotGridUnitsMenuBinder(pivotGrid, _parameterField);
         _pkAnalysisToolTipManager = pkAnalysisToolTipManager;
         _warningRepositoryEdit = new RepositoryItemIconTextEdit
         {
            ImageList = pivotGrid.ValueImages,
            ImageIndex = ApplicationIcons.Warning.Index
         };
      }

      public override void InitializeBinding()
      {
         pivotGrid.CustomCellEdit += (o, e) => OnEvent(onCustomCellEdit, e);
         _calculateBioAvailabilityRepository.ButtonClick += (o, e) => OnEvent(calculateBioAvailability, e);
         _calculateDDIRatioRepository.ButtonClick += (o, e) => OnEvent(calculateDDIRatio, e);
         _pkAnalysisToolTipManager.CreateForPivotGrid(pivotGrid);
      }

 
      private void calculateBioAvailability(ButtonPressedEventArgs e)
      {
         var compoundName = getCompoundNameFromCurrentFocusedCell();
         _presenter.CalculateBioAvailability(compoundName);
      }

      private string getCompoundNameFromCurrentFocusedCell()
      {
         var focusedCell = pivotGrid.Cells.FocusedCell;
         var cellInfo = pivotGrid.Cells.GetCellInfo(focusedCell.X, focusedCell.Y);
         var ds = cellInfo.CreateDrillDownDataSource();
         var compoundName = ds.StringValue(_compoundField);
         pivotGrid.CloseEditor();
         return compoundName;
      }

      private void calculateDDIRatio(ButtonPressedEventArgs e)
      {
         var compoundName = getCompoundNameFromCurrentFocusedCell();
         _presenter.CalculateDDIRatioFor(compoundName);
      }

      private void onCustomCellEdit(PivotCustomCellEditEventArgs e)
      {
         if (e.DataField != _valueField) return;
         var ds = e.CreateDrillDownDataSource();
         var parameterName = ds.StringValue(_parameterField);
         var compoundName = ds.StringValue(_compoundField);

         if (_presenter.ShouldCalculateBioAvailability(compoundName, parameterName))
            e.RepositoryItem = _calculateBioAvailabilityRepository;

         else if (_presenter.ShouldCalculateDDIRatio(compoundName, parameterName))
            e.RepositoryItem = _calculateDDIRatioRepository;

         else if (hasWarning(ds))
            e.RepositoryItem = _warningRepositoryEdit;
      }

      private bool hasWarning(PivotDrillDownDataSource ds)
      {
         var warning = ds.StringValue(PKSimConstants.PKAnalysis.Warning);
         return !string.IsNullOrEmpty(warning);
      }

      public void BindTo(GlobalPKAnalysisDTO globalPKAnalysisDTO)
      {
         pivotGrid.DataSource = globalPKAnalysisDTO.DataTable;
         pivotGrid.BestFitRowArea();
      }

      public void AttachPresenter(IGlobalPKAnalysisPresenter presenter)
      {
         _presenter = presenter;
         _columUnitsMenuBinder.BindTo(presenter);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemPivotGrid.TextVisible = false;
      }
   }
}