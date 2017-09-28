using OSPSuite.Utility.Collections;

namespace PKSim.Core.Snapshots.Mappers
{
   public class SnapshotClassificationContext
   {
      readonly Cache<Classification, Classification> _childParentCache = new Cache<Classification, Classification>();

      public void AddClassificationWithParent(Classification child, Classification parent)
      {
         _childParentCache.Add(child, parent);
      }

      public bool ContainsParentFor(Classification snapshot)
      {
         return _childParentCache.Contains(snapshot);
      }

      public Classification ParentFor(Classification snapshot)
      {
         return _childParentCache[snapshot];
      }
   }
}