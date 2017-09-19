using System.Collections.Generic;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model
{
   public class EventProperties
   {
      private readonly List<EventMapping> _eventMappings = new List<EventMapping>();

      public virtual EventProperties Clone(ICloneManager cloneManager)
      {
         var clone = new EventProperties();
         EventMappings.Each(em => clone.AddEventMapping(em.Clone(cloneManager)));
         return clone;
      }

      public virtual void AddEventMapping(EventMapping eventMapping)
      {
         _eventMappings.Add(eventMapping);
      }

      public virtual IReadOnlyList<EventMapping> EventMappings => _eventMappings;

      public virtual void ClearEventMapping()
      {
         _eventMappings.Clear();
      }
   }
}