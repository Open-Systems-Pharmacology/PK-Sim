using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Repositories;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.Core;
using PKSim.Presentation.Nodes;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class ObserverSetFolderContextMenu : BuildingBlockFolderContextMenu<ObserverSet>
   {
      public ObserverSetFolderContextMenu(ITreeNode<RootNodeType> treeNode, IMenuBarItemRepository repository, IExplorerPresenter presenter, IContainer container)
         : base(treeNode, repository, presenter, container, MenuBarItemIds.NewObserverSet, MenuBarItemIds.LoadObserverSet)
      {
      }
   }

   public class ObserverSetFolderTreeNodeContextMenuFactory : RootNodeContextMenuFactory
   {
      private readonly IContainer _container;

      public ObserverSetFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository, IContainer container)
         : base(PKSimRootNodeTypes.ObserverSetFolder, repository)
      {
         _container = container;
      }

      public override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new ObserverSetFolderContextMenu(treeNode, _repository, presenter.DowncastTo<IExplorerPresenter>(), _container);
      }
   }
}
