using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PKSim.Core
{
   public interface ISynchronizationContextUiDispatcher
   {
      void Post(Action action);
      Task InvokeAsync(Action action, CancellationToken ct = default);
      bool IsUiThread { get; }
   }

   public sealed class SynchronizationContextUiDispatcher : ISynchronizationContextUiDispatcher
   {
      private readonly SynchronizationContext _ui;

      public SynchronizationContextUiDispatcher(SynchronizationContext uiContext)
         => _ui = uiContext ?? throw new InvalidOperationException("Must be created on UI thread");

      public void Post(Action action) => _ui.Post(_ => action(), null);

      public Task InvokeAsync(Action action, CancellationToken ct = default)
      {
         var tcs = new TaskCompletionSource<object?>();
         _ui.Post(_ =>
         {
            if (ct.IsCancellationRequested) { tcs.TrySetCanceled(ct); return; }
            try { action(); tcs.SetResult(null); }
            catch (Exception ex) { tcs.SetException(ex); }
         }, null);
         return tcs.Task;
      }

      public bool IsUiThread => SynchronizationContext.Current == _ui;
   }

}
