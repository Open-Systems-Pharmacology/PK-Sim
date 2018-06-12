using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Extensions
{
   public static class CacheExtensions
   {
      public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this ICache<TKey, TValue> cache)
      {
         var dictionary = new Dictionary<TKey, TValue>();
         cache.KeyValues.Each(x => dictionary.Add(x.Key, x.Value));
         return dictionary;
      }
   }
}