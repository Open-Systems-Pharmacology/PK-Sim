using System.Collections.Generic;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using PKSim.Presentation.UICommands;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Assets;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultipleUsedObservedDataContextMenu : ContextMenu<IReadOnlyList<UsedObservedData>>
   {
      public MultipleUsedObservedDataContextMenu(IReadOnlyList<UsedObservedData> usedObservedData, IContainer container) : base(usedObservedData, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(IReadOnlyList<UsedObservedData> usedObservedData)
      {
         yield return CreateMenuButton.WithCaption(MenuNames.Remove)
            .WithCommandFor<DeleteUsedObservedDataUICommand, IReadOnlyList<UsedObservedData>>(usedObservedData, _container)
            .WithIcon(ApplicationIcons.Remove)
            .AsGroupStarter();
      }
   }

   public class MultipleUsedObservedDataContextMenuFactory : MultipleNodeContextMenuFactory<UsedObservedData>
   {
      private readonly IContainer _container;

      public MultipleUsedObservedDataContextMenuFactory(IContainer container)
      {
         _container = container;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<UsedObservedData> usedObservedData, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultipleUsedObservedDataContextMenu(usedObservedData, _container);
      }
   }
}