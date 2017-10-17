using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;
using PKSim.BatchTool.Views;
using PKSim.CLI.Core.Services;
using PKSim.Core.Batch;

namespace PKSim.BatchTool.Presenters
{
   public interface IBatchPresenter : IPresenter
   {
      bool Exit();
      Task RunBatch();
      void InitializeForStandAloneStart();
   }

   public abstract class BatchPresenter<TView, TPresenter, TBatchRunner, TRunOptions> : AbstractPresenter<TView, TPresenter>, IBatchPresenter
      where TBatchRunner : IBatchRunner<TRunOptions>
      where TView : IView<TPresenter>, IBatchView<TRunOptions>
      where TPresenter : IPresenter
      where TRunOptions : new()

   {
      protected readonly TBatchRunner _batchRunner;
      protected readonly IDialogCreator _dialogCreator;
      private readonly ILogPresenter _logPresenter;
      private readonly ILogger _batchLogger;
      protected bool _isRunning;
      protected TRunOptions _runOptionsDTO = new TRunOptions();

      protected BatchPresenter(TView view, TBatchRunner batchRunner, IDialogCreator dialogCreator, ILogPresenter logPresenter, ILogger batchLogger)
         : base(view)
      {
         _batchRunner = batchRunner;
         _dialogCreator = dialogCreator;
         _logPresenter = logPresenter;
         _batchLogger = batchLogger;
         _view.AddLogView(_logPresenter.View);
         AddSubPresenters(_logPresenter);
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
      }

      public virtual void InitializeForStandAloneStart()
      {
         _view.BindTo(_runOptionsDTO);
         _view.Show();
      }

      protected virtual Task StartBatch()
      {
         return _batchRunner.RunBatchAsync(_runOptionsDTO);
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
   }
}