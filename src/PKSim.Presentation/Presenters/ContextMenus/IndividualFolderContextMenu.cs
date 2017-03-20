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
   public class IndividualFolderContextMenu : BuildingBlockFolderContextMenu<Individual>
   {
      public IndividualFolderContextMenu(IMenuBarItemRepository repository, IBuildingBlockRepository buildingBlockRepository)
         : base(repository, buildingBlockRepository, MenuBarItemIds.NewIndividual, MenuBarItemIds.LoadIndividual)
      {
      }
   }

   public class IndividualFolderTreeNodeContextMenuFactory : RootNodeContextMenuFactory
   {
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public IndividualFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository, IBuildingBlockRepository buildingBlockRepository)
         : base(PKSimRootNodeTypes.IndividualFolder, repository)
      {
         _buildingBlockRepository = buildingBlockRepository;
      }

      public override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new IndividualFolderContextMenu(_repository, _buildingBlockRepository);
      }
   }
}