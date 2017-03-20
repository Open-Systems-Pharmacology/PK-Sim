using PKSim.Presentation.Presenters.Compounds;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Compounds
{
   public interface ICalculatedParameterValueView : IView<ICalculatedParameterValuePresenter>
   {
      void SetParameterView(IView view);
      string Description { set; }
   }
}