using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationCompoundCalculationMethodsSelectionPresenter : ContextSpecification<ISimulationCompoundCalculationMethodSelectionPresenter>
   {
      protected ISimulationCompoundCalculationMethodSelectionView _view;
      protected Simulation _simulation;
      protected Compound _compound;
      protected CompoundProperties _compoundProperties;
      protected ICalculationMethodSelectionPresenterForSimulation _calculationMethodSelectionPresenter;

      protected override void Context()
      {
         _view = A.Fake<ISimulationCompoundCalculationMethodSelectionView>();
         _calculationMethodSelectionPresenter = A.Fake<ICalculationMethodSelectionPresenterForSimulation>();

         sut = new SimulationCompoundCalculationMethodSelectionPresenter(_view, _calculationMethodSelectionPresenter);

         _compound = new Compound();
         _simulation = new IndividualSimulation {Properties = new SimulationProperties()};
         _compoundProperties = new CompoundProperties {Compound = _compound};
         _simulation.Properties.AddCompoundProperties(_compoundProperties);
      }
   }

   public class When_editing_the_calculation_methods_defined_for_a_given_compound_in_a_simulation : concern_for_SimulationCompoundCalculationMethodsSelectionPresenter
   {
      protected override void Because()
      {
         sut.EditSimulation(_simulation, _compound);
      }

      [Observation]
      public void should_edit_the_calculation_method_defined_in_the_compound_properties_for_the_given_compound()
      {
         A.CallTo(() => _calculationMethodSelectionPresenter.Edit(_compoundProperties, null)).MustHaveHappened();
      }
   }

   public class When_saving_the_calculation_method_selection_for_a_given_compound_in_a_simulation : concern_for_SimulationCompoundCalculationMethodsSelectionPresenter
   {
      protected override void Because()
      {
         sut.SaveConfiguration();
      }

      [Observation]
      public void should_have_save_the_selected_calculation_method_in_the_compound_properties()
      {
         A.CallTo(() => _calculationMethodSelectionPresenter.SaveConfiguration()).MustHaveHappened();
      }
   }
}