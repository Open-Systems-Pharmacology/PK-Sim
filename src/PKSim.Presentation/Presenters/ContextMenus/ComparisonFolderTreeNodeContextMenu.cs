using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Main;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Repositories;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class ComparisonFolderTreeNodeContextMenu : ContextMenu
   {
      public ComparisonFolderTreeNodeContextMenu(ITreeNode<RootNodeType> treeNode, ISimulationExplorerPresenter presenter)
      {
         _view.AddMenuItem(ClassificationCommonContextMenuItems.CreateClassificationUnderMenu(treeNode, presenter));
         _view.AddMenuItem(ClassificationCommonContextMenuItems.RemoveClassificationFolderMainMenu(treeNode, presenter).AsGroupStarter());
         _view.AddMenuItem(GenericMenu.ExportCollectionToPDFMenuFor<ISimulationComparison>().AsGroupStarter());
      }
   }

   public class ComparisonFolderTreeNodeContextMenuFactory : RootNodeContextMenuFactory
   {
      public ComparisonFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository)
         : base(RootNodeTypes.ComparisonFolder, repository) 
      {
      }

      public override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new ComparisonFolderTreeNodeContextMenu(treeNode, presenter.DowncastTo<ISimulationExplorerPresenter>());
      }
   }
}