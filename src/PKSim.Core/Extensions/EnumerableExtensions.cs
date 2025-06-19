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
      /// Creates a caption out of a list of strings using the path separator
      /// </summary>
      /// <param name="fieldValues"></param>
      /// <returns></returns>
      public static string ToCaption(this IEnumerable<string> fieldValues)
      {
         return fieldValues.DefaultIfEmpty(string.Empty).ToString(Constants.DISPLAY_PATH_SEPARATOR);
      }

      /// <summary>
      ///    Returns true if there are at least two elements in the list (>1) otherwise false
      /// </summary>
      public static bool HasAtLeastTwo<TSource>(this IEnumerable<TSource> list) => list.Count() > 1;
   }
}