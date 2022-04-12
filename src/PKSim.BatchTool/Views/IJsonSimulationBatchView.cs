using OSPSuite.Presentation.Views;
using PKSim.BatchTool.Presenters;
using PKSim.CLI.Core.RunOptions;

namespace PKSim.BatchTool.Views
{
   public interface IJsonSimulationBatchView : IView<IJsonSimulationBatchPresenter>, IBatchView<JsonRunOptions>
   {
      
   }
}