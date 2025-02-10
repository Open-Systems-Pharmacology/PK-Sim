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
using OSPSuite.Utility.Container;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MdiChildViewContextMenu : ContextMenu<IMdiChildView>
   {
      public MdiChildViewContextMenu(IMdiChildView view, IContainer container) : base(view, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(IMdiChildView view)
      {
         yield return CreateMenuButton.WithCaption(PKSimConstants.UI.CloseView)
            .WithIcon(ApplicationIcons.Close)
            .WithCommandFor<CloseMdiViewCommand, IMdiChildView>(view, _container);

         yield return CreateMenuButton.WithCaption(PKSimConstants.UI.CloseAll)
            .WithCommand<CloseAllMdiViewCommand>(_container);

         yield return CreateMenuButton.WithCaption(PKSimConstants.UI.CloseAllButThis)
            .WithCommandFor<CloseAllButMdiViewCommand, IMdiChildView>(view, _container);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Rename)
            .WithCommandFor<RenameSubjectUICommand, IMdiChildView>(view, _container)
            .WithIcon(ApplicationIcons.Rename)
            .AsGroupStarter();
      }
   }

   public class MdiChildViewContextMenuFactory : IContextMenuSpecificationFactory<IMdiChildView>
   {
      private readonly IContainer _container;

      public MdiChildViewContextMenuFactory(IContainer container)
      {
         _container = container;
      }
      
      public IContextMenu CreateFor(IMdiChildView view, IPresenterWithContextMenu<IMdiChildView> presenter)
      {
         return new MdiChildViewContextMenu(view, _container);
      }

      public bool IsSatisfiedBy(IMdiChildView view, IPresenterWithContextMenu<IMdiChildView> presenter)
      {
         return view != null;
      }
   }
}