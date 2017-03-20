using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Repositories;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Repositories;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public abstract class CompoundProcessFolderTreeNodeContextMenuFactory : RootNodeContextMenuFactory
   {
      protected CompoundProcessFolderTreeNodeContextMenuFactory(RootNodeType rootNodeType, IMenuBarItemRepository repository)
         : base(rootNodeType, repository)
      {
      }

      public override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return CreateFor(treeNode, presenter.DowncastTo<ICompoundProcessesPresenter>());
      }

      protected abstract IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, ICompoundProcessesPresenter presenter);

      public override bool IsSatisfiedBy(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return base.IsSatisfiedBy(treeNode, presenter) && presenter.IsAnImplementationOf<ICompoundProcessesPresenter>();
      }
   }
}