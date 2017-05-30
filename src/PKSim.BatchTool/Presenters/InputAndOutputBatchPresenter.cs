using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Validation;
using PKSim.BatchTool.Services;
using PKSim.BatchTool.Views;
using PKSim.Core;
using PKSim.Core.Batch;

namespace PKSim.BatchTool.Presenters
{
   public interface IInputAndOutputBatchPresenter : IBatchPresenter
   {
      void SelectInputFolder();
      void SelectOutputFolder();
   }

   public class InputAndOutputBatchPresenter<TBatchRunner> : BatchPresenter<IInputAndOutputBatchView, IInputAndOutputBatchPresenter, TBatchRunner>, IInputAndOutputBatchPresenter where TBatchRunner : IBatchRunner
   {
      protected InputAndOutputBatchDTO _dto;
      private string _logFileFullPath;

      public InputAndOutputBatchPresenter(IInputAndOutputBatchView view, TBatchRunner batchRunner, IDialogCreator dialogCreator, ILogPresenter logPresenter, IBatchLogger batchLogger)
         : base(view, batchRunner, dialogCreator, logPresenter, batchLogger)
      {
      }

      public virtual void SelectInputFolder()
      {
         var inputFolder = _dialogCreator.AskForFolder("Select input folder", CoreConstants.DirectoryKey.BATCH);
         if (string.IsNullOrEmpty(inputFolder)) return;
         _dto.InputFolder = inputFolder;
      }

      public virtual void SelectOutputFolder()
      {
         var outputFolder = _dialogCreator.AskForFolder("Select output folder where results will be exported", CoreConstants.DirectoryKey.BATCH);
         if (string.IsNullOrEmpty(outputFolder)) return;
         _dto.OutputFolder = outputFolder;
         _logFileFullPath = CoreConstants.DefaultBatchLogFullPath(outputFolder);
      }

      protected override Task StartBatch()
      {
         return _batchRunner.RunBatch(
            new
            {
               inputFolder = _dto.InputFolder,
               outputFolder = _dto.OutputFolder,
               logFileFullPath = _logFileFullPath,
               exportMode = exportModeFrom(_startedFromCommandLine),
               notificationType = notificationTypeFrom(_startedFromCommandLine),
            });
      }

      private NotificationType notificationTypeFrom(bool startedFromCommandLine)
      {
         return startedFromCommandLine ? NotificationType.Info | NotificationType.Error : NotificationType.All;
      }

      private BatchExportMode exportModeFrom(bool startedFromCommandLine)
      {
         return startedFromCommandLine ? BatchExportMode.Json | BatchExportMode.Xml : BatchExportMode.All;
      }

      public override async Task InitializeWith(BatchStartOptions startOptions)
      {
         _dto = new InputAndOutputBatchDTO();
         if (startOptions.IsValid())
         {
            _startedFromCommandLine = true;
            _logFileFullPath = startOptions.LogFileFullPath;
            _dto.InputFolder = startOptions.InputFolder;
            _dto.OutputFolder = startOptions.OutputFolder;
            await RunBatch();
         }
         else
         {
            _startedFromCommandLine = false;
            _view.BindTo(_dto);
            _view.Display();
         }
      }
   }
}