﻿using System.Collections.Generic;
using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Repositories;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters.Main;
using PKSim.Presentation.UICommands;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class SimulationFolderTreeNodeContextMenu : ContextMenu
   {
      public static IEnumerable<IMenuBarItem> AddSimulationMenuItems(IMenuBarItemRepository repository)
      {
         yield return repository[MenuBarItemIds.NewSimulation];
      }

      public SimulationFolderTreeNodeContextMenu(ITreeNode<RootNodeType> treeNode, IMenuBarItemRepository repository, ISimulationExplorerPresenter presenter, IContainer container) : base(container)
      {
         AddSimulationMenuItems(repository).Each(_view.AddMenuItem);

         _view.AddMenuItem(ClassificationCommonContextMenuItems.CreateClassificationUnderMenu(treeNode, presenter).AsGroupStarter());

         var groupMenu = createGroupByMenu(treeNode, presenter);
         if (groupMenu.AllItems().Any())
            _view.AddMenuItem(groupMenu);

         _view.AddMenuItem(SimulationClassificationCommonContextMenuItems.RemoveSimulationFolderMainMenu(treeNode, presenter).AsGroupStarter());

         _view.AddMenuItem(loadSimulationFromSnapshot(container).AsGroupStarter());
      }

      private IMenuBarItem loadSimulationFromSnapshot(IContainer container)
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AsDeveloperOnly("Load from Snapshot"))
            .WithCommand<LoadSimulationFromSnapshotUICommand>(container)
            .WithIcon(ApplicationIcons.SnapshotImport)
            .ForDeveloper();
      }

      private static IMenuBarSubMenu createGroupByMenu(ITreeNode<IClassification> treeNode, ISimulationExplorerPresenter presenter)
      {
         var groupMenu = CreateSubMenu.WithCaption(MenuNames.GroupBy);

         presenter.AvailableClassificationCategories(treeNode)
            .Each(classification => groupMenu.AddItem(
               CreateMenuButton.WithCaption(classification.ClassificationName)
                  .WithIcon(classification.Icon)
                  .WithActionCommand(() => presenter.AddToClassificationTree(treeNode, classification.ClassificationName))));

         return groupMenu;
      }
   }

   public class SimulationFolderTreeNodeContextMenuFactory : RootNodeContextMenuFactory
   {
      private readonly IContainer _container;

      public SimulationFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository, IContainer container) : base(RootNodeTypes.SimulationFolder, repository)
      {
         _container = container;
      }

      public override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new SimulationFolderTreeNodeContextMenu(treeNode, _repository, presenter.DowncastTo<ISimulationExplorerPresenter>(), _container);
      }
   }
}