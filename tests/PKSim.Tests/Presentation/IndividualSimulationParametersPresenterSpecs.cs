using PKSim.Presentation.Presenters;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Parameters;

using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualSimulationParametersPresenter : ContextSpecification<IIndividualSimulationParametersPresenter>
   {
      protected IParameterGroupsPresenter _parameterGroupPresenter;
      protected ISimulationParametersView _view;

      protected override void Context()
      {
         _view =A.Fake<ISimulationParametersView>();
         _parameterGroupPresenter  =A.Fake<IParameterGroupsPresenter>();
         A.CallTo(() => _parameterGroupPresenter.View).Returns(A.Fake<IParameterGroupsView>());
         sut = new IndividualSimulationParametersPresenter(_view, _parameterGroupPresenter);
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
}	