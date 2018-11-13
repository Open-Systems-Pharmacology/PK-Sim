using System.Collections.Generic;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.Populations;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation;
using OSPSuite.Presentation.Nodes;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Populations
{
   public partial class PopulationParameterGroupsView : BaseUserControl, IPopulationParameterGroupsView
   {
      private IPopulationParameterGroupsPresenter _presenter;
      private readonly UxImageTreeView _treeGroups;

      public PopulationParameterGroupsView(IImageListRetriever imageListRetriever)
      {
         InitializeComponent();
         filterTreeView.ShowDescendantNode = true;
         _treeGroups = filterTreeView.TreeView;
         _treeGroups.StateImageList = imageListRetriever.AllImages16x16;
      }

      public void AttachPresenter(IPopulationParameterGroupsPresenter presenter)
      {
         _presenter = presenter;
      }

      public ITreeNode SelectedNode
      {
         get { return _treeGroups.SelectedNode; }
      }

      public bool EnableFilter
      {
         set { filterTreeView.EnableFilter = value; }
      }

      public override void InitializeBinding()
      {
         _treeGroups.SelectedNodeChanged += activateNode;
      }

      private void activateNode(ITreeNode node)
      {
         OnEvent(() => _presenter.NodeSelected(node));
      }

      private void nodeDoubleClick(ITreeNode node)
      {
         OnEvent(() => _presenter.NodeDoubleClicked(node));
      }

      public void AddNodes(IEnumerable<ITreeNode> nodesToAdd)
      {
         removeHandlers();
         _treeGroups.DoWithinBatchUpdate(() => nodesToAdd.Each(_treeGroups.AddNode));
         addHandlers();
      }

      public ITreeNode NodeFor(IParameter parameter)
      {
         return _treeGroups.NodeWithTag(parameter);
      }

      public ITreeNode NodeById(string id)
      {
         return _treeGroups.NodeById(id);
      }

      public void RemoveNode(ITreeNode node)
      {
         _treeGroups.DestroyNode(node);
      }

      public void RemoveNodes(IEnumerable<ITreeNode> nodesToDelete)
      {
         removeHandlers();
         _treeGroups.DoWithinBatchUpdate(() => nodesToDelete.Each(RemoveNode));
         addHandlers();
      }

  

      private void addHandlers()
      {
         _treeGroups.SelectedNodeChanged += activateNode;
         _treeGroups.NodeDoubleClick += nodeDoubleClick;
      }

      private void removeHandlers()
      {
         _treeGroups.SelectedNodeChanged -= activateNode;
         _treeGroups.NodeDoubleClick -= nodeDoubleClick;
      }

      public void Clear()
      {
         _treeGroups.DestroyAllNodes();
      }

      public void SelectNode(ITreeNode node)
      {
         _treeGroups.SelectNode(node);
      }
   }
}