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
   public class ExpressionProfileFolderContextMenu : BuildingBlockFolderContextMenu<ExpressionProfile>
   {
      public ExpressionProfileFolderContextMenu(IMenuBarItemRepository repository, IBuildingBlockRepository buildingBlockRepository)
         : base(repository, buildingBlockRepository, MenuBarItemIds.NewExpressionProfile, MenuBarItemIds.LoadExpressionProfile)
      {
      }
   }

   public class ExpressionProfileFolderTreeNodeContextMenuFactory : RootNodeContextMenuFactory
   {
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public ExpressionProfileFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository, IBuildingBlockRepository buildingBlockRepository)
         : base(PKSimRootNodeTypes.ExpressionProfileFolder, repository)
      {
         _buildingBlockRepository = buildingBlockRepository;
      }

      public override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new ExpressionProfileFolderContextMenu(_repository, _buildingBlockRepository);
      }
   }
}