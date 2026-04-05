using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model
{
   /// <summary>
   ///    Tracks compound-dependent simulation parameter paths that have been modified by the user
   ///    but not yet committed to an <see cref="OverwriteParameterSet"/>.
   ///    Only parameters whose path matches a building block compound are tracked (not unnamed metabolites).
   ///    Persisted with the simulation so uncommitted changes survive save/load.
   /// </summary>
   //TODO: Consider storing compound name alongside each path to avoid re-resolving via
   //CompoundNameForParameterPath when grouping by compound in the commit workflow.
   public class SimulationParameterChangeTracker
   {
      private readonly HashSet<ObjectPath> _changedPaths = new();

      public bool HasUncommittedChanges => _changedPaths.Count > 0;

      public IReadOnlyList<ObjectPath> ChangedPaths => _changedPaths.ToList();

      public void Track(ObjectPath path) => _changedPaths.Add(path);

      public void Track(string path) => Track(path.ToObjectPath());

      public void Untrack(ObjectPath path) => _changedPaths.Remove(path);

      public void Untrack(string path) => Untrack(path.ToObjectPath());

      public bool IsTracked(string path) => _changedPaths.Contains(path.ToObjectPath());

      public void Clear() => _changedPaths.Clear();

      public SimulationParameterChangeTracker Clone()
      {
         var clone = new SimulationParameterChangeTracker();
         _changedPaths.Each(path => clone.Track(new ObjectPath(path)));
         return clone;
      }
   }
}
