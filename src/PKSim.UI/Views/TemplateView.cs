using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using OSPSuite.Assets;
using OSPSuite.Core.Extensions;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.UI.Services;
using OSPSuite.UI.Views;
using PKSim.Assets;
using PKSim.Core.Services;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;
using static OSPSuite.UI.UIConstants.Size;

namespace PKSim.UI.Views
{
   public partial class TemplateView : BaseModalView, ITemplateView
   {
      private readonly IImageListRetriever _imageListRetriever;
      private ITemplatePresenter _presenter;
      private readonly GridViewBinder<TemplateDTO> _gridViewBinder;
      private readonly RepositoryItemButtonEdit _editRemoveRepository;
      private readonly RepositoryItemTextEdit _disabledRepository;
      private readonly RepositoryItemHyperLinkEdit _urlRepository;
      private IGridViewColumn _colVersion;
      private IGridViewColumn _colUrl;
      private IGridViewColumn _colButtons;
      private readonly NullableBooleanFormatter _booleanFormatter = new NullableBooleanFormatter();
      private IGridViewColumn _colQualified;
      private readonly ScreenBinder<ITemplatePresenter> _screenBinder = new ScreenBinder<ITemplatePresenter> {BindingMode = BindingMode.TwoWay};

      public TemplateView(IImageListRetriever imageListRetriever, Shell shell)
         : base(shell)
      {
         _imageListRetriever = imageListRetriever;
         InitializeComponent();
         _gridViewBinder = new GridViewBinder<TemplateDTO>(gridView);
         gridView.MultiSelect = true;
         gridView.OptionsView.ShowPreview = true;
         gridView.OptionsView.AutoCalcPreviewLineCount = true;
         gridView.ShouldUseColorForDisabledCell = false;
         // Prevent the focused cell from being highlighted.
         gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
         // Draw a dotted focus rectangle around the entire row.
         gridView.FocusRectStyle = DrawFocusRectStyle.None;
         gridView.GroupFormat = "[#image]{1}";
         gridView.ShowingEditor += (o, e) => OnEvent(onShowingEditor, o, e);
         gridView.EndGrouping += (o, e) => gridView.ExpandAllGroups();
         gridView.SelectionChanged += (o, e) => OnEvent(onGridViewSelectionChanged);
         toolTipController.Initialize(imageListRetriever);
         _disabledRepository = new RepositoryItemTextEdit {Enabled = false, ReadOnly = true};
         _editRemoveRepository = createEditRemoveButtonRepository();
         _urlRepository = new RepositoryItemHyperLinkEdit {TextEditStyle = TextEditStyles.DisableTextEditor, SingleClick = true};
         lblDescription.AsDescription();
         gridView.PopupMenuShowing += (o, e) => OnEvent(onPopupMenuShowing, o, e);
      }

      private void onPopupMenuShowing(object o, PopupMenuShowingEventArgs e)
      {
         var gridViewMenu = e.Menu;
         if (gridViewMenu == null)
            return;

         //we only show the context menu if we are selecting cells and nothing else
         if (e.HitInfo.HitTest != GridHitTest.RowCell)
            return;

         //we allow delete only if we have items that can be edited (depends on dev mode)
         var editableTemplates = SelectedTemplates.Where(_presenter.CanEdit).ToList();
         if (!editableTemplates.Any())
            return;

         var deleteSelectedMenuItem = new DXMenuItem(PKSimConstants.MenuNames.Delete, (obj, args) => _presenter.Delete(editableTemplates), ApplicationIcons.Delete);
         gridViewMenu.Items.Insert(0, deleteSelectedMenuItem);
      }

      private void onShowingEditor(object sender, CancelEventArgs e)
      {
         if (gridView.FocusedColumn != _colButtons.XtraColumn)
            return;

         var templateDTO = _gridViewBinder.FocusedElement;
         if (templateDTO == null)
            return;

         e.Cancel = !_presenter.CanEdit(templateDTO);
      }

      private void onGridViewSelectionChanged() => _presenter.SelectedTemplatesChanged(SelectedTemplates);

      public IReadOnlyList<TemplateDTO> SelectedTemplates => dtoListFrom(gridView.GetSelectedRows());

