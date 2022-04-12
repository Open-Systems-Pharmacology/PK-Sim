using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Services;
using PKSim.BatchTool.Views;
using PKSim.CLI.Core.RunOptions;
using PKSim.CLI.Core.Services;
using PKSim.Core;

namespace PKSim.BatchTool.Presenters
{
   public interface IJsonSimulationBatchPresenter : IBatchPresenter
   {
      bool SelectInputFolder();
      bool SelectOutputFolder();
   }

   public class JsonSimulationBatchPresenter : BatchPresenter<IJsonSimulationBatchView, IJsonSimulationBatchPresenter, JsonSimulationRunner, JsonRunOptions>, IJsonSimulationBatchPresenter
   {
      public JsonSimulationBatchPresenter(IJsonSimulationBatchView view, JsonSimulationRunner batchRunner, IDialogCreator dialogCreator, ILogPresenter logPresenter, IOSPSuiteLogger batchLogger, DirectoryMapSettings directoryMapSettings)
         : base(view, batchRunner, dialogCreator, logPresenter, batchLogger)
      {
         _runOptionsDTO.InputFolder = directoryMapSettings.UsedDirectories[CoreConstants.DirectoryKey.BATCH_INPUT].Path;
         _runOptionsDTO.OutputFolder = directoryMapSettings.UsedDirectories[CoreConstants.DirectoryKey.BATCH_OUTPUT].Path;
         view.Caption = "PK-Sim BatchTool: Batch runner for json based PK-Sim simulations";
         _runOptionsDTO.ExportMode = SimulationExportMode.Json | SimulationExportMode.Csv;
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