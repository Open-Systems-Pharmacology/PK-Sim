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
using PKSim.Assets;
using PKSim.Presentation.Presenters.Main;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class ComparisonFolderTreeNodeContextMenu : ContextMenu
   {
      public ComparisonFolderTreeNodeContextMenu(ITreeNode<RootNodeType> treeNode, ISimulationExplorerPresenter presenter, IContainer container) : base(container)
      {
         _view.AddMenuItem(
            CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AddIndividualSimulationComparison)
               .WithCommand<CreateIndividualSimulationComparisonUICommand>(container)
               .WithIcon(ApplicationIcons.IndividualSimulationComparison));

         _view.AddMenuItem(
            CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AddPopulationSimulationComparison)
               .WithCommand<CreatePopulationSimulationComparisonUICommand>(container)
               .WithIcon(ApplicationIcons.PopulationSimulationComparison));

         _view.AddMenuItem(ClassificationCommonContextMenuItems.CreateClassificationUnderMenu(treeNode, presenter));
         _view.AddMenuItem(ClassificationCommonContextMenuItems.RemoveClassificationFolderMainMenu(treeNode, presenter).AsGroupStarter());
      }
   }

   public class ComparisonFolderTreeNodeContextMenuFactory : RootNodeContextMenuFactory
   {
      private readonly IContainer _container;

      public ComparisonFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository, IContainer container)
         : base(RootNodeTypes.ComparisonFolder, repository)
      {
         _container = container;
      }

      public override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new ComparisonFolderTreeNodeContextMenu(treeNode, presenter.DowncastTo<ISimulationExplorerPresenter>(), _container);
      }
   }
}