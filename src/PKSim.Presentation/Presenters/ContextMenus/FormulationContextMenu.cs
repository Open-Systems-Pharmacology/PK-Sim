using System.Collections.Generic;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using PKSim.Core.Model;
using PKSim.Presentation.UICommands;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class FormulationContextMenu : BuildingBlockContextMenu<Formulation>
   {
      public FormulationContextMenu(Formulation formulation, IContainer container) : base(formulation, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(Formulation formulation)
      {
         return AllStandardMenuItemsFor<EditFormulationCommand>(formulation);
      }
   }

   public class FormulationTreeNodeContextMenuFactory : NodeContextMenuFactory<Formulation>
   {
      private readonly IContainer _container;

      public FormulationTreeNodeContextMenuFactory(IContainer container)
      {
         _container = container;
      }

      public override IContextMenu CreateFor(Formulation formulation, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new FormulationContextMenu(formulation, _container);
      }
   }
}