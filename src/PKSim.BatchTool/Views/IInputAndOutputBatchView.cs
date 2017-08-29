using OSPSuite.Presentation.Views;
using PKSim.BatchTool.Presenters;

namespace PKSim.BatchTool.Views
{
   public interface IInputAndOutputBatchView<TStartOptions> : IView<IInputAndOutputBatchPresenter>, IBatchView<TStartOptions> where TStartOptions:IWithInputAndOutputFolders
   {
   }
}