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
   public class FormulationFolderContextMenu : BuildingBlockFolderContextMenu<Formulation>
   {
      public FormulationFolderContextMenu(IMenuBarItemRepository repository, IBuildingBlockRepository buildingBlockRepository)
         : base(repository, buildingBlockRepository, MenuBarItemIds.NewFormulation, MenuBarItemIds.LoadFormulationFromTemplate)
      {
      }
   }

   public class FormulationFolderTreeNodeContextMenuFactory : RootNodeContextMenuFactory
   {
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public FormulationFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository, IBuildingBlockRepository buildingBlockRepository)
         : base(PKSimRootNodeTypes.FormulationFolder, repository)
      {
         _buildingBlockRepository = buildingBlockRepository;
      }

      public override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new FormulationFolderContextMenu(_repository, _buildingBlockRepository);
      }
   }
}