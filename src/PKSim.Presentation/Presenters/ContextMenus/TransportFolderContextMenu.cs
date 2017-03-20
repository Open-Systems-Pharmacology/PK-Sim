using OSPSuite.Presentation.MenuAndBars;
using System.Collections.Generic;
using PKSim.Assets;
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
   public class TransportFolderContextMenu : ContextMenu<ITreeNode, ICompoundProcessesPresenter>
   {
      public TransportFolderContextMenu(ITreeNode nodeRequestingContextMenu, ICompoundProcessesPresenter presenter)
         : base(nodeRequestingContextMenu, presenter)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(ITreeNode nodeRequestingContextMenu, ICompoundProcessesPresenter presenter)
      {
         yield return
            CreateMenuButton.WithCaption(PKSimConstants.UI.AddTransportProtein)
               .WithActionCommand(presenter.AddTransport)
               .WithIcon(ApplicationIcons.Transporter);
      }   
   }

   public class TransportFolderTreeNodeContextMenuFactory : CompoundProcessFolderTreeNodeContextMenuFactory
   {
      public TransportFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository)
         : base(PKSimRootNodeTypes.CompoundTransportProteins, repository)
      {
      }

      protected override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, ICompoundProcessesPresenter presenter)
      {
         return new TransportFolderContextMenu(treeNode, presenter);
      }
   }
}