using System;

namespace PKSim.Core.Model.PopulationAnalyses
{
   [Flags]
   public enum PivotArea
   {
      RowArea = 2 << 0,
      ColorArea = 2 << 1,
      FilterArea = 2 << 2,
      DataArea = 2 << 3,
      SymbolArea = 2 << 4,
      ColumnArea = ColorArea | SymbolArea,
      All = DataArea | FilterArea | ColumnArea | RowArea,
   }

   public static class PivotAreaExtensions
   {
      public static bool Is(this PivotArea pivotArea, PivotArea pivotAreaToCompare)
      {
         return (pivotArea & pivotAreaToCompare) != 0;
      }
   }

}