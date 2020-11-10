using System.Collections.Generic;
using System.ComponentModel;
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
   public partial class ExpressionParametersView : BaseUserControlWithValueInGrid, IExpressionParametersView
   {
      private readonly IToolTipCreator _toolTipCreator;
      private readonly IImageListRetriever _imageListRetriever;

      private readonly GridViewBinder<ExpressionParameterDTO> _gridViewBinder;
      private IGridViewColumn _colGrouping;
      private IGridViewColumn _colParameterValue;
      private IGridViewColumn _colParameterName;
      private readonly ToolTipController _toolTipController = new ToolTipController();
      private readonly RepositoryItemTextEdit _standardParameterEditRepository = new RepositoryItemTextEdit();

      private readonly RepositoryItemProgressBar _progressBarRepository = new RepositoryItemProgressBar
         { Minimum = 0, Maximum = 100, PercentView = true, ShowTitle = true };

      private readonly UxRepositoryItemButtonImage _isFixedParameterEditRepository =
         new UxRepositoryItemButtonImage(ApplicationIcons.Reset, PKSimConstants.UI.ResetParameterToolTip)
            { TextEditStyle = TextEditStyles.Standard };

      private IExpressionParametersPresenter _presenter;

      public ExpressionParametersView(IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever)
      {
         _toolTipCreator = toolTipCreator;
         _imageListRetriever = imageListRetriever;

         InitializeComponent();
         InitializeWithGrid(_gridView);
         initializeRepositories();


         _toolTipController.GetActiveObjectInfo += onToolTipControllerGetActiveObjectInfo;
         _toolTipController.Initialize(_imageListRetriever);

         _gridViewBinder = new GridViewBinder<ExpressionParameterDTO>(_gridView);
      }

      protected override void InitializeWithGrid(UxGridView gridView)
      {
         base.InitializeWithGrid(gridView);
         gridView.ShouldUseColorForDisabledCell = false;
         gridView.OptionsView.AllowCellMerge = true;
         gridView.GroupFormat = "[#image]{1}";
         gridView.RowCellStyle += updateRowCellStyle;
         gridView.ShowingEditor += onShowingEditor;
         gridView.EndGrouping += (o, e) => gridView.ExpandAllGroups();
         gridView.CustomColumnSort += customColumnSort;
         gridView.GridControl.ToolTipController = _toolTipController;
      }


      private void onShowingEditor(object sender, CancelEventArgs e)
      {
         var parameterDTO = _gridViewBinder.FocusedElement?.Parameter;
         if (parameterDTO == null) return;
         e.Cancel = !_presenter.CanEditParameter(parameterDTO);
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

      public void AttachPresenter(IExpressionParametersPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(IEnumerable<ExpressionParameterDTO> expressionParameters)
      {
         _gridViewBinder.BindToSource(expressionParameters);
      }

      public override void InitializeBinding()
      {
         _colGrouping = _gridViewBinder.AutoBind(item => item.GroupingPathDTO)
            .WithRepository(dto => configureContainerRepository(dto.GroupingPathDTO))
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .AsReadOnly();
         _colGrouping.XtraColumn.GroupIndex = 0;
         _colGrouping.XtraColumn.SortMode = ColumnSortMode.Custom;

         _gridViewBinder.AutoBind(item => item.ContainerPathDTO)
            .WithRepository(dto => configureContainerRepository(dto.ContainerPathDTO))
            .WithCaption(PKSimConstants.UI.Organ)
            .AsReadOnly();

         _gridViewBinder.AutoBind(item => item.CompartmentPathDTO)
            .WithRepository(dto => configureContainerRepository(dto.CompartmentPathDTO))
            .WithCaption(PKSimConstants.UI.Compartment)
            .AsReadOnly();

         _colParameterName = _gridViewBinder.AutoBind(item => item.ParameterName)
            .WithCaption(PKSimConstants.UI.Parameter)
            .AsReadOnly();

         _colParameterValue = _gridViewBinder.AutoBind(item => item.Value)
            .WithFormat(parameterFormatter)
            .WithRepository(repoForParameter)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithOnValueUpdating((o, e) => OnEvent(() => _presenter.SetExpressionParameterValue(o.Parameter, e.NewValue)))
            .WithCaption(PKSimConstants.UI.Value);

         _colParameterName.XtraColumn.OptionsColumn.AllowMerge = DefaultBoolean.False;
         _colParameterValue.XtraColumn.OptionsColumn.AllowMerge = DefaultBoolean.False;


         var col = _gridViewBinder.AutoBind(item => item.NormalizedExpressionPercent)
            .WithCaption(PKSimConstants.UI.RelativeExpressionNorm)
            .WithRepository(repoForNormalizedExpression)
            .AsReadOnly();

         //necessary to align center since double value are aligned right by default
         col.XtraColumn.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
         col.XtraColumn.DisplayFormat.FormatType = FormatType.None;
         col.XtraColumn.OptionsColumn.AllowMerge = DefaultBoolean.False;


         _isFixedParameterEditRepository.ButtonClick += (o, e) => OnEvent(resetParameter);
      }

      private IFormatter<double> parameterFormatter(ExpressionParameterDTO expressionParameterDTO)
      {
         return expressionParameterDTO.Parameter.ParameterFormatter();
      }

      private void onToolTipControllerGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         var parameterDTO = _gridViewBinder.ElementAt(e)?.Parameter;
         if (parameterDTO == null)
            return;

         //check if subclass want to display a tool tip as well
         var superToolTip = GetToolTipFor(parameterDTO, _gridView.HitInfoAt(e.ControlMousePosition));
         if (superToolTip == null)
            return;

         e.Info = _toolTipCreator.ToolTipControlInfoFor(parameterDTO, superToolTip);
      }

      protected virtual SuperToolTip GetToolTipFor(IParameterDTO parameterDTO, GridHitInfo hi)
      {
         //don't show tooltips for value as it might contain error info that would be hidden
         if (hi.Column == _colParameterValue.XtraColumn)
            return null;

         return _toolTipCreator.ToolTipFor(parameterDTO);
      }

      protected override bool ColumnIsValue(GridColumn gridColumn)
      {
         if (_colParameterValue == null)
            return false;

         return _colParameterValue.XtraColumn == gridColumn;
      }

      private RepositoryItem repoForParameter(ExpressionParameterDTO expressionDTO)
      {
         if (_presenter.IsSetByUser(expressionDTO.Parameter))
            return _isFixedParameterEditRepository;

         return _standardParameterEditRepository;
      }

      private RepositoryItem repoForNormalizedExpression(ExpressionParameterDTO expressionDTO)
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
         var parameterDTO = _gridViewBinder.ElementAt(e.RowHandle)?.Parameter;
         if (parameterDTO == null)
            return;

         if (e.Column.OptionsColumn.ReadOnly)
            _gridView.AdjustAppearance(e, true);

         else if (!parameterDTO.Parameter.Editable)
            _gridView.AdjustAppearance(e, isEnabled: false);

         else if (_presenter.IsSetByUser(parameterDTO))
            _gridView.AdjustAppearance(e, PKSimColors.Changed);
         else
            e.CombineAppearance(_gridView.Appearance.Row);
      }

      public override bool HasError => _gridViewBinder.HasError;
   }
}
