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
   public class When_the_status_bar_presenter_is_told_to_initialize : ContextSpecification<IStatusBarPresenter>
   {
      private IStatusBarView _statusBarView;
      private IApplicationConfiguration _applicationConfiguration;
      private IEventPublisher _eventPublisher;
      private IInteractiveSimulationRunner _interactiveSimulationRunner;

      protected override void Context()
      {
         _eventPublisher = A.Fake<IEventPublisher>();
         _statusBarView = A.Fake<IStatusBarView>();
         _applicationConfiguration = A.Fake<IApplicationConfiguration>();
         _interactiveSimulationRunner = A.Fake<IInteractiveSimulationRunner>();
         sut = new StatusBarPresenter(_statusBarView, _applicationConfiguration, _eventPublisher, _interactiveSimulationRunner);
      }

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
}