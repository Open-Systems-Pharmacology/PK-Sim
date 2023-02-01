using OSPSuite.Presentation.Presenters;
using PKSim.Core.Model;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationCompoundConfigurationPresenter : IEditSimulationCompoundPresenter
   {
   }

   public class SimulationCompoundConfigurationPresenter : AbstractSubPresenter<ISimulationCompoundConfigurationView, ISimulationCompoundConfigurationPresenter>,
      ISimulationCompoundConfigurationPresenter
   {
      private readonly ISimulationCompoundParameterAlternativesSelectionPresenter _alternativesSelectionPresenter;
      private readonly ISimulationCompoundCalculationMethodSelectionPresenter _calculationMethodSelectionPresenter;

      public SimulationCompoundConfigurationPresenter(
         ISimulationCompoundConfigurationView view,
         ISimulationCompoundParameterAlternativesSelectionPresenter alternativesSelectionPresenter,
         ISimulationCompoundCalculationMethodSelectionPresenter calculationMethodSelectionPresenter)
         : base(view)
      {
         _alternativesSelectionPresenter = alternativesSelectionPresenter;
         _calculationMethodSelectionPresenter = calculationMethodSelectionPresenter;
         AddSubPresenters(_alternativesSelectionPresenter, _calculationMethodSelectionPresenter);
         view.AddCalculationMethodsView(_calculationMethodSelectionPresenter.View);
         view.AddParameterAlternativesView(_alternativesSelectionPresenter.View);
      }

      public void SaveConfiguration()
      {
         _alternativesSelectionPresenter.SaveConfiguration();
         _calculationMethodSelectionPresenter.SaveConfiguration();
      }

      public void EditSimulation(Simulation simulation, Compound compound)
      {
         _alternativesSelectionPresenter.EditSimulation(simulation, compound);
         _calculationMethodSelectionPresenter.EditSimulation(simulation, compound);
      }
   }
}