      private IReadOnlyList<TemplateDTO> dtoListFrom(IEnumerable<int> rowHandles)
      {
         //Selecting header might select the same DTO twice so we use a hashset to get unique dtos only
         var allSelectedDTO = new HashSet<TemplateDTO>(rowHandles.Select(_gridViewBinder.ElementAt));
         return allSelectedDTO.ToList();
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         _gridViewBinder.AutoBind(x => x.Name)
            .AsReadOnly();

         var colDatabaseType = _gridViewBinder.AutoBind(x => x.DatabaseTypeDisplay)
            .WithRepository(templateTypeRepository)
            .WithCaption(PKSimConstants.UI.TemplateSource)
            .AsReadOnly();

         _colVersion = _gridViewBinder.AutoBind(x => x.Version)
            .AsReadOnly();

         _colVersion.XtraColumn.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;

         _colQualified = _gridViewBinder.AutoBind(x => x.Qualified)
            .WithFormat(_booleanFormatter)
            .AsReadOnly();

         _colQualified.XtraColumn.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;

         _colUrl = _gridViewBinder.AutoBind(x => x.Url)
            .WithRepository(x => _urlRepository);

         var colDescription = _gridViewBinder.AutoBind(x => x.Description)
            .AsReadOnly();

         _colButtons = _gridViewBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(repositoryForTemplate)
            .WithFixedWidth(EMBEDDED_BUTTON_WIDTH * 2);

         gridView.PreviewFieldName = colDescription.PropertyName;
         colDescription.Visible = false;
         colDatabaseType.XtraColumn.GroupIndex = 0;

         _editRemoveRepository.ButtonClick += (o, e) => OnEvent(() => editRemoveRepositoryButtonClick(o, e, _gridViewBinder.FocusedElement));

         _screenBinder.Bind(x => x.ShowOnlyQualifiedTemplate)
            .To(chkShowQualifiedTemplate)
            .WithCaption(PKSimConstants.UI.OnlyShowQualifiedTemplates);
      }

      private RepositoryItem repositoryForTemplate(TemplateDTO templateDTO)
      {
         return _presenter.CanEdit(templateDTO) ? _editRemoveRepository : _disabledRepository;
      }

      private RepositoryItemButtonEdit createEditRemoveButtonRepository()
      {
         var repository = new RepositoryItemButtonEdit {TextEditStyle = TextEditStyles.HideTextEditor};
         repository.Buttons[0].Kind = ButtonPredefines.Ellipsis;
         repository.Buttons[0].ToolTip = PKSimConstants.MenuNames.Rename;
         repository.Buttons.Add(new EditorButton(ButtonPredefines.Delete) {ToolTip = PKSimConstants.MenuNames.Delete});
         return repository;
      }

      private void editRemoveRepositoryButtonClick(object sender, ButtonPressedEventArgs e, TemplateDTO templateDTO)
      {
         var editor = (ButtonEdit) sender;
         var buttonIndex = editor.Properties.Buttons.IndexOf(e.Button);
         if (buttonIndex == 0)
            _presenter.Rename(templateDTO);
         else
            _presenter.Delete(templateDTO);
      }

      private RepositoryItem templateTypeRepository(TemplateDTO template)
      {
         var templateRepository = new UxRepositoryItemImageComboBox(gridView, _imageListRetriever);
         return templateRepository.AddItem(template.DatabaseTypeDisplay, template.Icon);
      }

      public void AttachPresenter(ITemplatePresenter presenter)
      {
         _presenter = presenter;
      }

      public void SetIcon(ApplicationIcon icon)
      {
         Icon = icon;
      }

      public void SelectTemplate(TemplateDTO templateDTO)
      {
         if (templateDTO == null)
            return;

         var rowHandle = _gridViewBinder.RowHandleFor(templateDTO);
         gridView.FocusedRowHandle = rowHandle;
         gridView.SelectRow(rowHandle);
      }

      public void BindTo(IReadOnlyList<TemplateDTO> availableTemplates)
      {
         _gridViewBinder.BindToSource(availableTemplates);
         //First column is name
         var colIndex = 1;
         updateColumnVisibility(_colVersion, availableTemplates.Any(x => x.Version.StringIsNotEmpty()), colIndex++);
         updateColumnVisibility(_colQualified, availableTemplates.Any(x => x.Qualified.HasValue), colIndex++);
         updateColumnVisibility(_colUrl, availableTemplates.Any(x => x.Url.StringIsNotEmpty()), colIndex++);
         updateColumnVisibility(_colButtons, availableTemplates.Any(_presenter.CanEdit), colIndex++);

         gridView.BestFitColumns();
         _screenBinder.BindToSource(_presenter);
      }

      private void updateColumnVisibility(IGridViewColumn column, bool visible, int visibleIndex)
      {
         column.XtraColumn.Visible = visible;
         if (visible)
            column.XtraColumn.VisibleIndex = visibleIndex;
      }

      public string Description
      {
         get => lblDescription.Text;
         set => lblDescription.Text = value;
      }
   }
}