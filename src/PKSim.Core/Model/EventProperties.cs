using System.Collections.Generic;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class EventProperties
   {
      private readonly IList<IEventMapping> _eventMappings = new List<IEventMapping>();

      public virtual EventProperties Clone(ICloneManager cloneManager)
      {
         var clone = new EventProperties();
         EventMappings.Each(em => clone.AddEventMapping(em.Clone(cloneManager)));
         return clone;
      }

      public virtual void AddEventMapping(IEventMapping eventMapping)
      {
         _eventMappings.Add(eventMapping);
      }

      public virtual IEnumerable<IEventMapping> EventMappings
      {
         get { return _eventMappings; }
      }

      public virtual void ClearEventMapping()
      {
         _eventMappings.Clear();
      }
   }
}