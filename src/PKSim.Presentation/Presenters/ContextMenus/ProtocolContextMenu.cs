using System.Collections.Generic;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using PKSim.Core.Model;
using PKSim.Presentation.UICommands;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class ProtocolContextMenu : BuildingBlockContextMenu<Protocol>
   {
      public ProtocolContextMenu(Protocol protocol, IContainer container) : base(protocol, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(Protocol protocol)
      {
         return AllStandardMenuItemsFor<EditProtocolCommand>(protocol);
      }
   }

   public class ProtocolTreeNodeContextMenuFactory : NodeContextMenuFactory<Protocol>
   {
      private readonly IContainer _container;

      public ProtocolTreeNodeContextMenuFactory(IContainer container)
      {
         _container = container;
      }

      public override IContextMenu CreateFor(Protocol protocol, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new ProtocolContextMenu(protocol, _container);
      }
   }
}