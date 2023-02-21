using System.Collections.Generic;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;
using PKSim.Core.Model;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class EventContextMenu : BuildingBlockContextMenu<PKSimEvent>
   {
      public EventContextMenu(PKSimEvent pkSimEvent, IContainer container)
         : base(pkSimEvent, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(PKSimEvent pkSimEvent)
      {
         return AllStandardMenuItemsFor<EditEventCommand>(pkSimEvent);
      }
   }

   public class EventTreeNodeContextMenuFactory : NodeContextMenuFactory<PKSimEvent>
   {
      private readonly IContainer _container;

      public EventTreeNodeContextMenuFactory(IContainer container)
      {
         _container = container;
      }

      public override IContextMenu CreateFor(PKSimEvent pkSimEvent, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new EventContextMenu(pkSimEvent, _container);
      }
   }
}