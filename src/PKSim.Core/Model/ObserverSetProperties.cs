using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model
{
   public class ObserverSetProperties
   {
      private readonly List<ObserverSetMapping> _observerSetMappings = new List<ObserverSetMapping>();

      public virtual ObserverSetProperties Clone(ICloneManager cloneManager)
      {
         var clone = new ObserverSetProperties();
         clone.AddObserverSetMappings(ObserverSetMappings.Select(x => x.Clone(cloneManager)));
         return clone;
      }

      public virtual void AddObserverSetMapping(ObserverSetMapping observerSetMapping) => _observerSetMappings.Add(observerSetMapping);

      public virtual void AddObserverSetMappings(IEnumerable<ObserverSetMapping> observerSetMappings) => observerSetMappings.Each(AddObserverSetMapping);

      public virtual IReadOnlyList<ObserverSetMapping> ObserverSetMappings => _observerSetMappings;

      public virtual void ClearObserverSetMappings() => _observerSetMappings.Clear();
   }
}