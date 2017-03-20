using System.Collections.Generic;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public interface IWithCalculationMethods
   {
      CalculationMethodCache CalculationMethodCache { get; }
   }

   public static class WithCalculationMethodsExtension
   {
      public static IEnumerable<CalculationMethod> AllCalculationMethods(this IWithCalculationMethods withCalculationMethods)
      {
         return cacheFor(withCalculationMethods).All();
      }

      public static void AddCalculationMethod(this IWithCalculationMethods withCalculationMethods, CalculationMethod calculationMethod)
      {
         cacheFor(withCalculationMethods).AddCalculationMethod(calculationMethod);
      }

      public static void RemoveCalculationMethod(this IWithCalculationMethods withCalculationMethods, CalculationMethod calculationMethod)
      {
         cacheFor(withCalculationMethods).RemoveCalculationMethod(calculationMethod);
      }

      public static void ClearCalculationMethods(this IWithCalculationMethods withCalculationMethods)
      {
         cacheFor(withCalculationMethods).Clear();
      }

      public static CalculationMethod CalculationMethodFor(this IWithCalculationMethods withCalculationMethods, CalculationMethodCategory category)
      {
         return cacheFor(withCalculationMethods).CalculationMethodFor(category.Name);
      }

      public static CalculationMethod CalculationMethodFor(this IWithCalculationMethods withCalculationMethods, string categoryName)
      {
         return cacheFor(withCalculationMethods).CalculationMethodFor(categoryName);
      }
      public static void RemoveCalculationMethodFor(this IWithCalculationMethods withCalculationMethods, string categoryName)
      {
         var cache = cacheFor(withCalculationMethods);
         cache.RemoveCalculationMethod(cache.CalculationMethodFor(categoryName));
      }

      public static bool ContainsCalculationMethod(this IWithCalculationMethods withCalculationMethods, string calculationMethod)
      {
         return cacheFor(withCalculationMethods).Contains(calculationMethod);
      }

      private static CalculationMethodCache cacheFor(this IWithCalculationMethods withCalculationMethods)
      {
         return withCalculationMethods.CalculationMethodCache;
      }
   }
}