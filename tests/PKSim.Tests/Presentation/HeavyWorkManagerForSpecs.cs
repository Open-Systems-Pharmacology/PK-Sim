using System;
using System.Threading;
using OSPSuite.Core.Services;

namespace PKSim.Presentation
{
   public class HeavyWorkManagerForSpecs : IHeavyWorkManager
   {
      public event EventHandler<HeavyWorkEventArgs> HeavyWorkedFinished = delegate { };

      public bool Start(Action heavyWorkAction, CancellationTokenSource cts = null)
      {
         return Start(heavyWorkAction, string.Empty);
      }

      public virtual bool Start(Action heavyWorkAction, string caption, CancellationTokenSource cts = null)
      {
         heavyWorkAction();
         return true;
      }

      public void StartAsync(Action heavyWorkAction)
      {
         heavyWorkAction();
         HeavyWorkedFinished(this, new HeavyWorkEventArgs(true));
      }
   }

   public class HeavyWorkManagerFailingForSpecs : HeavyWorkManagerForSpecs
   {
      public override bool Start(Action heavyWorkAction, string caption, CancellationTokenSource cts = null)
      {
         base.Start(heavyWorkAction, caption);
         return false;
      }
   }
}