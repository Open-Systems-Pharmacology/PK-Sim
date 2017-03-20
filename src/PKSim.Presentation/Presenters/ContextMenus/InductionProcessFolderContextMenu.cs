using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using PKSim.Presentation.Core;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Repositories;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Repositories;
using OSPSuite.Assets;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class InductionProcessFolderContextMenu : ContextMenu<ITreeNode, ICompoundProcessesPresenter>
   {
      public InductionProcessFolderContextMenu(ITreeNode nodeRequestingContextMenu, ICompoundProcessesPresenter context)
         : base(nodeRequestingContextMenu, context)
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
      public InductionProcessFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository) : base(PKSimRootNodeTypes.InductionProcess, repository)
      {
      }

      protected override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, ICompoundProcessesPresenter presenter)
      {
         return new InductionProcessFolderContextMenu(treeNode, presenter);
      }
   }
}