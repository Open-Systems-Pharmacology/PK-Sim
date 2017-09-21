using System.Collections;
using System.Collections.Generic;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public class EventSelection: IWithName
   {
      public string Name { get; set; }
      public Parameter StartTime { get; set; }
   }

   public class EventSelections : IEnumerable<EventSelection>
   {
      private readonly List<EventSelection> _allEventSelections = new List<EventSelection>();

      public EventSelections()
      {
      }

      public EventSelections(IEnumerable<EventSelection> eventSelections)
      {
         _allEventSelections.AddRange(eventSelections);
      }

      public IEnumerator<EventSelection> GetEnumerator()
      {
         return _allEventSelections.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      public void AddEventSelection(EventSelection eventSelection) => _allEventSelections.Add(eventSelection);
   }
}