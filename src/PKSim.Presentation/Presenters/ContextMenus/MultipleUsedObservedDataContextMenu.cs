using System.Collections.Generic;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using PKSim.Presentation.UICommands;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Assets;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultipleUsedObservedDataContextMenu : ContextMenu<IReadOnlyList<UsedObservedData>>
   {
      public MultipleUsedObservedDataContextMenu(IReadOnlyList<UsedObservedData> usedObservedData) : base(usedObservedData)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(IReadOnlyList<UsedObservedData> usedObservedData)
      {
         yield return CreateMenuButton.WithCaption(MenuNames.Remove)
            .WithCommandFor<DeleteUsedObservedDataUICommand, IReadOnlyList<UsedObservedData>>(usedObservedData)
            .WithIcon(ApplicationIcons.Remove)
            .AsGroupStarter();
      }
   }

   public class MultipleUsedObservedDataContextMenuFactory : MultipleNodeContextMenuFactory<UsedObservedData>
   {
      protected override IContextMenu CreateFor(IReadOnlyList<UsedObservedData> usedObservedData, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultipleUsedObservedDataContextMenu(usedObservedData);
      }
   }
}