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
   public class CompoundFolderContextMenu : BuildingBlockFolderContextMenu<Compound>
   {
      public CompoundFolderContextMenu(ITreeNode<RootNodeType> treeNode, IMenuBarItemRepository repository, IExplorerPresenter presenter, IContainer container)
         : base(treeNode, repository, presenter, container, MenuBarItemIds.NewCompound, MenuBarItemIds.LoadCompound)
      {
      }
   }

   public class CompoundFolderTreeNodeContextMenuFactory : RootNodeContextMenuFactory
   {
      private readonly IContainer _container;

      public CompoundFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository, IContainer container)
         : base(PKSimRootNodeTypes.CompoundFolder, repository)
      {
         _container = container;
      }

      public override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new CompoundFolderContextMenu(treeNode, _repository, presenter.DowncastTo<IExplorerPresenter>(), _container);
      }
   }
}
