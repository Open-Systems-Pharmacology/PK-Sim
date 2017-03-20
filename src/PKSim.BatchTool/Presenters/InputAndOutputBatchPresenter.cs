using System.Threading.Tasks;
using OSPSuite.Utility.Validation;
using PKSim.BatchTool.Services;
using PKSim.BatchTool.Views;
using PKSim.Core;
using PKSim.Core.Batch;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;

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

      public InputAndOutputBatchPresenter(IInputAndOutputBatchView view, TBatchRunner batchRunner, IDialogCreator dialogCreator, ILogPresenter logPresenter, IBatchLogger batchLogger)
         : base(view, batchRunner, dialogCreator, logPresenter, batchLogger)
      {
      }

      public virtual void SelectInputFolder()
      {
         string inputFolder = _dialogCreator.AskForFolder("Select input folder", CoreConstants.DirectoryKey.BATCH);
         if (string.IsNullOrEmpty(inputFolder)) return;
         _dto.InputFolder = inputFolder;
      }

      public virtual void SelectOutputFolder()
      {
         string outputFolder = _dialogCreator.AskForFolder("Select output folder where results will be exported", CoreConstants.DirectoryKey.BATCH);
         if (string.IsNullOrEmpty(outputFolder)) return;
         _dto.OutputFolder = outputFolder;
      }

      protected override Task StartBatch()
      {
         return _batchRunner.RunBatch(
            new
            {
               inputFolder = _dto.InputFolder,
               outputFolder = _dto.OutputFolder
            });
      }

      public override void InitializeWith(BatchStartOptions startOptions)
      {
         {
            _dto = new InputAndOutputBatchDTO();
            if (startOptions.IsValid)
            {
               _startedFromCommandLine = true;
               _dto.InputFolder = startOptions.InputFolder;
               _dto.OutputFolder = startOptions.OutputFolder;
            }

            _view.BindTo(_dto);
            _view.Display();
            if (_dto.IsValid())
               RunBatch();
            else
               _startedFromCommandLine = false;
         }
      }
   }
}