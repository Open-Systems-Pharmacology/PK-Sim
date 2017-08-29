using System.Collections;
using System.Collections.Generic;

namespace PKSim.Core.Snapshots
{
   public class ExtendedProperties : SnapshotBase, IEnumerable<ExtendedProperty>
   {
      private readonly List<ExtendedProperty> _extendedProperties = new List<ExtendedProperty>();

      public ExtendedProperties()
      {
         
      }

      //This constructor is required for json deserialization
      public ExtendedProperties(IEnumerable<ExtendedProperty> allExtendedProperties)
      {
         _extendedProperties.AddRange(allExtendedProperties);
      }

      public IEnumerator<ExtendedProperty> GetEnumerator()
      {
         return _extendedProperties.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      public void Add(ExtendedProperty extendedProperty)
      {
         _extendedProperties.Add(extendedProperty);
      }
   }
}