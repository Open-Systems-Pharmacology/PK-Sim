using System.Windows.Forms;
using OSPSuite.UI.Services;
using OSPSuite.Assets;
using DevExpress.XtraBars;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views
{
   public partial class BuildingBlockFromTemplateView : BaseModalView, IBuildingBlockFromTemplateView, IViewWithPopup
   {
      private ITemplatePresenter _presenter;
      private readonly BarManager _barManager;
      public bool Updating { get; private set; }

      public BuildingBlockFromTemplateView(IImageListRetriever imageListRetriever, Shell shell)
         : base(shell)
      {
         InitializeComponent();
         treeView.StateImageList = imageListRetriever.AllImagesForTreeView;
         toolTipController.Initialize(imageListRetriever);
         treeView.ShouldExpandAddedNode = true;
         _barManager = new BarManager {Form = this, Images = imageListRetriever.AllImagesForContextMenu};
         treeView.NodeClick += nodeClick;
         tbDescription.Enabled = false;
      }

      private void nodeClick(MouseEventArgs e, ITreeNode selectedNode)
      {
         if (e.Button != MouseButtons.Right) return;
         _presenter.CreatePopupMenuFor(selectedNode).At(e.Location);
      }

      public void AttachPresenter(ITemplatePresenter presenter)
      {
         _presenter = presenter;
      }

      public IUxTreeView TreeView => treeView;

      public ITreeNode AddNode(ITreeNode node)
      {
         treeView.SelectedNodeChanged -= templateSelected;
         treeView.AddNode(node);
         treeView.SelectedNodeChanged += templateSelected;
         return node;
      }

      private void templateSelected(ITreeNode node)
      {
         OnEvent(() => _presenter.ActivateNode(node));
      }

      public void SetIcon(ApplicationIcon icon)
      {
         Icon = icon;
      }

      public string Description
      {
         set { tbDescription.Text = value; }
      }

      public BarManager PopupBarManager => _barManager;

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