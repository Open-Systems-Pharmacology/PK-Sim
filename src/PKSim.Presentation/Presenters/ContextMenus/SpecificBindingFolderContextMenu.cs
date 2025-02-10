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
using OSPSuite.Utility.Container;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class SpecificBindingFolderContextMenu : ContextMenu<ITreeNode, ICompoundProcessesPresenter>
   {
      public SpecificBindingFolderContextMenu(ITreeNode nodeRequestingContextMenu, ICompoundProcessesPresenter presenter, IContainer container)
         : base(nodeRequestingContextMenu, presenter, container)
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
      private readonly IContainer _container;

      public SpecificBindingFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository, IContainer container)
         : base(PKSimRootNodeTypes.CompoundProteinBindingPartners, repository)
      {
         _container = container;
      }

      protected override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, ICompoundProcessesPresenter presenter)
      {
         return new SpecificBindingFolderContextMenu(treeNode, presenter, _container);
      }
   }
}