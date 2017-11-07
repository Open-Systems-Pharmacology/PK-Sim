using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using PKSim.BatchTool.Views;
using PKSim.CLI.Core.RunOptions;
using PKSim.CLI.Core.Services;

namespace PKSim.BatchTool.Presenters
{
   public interface IJsonSimulationBatchPresenter : IBatchPresenter
   {
   }

   public class JsonSimulationBatchPresenter : InputAndOutputBatchPresenter<JsonSimulationRunner, JsonRunOptions>, IJsonSimulationBatchPresenter
   {
      public JsonSimulationBatchPresenter(IInputAndOutputBatchView<JsonRunOptions> view, JsonSimulationRunner batchRunner, IDialogCreator dialogCreator, ILogPresenter logPresenter, ILogger batchLogger) :
         base(view, batchRunner, dialogCreator, logPresenter, batchLogger)
      {
         view.Caption = "PK-Sim BatchTool: Batch runner for json based PK-Sim simulations";
         _runOptionsDTO.ExportMode = SimulationExportMode.All;
      }
   }
}