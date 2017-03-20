using System.Linq;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Repositories;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public abstract class BuildingBlockFolderContextMenu<TBuildingBlock> : ContextMenu where TBuildingBlock : class, IPKSimBuildingBlock
   {
      protected BuildingBlockFolderContextMenu(IMenuBarItemRepository repository, IBuildingBlockRepository buildingBlockRepository, params MenuBarItemId[] staticMenus)
      {
         staticMenus.Each(menu => _view.AddMenuItem(repository[menu]));

         var allBuildingBlocks = buildingBlockRepository.All<TBuildingBlock>();
         if (!allBuildingBlocks.Any())
            return;

         _view.AddMenuItem(GenericMenu.ExportCollectionToPDFMenuFor<TBuildingBlock>().AsGroupStarter());
      }
   }
}