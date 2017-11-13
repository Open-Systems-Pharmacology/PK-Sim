using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Services;
using PKSim.BatchTool.Views;
using PKSim.CLI.Core.RunOptions;
using PKSim.CLI.Core.Services;
using PKSim.Core;
using PKSim.Core.Batch;

namespace PKSim.BatchTool.Presenters
{
   public interface IInputAndOutputBatchPresenter : IBatchPresenter
   {
      bool SelectInputFolder();
      bool SelectOutputFolder();
   }

   public abstract class InputAndOutputBatchPresenter<TBatchRunner, TRunOptions> : BatchPresenter<IInputAndOutputBatchView<TRunOptions>, IInputAndOutputBatchPresenter, TBatchRunner, TRunOptions>, IInputAndOutputBatchPresenter
      where TBatchRunner : IBatchRunner<TRunOptions>
      where TRunOptions : IWithInputAndOutputFolders, new()
   {

      protected InputAndOutputBatchPresenter(IInputAndOutputBatchView<TRunOptions> view, TBatchRunner batchRunner, IDialogCreator dialogCreator, ILogPresenter logPresenter, ILogger batchLogger, DirectoryMapSettings directoryMapSettings)
         : base(view, batchRunner, dialogCreator, logPresenter, batchLogger)
      {
         _runOptionsDTO.InputFolder = directoryMapSettings.UsedDirectories[CoreConstants.DirectoryKey.BATCH_INPUT].Path;
         _runOptionsDTO.OutputFolder = directoryMapSettings.UsedDirectories[CoreConstants.DirectoryKey.BATCH_OUTPUT].Path;
      }

      public virtual bool SelectInputFolder()
      {
         var inputFolder = _dialogCreator.AskForFolder("Select input folder", CoreConstants.DirectoryKey.BATCH_INPUT);
         if (string.IsNullOrEmpty(inputFolder))
            return false;

         _runOptionsDTO.InputFolder = inputFolder;
         return true;
      }

      public virtual bool SelectOutputFolder()
      {
         var outputFolder = _dialogCreator.AskForFolder("Select output folder where results will be exported", CoreConstants.DirectoryKey.BATCH_OUTPUT);
         if (string.IsNullOrEmpty(outputFolder))
            return false;

         _runOptionsDTO.OutputFolder = outputFolder;
         return true;
      }
   }
}