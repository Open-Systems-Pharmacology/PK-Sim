using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationParametersView : IView<ISimulationParametersPresenter>
   {
      void AddParametersView(IView view);
   }
}