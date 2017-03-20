using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using PKSim.Presentation.UICommands;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.UICommands;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MdiChildViewContextMenu : ContextMenu<IMdiChildView>
   {
      public MdiChildViewContextMenu(IMdiChildView view) : base(view)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(IMdiChildView view)
      {
         yield return CreateMenuButton.WithCaption(PKSimConstants.UI.CloseView)
            .WithIcon(ApplicationIcons.Close)
            .WithCommandFor<CloseMdiViewCommand, IMdiChildView>(view);

         yield return CreateMenuButton.WithCaption(PKSimConstants.UI.CloseAll)
            .WithCommand<CloseAllMdiViewCommand>();

         yield return CreateMenuButton.WithCaption(PKSimConstants.UI.CloseAllButThis)
            .WithCommandFor<CloseAllButMdiViewCommand, IMdiChildView>(view);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Rename)
            .WithCommandFor<RenameSubjectUICommand, IMdiChildView>(view)
            .WithIcon(ApplicationIcons.Rename)
            .AsGroupStarter();
      }
   }

   public class MdiChildViewContextMenuFactory : IContextMenuSpecificationFactory<IMdiChildView>
   {
      public IContextMenu CreateFor(IMdiChildView view, IPresenterWithContextMenu<IMdiChildView> presenter)
      {
         return new MdiChildViewContextMenu(view);
      }

      public bool IsSatisfiedBy(IMdiChildView view, IPresenterWithContextMenu<IMdiChildView> presenter)
      {
         return view != null;
      }
   }
}