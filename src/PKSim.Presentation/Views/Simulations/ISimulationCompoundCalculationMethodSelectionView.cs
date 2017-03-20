using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationCompoundCalculationMethodSelectionView : IView<ISimulationCompoundCalculationMethodSelectionPresenter>, IResizableView
   {
      void AddCalculationMethodsView(IView view);
   }
}