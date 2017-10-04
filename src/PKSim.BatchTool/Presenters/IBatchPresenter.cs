using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.BatchTool.Services;
using PKSim.BatchTool.Views;
using PKSim.Core.Batch;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.BatchTool.Presenters
{
   public interface IBatchPresenter : IPresenter
   {
      bool Exit();
      Task RunBatch();
      void InitializeForStandAloneStart();
   }

   public interface IBatchPresenter<TStartOptions>: IBatchPresenter
   {
      Task InitializeForCommandLineRunWith(TStartOptions startOptions);
   }

   public abstract class BatchPresenter<TView, TPresenter, TBatchRunner, TStartOptions> : AbstractPresenter<TView, TPresenter>, IBatchPresenter<TStartOptions>
      where TBatchRunner : IBatchRunner<TStartOptions>
      where TView : IView<TPresenter>, IBatchView<TStartOptions>
      where TPresenter : IPresenter
      where TStartOptions:new()

   {
      protected readonly TBatchRunner _batchRunner;
      protected readonly IDialogCreator _dialogCreator;
      private readonly ILogPresenter _logPresenter;
      private readonly IBatchLogger _batchLogger;
      private bool _isRunning;
      protected bool _startedFromCommandLine;
      protected TStartOptions _startOptions = new TStartOptions();

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
            _batchLogger.AddError(e.ExceptionMessageWithStackTrace());
         }

         _isRunning = false;
         _view.CalculateEnabled = true;

         if (shouldClose)
            Exit();
      }

      public virtual void InitializeForStandAloneStart()
      {
         _startedFromCommandLine = false;
         _view.BindTo(_startOptions);
         _view.Display();
      }


      protected virtual Task StartBatch()
      {
         return _batchRunner.RunBatchAsync(_startOptions);
      }

      public bool Exit()
      {
         if (_isRunning)
         {
            var ans = _dialogCreator.MessageBoxYesNo("Batch is running. Really exit?");
            if (ans == ViewResult.No)
               return false;
         }

         Application.Exit();
         return true;
      }

      private bool shouldClose => _startedFromCommandLine;

      public virtual async Task InitializeForCommandLineRunWith(TStartOptions startOptions)
      {
         _startOptions = startOptions;
         _startedFromCommandLine = true;
         await RunBatch();
      }
   }
}