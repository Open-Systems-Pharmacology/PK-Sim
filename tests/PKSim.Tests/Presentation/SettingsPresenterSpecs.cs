using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;


namespace PKSim.Presentation
{
   public abstract class concern_for_SettingsPresenter : ContextSpecification<ISettingsPresenter>
   {
      private ISubPresenterItemManager<ISettingsItemPresenter> _subPresenterManager;
      protected ISettingsView _view;
      protected ISettingsItemPresenter _userSettings;
      protected ISettingsItemPresenter _appSettings;
      protected IDialogCreator _dialogCreator;
      private IWorkspace _workspace;
      protected IUserDisplayUnitsPresenter _userDisplayUnitSettings;

      protected override void Context()
      {
         _subPresenterManager = SubPresenterHelper.Create<ISettingsItemPresenter>();
         _userSettings = _subPresenterManager.CreateFake(SettingsItems.UserGeneralSettings);
         _appSettings = _subPresenterManager.CreateFake(SettingsItems.ApplicationSettings);
         _userDisplayUnitSettings = _subPresenterManager.CreateFake(SettingsItems.UserDisplayUnitsSettings);

         _dialogCreator = A.Fake<IDialogCreator>();
         _workspace = A.Fake<IWorkspace>();
         _view = A.Fake<ISettingsView>();
         sut = new SettingsPresenter(_view, _subPresenterManager, _dialogCreator, _workspace);
      }
   }

   public class When_initializing_the_settings_presenter : concern_for_SettingsPresenter
   {
      protected override void Because()
      {
         sut.Initialize();
      }

      [Observation]
      public void should_initialize_the_view_with_the_current_settings()
      {
         A.CallTo(() => _userSettings.EditSettings()).MustHaveHappened();
         A.CallTo(() => _appSettings.EditSettings()).MustHaveHappened();
         A.CallTo(() => _userDisplayUnitSettings.EditSettings()).MustHaveHappened();
      }

      [Observation]
      public void should_show_the_view()
      {
         A.CallTo(() => _view.Display()).MustHaveHappened();
      }
   }

   public class When_the_user_canceled_the_editing_of_its_user_config_ : concern_for_SettingsPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.Canceled).Returns(true);
      }

      protected override void Because()
      {
         sut.Initialize();
      }

      [Observation]
      public void should_not_save_the_settings()
      {
         A.CallTo(() => _userSettings.SaveSettings()).MustNotHaveHappened();
         A.CallTo(() => _appSettings.SaveSettings()).MustNotHaveHappened();
         A.CallTo(() => _userDisplayUnitSettings.SaveSettings()).MustNotHaveHappened();
      }
   }

   public class When_the_user_confirm_the_editing_of_its_user_config_ : concern_for_SettingsPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.Canceled).Returns(false);
      }

      protected override void Because()
      {
         sut.Initialize();
      }

      [Observation]
      public void should_save_the_settings()
      {
         A.CallTo(() => _userSettings.SaveSettings()).MustHaveHappened();
         A.CallTo(() => _appSettings.SaveSettings()).MustHaveHappened();
         A.CallTo(() => _userDisplayUnitSettings.SaveSettings()).MustHaveHappened();
      }
   }
}