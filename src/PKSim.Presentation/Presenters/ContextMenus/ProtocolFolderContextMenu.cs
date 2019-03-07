using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Repositories;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.Core;
using PKSim.Presentation.Nodes;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class ProtocolFolderContextMenu : BuildingBlockFolderContextMenu<Protocol>
   {
      public ProtocolFolderContextMenu(IMenuBarItemRepository repository, IBuildingBlockRepository buildingBlockRepository)
         : base(repository, buildingBlockRepository, MenuBarItemIds.NewProtocol, MenuBarItemIds.LoadProtocol)
      {
      }
   }

   public class ProtocolFolderTreeNodeContextMenuFactory : RootNodeContextMenuFactory
   {
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public ProtocolFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository, IBuildingBlockRepository buildingBlockRepository)
         : base(PKSimRootNodeTypes.ProtocolFolder, repository)
      {
         _buildingBlockRepository = buildingBlockRepository;
      }

      public override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new ProtocolFolderContextMenu(_repository, _buildingBlockRepository);
      }
   }
}