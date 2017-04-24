using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using OSPSuite.Utility.Extensions;
using PKSim.BatchTool.Services;
using PKSim.BatchTool.Views;
using PKSim.Core.Batch;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.BatchTool.Presenters
{
   public interface IBatchPresenter: IPresenter
   {
      void Exit();
      Task RunBatch();
      Task InitializeWith(BatchStartOptions startOptions);
   }

   public abstract class BatchPresenter<TView, TPresenter, TBatchRunner> : AbstractPresenter<TView, TPresenter>, IBatchPresenter
      where TBatchRunner : IBatchRunner
      where TView : IView<TPresenter>, IBatchView 
      where TPresenter : IPresenter

   {
      protected readonly TBatchRunner _batchRunner;
      protected readonly IDialogCreator _dialogCreator;
      private readonly ILogPresenter _logPresenter;
      private readonly IBatchLogger _batchLogger;
      private bool _isRunning;
      protected bool _startedFromCommandLine;

      protected BatchPresenter(TView view, TBatchRunner batchRunner, IDialogCreator dialogCreator, ILogPresenter logPresenter, IBatchLogger batchLogger)
         : base(view)
      {
         _batchRunner = batchRunner;
         _dialogCreator = dialogCreator;
         _logPresenter = logPresenter;
         _batchLogger = batchLogger;
         _view.AddLogView(_logPresenter.View);
      }

      public virtual async Task RunBatch()
      {
         if (_isRunning) return;
         _isRunning = true;
         _logPresenter.ClearLog();
         _view.CalculateEnabled = false;

         try
         {
            await StartBatch();
         }
         catch (Exception e)
         {
            _batchLogger.AddError(e.FullMessage());
         }

         _isRunning = false;
         _view.CalculateEnabled = true;

         if (shouldClose)
            Exit();
      }

      protected abstract Task StartBatch();

      public void Exit()
      {
         if (_isRunning)
         {
            var ans = _dialogCreator.MessageBoxYesNo("Batch is running. Really exit?");
            if (ans == ViewResult.No) return;
         }

         Application.Exit();
      }

      private bool shouldClose => _startedFromCommandLine ;//&& !Debugger.IsAttached;

      public virtual Task InitializeWith(BatchStartOptions startOptions)
      {
         return Task.FromResult(true);
      }
   }
}