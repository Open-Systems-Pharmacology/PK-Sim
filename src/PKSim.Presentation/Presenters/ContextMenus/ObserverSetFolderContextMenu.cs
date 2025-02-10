using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Repositories;
using OSPSuite.Utility.Container;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.Core;
using PKSim.Presentation.Nodes;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class ObserverSetFolderContextMenu : BuildingBlockFolderContextMenu<ObserverSet>
   {
      public ObserverSetFolderContextMenu(IMenuBarItemRepository repository, IBuildingBlockRepository buildingBlockRepository, IContainer container)
         : base(repository, buildingBlockRepository, container, MenuBarItemIds.NewObserverSet, MenuBarItemIds.LoadObserverSet)
      {
      }
   }

   public class ObserverSetFolderTreeNodeContextMenuFactory : RootNodeContextMenuFactory
   {
      private readonly IBuildingBlockRepository _buildingBlockRepository;
      private readonly IContainer _container;

      public ObserverSetFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository, IBuildingBlockRepository buildingBlockRepository, IContainer container)
         : base(PKSimRootNodeTypes.ObserverSetFolder, repository)
      {
         _buildingBlockRepository = buildingBlockRepository;
         _container = container;
      }

      public override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new ObserverSetFolderContextMenu(_repository, _buildingBlockRepository, _container);
      }
   }
}