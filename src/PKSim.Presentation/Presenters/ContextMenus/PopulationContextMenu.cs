using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Model;
using PKSim.Presentation.UICommands;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public abstract class PopulationContextMenu : BuildingBlockContextMenu<Population>
   {
      protected PopulationContextMenu(Population population)
         : base(population)
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
         return allMenuItems;
      }

      private static IMenuBarButton exportPopulationToCSVMenuFor(Population population)
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportToCSV)
            .WithIcon(ApplicationIcons.PopulationExportToCSV)
            .WithCommandFor<ExportPopulationToCSVCommand, Population>(population);
      }

      private static IMenuBarButton extractIndividualsMenuFor(Population population)
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExtractIndividualsMenu)
            .WithIcon(ApplicationIcons.Individual)
            .WithCommandFor<ExtractIndividualsFromPopulationCommand, Population>(population);
      }
   }
}