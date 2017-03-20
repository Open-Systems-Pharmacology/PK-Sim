using OSPSuite.Presentation.Nodes;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.Core;
using PKSim.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Repositories;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class PopulationFolderContextMenu : BuildingBlockFolderContextMenu<Population>
   {
      public PopulationFolderContextMenu(IMenuBarItemRepository repository, IBuildingBlockRepository buildingBlockRepository)
         : base(repository, buildingBlockRepository, MenuBarItemIds.NewPopulation, MenuBarItemIds.LoadPopulation, MenuBarItemIds.ImportPopulation)
      {
      }
   }

   public class PopulationFolderTreeNodeContextMenuFactory : RootNodeContextMenuFactory
   {
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public PopulationFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository, IBuildingBlockRepository buildingBlockRepository)
         : base(PKSimRootNodeTypes.PopulationFolder, repository)
      {
         _buildingBlockRepository = buildingBlockRepository;
      }

      public override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new PopulationFolderContextMenu(_repository, _buildingBlockRepository);
      }
   }
}