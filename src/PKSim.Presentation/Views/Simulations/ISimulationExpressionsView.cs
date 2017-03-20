using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationExpressionsView : IView<ISimulationExpressionsPresenter>
   {
      void BindTo(SimulationExpressionsDTO simulationExpressionsDTO);
   }
}