using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services
{
   public class StartableWarmup : IStartableWarmup
   {
      private Task _warmupTask;

      public void Begin(IReadOnlyList<IStartable> startables)
      {
         _warmupTask = Task.Factory.StartNew(
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

      public void AwaitCompletion()
      {
         var task = _warmupTask;
         if (task == null)
            return;

         _warmupTask = null;
         //GetResult rethrows the original warm-up exception (unwrapped) on the calling (UI) thread.
         task.GetAwaiter().GetResult();
      }
   }
}
