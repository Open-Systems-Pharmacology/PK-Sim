using System.Collections;
using FakeItEasy;

namespace PKSim.Core
{
   public abstract class GenericEqualityComparer<TEnumerable> : ArgumentEqualityComparer<TEnumerable> where TEnumerable : IEnumerable
   {
      protected override bool AreEqual(TEnumerable expectedValue, TEnumerable argumentValue)
      {
         return Equals(expectedValue, argumentValue);
      }
   }
}