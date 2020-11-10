using OSPSuite.Presentation.Views;
using PKSim.Presentation.Presenters.Simulations;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationExpressionsView : IView<ISimulationExpressionsPresenter>
   {
      void AddMoleculeParametersView(IView view);
      void AddExpressionParametersView(IView view);
   }
}