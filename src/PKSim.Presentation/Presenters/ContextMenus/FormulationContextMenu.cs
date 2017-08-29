using System.Collections.Generic;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using PKSim.Core.Model;
using PKSim.Presentation.UICommands;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class FormulationContextMenu : BuildingBlockContextMenu<Formulation>
   {
      public FormulationContextMenu(Formulation formulation) : base(formulation)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(Formulation formulation)
      {
         return AllStandardMenuItemsFor<EditFormulationCommand>(formulation);
      }
   }

   public class FormulationTreeNodeContextMenuFactory : NodeContextMenuFactory<Formulation>
   {
      public override IContextMenu CreateFor(Formulation formulation, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new FormulationContextMenu(formulation);
      }
   }
}