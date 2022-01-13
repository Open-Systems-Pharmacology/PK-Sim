using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraBars;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Parameters;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Views;
using OSPSuite.Assets;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Services;
using OSPSuite.UI.Views;
using OSPSuite.Utility.Extensions;

namespace PKSim.UI.Views.Parameters
{
   public partial class ParameterGroupsView : BaseUserControl, IParameterGroupsView, IViewWithPopup
   {
      private IParameterGroupsPresenter _presenter;
      private readonly ScreenBinder<IParameterGroupsPresenter> _groupModeBinder = new ScreenBinder<IParameterGroupsPresenter>();
      private readonly UxImageTreeView _treeView;
      public BarManager PopupBarManager { get; }

      public ParameterGroupsView(IImageListRetriever imageListRetriever)
      {
         InitializeComponent();
         _toolTipController.Initialize(imageListRetriever);
         _treeView = _filterTreeView.TreeView;
         _filterTreeView.ShowDescendantNode = true;
         _treeView.StateImageList = imageListRetriever.AllImages16x16;
         _treeView.ToolTipController = _toolTipController;
         groupParameters.AppearanceCaption.TextOptions.HotkeyPrefix = HKeyPrefix.None;
         _toolTipController.GetActiveObjectInfo += (o, e) => _treeView.ShowToolTip(e);
         PopupBarManager = new BarManager {Form = this};
      }

      private void activateNode(NodeChangingEventArgs e)
      {
         OnEvent(() =>
         {
            if (!_presenter.CanClose)
               e.Cancel = true;
            else
               _presenter.ActivateNode(e.Node);
         });
      }

      public override void InitializeBinding()
      {
         _groupModeBinder.Bind(x => x.ParameterGroupingMode)
            .To(cbGroupingMode)
            .WithValues(x => _presenter.AllGroupingModes);

         _treeView.SelectedNodeChanging += activateNode;
         _treeView.NodeClick += (e, node) => OnEvent(() => nodeClick(e, node));
      }

      private void nodeClick(MouseEventArgs e, ITreeNode selectedNode)
      {
         if (e.Button != MouseButtons.Right)
            return;

         _presenter.CreatePopupMenuFor(selectedNode).At(e.Location);
      }

      public void AddNodes(IEnumerable<ITreeNode> nodesToAdd)
      {
         _treeView.SelectedNodeChanging -= activateNode;
         _treeView.Clear();
         nodesToAdd.Each(_treeView.AddNode);
         _treeView.SelectedNodeChanging += activateNode;
      }

      public void ActivateView(IView parametersView)
      {
         groupParameters.FillWith(parametersView);
      }

      public void SelectNodeById(string nodeId)
      {
         var node = _treeView.NodeById(nodeId);
         if (node == null)
            _treeView.SelectFocusedNodeOrFirst();
         else
            _treeView.SelectNode(node);
      }

      public void BindToGroupingMode(IParameterGroupsPresenter parameterGroupsPresenter)
      {
         _groupModeBinder.BindToSource(parameterGroupsPresenter);
      }

      public string GroupCaption
      {
         set => groupParameters.Text = value;
         get => groupParameters.Text;
      }

      public override string Caption => PKSimConstants.UI.Parameters;

      public ApplicationIcon Icon => ApplicationIcons.Parameters;

      public void AttachPresenter(IParameterGroupsPresenter presenter)
      {
         _presenter = presenter;
      }
   }
}