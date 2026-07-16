using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Repositories;
using OSPSuite.Utility.Container;
using PKSim.Assets;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.Compounds;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class InductionProcessFolderContextMenu : ContextMenu<ITreeNode, ICompoundProcessesPresenter>
   {
      public InductionProcessFolderContextMenu(ITreeNode nodeRequestingContextMenu, ICompoundProcessesPresenter context, IContainer container)
         : base(nodeRequestingContextMenu, context, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(ITreeNode nodeRequestingContextMenu, ICompoundProcessesPresenter presenter)
      {
         yield return
            CreateMenuButton.WithCaption(PKSimConstants.UI.AddInductionProcess)
               .WithActionCommand(presenter.AddInductionProcess)
               .WithIcon(ApplicationIcons.Induction);
      }
   }

   public class InductionProcessFolderTreeNodeContextMenuFactory : CompoundProcessFolderTreeNodeContextMenuFactory
   {
      private readonly IContainer _container;

      public InductionProcessFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository, IContainer container) : base(PKSimRootNodeTypes.InductionProcess, repository)
      {
         _container = container;
      }

      protected override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, ICompoundProcessesPresenter presenter)
      {
         return new InductionProcessFolderContextMenu(treeNode, presenter, _container);
      }
   }
}