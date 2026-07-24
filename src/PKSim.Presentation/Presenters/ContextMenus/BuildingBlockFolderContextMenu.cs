using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Repositories;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public abstract class BuildingBlockFolderContextMenu<TBuildingBlock> : ContextMenu where TBuildingBlock : class, IPKSimBuildingBlock
   {
      protected BuildingBlockFolderContextMenu(ITreeNode<RootNodeType> treeNode, IMenuBarItemRepository repository, IExplorerPresenter presenter, IContainer container, params MenuBarItemId[] staticMenus) : base(container)
      {
         staticMenus.Each(menu => _view.AddMenuItem(repository[menu]));

         _view.AddMenuItem(GenericMenu.LoadBuildingBlockFromSnapshot<TBuildingBlock>(container));

         _view.AddMenuItem(ClassificationCommonContextMenuItems.CreateClassificationUnderMenu(treeNode, presenter).AsGroupStarter());

         var groupMenu = createGroupByMenu(treeNode, presenter);
         if (groupMenu.AllItems().Any())
            _view.AddMenuItem(groupMenu);

         _view.AddMenuItem(ClassificationCommonContextMenuItems.RemoveClassificationFolderMainMenu(treeNode, presenter).AsGroupStarter());
      }

      private static IMenuBarSubMenu createGroupByMenu(ITreeNode<RootNodeType> treeNode, IExplorerPresenter presenter)
      {
         var groupMenu = CreateSubMenu.WithCaption(MenuNames.GroupBy);

         presenter.AvailableClassificationCategories(treeNode)
            .Each(classification => groupMenu.AddItem(
               CreateMenuButton.WithCaption(classification.ClassificationName)
                  .WithActionCommand(() => presenter.AddToClassificationTree(treeNode, classification.ClassificationName))));

         return groupMenu;
      }
   }
}
