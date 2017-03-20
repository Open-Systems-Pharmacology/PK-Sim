using PKSim.BatchTool.Services;
using PKSim.BatchTool.Views;
using PKSim.Core.Batch;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;

namespace PKSim.BatchTool.Presenters
{
   public interface IJsonSimulationBatchPresenter : IBatchPresenter
   {
   }

   public class JsonSimulationBatchPresenter : InputAndOutputBatchPresenter<JsonSimulationRunner>, IJsonSimulationBatchPresenter
   {
      public JsonSimulationBatchPresenter(IInputAndOutputBatchView view, JsonSimulationRunner batchRunner, IDialogCreator dialogCreator, ILogPresenter logPresenter, IBatchLogger batchLogger) :
         base(view, batchRunner, dialogCreator, logPresenter, batchLogger)
      {
         view.Caption = "PK-Sim BatchTool: Batch runner for json based PK-Sim simulations";
      }
   }
}