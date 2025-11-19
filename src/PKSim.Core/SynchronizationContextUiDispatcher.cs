using System;
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

   /// <summary>
   /// Provides a dispatcher that posts actions to the UI thread using a <see cref="SynchronizationContext"/>.
   /// </summary>
   public sealed class SynchronizationContextUiDispatcher : ISynchronizationContextUiDispatcher
   {
      private readonly SynchronizationContext _ui;

      /// <summary>
      /// Initializes a new instance of the <see cref="SynchronizationContextUiDispatcher"/> class.
      /// </summary>
      /// <param name="uiContext">The synchronization context for the UI thread.</param>
      public SynchronizationContextUiDispatcher(SynchronizationContext uiContext)
         => _ui = uiContext ?? throw new InvalidOperationException("Must be created on UI thread");

      public void Post(Action action) => _ui.Post(_ => action(), null);

      /// <summary>
      /// Posts the specified action to the UI thread.
      /// </summary>
      /// <param name="action">The action to execute on the UI thread.</param>
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
