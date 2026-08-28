using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Container;
using PKSim.Assets;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services
{
   public class StartableWarmup : IStartableWarmup
   {
      private readonly IContainer _container;
      private readonly IOSPSuiteLogger _logger;
      private readonly object _lock = new object();

      //completes with the startables that failed to start (empty when the warm-up fully succeeded)
      private Task<IReadOnlyList<IStartable>> _warmupTask;

      public StartableWarmup(IContainer container, IOSPSuiteLogger logger)
      {
         _container = container;
         _logger = logger;
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

      public bool AwaitCompletion()
      {
         Task<IReadOnlyList<IStartable>> task;
         lock (_lock)
         {
            //hosts that never called Begin (CLI, R, qualification) warm up on first use
            _warmupTask ??= warmupTaskFor(_container.ResolveAll<IStartable>().ToList());
            task = _warmupTask;
         }

         //the task only faults on an out-of-memory failure, which must fail the operation rather than
         //degrade it silently: it rethrows on every call for the life of the memory-exhausted process
         return !task.GetAwaiter().GetResult().Any();
      }

      private Task<IReadOnlyList<IStartable>> warmupTaskFor(IReadOnlyList<IStartable> startables)
      {
         return Task.Factory.StartNew<IReadOnlyList<IStartable>>(
            () =>
            {
               //DB values are mapped using the current culture; match the culture InfrastructureRegister sets for the UI thread.
               Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
               Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
               //one broken startable must not leave the remaining ones cold
               return startables.Where(startable => !start(startable)).ToList();
            },
            CancellationToken.None,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
      }

      //a failed startable stays cold and is logged by name; its only retry is the lazy Start on first use:
      //the warm-up never runs Start again on a repository that may already have mutated part of its state
      private bool start(IStartable startable)
      {
         try
         {
            startable.Start();
            return true;
         }
         catch (Exception e) when (!e.IsOutOfMemory())
         {
            _logger.AddException(e);
            _logger.AddError(PKSimConstants.Error.StartableFailedToStart(startable.GetType().Name));
            return false;
         }
      }
   }
}
