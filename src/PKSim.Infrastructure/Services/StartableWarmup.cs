using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OSPSuite.Utility;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services
{
   public class StartableWarmup : IStartableWarmup
   {
      private readonly IContainer _container;
      private readonly object _lock = new object();
      private Task _warmupTask;

      public StartableWarmup(IContainer container)
      {
         _container = container;
      }

      public void Begin(IReadOnlyList<IStartable> startables)
      {
         lock (_lock)
         {
            if (_warmupTask != null)
               return;

            _warmupTask = warmupTaskFor(startables);
         }
      }

      public void AwaitCompletion()
      {
         Task task;
         lock (_lock)
         {
            //hosts that never called Begin (CLI, R, qualification) warm up on first use
            _warmupTask ??= warmupTaskFor(_container.ResolveAll<IStartable>().ToList());
            task = _warmupTask;
         }

         try
         {
            //GetResult rethrows the original warm-up exception (unwrapped) on the calling thread
            task.GetAwaiter().GetResult();
         }
         catch
         {
            //a failed warm-up is dropped so that the next call retries instead of rethrowing a stale exception
            lock (_lock)
            {
               if (_warmupTask == task)
                  _warmupTask = null;
            }

            throw;
         }
      }

      private static Task warmupTaskFor(IReadOnlyList<IStartable> startables)
      {
         return Task.Factory.StartNew(
            () =>
            {
               //DB values are mapped using the current culture; match the culture InfrastructureRegister sets for the UI thread.
               Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
               Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
               startables.Each(x => x.Start());
            },
            CancellationToken.None,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
      }
   }
}
