using PKSim.BatchTool.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.BatchTool.Views
{
   public interface IBatchMainView : IView<IBatchMainPresenter>
   {
      void Hide();
   }
}