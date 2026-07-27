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
   public class ExpressionProfileFolderContextMenu : BuildingBlockFolderContextMenu<ExpressionProfile>
   {
      public ExpressionProfileFolderContextMenu(ITreeNode<RootNodeType> treeNode, IMenuBarItemRepository repository, IExplorerPresenter presenter, IContainer container)
         : base(treeNode, repository, presenter, container, MenuBarItemIds.NewExpressionProfile, MenuBarItemIds.LoadExpressionProfile)
      {
      }
   }

   public class ExpressionProfileFolderTreeNodeContextMenuFactory : RootNodeContextMenuFactory
   {
      private readonly IContainer _container;

      public ExpressionProfileFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository, IContainer container)
         : base(PKSimRootNodeTypes.ExpressionProfileFolder, repository)
      {
         _container = container;
      }

      public override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new ExpressionProfileFolderContextMenu(treeNode, _repository, presenter.DowncastTo<IExplorerPresenter>(), _container);
      }
   }
}
