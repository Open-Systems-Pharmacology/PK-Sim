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
using PKSim.Core.Model;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters.Main;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class SimulationFolderTreeNodeContextMenu : ContextMenu
   {
      public SimulationFolderTreeNodeContextMenu(ITreeNode<RootNodeType> treeNode, IMenuBarItemRepository repository, ISimulationExplorerPresenter presenter)
      {
         _view.AddMenuItem(repository[MenuBarItemIds.NewSimulation]);

         _view.AddMenuItem(ClassificationCommonContextMenuItems.CreateClassificationUnderMenu(treeNode, presenter).AsGroupStarter());

         var groupMenu = createGroupByMenu(treeNode, presenter);
         if (groupMenu.AllItems().Any())
            _view.AddMenuItem(groupMenu);

         _view.AddMenuItem(SimulationClassificationCommonContextMenuItems.RemoveSimulationFolderMainMenu(treeNode, presenter).AsGroupStarter());

         _view.AddMenuItem(GenericMenu.ExportCollectionToPDFMenuFor<Simulation>().AsGroupStarter());

         _view.AddMenuItem(loadSimulationFromSnapshot().AsGroupStarter());
      }

      private static IMenuBarItem loadSimulationFromSnapshot() 
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadFromSnapshot)
            .WithCommand<LoadSimulationFromSnapshotUICommand>()
            //TODO ICON
            .WithIcon(ApplicationIcons.LoadFromTemplate);
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
      public SimulationFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository) : base(RootNodeTypes.SimulationFolder, repository)
      {
      }

      public override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new SimulationFolderTreeNodeContextMenu(treeNode, _repository, presenter.DowncastTo<ISimulationExplorerPresenter>());
      }
   }
}