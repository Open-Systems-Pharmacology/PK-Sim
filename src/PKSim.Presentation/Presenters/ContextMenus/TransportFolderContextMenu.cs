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
using OSPSuite.Utility.Container;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class TransportFolderContextMenu : ContextMenu<ITreeNode, ICompoundProcessesPresenter>
   {
      public TransportFolderContextMenu(ITreeNode nodeRequestingContextMenu, ICompoundProcessesPresenter presenter, IContainer container)
         : base(nodeRequestingContextMenu, presenter, container)
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
      private readonly IContainer _container;

      public TransportFolderTreeNodeContextMenuFactory(IMenuBarItemRepository repository, IContainer container)
         : base(PKSimRootNodeTypes.CompoundTransportProteins, repository)
      {
         _container = container;
      }

      protected override IContextMenu CreateFor(ITreeNode<RootNodeType> treeNode, ICompoundProcessesPresenter presenter)
      {
         return new TransportFolderContextMenu(treeNode, presenter, _container);
      }
   }
}