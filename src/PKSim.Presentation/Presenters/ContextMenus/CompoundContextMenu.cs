using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class CompoundContextMenu : BuildingBlockContextMenu<Compound>
   {
      public CompoundContextMenu(Compound compound, IContainer container)
         : base(compound, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(Compound compound)
      {
         var allMenuItems = new List<IMenuBarItem>();
         allMenuItems.AddRange(EditContextMenusFor<EditCompoundCommand>(compound));
         allMenuItems.Add(addObservedDataMenuFor(compound));
         allMenuItems.AddRange(ExportAndDeleteContextMenusFor(compound));
         return allMenuItems;
      }

      private IMenuBarButton addObservedDataMenuFor(Compound compound)
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AddObservedData)
            .WithCommandFor<AddObservedDataForCompoundUICommand, Compound>(compound, _container)
            .WithIcon(ApplicationIcons.ObservedData);
      }
   }

   public class CompoundTreeNodeContextMenuFactory : NodeContextMenuFactory<Compound>
   {
      private readonly IContainer _container;

      public CompoundTreeNodeContextMenuFactory(IContainer container)
      {
         _container = container;
      }

      public override IContextMenu CreateFor(Compound compound, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new CompoundContextMenu(compound, _container);
      }
   }
}