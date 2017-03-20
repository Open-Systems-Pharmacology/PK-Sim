using PKSim.Presentation.Presenters.Compounds;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Compounds
{
   public interface IMolWeightHalogensView : IView<IMolWeightHalogensPresenter>
   {
      void FillWithParameterView(IView parameterView);
   }
}