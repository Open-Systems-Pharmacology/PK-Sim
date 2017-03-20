using System.Linq;

namespace PKSim.Core.Extensions
{
   public static class ObjectExtensions
   {
      public static bool OneOf<T>(this T objectToCheck, params T[] items)
      {
         return items.Any(item => Equals(item, (objectToCheck)));
      }
   }
}