using System.Collections.Generic;
using System.Drawing;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.Classifications;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Main;
using OSPSuite.Presentation.Regions;
using OSPSuite.Presentation.Services;
using OSPSuite.Presentation.Views;
using OSPSuite.Utility.Events;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Services;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation.Presenters.Main
{
   public interface IExplorerPresenter : OSPSuite.Presentation.Presenters.IExplorerPresenter, IMainViewItemPresenter,
      IListener<BuildingBlockAddedEvent>,
      IListener<BuildingBlockRemovedEvent>,
      IListener<SimulationRunStartedEvent>,
      IListener<SimulationRunFinishedEvent>
   {
   }

   public abstract class ExplorerPresenter<TView, TPresenter> : AbstractExplorerPresenter<TView, TPresenter>, IExplorerPresenter
      where TView : IView<TPresenter>, IExplorerView
      where TPresenter : IExplorerPresenter
   {
      protected readonly ITreeNodeFactory _treeNodeFactory;
      private readonly ITreeNodeContextMenuFactory _treeNodeContextMenuFactory;
      private readonly IMultipleTreeNodeContextMenuFactory _multipleTreeNodeContextMenuFactory;
      protected readonly IBuildingBlockIconRetriever _buildingBlockIconRetriever;
      protected readonly IBuildingBlockTask _buildingBlockTask;

      protected ExplorerPresenter(TView view, ITreeNodeFactory treeNodeFactory, ITreeNodeContextMenuFactory treeNodeContextMenuFactory,
         IMultipleTreeNodeContextMenuFactory multipleTreeNodeContextMenuFactory,
         IBuildingBlockIconRetriever buildingBlockIconRetriever, IRegionResolver regionResolver, IBuildingBlockTask buildingBlockTask, RegionName regionName,
         IProjectRetriever projectRetriever, IClassificationPresenter classificationPresenter, IToolTipPartCreator toolTipPartCreator) :
         base(view, regionResolver, classificationPresenter, toolTipPartCreator, regionName, projectRetriever)
      {
         _treeNodeFactory = treeNodeFactory;
         _treeNodeContextMenuFactory = treeNodeContextMenuFactory;
         _multipleTreeNodeContextMenuFactory = multipleTreeNodeContextMenuFactory;
         _buildingBlockIconRetriever = buildingBlockIconRetriever;
         _buildingBlockTask = buildingBlockTask;
      }

      protected abstract ITreeNode AddBuildingBlockToTree(IPKSimBuildingBlock buildingBlock);

      public void Handle(BuildingBlockAddedEvent eventToHandle)
      {
         var node = AddBuildingBlockToTree(eventToHandle.BuildingBlock);
         EnsureNodeVisible(node);
      }

      public void Handle(BuildingBlockRemovedEvent eventToHandle)
      {
         RemoveNodeFor(eventToHandle.BuildingBlock);
      }

      public void Handle(SimulationRunStartedEvent eventToHandle)
      {
         _view.Enabled = false;
      }

      public void Handle(SimulationRunFinishedEvent eventToHandle)
      {
         _view.Enabled = true;
      }

      public override void NodeDoubleClicked(ITreeNode node)
      {
         var buildingBlock = node.TagAsObject as IPKSimBuildingBlock;
         if (buildingBlock != null)
            EditBuildingBlock(buildingBlock);

         else if (IsFolderNode(node))
            base.NodeDoubleClicked(node);

         else
         {
            var contextMenu = _treeNodeContextMenuFactory.CreateFor(node, this);
            contextMenu.ActivateFirstMenu();
         }
      }

      public override void ShowContextMenu(IReadOnlyList<ITreeNode> treeNodes, Point popupLocation)
      {
         var contextMenu = _multipleTreeNodeContextMenuFactory.CreateFor(treeNodes, this);
         contextMenu.Show(View, popupLocation);
      }

      public override void ShowContextMenu(ITreeNode node, Point popupLocation)
      {
         var contextMenu = _treeNodeContextMenuFactory.CreateFor(node, this);
         contextMenu.Show(View, popupLocation);
      }

      protected void EditBuildingBlock(IPKSimBuildingBlock buildingBlock)
      {
         _buildingBlockTask.Edit(buildingBlock);
      }
   }
}