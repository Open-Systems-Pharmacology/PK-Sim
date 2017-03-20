using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using PKSim.Presentation.Core;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Repositories;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Repositories;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class InhibitionProcessFolderContextMenu : ContextMenu<ITreeNode, ICompoundProcessesPresenter>
   {
      public InhibitionProcessFolderContextMenu(ITreeNode nodeRequestingContextMenu, ICompoundProcessesPresenter context)
         : base(nodeRequestingContextMenu, context)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(ITreeNode nodeRequestingContextMenu, ICompoundProcessesPresenter presenter)
      {
         yield return
            CreateMenuButton.WithCaption(PKSimConstants.UI.AddInhibitionProcess)
               .WithActionCommand(presenter.AddInhibitionProcess)
               .WithIcon(ApplicationIcons.Inhibition);
      }
   }

   public class InhibitionProcessFolderTreeNodeContextMenuFactory : CompoundProcessFolderTreeNodeContextMenuFactory
   {
      public InhibitionProcessFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository)
         : base(PKSimRootNodeTypes.InhibitionProcess, repository)
      {
      }

      protected override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, ICompoundProcessesPresenter presenter)
      {
         return new InhibitionProcessFolderContextMenu(treeNode, presenter);
      }
   }
}