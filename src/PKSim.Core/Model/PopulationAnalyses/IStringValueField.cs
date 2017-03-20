using System.Collections.Generic;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public interface IStringValueField : IPopulationAnalysisField
   {
      IReadOnlyList<GroupingItem> GroupingItems { get; }
      GroupingItem ReferenceGroupingItem { get; set; }
   }

   public static class StringValueFieldExtensions
   {
      public static GroupingItem GroupingByName(this IStringValueField field, string name)
      {
         return field.GroupingItems.Find(gi => string.Equals(gi.Label, name));
      }

      public static IReadOnlyList<GroupingItem> GroupingItemsWithReference(this IStringValueField field, IReadOnlyList<GroupingItem> groupingItems )
      {
         var groupings = new List<GroupingItem>(groupingItems);
         if (field.ReferenceGroupingItem != null)
            groupings.Add(field.ReferenceGroupingItem);

         return groupings;
      }

      public static bool CouldCompareValuesToReference(this IStringValueField field, object value1, object value2, out int result)
      {
         result = 0;
         if (field.ReferenceGroupingItem == null)
            return false;

    
         var value1IsLabel = string.Equals(value1.ToString(), field.ReferenceGroupingItem.Label);
         var value2IsLabel = string.Equals(value2.ToString(), field.ReferenceGroupingItem.Label);

         if (!value1IsLabel && value2IsLabel)
         {
            result = -1;
            return true;
         }

         if (value1IsLabel && value2IsLabel)
         {
            result = 0;
            return true;
         }

         if (value1IsLabel && !value2IsLabel)
         {
            result = 1;
            return true;
         }

         return false;
      }
   }
}