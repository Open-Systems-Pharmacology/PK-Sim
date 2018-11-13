using System.Windows.Forms;
using OSPSuite.UI.Services;
using OSPSuite.UI.Extensions;
using OSPSuite.Assets;
using DevExpress.XtraBars;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Presentation;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views.Individuals
{
   public partial class MoleculesView : BaseUserControl, IMoleculesView, IViewWithPopup
   {
      private IMoleculesPresenter _presenter;
      private readonly BarManager _barManager;
      public bool Updating { get; private set; }

      public MoleculesView(IImageListRetriever imageListRetriever)
      {
         InitializeComponent();
         treeView.StateImageList = imageListRetriever.AllImagesForTreeView;
         treeView.OptionsBehavior.AllowExpandOnDblClick = false;
         _barManager = new BarManager {Form = this, Images = imageListRetriever.AllImagesForContextMenu};
      }

      public override void InitializeBinding()
      {
         treeView.NodeClick += nodeClick;
         treeView.NodeDoubleClick += nodeDoubleClicked;
         treeView.SelectedNodeChanged += nodeSelected;
      }

      private void nodeDoubleClicked(ITreeNode selectedNode)
      {
         this.DoWithinWaitCursor(() => OnEvent(() => _presenter.NodeDoubleClicked(selectedNode)));
      }

      private void nodeClick(MouseEventArgs e, ITreeNode selectedNode)
      {
         switch (e.Button)
         {
            case MouseButtons.Left:
               break;
            case MouseButtons.Right:
               _presenter.CreatePopupMenuFor(selectedNode).At(e.Location);
               break;
            default:
               break;
            //nothing to do
         }
      }

      public void ActivateView(IView expressionsView)
      {
         groupExpressions.FillWith(expressionsView);
      }

      public string GroupCaption
      {
         set => groupExpressions.Text = value;
      }

      public void AttachPresenter(IMoleculesPresenter presenter)
      {
         _presenter = presenter;
      }

      public override string Caption => PKSimConstants.UI.Expression;

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.ProteinExpression;

      public IUxTreeView TreeView => treeView;

      public ITreeNode AddNode(ITreeNode nodeToAdd)
      {
         treeView.SelectedNodeChanged -= nodeSelected;
         treeView.AddNode(nodeToAdd);
         treeView.SelectedNodeChanged += nodeSelected;
         return nodeToAdd;
      }

      private void nodeSelected(ITreeNode proteinNode)
      {
         this.DoWithinWaitCursor(() => OnEvent(() => _presenter.ActivateNode(proteinNode)));
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