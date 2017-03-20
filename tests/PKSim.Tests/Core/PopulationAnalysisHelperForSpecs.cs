using System.Collections.Generic;
using System.Drawing;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Chart;

namespace PKSim.Core
{
   public static class PopulationAnalysisHelperForSpecs
   {
      public static GroupingItem GroupingItemChildren = new GroupingItem {Label = "Children", Color = Color.Blue};
      public static GroupingItem GroupingItemAdolescents = new GroupingItem { Label = "Adolescents", Color = Color.Red };
      public static GroupingItem GroupingItemAdults = new GroupingItem { Label = "Adults", Color = Color.Green };
      public static GroupingItem GroupingItemMale = new GroupingItem {Label = "Male", Color = PKSimColors.Male, Symbol = Symbols.Circle};
      public static GroupingItem GroupingItemFemale = new GroupingItem {Label = "Female", Color = PKSimColors.Female, Symbol = Symbols.Diamond};
      public static GroupingItem GroupingItemThin = new GroupingItem { Label = "Thin", Color = Color.Orange };
      public static GroupingItem GroupingItemBig = new GroupingItem {Label = "Big", Color = Color.Violet};

      public static List<GroupingItem> BMIGroups = new List<GroupingItem> {GroupingItemThin, GroupingItemBig};
      public static List<GroupingItem> AgeGroups = new List<GroupingItem> {GroupingItemChildren, GroupingItemAdolescents, GroupingItemAdults};
      public static List<GroupingItem> GenderGroups = new List<GroupingItem> {GroupingItemMale, GroupingItemFemale};
   }
}