using System.Windows.Forms;
using OSPSuite.UI.Services;
using OSPSuite.UI.Extensions;
using OSPSuite.Assets;
using DevExpress.XtraBars;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
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
      public bool Updating { get; private set; }

      public MoleculesView(IImageListRetriever imageListRetriever)
      {
         InitializeComponent();
         treeView.StateImageList = imageListRetriever.AllImagesForTreeView;
         treeView.OptionsBehavior.AllowExpandOnDblClick = false;
         PopupBarManager = new BarManager {Form = this, Images = imageListRetriever.AllImagesForContextMenu};
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

      public string LinkedExpressionProfileCaption
      {
         set => lblLinkedExpressionProfile.Text = value;
      }

      public void ActivateView(IView expressionsView)
      {
         panelExpression.FillWith(expressionsView);
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

      public override void InitializeResources()
      {
         base.InitializeResources();
         lblLinkedExpressionProfile.AsDescription();
         lblLinkedExpressionProfile.Text = string.Empty;
      }
   }
}