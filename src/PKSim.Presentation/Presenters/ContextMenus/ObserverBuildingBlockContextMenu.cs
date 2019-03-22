using System.Collections.Generic;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using PKSim.Core.Model;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class ObserverBuildingBlockContextMenu : BuildingBlockContextMenu<PKSimObserverBuildingBlock>
   {
      public ObserverBuildingBlockContextMenu(PKSimObserverBuildingBlock observerBuildingBlock) : base(observerBuildingBlock)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(PKSimObserverBuildingBlock observerBuildingBlock)
      {
         return AllStandardMenuItemsFor<EditObserverBuildingBlockCommand>(observerBuildingBlock);
      }
   }

   public class ObserverBuildingBlockCTreeNodeContextMenuFactory : NodeContextMenuFactory<PKSimObserverBuildingBlock>
   {
      public override IContextMenu CreateFor(PKSimObserverBuildingBlock observerBuildingBlock, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new ObserverBuildingBlockContextMenu(observerBuildingBlock);
      }
   }
}