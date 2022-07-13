using OSPSuite.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Extensions
{
   public static class EnumerableExtensions
   {
      /// <summary>
      ///    Returns the index of the first occurrence in a sequence
      /// </summary>
      /// <typeparam name="TSource">The type of the elements of source.</typeparam>
      /// <param name="list">A sequence in which to locate a value.</param>
      /// <param name="value">The object to locate in the sequence</param>
      /// <returns>The zero-based index of the first occurrence of value within the entire sequence, if found; otherwise, –1.</returns>
      public static int IndexOf<TSource>(this IEnumerable<TSource> list, TSource value)
      {
         return list.IndexOf(value, (x, y) => Equals(x, y));
      }

      /// <summary>
      ///    Returns the index of the first occurrence in a sequence by using a specified IEqualityComparer.
      /// </summary>
      /// <typeparam name="TSource">The type of the elements of source.</typeparam>
      /// <param name="list">A sequence in which to locate a value.</param>
      /// <param name="value">The object to locate in the sequence</param>
      /// <param name="comparer">An equality function to compare values.</param>
      /// <returns>The zero-based index of the first occurrence of value within the entire sequence, if found; otherwise, –1.</returns>
      public static int IndexOf<TSource>(this IEnumerable<TSource> list, TSource value, Func<TSource, TSource, bool> comparer)
      {
         var index = 0;
         foreach (var item in list)
         {
            if (comparer(item, value))
               return index;

            index++;
         }

         return -1;
      }

      /// <summary>
      /// Creates a caption out of a list of strings using the path separator
      /// </summary>
      /// <param name="fieldValues"></param>
      /// <returns></returns>
      public static string CaptionFrom(this IEnumerable<string> fieldValues)
      {
         return fieldValues.DefaultIfEmpty(string.Empty).ToString(Constants.DISPLAY_PATH_SEPARATOR);
      }

      /// <summary>
      ///    Returns the index of the first occurrence in a sequence by using a specified IEqualityComparer.
      /// </summary>
      /// <typeparam name="TSource">The type of the elements of source.</typeparam>
      /// <param name="list">A sequence in which to locate a value.</param>
      /// <param name="value">The object to locate in the sequence</param>
      /// <param name="comparer">An equality comparer to compare values.</param>
      /// <returns>The zero-based index of the first occurrence of value within the entire sequence, if found; otherwise, –1.</returns>
      public static int IndexOf<TSource>(this IEnumerable<TSource> list, TSource value, IEqualityComparer<TSource> comparer)
         => list.IndexOf(value, comparer.Equals);

      /// <summary>
      ///    Returns true if there are at least two elements in the list (>1) otherwise false
      /// </summary>
      public static bool HasAtLeastTwo<TSource>(this IEnumerable<TSource> list) => list.Count() > 1;
   }
}