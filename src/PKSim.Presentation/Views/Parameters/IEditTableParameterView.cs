using PKSim.Presentation.Presenters.Parameters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Parameters
{
   public interface IEditTableParameterView : IModalView<IEditTableParameterPresenter>
   {
      void AddView(IView baseView);
      void AddChart(IView baseView);
   }
}