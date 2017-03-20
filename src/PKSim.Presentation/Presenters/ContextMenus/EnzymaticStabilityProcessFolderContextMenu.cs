using OSPSuite.Presentation.MenuAndBars;
using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Presentation.Nodes;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.Compounds;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Repositories;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class EnzymaticStabilityProcessFolderContextMenu : ContextMenu<ITreeNode, ICompoundProcessesPresenter>
   {
      public EnzymaticStabilityProcessFolderContextMenu(ITreeNode nodeRequestingContextMenu, ICompoundProcessesPresenter presenter)
         : base(nodeRequestingContextMenu, presenter)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(ITreeNode nodeRequestingContextMenu, ICompoundProcessesPresenter presenter)
      {
         yield return
            CreateMenuButton.WithCaption(PKSimConstants.UI.AddMetabolizingEnzyme)
               .WithActionCommand(presenter.AddEnzymaticPartialProcess)
               .WithIcon(ApplicationIcons.Enzyme);
      }
   }

   public class EnzymaticStabilityProcessFolderTreeNodeContextMenuFactory : CompoundProcessFolderTreeNodeContextMenuFactory
   {
      public EnzymaticStabilityProcessFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository) : base(PKSimRootNodeTypes.CompoundMetabolizingEnzymes, repository)
      {
      }

      protected override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, ICompoundProcessesPresenter presenter)
      {
         return new EnzymaticStabilityProcessFolderContextMenu(treeNode, presenter);
      }
   }
}