using System.Collections.Generic;
using OSPSuite.Utility;

namespace PKSim.Core.Services
{
   /// <summary>
   ///    Warms up a set of <see cref="IStartable" /> repositories on a dedicated background thread so their
   ///    (DB-backed) loading overlaps the main window construction instead of blocking startup.
   /// </summary>
   public interface IStartableWarmup
   {
      /// <summary>
      ///    Starts warming up the given <paramref name="startables" /> on a background thread and returns immediately.
      /// </summary>
      void Begin(IReadOnlyList<IStartable> startables);

      /// <summary>
      ///    Blocks until the warm-up started by <see cref="Begin" /> has completed, rethrowing any exception it raised.
      ///    A no-op when no warm-up was started.
      /// </summary>
      void AwaitCompletion();
   }
}
