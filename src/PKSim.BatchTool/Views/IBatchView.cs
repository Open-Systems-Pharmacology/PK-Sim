using PKSim.BatchTool.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.BatchTool.Views
{
   public interface IBatchView
   {
      bool CalculateEnabled { set; }
      void Display();
      void AddLogView(IView view);
   }

   public interface IInputAndOutputBatchView : IView<IInputAndOutputBatchPresenter>, IBatchView
   {
      void BindTo(InputAndOutputBatchDTO batchDTO);
   }
}