using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using PKSim.BatchTool.Views;
using PKSim.CLI.Core.RunOptions;
using PKSim.CLI.Core.Services;
using PKSim.Core;
using PKSim.Core.Batch;

namespace PKSim.BatchTool.Presenters
{
   public interface IJsonSimulationBatchPresenter : IBatchPresenter
   {
   }

   public class JsonSimulationBatchPresenter : InputAndOutputBatchPresenter<JsonSimulationRunner, JsonRunOptions>, IJsonSimulationBatchPresenter
   {
      public JsonSimulationBatchPresenter(IInputAndOutputBatchView<JsonRunOptions> view, JsonSimulationRunner batchRunner, IDialogCreator dialogCreator, ILogPresenter logPresenter, IBatchLogger batchLogger) :
         base(view, batchRunner, dialogCreator, logPresenter, batchLogger)
      {
         view.Caption = "PK-Sim BatchTool: Batch runner for json based PK-Sim simulations";
         _runOptionsDTO.ExportMode = BatchExportMode.All;
         _runOptionsDTO.NotificationType = NotificationType.All;
      }

      public override bool SelectOutputFolder()
      {
         if (!base.SelectOutputFolder())
            return false;

         _runOptionsDTO.LogFileFullPath = CoreConstants.DefaultBatchLogFullPath(_runOptionsDTO.OutputFolder);
         return true;
      }
   }
}