using PKSim.Core.Model;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationCompoundCalculationMethodSelectionPresenter : IEditSimulationCompoundPresenter, IPresenter<ISimulationCompoundCalculationMethodSelectionView>
   {
   }

   public class SimulationCompoundCalculationMethodSelectionPresenter : AbstractSubPresenter<ISimulationCompoundCalculationMethodSelectionView, ISimulationCompoundCalculationMethodSelectionPresenter>, ISimulationCompoundCalculationMethodSelectionPresenter
   {
      private readonly ICalculationMethodSelectionPresenterForSimulation _calculationMethodSelectionPresenter;

      public SimulationCompoundCalculationMethodSelectionPresenter(
         ISimulationCompoundCalculationMethodSelectionView view, ICalculationMethodSelectionPresenterForSimulation calculationMethodSelectionPresenter)
         : base(view)
      {
         _calculationMethodSelectionPresenter = calculationMethodSelectionPresenter;
         AddSubPresenters(_calculationMethodSelectionPresenter);
         _view.AddCalculationMethodsView(_calculationMethodSelectionPresenter.BaseView);
      }

      public void SaveConfiguration()
      {
         _calculationMethodSelectionPresenter.SaveConfiguration();
      }

      public void EditSimulation(Simulation simulation, Compound compound)
      {
         var compoundProperties = simulation.CompoundPropertiesFor(compound);
         _calculationMethodSelectionPresenter.Edit(compoundProperties);
      }
   }
}