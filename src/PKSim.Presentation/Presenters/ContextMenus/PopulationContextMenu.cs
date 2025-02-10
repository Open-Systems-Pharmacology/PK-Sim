using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Model;
using PKSim.Presentation.UICommands;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Utility.Container;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public abstract class PopulationContextMenu : BuildingBlockContextMenu<Population>
   {
      protected PopulationContextMenu(Population population, IContainer container)
         : base(population, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(Population population)
      {
         var allMenuItems = new List<IMenuBarItem>();
         allMenuItems.AddRange(EditContextMenusFor<EditPopulationCommand>(population));
         allMenuItems.AddRange(ExportContextMenusFor(population));
         allMenuItems.Add(exportPopulationToCSVMenuFor(population));
         allMenuItems.Add(extractIndividualsMenuFor(population));
         allMenuItems.AddRange(DeleteContextMenusFor(population));
         allMenuItems.AddRange(DebugContextMenusFor(population));
         return allMenuItems;
      }

      private IMenuBarButton exportPopulationToCSVMenuFor(Population population)
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportToCSV)
            .WithIcon(ApplicationIcons.PopulationExportToCSV)
            .WithCommandFor<ExportPopulationToCSVCommand, Population>(population, _container);
      }

      private IMenuBarButton extractIndividualsMenuFor(Population population)
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExtractIndividualsMenu)
            .WithIcon(ApplicationIcons.Individual)
            .WithCommandFor<ExtractIndividualsFromPopulationCommand, Population>(population, _container);
      }
   }
}