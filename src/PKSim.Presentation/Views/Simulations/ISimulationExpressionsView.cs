using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationExpressionsView : IView<ISimulationExpressionsPresenter>
   {
      void BindTo(SimulationExpressionsDTO simulationExpressionsDTO);
      void AddMoleculeParametersView(IView view);
   }
}