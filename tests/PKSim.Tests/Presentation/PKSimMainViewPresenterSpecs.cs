using System.Collections.Generic;
using System.ComponentModel;
using OSPSuite.Utility.Collections;
using PKSim.Core.Services;
using PKSim.Extensions;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Events;
using PKSim.Core;
using PKSim.Presentation.Presenters.ContextMenus;
using PKSim.Presentation.Presenters.Main;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using FakeItEasy;
using PKSim.Assets;
using PKSim.Presentation.Views.Main;

using OSPSuite.Core.Domain;
using OSPSuite.Presentation;
using OSPSuite.Presentation.Events;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Main;
using OSPSuite.Presentation.Views;
using OSPSuite.Assets;
using OSPSuite.Core.Services;
using IContainer = System.ComponentModel.IContainer;

namespace PKSim.Presentation
{
   public abstract class concern_for_PKSimMainViewPresenter : ContextSpecification<IPKSimMainViewPresenter>
   {
      protected IPKSimMainView _view;
      protected IContainer _container;
      protected IExitCommand _exitCommand;
      protected IEventPublisher _eventPublisher;
      protected IRepository<IMainViewItemPresenter> _presenterRepository;
      protected IUserSettings _userSettings;
      protected IProjectTask _projectTask;
      protected IVersionChecker _versionChecker;
      private ITabbedMdiChildViewContextMenuFactory _contextMenuFactory;
      protected IPKSimConfiguration _configuration;
      protected IWatermarkStatusChecker _watermarkStatusChecker;

      protected override void Context()
      {
         _view = A.Fake<IPKSimMainView>();
         _presenterRepository = A.Fake<IRepository<IMainViewItemPresenter>>();
         _exitCommand = A.Fake<IExitCommand>();
         _eventPublisher = A.Fake< IEventPublisher>();
         _userSettings = A.Fake<IUserSettings>();
         _projectTask = A.Fake<IProjectTask>(); 
         _versionChecker =A.Fake<IVersionChecker>();
         _contextMenuFactory = A.Fake<ITabbedMdiChildViewContextMenuFactory>();
         _configuration = A.Fake<IPKSimConfiguration>();
         _watermarkStatusChecker= A.Fake<IWatermarkStatusChecker>(); 
         A.CallTo(() => _configuration.ProductDisplayName).Returns("AA");
         sut = new PKSimMainViewPresenter(_view, _eventPublisher,_contextMenuFactory, _presenterRepository, _exitCommand, _userSettings, _projectTask, _versionChecker, _configuration, _watermarkStatusChecker);
      }
   }

   
   public class When_the_main_view_presenter_is_told_to_initialize : concern_for_PKSimMainViewPresenter
   {
      private IMainViewItemPresenter _presenter1;
      private IMainViewItemPresenter _presenter2;

      protected override void Context()
      {
         base.Context();
         _presenter1 = A.Fake<IMainViewItemPresenter>();
         _presenter2 = A.Fake<IMainViewItemPresenter>();
         A.CallTo(() => _presenterRepository.All()).Returns(new List<IMainViewItemPresenter> {_presenter1, _presenter2});
      }

      protected override void Because()
      {
         sut.Initialize();
      }

