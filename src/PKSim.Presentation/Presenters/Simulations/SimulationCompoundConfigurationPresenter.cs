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
      private readonly ISimulationCompoundOverwriteParameterSetSelectionPresenter _overwriteParameterSetSelectionPresenter;

      public SimulationCompoundConfigurationPresenter(
         ISimulationCompoundConfigurationView view,
         ISimulationCompoundParameterAlternativesSelectionPresenter alternativesSelectionPresenter,
         ISimulationCompoundCalculationMethodSelectionPresenter calculationMethodSelectionPresenter,
         ISimulationCompoundOverwriteParameterSetSelectionPresenter overwriteParameterSetSelectionPresenter)
         : base(view)
      {
         _alternativesSelectionPresenter = alternativesSelectionPresenter;
         _calculationMethodSelectionPresenter = calculationMethodSelectionPresenter;
         _overwriteParameterSetSelectionPresenter = overwriteParameterSetSelectionPresenter;
         AddSubPresenters(_alternativesSelectionPresenter, _calculationMethodSelectionPresenter, _overwriteParameterSetSelectionPresenter);
         view.AddCalculationMethodsView(_calculationMethodSelectionPresenter.View);
         view.AddParameterAlternativesView(_alternativesSelectionPresenter.View);
         view.AddOverwriteParameterSetSelectionView(_overwriteParameterSetSelectionPresenter.View);
      }

      public void SaveConfiguration()
      {
         _alternativesSelectionPresenter.SaveConfiguration();
         _calculationMethodSelectionPresenter.SaveConfiguration();
         _overwriteParameterSetSelectionPresenter.SaveConfiguration();
      }

      public void EditSimulation(Simulation simulation, Compound compound)
      {
         _alternativesSelectionPresenter.EditSimulation(simulation, compound);
         _calculationMethodSelectionPresenter.EditSimulation(simulation, compound);
         _overwriteParameterSetSelectionPresenter.EditSimulation(simulation, compound);
      }
   }
}