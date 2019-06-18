using System.Collections.Generic;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using PKSim.Core.Model;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class ObserverSetContextMenu : BuildingBlockContextMenu<ObserverSet>
   {
      public ObserverSetContextMenu(ObserverSet observerSet) : base(observerSet)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(ObserverSet observerSet)
      {
         return AllStandardMenuItemsFor<EditObserverSetCommand>(observerSet);
      }
   }

   public class ObserverSetTreeNodeContextMenuFactory : NodeContextMenuFactory<ObserverSet>
   {
      public override IContextMenu CreateFor(ObserverSet observerSet, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new ObserverSetContextMenu(observerSet);
      }
   }
}