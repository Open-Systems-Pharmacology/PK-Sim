using OSPSuite.Presentation.Views;
using PKSim.BatchTool.Presenters;
using PKSim.CLI.Core.RunOptions;

namespace PKSim.BatchTool.Views
{
   public interface IInputAndOutputBatchView<TStartOptionsDTO> : IView<IInputAndOutputBatchPresenter>, IBatchView<TStartOptionsDTO> where TStartOptionsDTO : IWithInputAndOutputFolders
   {
   }
}