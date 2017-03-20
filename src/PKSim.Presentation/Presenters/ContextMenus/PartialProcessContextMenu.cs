using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Model;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters.Compounds;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters.ContextMenus;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class PartialProcessContextMenu : CompoundProcessContextMenu<PartialProcess>
   {
      public PartialProcessContextMenu(PartialProcess compoundProcess, ICompoundProcessesPresenter presenter)
         : base(compoundProcess, presenter)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(PartialProcess partialProcess, ICompoundProcessesPresenter presenter)
      {
         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Rename)
            .WithActionCommand(() => presenter.RenameDataSourceInProcess(partialProcess))
            .WithIcon(ApplicationIcons.Rename);

         yield return DeleteMenuFor(partialProcess, presenter);
      }
   }

   public class PartialProcessTreeNodeContextMenuFactory : CompoundProcessTreeNodeContextMenuFactory<PartialProcess>
   {
      protected override IContextMenu CreateFor(PartialProcess compoundProcess, ICompoundProcessesPresenter compoundProcessesPresenter)
      {
         return new PartialProcessContextMenu(compoundProcess, compoundProcessesPresenter);
      }
   }
}