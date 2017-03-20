using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using OSPSuite.Assets;
using OSPSuite.Presentation.Regions;

namespace PKSim.Presentation.Regions
{
   public static class RegionNames
   {
      private static readonly IList<RegionName> _allRegions = new List<RegionName>();

      public static RegionName BuildingBlockExplorer = createRegionName("BuildingBlockExplorer", PKSimConstants.UI.BuildingBlockExplorer, ApplicationIcons.BuildingBlockExplorer);
      public static RegionName SimulationExplorer = createRegionName("SimulationExplorer", PKSimConstants.UI.SimulationExplorer, ApplicationIcons.SimulationExplorer);
      public static RegionName History = createRegionName("History", PKSimConstants.UI.History, ApplicationIcons.History);
      public static RegionName Comparison = createRegionName("Comparison", "Comparison", ApplicationIcons.Comparison);
      public static RegionName Journal = createRegionName("Journal", Captions.Journal.JournalView, ApplicationIcons.Journal);
      public static RegionName JournalDiagram = createRegionName("JournalDiagram", Captions.Journal.JournalDiagram, ApplicationIcons.JournalDiagram);

      private static RegionName createRegionName(string name, string caption, ApplicationIcon icon)
      {
         var newRegion = new RegionName(name, caption, icon);
         _allRegions.Add(newRegion);
         return newRegion;
      }

      public static IEnumerable<RegionName> All()
      {
         return _allRegions.All();
      }
   }
}