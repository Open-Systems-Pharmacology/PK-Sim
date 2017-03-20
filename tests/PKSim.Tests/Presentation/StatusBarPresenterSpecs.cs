using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters.Main;
using OSPSuite.Core;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation
{
   public class When_the_status_bar_presenter_is_told_to_initialize : ContextSpecification<IStatusBarPresenter>
   {
      private IStatusBarView _statusBarView;
      private IApplicationConfiguration _applicationConfiguration;

      protected override void Context()
      {
         _statusBarView = A.Fake<IStatusBarView>();
         _applicationConfiguration = A.Fake<IApplicationConfiguration>();
         sut = new StatusBarPresenter(_statusBarView, _applicationConfiguration);
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