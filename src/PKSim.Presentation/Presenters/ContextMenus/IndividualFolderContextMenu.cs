using OSPSuite.Presentation.Nodes;
using PKSim.Core.Model;
using PKSim.Presentation.Core;
using PKSim.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Repositories;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class IndividualFolderContextMenu : BuildingBlockFolderContextMenu<Individual>
   {
      public IndividualFolderContextMenu(ITreeNode<RootNodeType> treeNode, IMenuBarItemRepository repository, IExplorerPresenter presenter, IContainer container)
         : base(treeNode, repository, presenter, container, MenuBarItemIds.NewIndividual, MenuBarItemIds.LoadIndividual)
      {
      }
   }

   public class IndividualFolderTreeNodeContextMenuFactory : RootNodeContextMenuFactory
   {
      private readonly IContainer _container;

      public IndividualFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository, IContainer container)
         : base(PKSimRootNodeTypes.IndividualFolder, repository)
      {
         _container = container;
      }

      public override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new IndividualFolderContextMenu(treeNode, _repository, presenter.DowncastTo<IExplorerPresenter>(), _container);
      }
   }
}
