using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Presentation.DTO;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Format;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Individuals
{
   public partial class ExpressionParametersView<TExpressionParameterDTO> : BaseUserControlWithValueInGrid,
      IExpressionParametersView<TExpressionParameterDTO> where TExpressionParameterDTO : ExpressionParameterDTO
   {
      protected readonly IToolTipCreator _toolTipCreator;
      protected readonly IImageListRetriever _imageListRetriever;

      protected readonly GridViewBinder<TExpressionParameterDTO> _gridViewBinder;
      private IGridViewColumn _colGrouping;
      private IGridViewColumn _colParameterValue;
      private IGridViewColumn _colParameterName;
      private IGridViewColumn _colCompartment;

      private readonly ToolTipController _toolTipController = new ToolTipController();
      private readonly RepositoryItemTextEdit _standardParameterEditRepository = new RepositoryItemTextEdit();
      public bool EmphasisRelativeExpressionParameters { get; set; } = false;

      public virtual bool ReadOnly
      {
         set => _colParameterValue.ReadOnly = value;
      }

      private readonly RepositoryItemProgressBar _progressBarRepository = new RepositoryItemProgressBar
         {Minimum = 0, Maximum = 100, PercentView = true, ShowTitle = true};

      private readonly UxRepositoryItemButtonImage _isFixedParameterEditRepository =
         new UxRepositoryItemButtonImage(ApplicationIcons.Reset, PKSimConstants.UI.ResetParameterToolTip)
            {TextEditStyle = TextEditStyles.Standard};

      protected IExpressionParametersPresenter<TExpressionParameterDTO> _presenter;

      private readonly ScreenBinder<IExpressionParametersPresenter<TExpressionParameterDTO>> _screenBinder =
         new ScreenBinder<IExpressionParametersPresenter<TExpressionParameterDTO>>();


      public ExpressionParametersView(IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever)
      {
         _toolTipCreator = toolTipCreator;
         _imageListRetriever = imageListRetriever;

         InitializeComponent();
         InitializeWithGrid(_gridView);
         initializeRepositories();

         _toolTipController.GetActiveObjectInfo += onToolTipControllerGetActiveObjectInfo;
         _toolTipController.Initialize(_imageListRetriever);

         _gridViewBinder = new GridViewBinder<TExpressionParameterDTO>(_gridView);
      }

      protected override void InitializeWithGrid(UxGridView gridView)
      {
         base.InitializeWithGrid(gridView);
         gridView.ShouldUseColorForDisabledCell = true;
         gridView.OptionsView.AllowCellMerge = true;
         gridView.GroupFormat = "[#image]{1}";
         gridView.RowCellStyle += updateRowCellStyle;
         gridView.ShowingEditor += onShowingEditor;
         gridView.EndGrouping += (o, e) => gridView.ExpandAllGroups();
         gridView.CustomColumnSort += customColumnSort;
         gridView.GridControl.ToolTipController = _toolTipController;
         gridView.CellMerge += (o, e) => OnEvent(onCellMerge, e);
      }

      private void onCellMerge(CellMergeEventArgs e)
      {
         var p1 = _gridViewBinder.ElementAt(e.RowHandle1);
         var p2 = _gridViewBinder.ElementAt(e.RowHandle2);

         if (p1 == null || p2 == null)
            return;

         var representSameOrgan = string.Equals(p1.ContainerName, p2.ContainerName);

         e.Handled = !ShouldMergeCell(e.Column, p1, p2, representSameOrgan);
      }

      protected virtual bool ShouldMergeCell(GridColumn column, TExpressionParameterDTO p1, TExpressionParameterDTO p2, bool representSameOrgan)
      {
         //We only merge compartment values if they represent the same organ
         if (Equals(column, _colCompartment.XtraColumn))
            return  representSameOrgan;

         return true;
      }

      private void onShowingEditor(object sender, CancelEventArgs e)
      {
         if (_gridView.FocusedColumn == null)
            return;

         if (_gridView.FocusedColumn.ReadOnly)
            e.Cancel = true;
         
         e.Cancel = !CanEditValueAt(_gridViewBinder.FocusedElement);
      }

      protected virtual bool CanEditValueAt(TExpressionParameterDTO expressionParameterDTO)
      {
         var parameterDTO = _gridViewBinder.FocusedElement?.Parameter;
         return parameterDTO != null && _presenter.CanEditParameter(parameterDTO);
      }

      private void customColumnSort(object sender, CustomColumnSortEventArgs e)
      {
         if (e.Column != _colGrouping.XtraColumn) return;
         var container1 = e.RowObject1 as ExpressionParameterDTO;
         var container2 = e.RowObject2 as ExpressionParameterDTO;
         if (container1 == null || container2 == null) return;
         e.Handled = true;

         e.Result = container1.Sequence.CompareTo(container2.Sequence);
         if (e.Result != 0)
            return;

         if (container1.ContainerName != container2.ContainerName)
            return;

         //Same container and compartment, return
         if (container1.CompartmentName == container2.CompartmentName)
            return;

         //One of the two has an empty compartment. We pull this one ahead
         if (!string.IsNullOrEmpty(container1.CompartmentName) && !string.IsNullOrEmpty(container2.CompartmentName))
            return;

         // -1 will move the container1 above container2
         e.Result = string.IsNullOrEmpty(container1.CompartmentName) ? -1 : 1;

         //         Debug.Print($"{container1} && {container2}  = {e.Result}");
      }

      //https://github.com/DevExpress-Examples/custom-gridcontrol-how-to-hide-particular-grouprow-headers-footers-t264208/blob/16.1.4%2B/CS/CustomGridControl/MyDataController.cs

      public virtual void AttachPresenter(IExpressionParametersPresenter<TExpressionParameterDTO> presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(IEnumerable<TExpressionParameterDTO> expressionParameters)
      {
         _gridViewBinder.BindToSource(expressionParameters);
         _screenBinder.BindToSource(_presenter);
      }

      protected void InitializeGroupBinding()
      {
         _colGrouping = _gridViewBinder.AutoBind(item => item.GroupingPathDTO)
            .WithRepository(dto => configureContainerRepository(dto.GroupingPathDTO))
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .AsReadOnly();
         _colGrouping.XtraColumn.GroupIndex = 0;
         _colGrouping.XtraColumn.SortMode = ColumnSortMode.Custom;
      }

      public override void InitializeBinding()
      {
         InitializeGroupBinding();
         InitializeContainerBinding();
         InitializeParameterNameBinding();
         InitializeValueBinding();
         InitializeShowInitialConcentrationBinding();
      }

      protected void InitializeShowInitialConcentrationBinding()
      {
         _screenBinder.Bind(x => x.ShowInitialConcentration)
            .To(chkShowInitialConcentration)
            .WithCaption(PKSimConstants.UI.ShowInitialConcentrationParameter);
      }

      protected void InitializeValueBinding()
      {
         _colParameterValue = _gridViewBinder.AutoBind(item => item.Value)
            .WithFormat(parameterFormatter)
            .WithRepository(repoForParameter)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithOnValueUpdating((o, e) => OnEvent(() => _presenter.SetExpressionParameterValue(o.Parameter, e.NewValue)))
            .WithCaption(PKSimConstants.UI.Value);

         _colParameterValue.XtraColumn.OptionsColumn.AllowMerge = DefaultBoolean.False;

         var col = _gridViewBinder.AutoBind(item => item.NormalizedExpressionPercent)
            .WithCaption(PKSimConstants.UI.RelativeExpressionNorm)
            .WithRepository(repoForNormalizedExpression)
            .AsReadOnly();

         //necessary to align center since double value are aligned right by default
         col.XtraColumn.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
         col.XtraColumn.DisplayFormat.FormatType = FormatType.None;
         col.XtraColumn.OptionsColumn.AllowMerge = DefaultBoolean.False;
      }

      protected void InitializeParameterNameBinding()
      {
         _colParameterName = _gridViewBinder.AutoBind(item => item.ParameterName)
            .WithCaption(PKSimConstants.UI.Parameter)
            .AsReadOnly();

         _colParameterName.XtraColumn.OptionsColumn.AllowMerge = DefaultBoolean.False;
      }

      protected void InitializeContainerBinding()
      {
         _gridViewBinder.AutoBind(item => item.ContainerPathDTO)
            .WithRepository(dto => configureContainerRepository(dto.ContainerPathDTO))
            .WithCaption(PKSimConstants.UI.Organ)
            .AsReadOnly();


         _colCompartment= _gridViewBinder.AutoBind(item => item.CompartmentPathDTO)
            .WithRepository(dto => configureContainerRepository(dto.CompartmentPathDTO))
            .WithCaption(PKSimConstants.UI.Compartment)
            .AsReadOnly();
      }

      private IFormatter<double> parameterFormatter(TExpressionParameterDTO expressionParameterDTO)
      {
         return expressionParameterDTO.Parameter.ParameterFormatter();
      }

      private void onToolTipControllerGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         var expressionParameterDTO = _gridViewBinder.ElementAt(e);
         if (expressionParameterDTO == null)
            return;

         //check if subclass want to display a tool tip as well
         var superToolTip = GetToolTipFor(expressionParameterDTO, _gridView.HitInfoAt(e.ControlMousePosition));
         if (superToolTip == null)
            return;

         e.Info = _toolTipCreator.ToolTipControlInfoFor(expressionParameterDTO, superToolTip);
      }

      protected virtual SuperToolTip GetToolTipFor(TExpressionParameterDTO expressionParameterDTO, GridHitInfo hi)
      {
         //don't show tooltips for value as it might contain error info that would be hidden
         if (hi.Column == _colParameterValue.XtraColumn)
            return null;

         return _toolTipCreator.ToolTipFor(expressionParameterDTO.Parameter);
      }

      protected override bool ColumnIsValue(GridColumn gridColumn)
      {
         if (_colParameterValue == null)
            return false;

         return _colParameterValue.XtraColumn == gridColumn;
      }

      private RepositoryItem repoForParameter(TExpressionParameterDTO expressionDTO)
      {
         if (_presenter.IsSetByUser(expressionDTO.Parameter))
            return _isFixedParameterEditRepository;

         return _standardParameterEditRepository;
      }

      private RepositoryItem repoForNormalizedExpression(TExpressionParameterDTO expressionDTO)
      {
         if (expressionDTO.NormalizedExpressionPercent == null)
            return _standardParameterEditRepository;

         return _progressBarRepository;
      }

      private void resetParameter()
      {
         _presenter.ResetParameter(_gridViewBinder.FocusedElement?.Parameter);
         _gridView.CloseEditor();
      }

      private void initializeRepositories()
      {
         _standardParameterEditRepository.ConfigureWith(typeof(double));
         _standardParameterEditRepository.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
         _isFixedParameterEditRepository.Buttons[0].IsLeft = true;
         _isFixedParameterEditRepository.ButtonClick += (o, e) => OnEvent(resetParameter);
      }

      private RepositoryItem configureContainerRepository(PathElement pathElement)
      {
         var containerRepository = new UxRepositoryItemImageComboBox(_gridView, _imageListRetriever);
         return containerRepository.AddItem(pathElement, pathElement.IconName);
      }

      protected override void OnValueColumnMouseDown(UxGridView gridView, GridColumn col, int rowHandle)
      {
         var parameterDTO = _gridViewBinder.ElementAt(rowHandle)?.Parameter;
         if (parameterDTO == null)
            return;

         _gridView.EditorShowMode = _presenter.IsSetByUser(parameterDTO) ? EditorShowMode.Default : EditorShowMode.MouseUp;
      }

      private void updateRowCellStyle(object sender, RowCellStyleEventArgs e)
      {
         var expressionDTO = _gridViewBinder.ElementAt(e.RowHandle);

         //Make sure the name and value of a relative expression parameter are using a bold font
         if (shouldEmphasisCellAppearance(expressionDTO, e))
            e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Bold);
         else if (e.Column.OptionsColumn.ReadOnly)
            _gridView.AdjustAppearance(e, true);
         else
            UpdateExpressionParameterAppearance(expressionDTO, e);
      }

      protected virtual void UpdateExpressionParameterAppearance(TExpressionParameterDTO expressionParameterDTO, RowCellStyleEventArgs e)
      {
         var parameterDTO = expressionParameterDTO?.Parameter;
         if (parameterDTO == null)
            return;

         if (!parameterDTO.Parameter.Editable)
            _gridView.AdjustAppearance(e, isEnabled: false);

         else if (_presenter.IsSetByUser(parameterDTO))
            _gridView.AdjustAppearance(e, PKSimColors.Changed);

         else
            e.CombineAppearance(_gridView.Appearance.Row);
      }

      private bool shouldEmphasisCellAppearance(TExpressionParameterDTO expressionDTO, RowCellStyleEventArgs e) =>
         EmphasisRelativeExpressionParameters &&
         expressionDTO.NormalizedExpressionPercent != null &&
         e.Column.IsOneOf(_colParameterName.XtraColumn, _colParameterValue.XtraColumn);

      public override bool HasError => _gridViewBinder.HasError;
   }
}