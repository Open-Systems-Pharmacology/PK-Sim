using OSPSuite.Presentation.MenuAndBars;
using System.Collections.Generic;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters.Compounds;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class SystemicProcessContextMenu : CompoundProcessContextMenu<SystemicProcess>
   {
      public SystemicProcessContextMenu(SystemicProcess clearanceProcess, ICompoundProcessesPresenter presenter, IContainer container)
         : base(clearanceProcess, presenter, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(SystemicProcess systemicProcess, ICompoundProcessesPresenter presenter)
      {
         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Rename)
            .WithActionCommand(() => presenter.RenameDataSourceInProcess(systemicProcess))
            .WithIcon(ApplicationIcons.Rename);

         yield return DeleteMenuFor(systemicProcess, presenter).AsGroupStarter();
      }
   }

   public class SystemicProcessTreeNodeContextMenuFactory : CompoundProcessTreeNodeContextMenuFactory<SystemicProcess>
   {
      private readonly IContainer _container;

      public SystemicProcessTreeNodeContextMenuFactory(IContainer container)
      {
         _container = container;
      }

      protected override IContextMenu CreateFor(SystemicProcess compoundProcess, ICompoundProcessesPresenter compoundProcessesPresenter)
      {
         return new SystemicProcessContextMenu(compoundProcess, compoundProcessesPresenter, _container);
      }
   }
}