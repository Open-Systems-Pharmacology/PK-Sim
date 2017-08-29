using System.Collections;
using System.Collections.Generic;

namespace PKSim.Core.Snapshots
{
   public class OutputSchema : IEnumerable<OutputInterval>
   {
      private readonly List<OutputInterval> _allOutputIntervals = new List<OutputInterval>();

      public OutputSchema()
      {
      }

      public OutputSchema(IEnumerable<OutputInterval> outputIntervals)
      {
         _allOutputIntervals.AddRange(outputIntervals);
      }

      public IEnumerator<OutputInterval> GetEnumerator()
      {
         return _allOutputIntervals.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      public void Add(OutputInterval outputInterval) => _allOutputIntervals.Add(outputInterval);
   }
}