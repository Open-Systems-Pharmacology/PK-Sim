using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Presentation.DTO;
using OSPSuite.UI;
using OSPSuite.UI.Binders;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.UI.Services;
using OSPSuite.UI.Views;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.UI.Extensions;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Parameters
{
   public partial class ParameterSetView : BaseUserControlWithValueInGrid, IViewWithPopup, OSPSuite.Presentation.Views.IView<IParameterSetPresenter>
   {
      protected readonly IToolTipCreator _toolTipCreator;
      private readonly IImageListRetriever _imageListRetriever;
      private UxGridView _gridView;
      protected GridViewBinder<ParameterDTO> _gridViewBinder;
      private ComboBoxUnitParameter _comboBoxUnit;
      protected IGridViewColumn _columnValueDescription;
      protected IGridViewColumn _columnValue;
      private IParameterSetPresenter _presenter;
      private UxRepositoryItemImageComboBox _discreteParameterRepository;
      private readonly RepositoryItemTextEdit _stantdardParameterEditRepository = new RepositoryItemTextEdit();
      private readonly RepositoryItemButtonEdit _isFixedParameterEditRepository;
      private readonly UxRepositoryItemButtonEdit _editTableParameterRepository = new UxRepositoryItemButtonEdit(ButtonPredefines.Glyph);
      private readonly UxRepositoryItemButtonEdit _showTableParameterRepository = new UxRepositoryItemButtonEdit(ButtonPredefines.Glyph);
      private readonly ToolTipController _toolTipController;
      private RepositoryItem _favoriteRepository;
      protected IGridViewColumn _columnFavorites;
      private readonly PKSim.UI.Binders.ValueOriginBinder<ParameterDTO> _valueOriginBinder;

      public ParameterSetView(IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever, PKSim.UI.Binders.ValueOriginBinder<ParameterDTO> valueOriginBinder)
      {
         InitializeComponent();
         _imageListRetriever = imageListRetriever;
         _toolTipController = new ToolTipController();
         _toolTipCreator = toolTipCreator;
         _isFixedParameterEditRepository = new UxRepositoryItemButtonImage(ApplicationIcons.Reset, PKSimConstants.UI.ResetParameterToolTip) {TextEditStyle = TextEditStyles.Standard};
         _toolTipController.GetActiveObjectInfo += onToolTipControllerGetActiveObjectInfo;
         _toolTipController.Initialize(_imageListRetriever);
         PopupBarManager = new BarManager {Form = this, Images = imageListRetriever.AllImagesForContextMenu};
         _stantdardParameterEditRepository.ConfigureWith(typeof(double));
         _stantdardParameterEditRepository.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
         _isFixedParameterEditRepository.Buttons[0].IsLeft = true;
         _valueOriginBinder = valueOriginBinder;
      }

      public void Initialize(UxGridView gridView)
      {
         _gridView = gridView;
         _gridView.HiddenEditor += (o, e) => hideEditor();
         _gridView.ShowingEditor += onShowingEditor;
         _gridView.RowCellStyle += updateRowCellStyle;
         _gridView.GridControl.ToolTipController = _toolTipController;
         _gridView.GroupFormat = "[#image]{1}";
         _gridView.EndGrouping += (o, e) => _gridView.ExpandAllGroups();
         _comboBoxUnit = new ComboBoxUnitParameter(_gridView.GridControl);
         _favoriteRepository = new UxRepositoryItemCheckEdit(_gridView);
         _discreteParameterRepository = new UxRepositoryItemImageComboBox(_gridView, _imageListRetriever) {LargeImages = _imageListRetriever.AllImages16x16};
         InitializeWithGrid(_gridView);
         _gridViewBinder = new GridViewBinder<ParameterDTO>(_gridView)
         {
            ValidationMode = ValidationMode.LeavingRow,
            BindingMode = BindingMode.OneWay
         };
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         _editTableParameterRepository.Buttons[0].Caption = PKSimConstants.UI.EditTable;
         _showTableParameterRepository.Buttons[0].Caption = PKSimConstants.UI.ShowTable;
      }

      public void AttachPresenter(IParameterSetPresenter presenter)
      {
         _presenter = presenter;
      }

      protected void InitializeValueDescriptionBinding()
      {
         _valueOriginBinder.InitializeBinding(_gridViewBinder, updateValueOrigin);
         //TODO MBD
//         _columnValueDescription = _gridViewBinder.AutoBind(param => param.ValueDescription)
//            .WithWidth(UIConstants.Size.EMBEDDED_DESCRIPTION_WIDTH)
//            .WithCaption(PKSimConstants.UI.ValueDescription)
//            .WithOnValueUpdating((o, e) => OnEvent(() => _presenter.SetParameterValueDescription(o, e.NewValue)));
      }

      private void updateValueOrigin(ParameterDTO parameterDTO , ValueOrigin newValueOrigin)
      {
         OnEvent(() => _presenter.SetParameterValueOrigin(parameterDTO, newValueOrigin));
      }

      protected void InitializeValueBinding()
      {
         //Use Autobind to enable error notification
         _columnValue = _gridViewBinder.AutoBind(param => param.Value)
            .WithFormat(p => p.ParameterFormatter())
            .WithCaption(PKSimConstants.UI.Value)
            .WithRepository(repoForParameter)
            .WithEditorConfiguration(configureRepository)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithOnValueUpdating((o, e) => OnEvent(() => _presenter.SetParameterValue(o, e.NewValue)));

         _comboBoxUnit.ParameterUnitSet += setParameterUnit;

         _editTableParameterRepository.ButtonClick += (o, e) => OnEvent(editTable);
         _showTableParameterRepository.ButtonClick += (o, e) => OnEvent(editTable);
         _isFixedParameterEditRepository.ButtonClick += (o, e) => OnEvent(resetParameter);
      }

      private void editTable()
      {
         _presenter.EditTableFor(_gridViewBinder.FocusedElement);
      }

      private void setParameterUnit(IParameterDTO parameterDTO, Unit newUnit)
      {
         OnEvent(() =>
         {
            _gridView.CloseEditor();
            _presenter.SetParameterUnit(parameterDTO, newUnit);
         });
      }

      protected void InitializeFavoriteBinding()
      {
         _columnFavorites = _gridViewBinder.Bind(x => x.IsFavorite)
            .WithCaption(PKSimConstants.UI.Favorites)
            .WithWidth(UIConstants.Size.EMBEDDED_CHECK_BOX_WIDTH)
            .WithRepository(x => _favoriteRepository)
            .WithToolTip(PKSimConstants.UI.FavoritesToolTip)
            .WithOnValueUpdating((o, e) => OnEvent(() => _presenter.SetFavorite(o, e.NewValue)));
      }

      private void resetParameter()
      {
         _presenter.ResetParameter(_gridViewBinder.FocusedElement);
         _gridView.CloseEditor();
      }

      public bool FavoritesVisible
      {
         set => _columnFavorites.UpdateVisibility(value);
      }

      private void configureRepository(BaseEdit activeEditor, IParameterDTO parameter)
      {
         if (parameter.FormulaType == FormulaType.Table)
            return;

         if (parameter.IsDiscrete)
            activeEditor.FillImageComboBoxEditorWith(parameter.ListOfValues.Keys, x => -1, d => parameter.ListOfValues[d]);
         else
            _comboBoxUnit.UpdateUnitsFor(activeEditor, parameter);
      }

      private RepositoryItem repoForParameter(IParameterDTO parameterDTO)
      {
         if (parameterDTO.IsDiscrete)
            return _discreteParameterRepository;

         if (parameterDTO.FormulaType == FormulaType.Table)
            return parameterDTO.Editable ? _editTableParameterRepository : _showTableParameterRepository;

         if (_presenter.IsSetByUser(parameterDTO))
            return _isFixedParameterEditRepository;

         return _stantdardParameterEditRepository;
      }

      protected override bool ColumnIsValue(GridColumn gridColumn)
      {
         if (_columnValue == null) return false;
         return _columnValue.XtraColumn == gridColumn;
      }

      protected override void OnValueColumnMouseDown(UxGridView gridView, GridColumn col, int rowHandle)
      {
         var parameterDTO = _gridViewBinder.ElementAt(rowHandle);
         if (parameterDTO == null) return;

         _gridView.EditorShowMode = hasTextEditor(parameterDTO) ? EditorShowMode.MouseUp : EditorShowMode.Default;
      }

      private bool hasTextEditor(IParameterDTO parameterDTO)
      {
         return parameterDTO.FormulaType != FormulaType.Table &&
                !_presenter.IsSetByUser(parameterDTO) &&
                !parameterDTO.IsDiscrete;
      }

      private void onToolTipControllerGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         var parameterDTO = _gridViewBinder.ElementAt(e);
         if (parameterDTO == null) return;

         //check if subclass want to display a tool tip as well
         var superToolTip = GetToolTipFor(parameterDTO, _gridView.HitInfoAt(e.ControlMousePosition));
         if (superToolTip == null)
            return;

         e.Info = _toolTipCreator.ToolTipControlInfoFor(parameterDTO, superToolTip);
      }

      protected virtual SuperToolTip GetToolTipFor(IParameterDTO parameterDTO, GridHitInfo hi)
      {
         //don't show tooltips for value as it might contain error info that would be hidden
         if (hi.Column == _columnValue.XtraColumn)
            return null;

         return _toolTipCreator.ToolTipFor(parameterDTO);
      }

      private void onShowingEditor(object sender, CancelEventArgs e)
      {
         var parameterDTO = _gridViewBinder.FocusedElement;
         if (parameterDTO == null) return;
         if (ColumnIsAlwaysActive(_gridView.FocusedColumn)) return;
         e.Cancel = !_presenter.CanEditParameter(parameterDTO);
      }

      private void hideEditor()
      {
         _comboBoxUnit.Hide();
      }

      protected override void OnGridViewMouseDown(UxGridView gridView, MouseEventArgs e)
      {
         if (e.Button == MouseButtons.Left)
         {
            base.OnGridViewMouseDown(_gridView, e);
         }
         else if (e.Button == MouseButtons.Right)
         {
            //valid parameter?
            var parameterDTO = _gridViewBinder.ElementAt(_gridView.RowHandleAt(e));
            if (parameterDTO == null) return;
            _gridView.PostEditor();
            ShowPopup(parameterDTO, e.Location);
         }
      }

      protected virtual void ShowPopup(IParameterDTO parameterDTO, Point location)
      {
         //should be overriden if popup are required for given view
      }

      public override bool HasError
      {
         get { return _gridViewBinder.HasError; }
      }

      private void updateRowCellStyle(object sender, RowCellStyleEventArgs e)
      {
         var parameterDTO = _gridViewBinder.ElementAt(e.RowHandle);
         if (parameterDTO == null) return;

         if (ColumnIsAlwaysActive(e.Column))
            _gridView.AdjustAppearance(e, true);

         else if (e.Column.OptionsColumn.ReadOnly)
            _gridView.AdjustAppearance(e, false);

         else if (!parameterDTO.Parameter.Editable)
            _gridView.AdjustAppearance(e, false);

         else if (_presenter.IsFormulaNotFixed(parameterDTO))
            _gridView.AdjustAppearance(e, PKSimColors.Formula);

         else if (_presenter.IsSetByUser(parameterDTO))
            _gridView.AdjustAppearance(e, PKSimColors.Changed);
         else
            e.CombineAppearance(_gridView.Appearance.Row);
      }

      protected virtual bool ColumnIsAlwaysActive(GridColumn column)
      {
         return false;
      }

      public BarManager PopupBarManager { get; }
   }
}