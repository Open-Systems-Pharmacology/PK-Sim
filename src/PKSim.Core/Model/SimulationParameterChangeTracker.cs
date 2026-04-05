using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model
{
   public class SimulationParameterChangeTracker
   {
      private readonly HashSet<ObjectPath> _changedPaths = new();

      public bool HasUncommittedChanges => _changedPaths.Count > 0;

      public IReadOnlyList<ObjectPath> ChangedPaths => _changedPaths.ToList();

      public void Track(ObjectPath path) => _changedPaths.Add(path);

      public void Track(string path) => Track(path.ToObjectPath());

      public void Untrack(ObjectPath path) => _changedPaths.Remove(path);

      public void Untrack(string path) => Untrack(path.ToObjectPath());

      public void Clear() => _changedPaths.Clear();

      public IReadOnlyList<ObjectPath> GetChangedPaths() => _changedPaths.ToList();

      public SimulationParameterChangeTracker Clone()
      {
         var clone = new SimulationParameterChangeTracker();
         _changedPaths.Each(path => clone.Track(new ObjectPath(path)));
         return clone;
      }
   }
}
