using System.Linq;
using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters.Main;
using PKSim.Presentation.UICommands;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Repositories;
using OSPSuite.Assets;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class ObservedDataFolderContextMenu : ContextMenu
   {
      public ObservedDataFolderContextMenu(
         ITreeNode<RootNodeType> treeNode,
         IMenuBarItemRepository repository,
         IBuildingBlockRepository buildingBlockRepository,
         IBuildingBlockExplorerPresenter presenter)
      {
         var allCompounds = buildingBlockRepository.All<Compound>().ToList();

         _view.AddMenuItem(repository[MenuBarItemIds.ImportObservedData]);

         //create sub menu containing all compounds
         var subMenu = CreateSubMenu.WithCaption(PKSimConstants.MenuNames.AddObservedDataFor)
            .WithIcon(ApplicationIcons.ObservedDataForMolecule);

         foreach (var compound in allCompounds)
         {
            subMenu.AddItem(CreateMenuButton.WithCaption(compound.Name)
               .WithCommandFor<AddObservedDataForCompoundUICommand, Compound>(compound));
         }

         if (allCompounds.Any())
            _view.AddMenuItem(subMenu);

         _view.AddMenuItem(repository[MenuBarItemIds.ImportFractionData].AsGroupStarter());

         _view.AddMenuItem(CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadFromTemplate)
            .WithCommand<LoadObservedDataFromTemplateUICommand>()
            .WithIcon(ApplicationIcons.LoadFromTemplate)
            .AsGroupStarter());

         _view.AddMenuItem(CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadFromSnapshot)
            .WithCommand<LoadObservedDataFromSnapshotUICommand>()
            //TODO ICON
            .WithIcon(ApplicationIcons.LoadFromTemplate));

         if (treeNode.AllLeafNodes.OfType<ObservedDataNode>().Any())
            _view.AddMenuItem(ObservedDataClassificationCommonContextMenuItems.CreateEditMultipleMetaDataMenuButton(treeNode).AsGroupStarter());


         _view.AddMenuItem(ClassificationCommonContextMenuItems.CreateClassificationUnderMenu(treeNode, presenter));

         var groupMenu = createGroupingSubMenu(treeNode, presenter);
         if (groupMenu.AllItems().Any())
            _view.AddMenuItem(groupMenu);

         _view.AddMenuItem(createDeleteSubMenu(treeNode, presenter));
      }

      private static IMenuBarSubMenu createGroupingSubMenu(ITreeNode<RootNodeType> treeNode, IBuildingBlockExplorerPresenter presenter)
      {
         var groupMenu = CreateSubMenu.WithCaption(MenuNames.GroupBy);

         presenter.AvailableClassificationCategories(treeNode)
            .Each(classification => groupMenu.AddItem(
               CreateMenuButton.WithCaption(classification.ClassificationName)
                  .WithActionCommand(() => presenter.AddToClassificationTree(treeNode, classification.ClassificationName))));

         return groupMenu;
      }

      private static IMenuBarItem createDeleteSubMenu(ITreeNode<RootNodeType> treeNode, IBuildingBlockExplorerPresenter presenter)
      {
         return ClassificationCommonContextMenuItems.RemoveClassificationFolderMainMenu(treeNode, presenter);
      }
   }

   public class ObservedDataFolderTreeNodeContextMenuFactory : RootNodeContextMenuFactory
   {
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public ObservedDataFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository, IBuildingBlockRepository buildingBlockRepository)
         : base(RootNodeTypes.ObservedDataFolder, repository)
      {
         _buildingBlockRepository = buildingBlockRepository;
      }

      public override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new ObservedDataFolderContextMenu(treeNode, _repository, _buildingBlockRepository, presenter.DowncastTo<IBuildingBlockExplorerPresenter>());
      }
   }
}