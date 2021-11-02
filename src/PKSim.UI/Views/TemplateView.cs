using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using OSPSuite.Assets;
using OSPSuite.Core.Extensions;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.UI.Services;
using OSPSuite.UI.Views;
using PKSim.Assets;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;

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

         _colUrl = _gridViewBinder.AutoBind(x => x.Url)
            .WithRepository(x => _urlRepository);

         var colDescription = _gridViewBinder.AutoBind(x => x.Description)
            .AsReadOnly();

         _colButtons = _gridViewBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(repositoryForTemplate)
            .WithFixedWidth(UIConstants.Size.EMBEDDED_BUTTON_WIDTH * 2);

         gridView.PreviewFieldName = colDescription.PropertyName;
         colDescription.Visible = false;
         colDatabaseType.XtraColumn.GroupIndex = 0;

         _editRemoveRepository.ButtonClick += (o, e) => OnEvent(() => editRemoveRepositoryButtonClick(o, e, _gridViewBinder.FocusedElement));
      }

      private RepositoryItem repositoryForTemplate(TemplateDTO templateDTO)
      {
         return _presenter.CanEdit(templateDTO) ? _editRemoveRepository : _disabledRepository;
      }

      private RepositoryItemButtonEdit createEditRemoveButtonRepository()
      {
         var schemaItemButtonRepository = new RepositoryItemButtonEdit {TextEditStyle = TextEditStyles.HideTextEditor};
         schemaItemButtonRepository.Buttons[0].Kind = ButtonPredefines.Ellipsis;
         schemaItemButtonRepository.Buttons[0].ToolTip = PKSimConstants.MenuNames.Rename;
         schemaItemButtonRepository.Buttons.Add(new EditorButton(ButtonPredefines.Delete) {ToolTip = PKSimConstants.MenuNames.Delete});
         return schemaItemButtonRepository;
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
         _colUrl.Visible = availableTemplates.Any(x => x.Url.StringIsNotEmpty());
         _colVersion.Visible = availableTemplates.Any(x => x.Version.StringIsNotEmpty());
         gridView.BestFitColumns();
      }

      public string Description
      {
         get => lblDescription.Text;
         set => lblDescription.Text = value;
      }
   }
}