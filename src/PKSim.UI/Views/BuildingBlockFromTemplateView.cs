using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OSPSuite.UI.Services;
using OSPSuite.Assets;
using DevExpress.XtraBars;
using DevExpress.XtraTreeList;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using PKSim.Core.Model;

namespace PKSim.UI.Views
{
   public partial class BuildingBlockFromTemplateView : BaseModalView, IBuildingBlockFromTemplateView, IViewWithPopup
   {
      private ITemplatePresenter _presenter;
      public bool Updating { get; private set; }

      public BuildingBlockFromTemplateView(IImageListRetriever imageListRetriever, Shell shell)
         : base(shell)
      {
         InitializeComponent();
         treeView.StateImageList = imageListRetriever.AllImagesForTreeView;
         toolTipController.Initialize(imageListRetriever);
         treeView.ShouldExpandAddedNode = true;
         PopupBarManager = new BarManager {Form = this, Images = imageListRetriever.AllImagesForContextMenu};
         treeView.NodeClick += nodeClick;
         tbDescription.Enabled = false;
         treeView.OptionsSelection.MultiSelect = true;
         treeView.OptionsSelection.MultiSelectMode = TreeListMultiSelectMode.RowSelect;
      }

      private void nodeClick(MouseEventArgs e, ITreeNode selectedNode)
      {
         if (e.Button == MouseButtons.Right)
         {
            selectNode(selectedNode);
            _presenter.CreatePopupMenuFor(selectedNode).At(e.Location);
         }
         else
         {
            var treeNodes = treeView.Selection.Select(treeView.NodeFrom).ToList();
            _presenter.ActivateNodes(treeNodes);
         }
      }

      private void selectNode(ITreeNode nodeToSelect)
      {
         treeView.Selection.Clear();

         if (nodeToSelect == null)
            return;

         //Ensure we only have one node selected for context menu
         treeView.FocusedNode = treeView.NodeFrom(nodeToSelect);
         treeView.Selection.Add(treeView.FocusedNode);
      }

      public void AttachPresenter(ITemplatePresenter presenter)
      {
         _presenter = presenter;
      }

      public IUxTreeView TreeView => treeView;

      public ITreeNode AddNode(ITreeNode node)
      {
         treeView.AddNode(node);
         return node;
      }

      public void SetIcon(ApplicationIcon icon)
      {
         Icon = icon;
      }

      public string Description
      {
         set => tbDescription.Text = value;
         get => tbDescription.Text;
      }

      public void SelectTemplate(Template template)
      {
         var node = treeView.NodeById(template.Id);
         treeView.SelectNode(node);
      }

      public BarManager PopupBarManager { get; }

      public void BeginUpdate()
      {
         treeView.BeginUpdate();
         Updating = true;
      }

      public void EndUpdate()
      {
         treeView.EndUpdate();
         Updating = false;
      }
   }
}