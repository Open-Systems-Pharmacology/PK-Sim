using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OSPSuite.Utility;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
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

         //a failed warm-up stays failed: every call rethrows the same exception, keeping the broken
         //installation loud without running Start again on a repository that already mutated its state
         task.GetAwaiter().GetResult();
      }

      private static Task warmupTaskFor(IReadOnlyList<IStartable> startables)
      {
         return Task.Factory.StartNew(
            () =>
            {
               //DB values are mapped using the current culture; match the culture InfrastructureRegister sets for the UI thread.
               Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
               Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
               startables.Each(start);
            },
            CancellationToken.None,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
      }

      private static void start(IStartable startable)
      {
         try
         {
            startable.Start();
         }
         catch (Exception e)
         {
            throw new PKSimException($"'{startable.GetType().Name}' failed to start", e);
         }
      }
   }
}
