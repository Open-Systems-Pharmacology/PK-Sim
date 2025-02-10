using OSPSuite.Presentation.Views;
using PKSim.Presentation.Presenters.Parameters;

namespace PKSim.Presentation.Views.Parameters
{
   public interface IEditTableParameterView : IModalView<IEditTableParameterPresenter>
   {
      void AddView(IView baseView);
      void AddChart(IView baseView);
   }
}
