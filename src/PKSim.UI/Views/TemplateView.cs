using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using OSPSuite.Assets;
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
   public partial class TemplateView : BaseModalView, ITemplateView, IViewWithPopup
   {
      private readonly IImageListRetriever _imageListRetriever;
      private ITemplatePresenter _presenter;
      private readonly GridViewBinder<TemplateDTO> _gridViewBinder;
      private readonly RepositoryItemButtonEdit _editRemoveRepository;
      private readonly RepositoryItemTextEdit _disabledRepository;
      public bool Updating { get; private set; }

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
         gridView.GroupFormat = "[#image]{1}";
         gridView.ShowingEditor += (o, e) => OnEvent(onShowingEditor, o, e);
         gridView.EndGrouping += (o, e) => gridView.ExpandAllGroups();
         gridView.SelectionChanged += (o, e) => OnEvent(onGridViewSelectionChanged);
         toolTipController.Initialize(imageListRetriever);
         PopupBarManager = new BarManager {Form = this, Images = imageListRetriever.AllImagesForContextMenu};
         _editRemoveRepository = createEditRemoveButtonRepository();
         _disabledRepository = new RepositoryItemTextEdit {Enabled = false, ReadOnly = true};
      }

      private void onShowingEditor(object sender, CancelEventArgs e)
      {
         var templateDTO = _gridViewBinder.FocusedElement;
         if (templateDTO == null) return;
         e.Cancel = !_presenter.CanEdit(templateDTO);
      }

      private void onGridViewSelectionChanged() => _presenter.SelectedTemplatesChanged(SelectedTemplates);

      public IReadOnlyList<TemplateDTO> SelectedTemplates => dtoListFrom(gridView.GetSelectedRows());

      private IReadOnlyList<TemplateDTO> dtoListFrom(IEnumerable<int> rowHandles) => rowHandles.Select(_gridViewBinder.ElementAt).ToList();

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         _gridViewBinder.AutoBind(x => x.Name)
            .AsReadOnly();

         var colDatabaseType = _gridViewBinder.AutoBind(x => x.DatabaseType)
            .WithRepository(templateTypeRepository)
            .AsReadOnly();

         _gridViewBinder.AutoBind(x => x.Version)
            .AsReadOnly();

         var colDescription = _gridViewBinder.AutoBind(x => x.Description)
            .AsReadOnly();

         _gridViewBinder.AddUnboundColumn()
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
         return templateRepository.AddItem(template.DatabaseType, template.Icon);
      }
      // private void nodeClick(MouseEventArgs e, ITreeNode selectedNode)
      // {
      //    if (e.Button == MouseButtons.Right)
      //    {
      //       selectNode(selectedNode);
      //       _presenter.CreatePopupMenuFor(selectedNode).At(e.Location);
      //    }
      //    else
      //    {
      //       var treeNodes = treeView.Selection.Select(treeView.NodeFrom).ToList();
      //       _presenter.ActivateNodes(treeNodes);
      //    }
      // }

      // private void selectNode(ITreeNode nodeToSelect)
      // {
      //    treeView.Selection.Clear();
      //
      //    if (nodeToSelect == null)
      //       return;
      //
      //    //Ensure we only have one node selected for context menu
      //    treeView.FocusedNode = treeView.NodeFrom(nodeToSelect);
      //    treeView.Selection.Add(treeView.FocusedNode);
      // }

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
      }

      public BarManager PopupBarManager { get; }

      public void BeginUpdate()
      {
         gridView.BeginUpdate();
         Updating = true;
      }

      public void EndUpdate()
      {
         gridView.EndUpdate();
         Updating = false;
      }
   }
}