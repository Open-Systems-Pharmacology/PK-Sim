using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class SimulationParameterChangeTracker
   {
      private readonly HashSet<ObjectPath> _changedPaths = new();

      public bool HasUncommittedChanges => _changedPaths.Count > 0;

      public IReadOnlyList<ObjectPath> ChangedPaths => _changedPaths.ToList();

      public void Track(ObjectPath path) => _changedPaths.Add(path);

      public void Untrack(ObjectPath path) => _changedPaths.Remove(path);

      public void Clear() => _changedPaths.Clear();

      public IReadOnlyList<ObjectPath> GetChangedPaths() => _changedPaths.ToList();

      public SimulationParameterChangeTracker Clone()
      {
         var clone = new SimulationParameterChangeTracker();
         foreach (var path in _changedPaths)
            clone.Track(new ObjectPath(path));
         return clone;
      }
   }
}
