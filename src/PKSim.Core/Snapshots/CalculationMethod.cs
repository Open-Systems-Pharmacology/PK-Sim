using System.Collections;
using System.Collections.Generic;

namespace PKSim.Core.Snapshots
{
   public class CalculationMethodCache : SnapshotBase, IEnumerable<string>
   {
      private readonly List<string> _allCalculationMethods = new List<string>();

      public CalculationMethodCache()
      {
      }

      //This constructor is required for json deserialization
      public CalculationMethodCache(IEnumerable<string> allCalculationMethods)
      {
         _allCalculationMethods.AddRange(allCalculationMethods);
      }

      public void Add(string calculationMethod)
      {
         _allCalculationMethods.Add(calculationMethod);
      }

      public IEnumerator<string> GetEnumerator()
      {
         return _allCalculationMethods.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }
}