      [Observation]
      public void should_initialize_the_view()
      {
         A.CallTo(() => _view.Initialize()).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_caption_to_the_name_of_the_product()
      {
          _view.Caption.ShouldBeEqualTo(_configuration.ProductDisplayName);
      }

      [Observation]
      public void should_initialize_all_available_presenters_belonging_to_the_mainview()
      {
         A.CallTo(() => _presenter1.Initialize()).MustHaveHappened();
         A.CallTo(() => _presenter2.Initialize()).MustHaveHappened();
      }

      [Observation]
      public void should_notify_an_application_initialized_event()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<ApplicationInitializedEvent>._)).MustHaveHappened();
      }
   }

   
   public class When_the_main_view_presenter_is_notified_that_a_heavy_work_is_starting_and_the_hour_glass_should_be_forced_to_display: concern_for_PKSimMainViewPresenter
   {
      protected override void Because()
      {
         sut.Handle(new HeavyWorkStartedEvent(true));
      }

      [Observation]
      public void should_tell_the_view_to_show_the_wait_cursor()
      {
         A.CallTo(() => _view.InWaitCursor(true,true)).MustHaveHappened();
      }
   }
   
   public class When_the_main_view_presenter_is_notified_that_a_heavy_work_is_starting_and_the_hour_glass_should_not_be_forced_to_display : concern_for_PKSimMainViewPresenter
   {
      protected override void Because()
      {
         sut.Handle(new HeavyWorkStartedEvent(false));
      }

      [Observation]
      public void should_tell_the_view_to_show_the_wait_cursor()
      {
         A.CallTo(() => _view.InWaitCursor(true, false)).MustHaveHappened();
      }
   }
   
   public class When_the_main_view_presenter_is_notified_that_a_heavy_work_has_finished_and_the_hour_glass_was_forced_to_be_displayed : concern_for_PKSimMainViewPresenter
   {
      protected override void Because()
      {
         sut.Handle(new HeavyWorkFinishedEvent(true));
      }

      [Observation]
      public void should_tell_the_view_to_show_the_default_cursor()
      {
         A.CallTo(() => _view.InWaitCursor(false, true)).MustHaveHappened();
      }
   }
   
   public class When_the_main_view_presenter_is_notified_that_a_heavy_work_has_finished_and_the_hour_glass_was_not_forced_to_be_displayed : concern_for_PKSimMainViewPresenter
   {
      protected override void Because()
      {
         sut.Handle(new HeavyWorkFinishedEvent(false));
      }

      [Observation]
      public void should_tell_the_view_to_show_the_default_cursor()
      {
         A.CallTo(() => _view.InWaitCursor(false, false)).MustHaveHappened();
      }
   }
   
   public class When_the_main_view_is_closing : concern_for_PKSimMainViewPresenter
   {
      private CancelEventArgs _cancelEventArgs;

      protected override void Context()
      {
         base.Context();
         _cancelEventArgs = new CancelEventArgs();
         A.CallTo(() => _exitCommand.Canceled).Returns(true);
         A.CallTo(() => _presenterRepository.All()).Returns(new List<IMainViewItemPresenter>());
         sut.Initialize();
      }

      protected override void Because()
      {
         _view.Closing+= Raise.FreeForm.With(_view, _cancelEventArgs);
      }

      [Observation]
      public void should_execute_the_exit_command()
      {
         A.CallTo(() => _exitCommand.Execute()).MustHaveHappened();
      }

      [Observation]
      public void should_return_the_user_cancel_action()
      {
         _cancelEventArgs.Cancel.ShouldBeEqualTo(_exitCommand.Canceled);
      }
   }

   
   public class When_the_main_view_presenter_is_being_notified_that_one_child_view_is_being_activated : concern_for_PKSimMainViewPresenter
   {
      private IMdiChildView _viewToActivate;
      private ScreenActivatedEvent _event;

      protected override void Context()
      {
         base.Context();
         _viewToActivate = A.Fake<IMdiChildView>();
         A.CallTo(() => _viewToActivate.Presenter).Returns(A.Fake<ISingleStartPresenter>());
         A.CallTo(() => _presenterRepository.All()).Returns(new List<IMainViewItemPresenter>());
         A.CallTo(() => _eventPublisher.PublishEvent(A<ScreenActivatedEvent>.Ignored)).Invokes(
            x => _event = x.GetArgument<ScreenActivatedEvent>(0));
      }

      protected override void Because()
      {
         sut.Activate(_viewToActivate);
      }

      [Observation]
      public void shold_notify_the_activation_of_the_given_screen()
      {
         _event.Presenter.ShouldBeEqualTo(_viewToActivate.Presenter);
      }
   }

   
   public class When_the_main_view_presenter_is_being_notified_that_no_screen_is_active : concern_for_PKSimMainViewPresenter
   {
      protected override void Because()
      {
         sut.Activate(null);
      }

      [Observation]
      public void should_not_crash()
      {
      }
   }

   
   public class When_retrieving_the_active_screen_when_a_screen_has_been_selected_by_the_user : concern_for_PKSimMainViewPresenter
   {
      private ISingleStartPresenter _result;
      private IMdiChildView _childView;

      protected override void Context()
      {
         base.Context();
         _childView = A.Fake<IMdiChildView>();
         A.CallTo(() => _childView.Presenter).Returns(A.Fake<ISingleStartPresenter>());
         A.CallTo(() => _view.ActiveView).Returns(_childView);
      }

      protected override void Because()
      {
         _result = sut.ActivePresenter;
      }

      [Observation]
      public void should_retrieve_the_activated_view_from_the_main_view_and_return_its_presenter()
      {
         _result.ShouldBeEqualTo(_childView.Presenter);
      }
   }

   
   public class When_retrieving_the_active_screen_when_a_no_screen_was_selected_by_the_user : concern_for_PKSimMainViewPresenter
   {
      private ISingleStartPresenter _result;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.ActiveView).Returns(null);
      }

      protected override void Because()
      {
         _result = sut.ActivePresenter;
      }

      [Observation]
      public void should_return_null()
      {
         _result.ShouldBeNull();
      }
   }

   
   public class When_the_main_view_presenter_is_asked_to_save_changes_when_no_active_presenter_is_available : concern_for_PKSimMainViewPresenter
   {
      private ISingleStartPresenter _activePresenter;

      protected override void Context()
      {
         base.Context();
         _activePresenter =A.Fake<ISingleStartPresenter>();
         var childView = A.Fake<IMdiChildView>();
         A.CallTo(() => childView.Presenter).Returns(_activePresenter);
         A.CallTo(() => _view.ActiveView).Returns(childView);
      }

      protected override void Because()
      {
         sut.SaveChanges();
      }

      [Observation]
      public void should_save_the_change_in_the_active_presenter()
      {
         A.CallTo(() => _activePresenter.SaveChanges()).MustHaveHappened();
      }
   }

   
   public class When_the_main_view_presenter_is_asked_to_save_changes_when_an_active_presenter_is_available : concern_for_PKSimMainViewPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.ActiveView).Returns(null);
      }

      protected override void Because()
      {
         sut.SaveChanges();
      }

      [Observation]
      public void should_not_crash()
      {

      }
   }

   
   public class When_the_main_view_presenter_is_running_some_defined_startup_options : concern_for_PKSimMainViewPresenter
   {
      private StartOptions _startOptions;

      protected override void Context()
      {
         base.Context();
         _startOptions =A.Fake<StartOptions>();
         sut.StartOptions = _startOptions;
      }

      protected override void Because()
      {
         sut.Run();
      }

      [Observation]
      public void should_run_them_through_the_project_task()
      {
         A.CallTo(() => _projectTask.Run(_startOptions)).MustHaveHappened();  
      }

      [Observation]
      public void should_validate_the_watermark_usage()
      {
         A.CallTo(() => _watermarkStatusChecker.CheckWatermarkStatus()).MustHaveHappened();
      }
   }
   
   public class When_starting_the_application_and_the_user_wants_to_be_notified_of_any_version_update : concern_for_PKSimMainViewPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _userSettings.ShowUpdateNotification).Returns(true);
      }
      protected override void Because()
      {
         sut.Run();
      }

      [Observation]
      public void should_start_the_version_check()
      {
         A.CallTo(() => _versionChecker.NewVersionIsAvailableAsync()).MustHaveHappened();
      }
   }

   
   public class When_starting_the_application_and_the_user_does_not_want_to_be_notified_of_any_version_update : concern_for_PKSimMainViewPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _userSettings.ShowUpdateNotification).Returns(false);
      }
      protected override void Because()
      {
         sut.Run();
      }

      [Observation]
      public void should_start_the_version_check()
      {
         A.CallTo(() => _versionChecker.NewVersionIsAvailable()).MustNotHaveHappened();
      }
   }

   
   public class When_being_notified_that_a_new_version_is_available_and_the_user_wants_to_be_be_notified_for_that_update: concern_for_PKSimMainViewPresenter
   {
      private string _newVersion;

      protected override void Context()
      {
         base.Context();
         _newVersion =  "1.2.3";
         _versionChecker.CurrentVersion = "1.1.1";
         _userSettings.LastIgnoredVersion = string.Empty;
         A.CallTo(() => _userSettings.ShowUpdateNotification).Returns(true);
         A.CallTo(() => _versionChecker.NewVersionIsAvailableAsync()).Returns(true);
         A.CallTo(() => _versionChecker.LatestVersion).Returns(new VersionInfo{Version = _newVersion});
      }
      protected override void Because()
      {
         sut.Run();
      }

      [Observation]
      public void should_show_the_notification_to_the_user_containing_the_new_version()
      {
         A.CallTo(() => _view.DisplayNotification(PKSimConstants.UI.UpdateAvailable,
            PKSimConstants.Information.NewVersionIsAvailable(_newVersion, Constants.PRODUCT_SITE),
            Constants.PRODUCT_SITE_DOWNLOAD)).MustHaveHappened();
      }
   }

   
   public class When_being_notified_that_a_new_version_is_available_and_the_user_does_not_want_to_be_be_notified_for_that_update : concern_for_PKSimMainViewPresenter
   {
      private string _newVersion;

      protected override void Context()
      {
         base.Context();
         _newVersion = "1.2.3";
         _versionChecker.CurrentVersion = "1.1.1";
         A.CallTo(() => _userSettings.ShowUpdateNotification).Returns(true);
         A.CallTo(() => _userSettings.LastIgnoredVersion).Returns(_newVersion);
         A.CallTo(() => _versionChecker.NewVersionIsAvailableAsync()).Returns(true);
         A.CallTo(() => _versionChecker.LatestVersion).Returns(new VersionInfo { Version = _newVersion });
      }
      protected override void Because()
      {
         sut.Run();
      }

      [Observation]
      public void should_not_show_the_notification_to_the_user_containing_the_new_version()
      {
         A.CallTo(() => _view.DisplayNotification(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
      }
   }

   
   public class When_being_notified_that_a_new_version_is_available_and_the_user_does_not_want_to_be_be_notified_for_an_older_update : concern_for_PKSimMainViewPresenter
   {
      private string _newVersion;

      protected override void Context()
      {
         base.Context();
         _newVersion = "1.2.3";
         _versionChecker.CurrentVersion = "1.1.1";
         A.CallTo(() => _userSettings.ShowUpdateNotification).Returns(true);
         A.CallTo(() => _userSettings.LastIgnoredVersion).Returns("1.2.1");
         A.CallTo(() => _versionChecker.NewVersionIsAvailableAsync()).Returns(true);
         A.CallTo(() => _versionChecker.LatestVersion).Returns(new VersionInfo { Version = _newVersion });
      }
      protected override void Because()
      {
         sut.Run();
      }

      [Observation]
      public void should_show_the_notification_to_the_user_containing_the_new_version()
      {
         A.CallTo(() => _view.DisplayNotification(PKSimConstants.UI.UpdateAvailable,
            PKSimConstants.Information.NewVersionIsAvailable(_newVersion, Constants.PRODUCT_SITE),
            Constants.PRODUCT_SITE_DOWNLOAD)).MustHaveHappened();
      }
   }
}