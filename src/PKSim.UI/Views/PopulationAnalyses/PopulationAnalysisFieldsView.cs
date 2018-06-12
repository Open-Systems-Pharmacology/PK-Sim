using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Services;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout.Utils;
using PKSim.Assets;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using PKSim.UI.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Assets;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Views;
using UIConstants = OSPSuite.UI.UIConstants;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class PopulationAnalysisFieldsView : BaseUserControl, IPopulationAnalysisFieldsView, IViewWithPopup
   {
      private readonly IToolTipCreator _toolTipCreator;
      private IPopulationAnalysisFieldsPresenter _presenter;
      protected readonly GridViewBinder<PopulationAnalysisFieldDTO> _gridViewBinder;
      public BarManager PopupBarManager { get; private set; }
      private readonly UxRepositoryItemButtonEdit _removeField;
      private readonly UxRepositoryItemButtonEdit _editGroupAndRemoveField;
      private readonly UxRepositoryItemComboBox _unitComboBoxRepository;
      private readonly UxRepositoryItemComboBox _scalingComboBoxRepository;
      private readonly RepositoryItemTextEdit _repositoryItemDisabled;
      private IGridViewColumn _colDelete;
      private IGridViewColumn _colUnit;
      private IGridViewColumn _colScaling;
      private IGridViewColumn _colColor;
      private readonly ToolTipController _toolTipController;
      private readonly RepositoryItem _colorRepository;
      private const int _editButtonIndex = 0;
      private const int _deleteButtonIndex = 1;

      public PopulationAnalysisFieldsView(IImageListRetriever imageListRetriever, IToolTipCreator toolTipCreator)
      {
         _toolTipCreator = toolTipCreator;
         _toolTipController = new ToolTipController();
         _toolTipController.Initialize(imageListRetriever);

         InitializeComponent();
         _gridViewBinder = new GridViewBinder<PopulationAnalysisFieldDTO>(gridView);
         gridView.AllowsFiltering = false;
         gridView.OptionsSelection.EnableAppearanceFocusedRow = true;
         gridControl.ToolTipController = _toolTipController;

         //this makes sure that the field is not in edit mode as soon as we click on the field. This is required for a nice user experience with popup
         gridView.EditorShowMode = EditorShowMode.Click;
         PopupBarManager = new BarManager {Form = this, Images = imageListRetriever.AllImagesForContextMenu};

         _colorRepository = new UxRepositoryItemColorPickEditWithHistory(gridView);
         _unitComboBoxRepository = new UxRepositoryItemComboBox(gridView);
         _scalingComboBoxRepository = new UxRepositoryItemComboBox(gridView);
         _removeField = createEditAndRemoveRepo();
         _removeField.Buttons[_editButtonIndex].Enabled = false;
         _editGroupAndRemoveField = createEditAndRemoveRepo();

         _repositoryItemDisabled = new RepositoryItemTextEdit {Enabled = false, ReadOnly = true};
         _repositoryItemDisabled.CustomDisplayText += (o, e) => OnEvent(customDisplayText, e);

         _scalingComboBoxRepository.FillComboBoxRepositoryWith(EnumHelper.AllValuesFor<Scalings>());
         gridView.ShowingEditor += (o, e) => OnEvent(showingEditor, e);
         gridView.CustomDrawEmptyForeground += (o, e) => OnEvent(addMessageInEmptyArea, e);
      }

      private UxRepositoryItemButtonImage createEditAndRemoveRepo()
      {
         var repo = new UxRepositoryItemButtonImage(ApplicationIcons.Edit, PKSimConstants.UI.Edit);
         repo.AddButton(ButtonPredefines.Delete).ToolTip = PKSimConstants.UI.DeleteEntry;
         return repo;
      }

      public void AttachPresenter(IPopulationAnalysisFieldsPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _gridViewBinder.Bind(x => x.Name)
            .WithOnValueUpdating((o, e) => OnEvent(() => _presenter.FieldNameChanged(o, e.OldValue, e.NewValue)));

         _colUnit = _gridViewBinder.AutoBind(x => x.DisplayUnit)
            .WithCaption(PKSimConstants.UI.Unit)
            .WithRepository(repositoryItemForUnits)
            .WithEditorConfiguration(configureUnitsRepository)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithOnValueUpdating((o, e) => OnEvent(() => _presenter.FieldUnitChanged(o, e.OldValue, e.NewValue)));

         _colScaling = _gridViewBinder.Bind(x => x.Scaling)
            .WithCaption(Captions.Scaling)
            .WithRepository(repositoryItemForScaling)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithOnValueUpdating((o, e) => OnEvent(() => _presenter.FieldScalingChanged(o, e.OldValue, e.NewValue)));

         _colColor = _gridViewBinder.Bind(x => x.Color)
            .WithCaption(PKSimConstants.UI.Color)
            .WithRepository(x => _colorRepository)
            .WithOnValueUpdating((o, e) => OnEvent(() => _presenter.FieldColorChanged(o, e.OldValue, e.NewValue)));

         _colDelete = _gridViewBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(getAddRemoveRepository)
            .WithFixedWidth(UIConstants.Size.EMBEDDED_BUTTON_WIDTH * 2);

         gridView.FocusedRowChanged += (o, e) => OnEvent(gridViewRowChanged, e);
         gridView.MouseDown += (o, e) => OnEvent(onGridViewMouseDown, e);
         _removeField.ButtonClick += (o, e) => OnEvent(() => editOrRemoveFieldButtonClicked(e.Button, _gridViewBinder.FocusedElement.Field));
         _editGroupAndRemoveField.ButtonClick += (o, e) => OnEvent(() => editOrRemoveFieldButtonClicked(e.Button, _gridViewBinder.FocusedElement.Field));
         btnCreateDerivedField.Click += (o, e) => OnEvent(_presenter.CreateDerivedField);
         _toolTipController.GetActiveObjectInfo += (o, e) => OnEvent(onToolTipControllerGetActiveObjectInfo, e);
      }

      private void editOrRemoveFieldButtonClicked(EditorButton button, IPopulationAnalysisField field)
      {
         if (button.Index == _editButtonIndex)
            _presenter.EditDerivedField(field.DowncastTo<PopulationAnalysisDerivedField>());
         else
            _presenter.RemoveField(field);
      }

      private RepositoryItem getAddRemoveRepository(PopulationAnalysisFieldDTO popAnalysisFieldDTO)
      {
         if (popAnalysisFieldDTO.Field.IsAnImplementationOf<PopulationAnalysisDerivedField>())
            return _editGroupAndRemoveField;

         return _removeField;
      }

      private void addMessageInEmptyArea(CustomDrawEventArgs e)
      {
         gridView.AddMessageInEmptyArea(e, _presenter.EmptySelectionMessage);
      }

      private void onToolTipControllerGetActiveObjectInfo(ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         if (e.SelectedControl != gridControl) return;
         var populationAnalysisField = _gridViewBinder.ElementAt(e);
         if (populationAnalysisField == null) return;

         var superToolTip = _toolTipCreator.ToolTipFor(populationAnalysisField.Field);
         e.Info = _toolTipCreator.ToolTipControlInfoFor(populationAnalysisField.Field, superToolTip);
      }

      private RepositoryItem repositoryItemForUnits(PopulationAnalysisFieldDTO dto)
      {
         return dto.Units.Count > 1 ? _unitComboBoxRepository : _repositoryItemDisabled;
      }

      private RepositoryItem repositoryItemForScaling(PopulationAnalysisFieldDTO dto)
      {
         return dto.IsNumericField ? _scalingComboBoxRepository : _repositoryItemDisabled;
      }

      private void configureUnitsRepository(BaseEdit baseEdit, PopulationAnalysisFieldDTO dto)
      {
         var units = dto.Units;
         baseEdit.FillComboBoxEditorWith(units);
         baseEdit.Enabled = (units.Count > 1);
      }

      //do not allow editor to be shown when the field is not numeric
      private void showingEditor(CancelEventArgs e)
      {
         var dto = _gridViewBinder.FocusedElement;
         if (dto == null || dto.IsNumericField) return;
         if (!gridView.FocusedColumn.IsOneOf(_colScaling.XtraColumn))
            return;

         e.Cancel = true;
      }

      private void customDisplayText(CustomDisplayTextEventArgs e)
      {
         e.DisplayText = string.Empty;
      }

      private void onGridViewMouseDown(MouseEventArgs e)
      {
         if (e.Button == MouseButtons.Left)
         {
            gridView.EditorShowMode = editorModeForColumnAt(e);
         }
         else if (e.Button == MouseButtons.Right)
         {
            //popup
            var dto = _gridViewBinder.ElementAt(e.Location);
            if (dto == null) return;
            //required if feld was also being edited
            gridView.PostEditor();
            _presenter.CreatePopupMenuFor(dto.Field).At(e.Location);
         }
      }

      private EditorShowMode editorModeForColumnAt(MouseEventArgs e)
      {
         var col = gridView.ColumnAt(e);
         if (col.IsOneOf(_colDelete.XtraColumn, _colUnit.XtraColumn, _colScaling.XtraColumn, _colColor.XtraColumn))
            return EditorShowMode.Default;

         return EditorShowMode.Click;
      }

      private void gridViewRowChanged(FocusedRowChangedEventArgs e)
      {
         var selectedItem = _gridViewBinder.ElementAt(e.FocusedRowHandle);
         if (selectedItem == null) return;
         _presenter.FieldSelected(selectedItem.Field);
      }

      public virtual void BindTo(IEnumerable<PopulationAnalysisFieldDTO> parameterFields)
      {
         _gridViewBinder.BindToSource(parameterFields);
      }

      public PopulationAnalysisFieldDTO SelectedField
      {
         get => _gridViewBinder.FocusedElement;
         set
         {
            var rowHandle = _gridViewBinder.RowHandleFor(value);
            gridView.FocusedRowHandle = rowHandle;
         }
      }

      public bool CreateGroupingButtonEnabled
      {
         get => btnCreateDerivedField.Enabled;
         set => btnCreateDerivedField.Enabled = value;
      }

      public bool ScalingVisible
      {
         get => _colScaling.Visible;
         set => _colScaling.UpdateVisibility(value);
      }

      public bool CreateGroupingButtonVisible
      {
         get => layoutItemCreatedDerivedField.Visible;
         set
         {
            layoutItemCreatedDerivedField.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
            emptySpaceItem.Visibility = layoutItemCreatedDerivedField.Visibility;
         }
      }

      public bool ColorSelectionVisible
      {
         get => _colColor.Visible;
         set => _colColor.UpdateVisibility(value);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         btnCreateDerivedField.InitWithImage(ApplicationIcons.Create, PKSimConstants.UI.CreateGrouping);
         layoutItemCreatedDerivedField.AdjustLargeButtonSize();
      }
   }
}