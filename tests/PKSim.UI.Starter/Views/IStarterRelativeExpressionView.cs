using OSPSuite.Presentation.Views;
using PKSim.UI.Starter.Presenters;

namespace PKSim.UI.Starter.Views
{
   public interface IStarterRelativeExpressionView : IModalView<IStarterRelativeExpressionPresenter>
   {
      void AddExpressionPresenter(IView view);
   }
}