using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationCompoundConfigurationPresenter : ContextSpecification<ISimulationCompoundConfigurationPresenter>
   {
      protected ISimulationCompoundConfigurationView _view;
      protected ISimulationCompoundParameterAlternativesSelectionPresenter _alternativesSelectionPresenter;
      protected ISimulationCompoundCalculationMethodSelectionPresenter _calculationMethodSelectionPresenter;

      protected override void Context()
      {
         _view = A.Fake<ISimulationCompoundConfigurationView>();
         _alternativesSelectionPresenter = A.Fake<ISimulationCompoundParameterAlternativesSelectionPresenter>();
         _calculationMethodSelectionPresenter = A.Fake<ISimulationCompoundCalculationMethodSelectionPresenter>();
         sut = new SimulationCompoundConfigurationPresenter(_view, _alternativesSelectionPresenter, _calculationMethodSelectionPresenter);
      }
   }

   public class When_saving_the_simulation_configuration_for_a_given_compound : concern_for_SimulationCompoundConfigurationPresenter
   {
      protected override void Because()
      {
         sut.SaveConfiguration();
      }

      [Observation]
      public void should_save_the_alternative_selections()
      {
         A.CallTo(() => _alternativesSelectionPresenter.SaveConfiguration()).MustHaveHappened();
      }

      [Observation]
      public void should_save_the_calculation_methods_selection()
      {
         A.CallTo(() => _calculationMethodSelectionPresenter.SaveConfiguration()).MustHaveHappened();
      }
   }

   public class When_editing_the_configurationn_of_a_specific_compound_in_a_simulation : concern_for_SimulationCompoundConfigurationPresenter
   {
      private Compound _compound;
      private Simulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
         _compound = A.Fake<Compound>();
      }

      protected override void Because()
      {
         sut.EditSimulation(_simulation, _compound);
      }

      [Observation]
      public void should_edit_the_alternative_selections()
      {
         A.CallTo(() => _alternativesSelectionPresenter.EditSimulation(_simulation, _compound)).MustHaveHappened();
      }

      [Observation]
      public void should_edit_the_calculation_methods_selection()
      {
         A.CallTo(() => _calculationMethodSelectionPresenter.EditSimulation(_simulation, _compound)).MustHaveHappened();
      }
   }
}