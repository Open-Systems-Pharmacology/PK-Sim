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
using System;
using OSPSuite.Utility.Container;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class ObservedDataFolderContextMenu : ContextMenu
   {
      public ObservedDataFolderContextMenu(
         ITreeNode<RootNodeType> treeNode,
         IMenuBarItemRepository repository,
         IBuildingBlockRepository buildingBlockRepository,
         IBuildingBlockExplorerPresenter presenter,
         IUserSettings userSettings, IContainer container) : base(container)
      {
         var allCompounds = buildingBlockRepository.All<Compound>().ToList();

         //create sub menu containing all compounds
         var addObservedDataFor = CreateSubMenu.WithCaption(PKSimConstants.MenuNames.AddObservedDataFor)
            .WithIcon(ApplicationIcons.ObservedDataForMolecule);


         foreach (var compound in allCompounds)
         {
            addObservedDataFor.AddItem(CreateMenuButton.WithCaption(compound.Name)
               .WithCommandFor<AddObservedDataForCompoundUICommand, Compound>(compound, container));  
         }

         _view.AddMenuItem(repository[MenuBarItemIds.AddObservedData]);
         if (allCompounds.Any())
            _view.AddMenuItem(addObservedDataFor);

         if ( treeNode.HasChildren) 
            _view.AddMenuItem(ObservedDataClassificationCommonContextMenuItems.ColorGroupObservedData(userSettings));


         _view.AddMenuItem(CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadFromTemplate)
            .WithCommand<LoadObservedDataFromTemplateUICommand>(container)
            .WithIcon(ApplicationIcons.LoadFromTemplate)
            .AsGroupStarter());

         _view.AddMenuItem(CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AsDeveloperOnly("Load from Snapshot"))
            .WithCommand<LoadObservedDataFromSnapshotUICommand>(container)
            .WithIcon(ApplicationIcons.SnapshotImport)
            .ForDeveloper());
           

         if (treeNode.AllLeafNodes.OfType<ObservedDataNode>().Any())
            _view.AddMenuItem(ObservedDataClassificationCommonContextMenuItems.EditMultipleMetaData(treeNode, container).AsGroupStarter());


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
      private readonly IUserSettings _userSettings;
      private readonly IContainer _container;

      public ObservedDataFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository, IBuildingBlockRepository buildingBlockRepository, IUserSettings userSettings, IContainer container)
         : base(RootNodeTypes.ObservedDataFolder, repository)
      {
         _buildingBlockRepository = buildingBlockRepository;
         _userSettings = userSettings;
         _container = container;
      }

      public override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new ObservedDataFolderContextMenu(treeNode, _repository, _buildingBlockRepository, presenter.DowncastTo<IBuildingBlockExplorerPresenter>(), _userSettings, _container);
      }
   }
}