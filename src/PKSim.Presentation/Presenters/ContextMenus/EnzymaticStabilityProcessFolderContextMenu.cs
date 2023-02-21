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
   public class EnzymaticStabilityProcessFolderContextMenu : ContextMenu<ITreeNode, ICompoundProcessesPresenter>
   {
      public EnzymaticStabilityProcessFolderContextMenu(ITreeNode nodeRequestingContextMenu, ICompoundProcessesPresenter presenter, IContainer container)
         : base(nodeRequestingContextMenu, presenter, container)
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
      private readonly IContainer _container;

      public EnzymaticStabilityProcessFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository, IContainer container) : base(PKSimRootNodeTypes.CompoundMetabolizingEnzymes, repository)
      {
         _container = container;
      }

      protected override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, ICompoundProcessesPresenter presenter)
      {
         return new EnzymaticStabilityProcessFolderContextMenu(treeNode, presenter, _container);
      }
   }
}