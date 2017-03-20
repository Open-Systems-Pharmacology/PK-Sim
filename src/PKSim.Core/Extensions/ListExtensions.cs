using System.Collections.Generic;

namespace PKSim.Core.Extensions
{
   public static class ListExtensions
   {
      public static void AddRangeSafe<T>(this List<T> list, IEnumerable<T> objectsToAdd)
      {
         if (objectsToAdd == null)
            return;
         list.AddRange(objectsToAdd);
      }
   }
}