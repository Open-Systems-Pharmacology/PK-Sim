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
      ///    The first initialization wins: a warm-up that already began or ran keeps its startable set.
      /// </summary>
      void Begin(IReadOnlyList<IStartable> startables);

      /// <summary>
      ///    Blocks until the warm-up has completed, rethrowing any exception it raised. When no warm-up was started
      ///    with <see cref="Begin" /> (CLI, R and qualification hosts), the first call starts it by resolving every
      ///    <see cref="IStartable" /> itself. A failed warm-up stays failed: every call rethrows the same exception.
      /// </summary>
      void AwaitCompletion();
   }
}
