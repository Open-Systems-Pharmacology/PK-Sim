using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;
using OSPSuite.Assets;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.UI.Services;
using OSPSuite.UI.Views;
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
         gridView.EndGrouping += (o, e) => gridView.ExpandAllGroups();
         gridView.SelectionChanged += (o, e) => OnEvent(onGridViewSelectionChanged);
         toolTipController.Initialize(imageListRetriever);
         PopupBarManager = new BarManager {Form = this, Images = imageListRetriever.AllImagesForContextMenu};
         // gridView.SelectionChanged += (o, e) => OnEvent(_presenter.SelectedParametersChanged);
         // treeView.NodeClick += nodeClick;
         // tbDescription.Enabled = false;
         // treeView.OptionsSelection.MultiSelect = true;
         // treeView.OptionsSelection.MultiSelectMode = TreeListMultiSelectMode.RowSelect;
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

         var colDescription = _gridViewBinder.AutoBind(x => x.Description)
            .AsReadOnly();

         gridView.PreviewFieldName = colDescription.PropertyName;
         colDescription.Visible = false;
         colDatabaseType.XtraColumn.GroupIndex = 0;
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
         gridView.SelectRow(rowHandle);
         gridView.FocusedRowHandle = rowHandle;
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