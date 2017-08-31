using System.Linq;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Repositories;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public abstract class BuildingBlockFolderContextMenu<TBuildingBlock> : ContextMenu where TBuildingBlock : class, IPKSimBuildingBlock
   {
      protected BuildingBlockFolderContextMenu(IMenuBarItemRepository repository, IBuildingBlockRepository buildingBlockRepository, params MenuBarItemId[] staticMenus)
      {
         staticMenus.Each(menu => _view.AddMenuItem(repository[menu]));

         _view.AddMenuItem(GenericMenu.LoadBuildingBlockFromSnapshot<TBuildingBlock>());

         var allBuildingBlocks = buildingBlockRepository.All<TBuildingBlock>();
         if (!allBuildingBlocks.Any())
            return;
      }
   }
}