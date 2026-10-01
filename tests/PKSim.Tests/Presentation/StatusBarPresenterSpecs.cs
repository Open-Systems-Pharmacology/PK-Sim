using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters.Main;
using OSPSuite.Core;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Views;
using OSPSuite.Utility.Events;
using PKSim.Core.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_StatusBarPresenter : ContextSpecification<IStatusBarPresenter>
   {
      protected IStatusBarView _statusBarView;
      protected IApplicationConfiguration _applicationConfiguration;
      protected IEventPublisher _eventPublisher;
      protected IInteractiveSimulationRunner _interactiveSimulationRunner;

      protected override void Context()
      {
         _eventPublisher = A.Fake<IEventPublisher>();
         _statusBarView = A.Fake<IStatusBarView>();
         _applicationConfiguration = A.Fake<IApplicationConfiguration>();
         _interactiveSimulationRunner = A.Fake<IInteractiveSimulationRunner>();
         sut = new StatusBarPresenter(_statusBarView, _applicationConfiguration, _eventPublisher, _interactiveSimulationRunner);
      }
   }

   public class When_the_status_bar_presenter_is_told_to_initialize : concern_for_StatusBarPresenter
   {
      protected override void Because()
      {
         sut.Initialize();
      }

      [Observation]
      public void should_add_the_predefined_components_to_the_status_bar()
      {
         foreach (var element in StatusBarElements.All())
         {
            StatusBarElement elementToAdd = element;
            A.CallTo(() => _statusBarView.AddItem(elementToAdd)).MustHaveHappened();
         }
      }
   }

   public class When_the_status_bar_presenter_is_notified_that_a_progress_started_while_no_simulation_is_running : concern_for_StatusBarPresenter
   {
      private IStatusBarElementExpression _progressBar;

      protected override void Context()
      {
         base.Context();
         _progressBar = A.Fake<IStatusBarElementExpression>();
         var connection = new StatusBarPanelExpressionConnection(_progressBar);
         A.CallTo(() => _progressBar.WithValue(A<object>._)).Returns(connection);
         A.CallTo(() => _progressBar.Visible(A<bool>._)).Returns(connection);
         A.CallTo(() => _statusBarView.BarElementExpressionFor(StatusBarElements.ProgressBar)).Returns(_progressBar);
         A.CallTo(() => _interactiveSimulationRunner.ActiveSimulationsCount).Returns(0);
      }

      protected override void Because()
      {
         sut.Handle(new ProgressInitEvent(100, "Creating population"));
      }

      [Observation]
      public void should_show_the_progress_bar()
      {
         A.CallTo(() => _progressBar.Visible(true)).MustHaveHappened();
         A.CallTo(() => _progressBar.Visible(false)).MustNotHaveHappened();
      }
   }
}