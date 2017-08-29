using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using PKSim.BatchTool.Services;
using PKSim.BatchTool.Views;
using PKSim.Core;
using PKSim.Core.Batch;

namespace PKSim.BatchTool.Presenters
{
   public interface IInputAndOutputBatchPresenter : IBatchPresenter
   {
      bool SelectInputFolder();
      bool SelectOutputFolder();
   }

   public abstract class InputAndOutputBatchPresenter<TBatchRunner, TStartOptions> : BatchPresenter<IInputAndOutputBatchView<TStartOptions>, IInputAndOutputBatchPresenter, TBatchRunner, TStartOptions>, IInputAndOutputBatchPresenter
      where TBatchRunner : IBatchRunner<TStartOptions>
      where TStartOptions : IWithInputAndOutputFolders, new()
   {
      protected InputAndOutputBatchPresenter(IInputAndOutputBatchView<TStartOptions> view, TBatchRunner batchRunner, IDialogCreator dialogCreator, ILogPresenter logPresenter, IBatchLogger batchLogger)
         : base(view, batchRunner, dialogCreator, logPresenter, batchLogger)
      {
      }

      public virtual bool SelectInputFolder()
      {
         var inputFolder = _dialogCreator.AskForFolder("Select input folder", CoreConstants.DirectoryKey.BATCH);
         if (string.IsNullOrEmpty(inputFolder))
            return false;

         _startOptions.InputFolder = inputFolder;
         return true;
      }

      public virtual bool SelectOutputFolder()
      {
         var outputFolder = _dialogCreator.AskForFolder("Select output folder where results will be exported", CoreConstants.DirectoryKey.BATCH);
         if (string.IsNullOrEmpty(outputFolder))
            return false;

         _startOptions.OutputFolder = outputFolder;
         return true;
      }
   }
}