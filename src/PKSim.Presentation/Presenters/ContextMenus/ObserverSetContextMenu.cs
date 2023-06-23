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
   public class ObserverSetContextMenu : BuildingBlockContextMenu<ObserverSet>
   {
      public ObserverSetContextMenu(ObserverSet observerSet, IContainer container) : base(observerSet, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(ObserverSet observerSet)
      {
         return AllStandardMenuItemsFor<EditObserverSetCommand>(observerSet);
      }
   }

   public class ObserverSetTreeNodeContextMenuFactory : NodeContextMenuFactory<ObserverSet>
   {
      private readonly IContainer _container;

      public ObserverSetTreeNodeContextMenuFactory(IContainer container)
      {
         _container = container;
      }

      public override IContextMenu CreateFor(ObserverSet observerSet, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new ObserverSetContextMenu(observerSet, _container);
      }
   }
}