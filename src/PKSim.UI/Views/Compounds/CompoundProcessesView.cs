using System.Windows.Forms;
using OSPSuite.UI.Services;
using OSPSuite.UI.Extensions;
using OSPSuite.Assets;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraGrid;
using DevExpress.XtraTreeList;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views.Compounds
{
   public partial class CompoundProcessesView : BaseUserControl, ICompoundProcessesView, IViewWithPopup
   {
      private ICompoundProcessesPresenter _presenter;
      private readonly BarManager _barManager;
      public bool Updating { get; private set; }

      public CompoundProcessesView(IImageListRetriever imageListRetriever)
      {
         InitializeComponent();
         treeView.StateImageList = imageListRetriever.AllImagesForTreeView;
         treeView.OptionsBehavior.AllowExpandOnDblClick = false;
         _barManager = new BarManager {Form = this, Images = imageListRetriever.AllImagesForContextMenu};
         groupProcesses.AppearanceCaption.TextOptions.HotkeyPrefix = HKeyPrefix.None;
         treeView.DataColumn.SortOrder = SortOrder.Ascending;
         treeView.DataColumn.SortMode = ColumnSortMode.Custom;
         treeView.CompareNodeValues += compareNodeValues;
      }

      public void AttachPresenter(ICompoundProcessesPresenter presenter)
      {
         _presenter = presenter;
      }

      private void compareNodeValues(object sender, CompareNodeValuesEventArgs e)
      {
         //we only want to sort for the top nodes (level 0)
         if (e.Node1 == null)
            return;

         //the first two levels are defined in presenter.
         if (e.Node1.Level < 2)
            e.Result = 0;
      }

      public override void InitializeBinding()
      {
         treeView.NodeClick += nodeClick;
         treeView.NodeDoubleClick += node => OnEvent(() => _presenter.NodeDoubleClicked(node));
         treeView.SelectedNodeChanged += processSelected;
      }

      private void nodeClick(MouseEventArgs e, ITreeNode selectedNode)
      {
         switch (e.Button)
         {
            case MouseButtons.Left:
               break;
            case MouseButtons.Right:
               OnEvent(() => _presenter.CreatePopupMenuFor(selectedNode).At(e.Location));
               break;
            default:
               break;
               //nothing to do
         }
      }

      public string GroupCaption
      {
         set { groupProcesses.Text = value; }
      }

      public void Clear()
      {
         groupProcesses.Clear();
      }

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.BiologicalProperties;

      public IUxTreeView TreeView => treeView;

      public ITreeNode AddNode(ITreeNode nodeToAdd)
      {
         treeView.SelectedNodeChanged -= processSelected;
         treeView.AddNode(nodeToAdd);
         treeView.SelectedNodeChanged += processSelected;
         return nodeToAdd;
      }

      public void ActivateView(IView view)
      {
         groupProcesses.FillWith(view);
      }

      public override void InitializeResources()
      {
         Caption = PKSimConstants.UI.ADME;
      }

      private void processSelected(ITreeNode node)
      {
         OnEvent(() => _presenter.ActivateNode(node));
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