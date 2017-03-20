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
   public class SpecificBindingFolderContextMenu : ContextMenu<ITreeNode, ICompoundProcessesPresenter>
   {
      public SpecificBindingFolderContextMenu(ITreeNode nodeRequestingContextMenu, ICompoundProcessesPresenter presenter)
         : base(nodeRequestingContextMenu, presenter)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(ITreeNode nodeRequestingContextMenu, ICompoundProcessesPresenter presenter)
      {
         yield return
            CreateMenuButton.WithCaption(PKSimConstants.UI.AddSpecificBindingPartner)
               .WithActionCommand(presenter.AddSpecificBinding)
               .WithIcon(ApplicationIcons.Protein);
      }
   }

   public class SpecificBindingFolderTreeNodeContextMenuFactory: CompoundProcessFolderTreeNodeContextMenuFactory
   {
      public SpecificBindingFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository)
         : base(PKSimRootNodeTypes.CompoundProteinBindingPartners, repository)
      {
      }

      protected override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, ICompoundProcessesPresenter presenter)
      {
         return new SpecificBindingFolderContextMenu(treeNode, presenter);
      }
   }
}