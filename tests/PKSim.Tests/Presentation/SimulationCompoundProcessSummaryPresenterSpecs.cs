using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationProcessSummaryPresenter : ContextSpecification<ISimulationCompoundProcessSummaryPresenter>
   {
      protected ISimulationCompoundProcessSummaryView _view;
      protected Compound _compound;
      protected Individual _individual;
      protected Simulation _simulation;
      protected CompoundProperties _compoundProperties;
      protected ISimulationCompoundEnzymaticProcessPresenter _simulationEnzymaticProcessPresenter;
      protected ISimulationCompoundTransportAndExcretionPresenter _simulationTransportAndExcretionPresenter;
      protected ISimulationCompoundSpecificBindingPresenter _simulationSpecificBindingPresenter;

      protected override void Context()
      {
         _view = A.Fake<ISimulationCompoundProcessSummaryView>();
         _simulationEnzymaticProcessPresenter = A.Fake<ISimulationCompoundEnzymaticProcessPresenter>();
         _simulationTransportAndExcretionPresenter = A.Fake<ISimulationCompoundTransportAndExcretionPresenter>();
         _simulationSpecificBindingPresenter = A.Fake<ISimulationCompoundSpecificBindingPresenter>();
         sut = new SimulationCompoundProcessSummaryPresenter(_view, _simulationEnzymaticProcessPresenter, _simulationTransportAndExcretionPresenter, _simulationSpecificBindingPresenter);

         _simulation = new IndividualSimulation {Properties = new SimulationProperties()};
         _compound = A.Fake<Compound>();
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("template",PKSimBuildingBlockType.Compound) { BuildingBlock = _compound });
         _individual = A.Fake<Individual>();
         _compoundProperties = new CompoundProperties {Compound = _compound};
         _simulation.Properties.AddCompoundProperties(_compoundProperties);
      }
   }

   public class When_editing_simulation_with_at_least_one_process_defined : concern_for_SimulationProcessSummaryPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _simulationEnzymaticProcessPresenter.HasProcessesDefined).Returns(true);
      }
      protected override void Because()
      {
         sut.EditSimulation(_simulation, _compound);
      }

      [Observation]
      public void should_have_processes_defined()
      {
         sut.HasProcessesDefined.ShouldBeTrue();
      }
   }

   public class When_editing_simulation_without_any_processes_defined : concern_for_SimulationProcessSummaryPresenter
   {
      protected override void Because()
      {
         sut.EditSimulation(_simulation, _compound);
      }

      [Observation]
      public void should_not_have_any_processes_defined()
      {
         sut.HasProcessesDefined.ShouldBeFalse();
      }
   }

   public class When_intializing_the_simulation_compound_process_summary_presenter : concern_for_SimulationProcessSummaryPresenter
   {
      protected override void Because()
      {
         sut.EditSimulation(_simulation, _compound);
      }

      [Observation]
      public void should_set_the_metabolism_view_into_its_view()
      {
         A.CallTo(() => _view.AddProcessView(_simulationEnzymaticProcessPresenter.View)).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_transport_and_excretion_view_into_its_view()
      {
         A.CallTo(() => _view.AddProcessView(_simulationTransportAndExcretionPresenter.View)).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_specific_binding_view_into_its_view()
      {
         A.CallTo(() => _view.AddProcessView(_simulationSpecificBindingPresenter.View)).MustHaveHappened();
      }
   }

   public class When_displaying_all_avalaible_process_processes_for_a_given_compound_and_individual : concern_for_SimulationProcessSummaryPresenter
   {
      protected override void Because()
      {
         sut.EditSimulation(_simulation, _compound);
      }

      [Observation]
      public void should_retrieve_all_the_metabolization_processes_available_for_the_simulation_and_displayed_them()
      {
         A.CallTo(() => _simulationEnzymaticProcessPresenter.EditProcessesIn(_simulation, _compoundProperties)).MustHaveHappened();
      }

      [Observation]
      public void should_retrieve_all_the_tansport_and_excretion_processes_available_for_the_simulation_and_displayed_them()
      {
         A.CallTo(() => _simulationTransportAndExcretionPresenter.EditProcessesIn(_simulation, _compoundProperties)).MustHaveHappened();
      }

      [Observation]
      public void should_retrieve_all_the_specific_binding_processes_available_for_the_simulation_and_displayed_them()
      {
         A.CallTo(() => _simulationSpecificBindingPresenter.EditProcessesIn(_simulation, _compoundProperties)).MustHaveHappened();
      }
   }

   public class When_saving_the_process_configuration : concern_for_SimulationProcessSummaryPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.EditSimulation(_simulation, _compound);
      }

      protected override void Because()
      {
         sut.SaveConfiguration();
      }

      [Observation]
      public void should_save_the_mapping_made_for_the_metabolism_processes()
      {
         A.CallTo(() => _simulationEnzymaticProcessPresenter.SaveConfiguration()).MustHaveHappened();
      }

      [Observation]
      public void should_save_the_mapping_made_for_the_transport_and_excretion_processes()
      {
         A.CallTo(() => _simulationTransportAndExcretionPresenter.SaveConfiguration()).MustHaveHappened();
      }

      [Observation]
      public void should_save_the_mapping_made_for_the_specific_binding_processes()
      {
         A.CallTo(() => _simulationSpecificBindingPresenter.SaveConfiguration()).MustHaveHappened();
      }
   }
}