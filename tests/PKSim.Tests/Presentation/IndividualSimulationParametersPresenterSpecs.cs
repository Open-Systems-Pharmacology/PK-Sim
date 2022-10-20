using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Parameters;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualSimulationParametersPresenter : ContextSpecification<IIndividualSimulationParametersPresenter>
   {
      protected IParameterGroupsPresenter _parameterGroupPresenter;
      protected ISimulationParametersView _view;
      protected IndividualSimulation _simulation;

      protected override void Context()
      {
         _view = A.Fake<ISimulationParametersView>();
         _parameterGroupPresenter = A.Fake<IParameterGroupsPresenter>();
         _simulation = new IndividualSimulation {Model = A.Fake<IModel>()};
         A.CallTo(() => _parameterGroupPresenter.View).Returns(A.Fake<IParameterGroupsView>());
         sut = new IndividualSimulationParametersPresenter(_view, _parameterGroupPresenter);

         sut.EditSimulation(_simulation);
      }
   }

   public class When_the_simulation_parameter_presenter_is_initializing : concern_for_IndividualSimulationParametersPresenter
   {
      private ICommandCollectorPresenter _commandRegister;

      protected override void Context()
      {
         base.Context();
         _commandRegister = A.Fake<ICommandCollectorPresenter>();
      }

      protected override void Because()
      {
         sut.InitializeWith(_commandRegister);
      }

      [Observation]
      public void should_set_the_command_register_to_the_parameter_presenter()
      {
         A.CallTo(() => _parameterGroupPresenter.InitializeWith(sut)).MustHaveHappened();
      }
   }

   public class When_the_simulation_parameter_presenter_is_being_created : concern_for_IndividualSimulationParametersPresenter
   {
      [Observation]
      public void should_set_the_parameter_group_view_as_main_view()
      {
         A.CallTo(() => _view.AddParametersView(_parameterGroupPresenter.View)).MustHaveHappened();
      }
   }

   public class When_notified_that_the_edited_simulation_was_updated : concern_for_IndividualSimulationParametersPresenter
   {
      protected override void Because()
      {
         sut.Handle(new SimulationUpdatedEvent(_simulation));
      }

      [Observation]
      public void should_refresh_the_active_parameter_presenter()
      {
         A.CallTo(() => _parameterGroupPresenter.RefreshActivePresenter()).MustHaveHappened();
      }
   }

   public class When_notified_that_a_simulation_was_updated_that_is_not_the_edited_simulation : concern_for_IndividualSimulationParametersPresenter
   {
      protected override void Because()
      {
         sut.Handle(new SimulationUpdatedEvent(new IndividualSimulation()));
      }

      [Observation]
      public void should_not_refresh_the_active_parameter_presenter()
      {
         A.CallTo(() => _parameterGroupPresenter.RefreshActivePresenter()).MustNotHaveHappened();
      }
   }
}