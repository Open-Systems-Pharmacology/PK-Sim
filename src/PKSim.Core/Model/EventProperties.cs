using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class EventProperties
   {
      private readonly List<EventMapping> _eventMappings = new List<EventMapping>();

      public virtual EventProperties Clone(ICloneManager cloneManager)
      {
         var clone = new EventProperties();
         clone.AddEventMappings(EventMappings.Select(x => x.Clone(cloneManager)));
         return clone;
      }

      public virtual void AddEventMapping(EventMapping eventMapping) => _eventMappings.Add(eventMapping);

      public virtual void AddEventMappings(IEnumerable<EventMapping> eventMappings) => _eventMappings.AddRange(eventMappings);

      public virtual IReadOnlyList<EventMapping> EventMappings => _eventMappings;

      public virtual void ClearEventMapping() => _eventMappings.Clear();
   }
}