using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using PKSim.BatchTool.Services;
using PKSim.BatchTool.Views;
using PKSim.Core;
using PKSim.Core.Batch;

namespace PKSim.BatchTool.Presenters
{
   public interface IJsonSimulationBatchPresenter : IBatchPresenter<JsonRunOptions>
   {
   }

   public class JsonSimulationBatchPresenter : InputAndOutputBatchPresenter<JsonSimulationRunner, JsonRunOptions>, IJsonSimulationBatchPresenter
   {
      public JsonSimulationBatchPresenter(IInputAndOutputBatchView<JsonRunOptions> view, JsonSimulationRunner batchRunner, IDialogCreator dialogCreator, ILogPresenter logPresenter, IBatchLogger batchLogger) :
         base(view, batchRunner, dialogCreator, logPresenter, batchLogger)
      {
         view.Caption = "PK-Sim BatchTool: Batch runner for json based PK-Sim simulations";
         _startOptions.ExportMode = BatchExportMode.All;
         _startOptions.NotificationType = NotificationType.All;
      }

      public override bool SelectOutputFolder()
      {
         if (!base.SelectOutputFolder())
            return false;

         _startOptions.LogFileFullPath = CoreConstants.DefaultBatchLogFullPath(_startOptions.OutputFolder);
         return true;
      }
   }
}