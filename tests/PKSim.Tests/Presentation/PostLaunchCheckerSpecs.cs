using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Events;
using PKSim.Assets;
using PKSim.Core.Services;
using PKSim.Presentation.Events;
using PKSim.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_PostLaunchChecker : ContextSpecificationAsync<IPostLaunchChecker>
   {
      protected IVersionChecker _versionChecker;
      protected IWatermarkStatusChecker _watermarkStatusChecker;
      protected ITemplateDatabaseCreator TemplateDatabaseCreator;
      protected IUserSettings _userSettings;
      protected IEventPublisher _eventPublisher;

      protected override Task Context()
      {
         _versionChecker = A.Fake<IVersionChecker>();
         _watermarkStatusChecker = A.Fake<IWatermarkStatusChecker>();
         TemplateDatabaseCreator = A.Fake<ITemplateDatabaseCreator>();
         _userSettings = A.Fake<IUserSettings>();
         _eventPublisher = A.Fake<IEventPublisher>();

         sut = new PostLaunchChecker(_versionChecker, _watermarkStatusChecker, TemplateDatabaseCreator, _eventPublisher, _userSettings);

         return _completed;
      }
   }

   public class When_starting_the_post_launch_check : concern_for_PostLaunchChecker
   {
      protected override async Task Because()
      {
         await sut.PerformPostLaunchCheckAsync();
      }

      [Observation]
      public void should_create_the_default_user_template_database()
      {
         A.CallTo(() => TemplateDatabaseCreator.CreateDefaultTemplateDatabase()).MustHaveHappened();
      }

      [Observation]
      public void should_verify_the_watermark_status()
      {
         A.CallTo(() => _watermarkStatusChecker.CheckWatermarkStatus()).MustHaveHappened();
      }
   }

   public class When_starting_the_post_launch_check_and_the_user_does_not_want_to_be_notifed_of_new_version_of_the_app : concern_for_PostLaunchChecker
   {
      protected override async Task Context()
      {
         await base.Context();
         _userSettings.ShowUpdateNotification = false;
      }

      protected override async Task Because()
      {
         await sut.PerformPostLaunchCheckAsync();
      }

      [Observation]
      public void should_not_check_for_a_new_version()
      {
         A.CallTo(() => _versionChecker.NewVersionIsAvailableAsync()).MustNotHaveHappened();
      }
   }

   public class When_starting_the_post_launch_check_and_the_user_wants_to_be_notifed_of_new_version_of_the_app_and_a_new_version_is_available : concern_for_PostLaunchChecker
   {
      private readonly VersionInfo _newVersion = new VersionInfo {Version = "1.2.3"};
      private ShowNotificationEvent _event;

      protected override async Task Context()
      {
         await base.Context();
         _userSettings.ShowUpdateNotification = true;
         A.CallTo(() => _versionChecker.NewVersionIsAvailableAsync()).Returns(true);
         A.CallTo(() => _versionChecker.LatestVersion).Returns(_newVersion);

         A.CallTo(() => _eventPublisher.PublishEvent(A<ShowNotificationEvent>._))
            .Invokes(x => _event = x.GetArgument<ShowNotificationEvent>(0));

      }

      protected override async Task Because()
      {
         await sut.PerformPostLaunchCheckAsync();
      }

      [Observation]
      public void should_publish_a_new_version_notification()
      {
         _event.ShouldNotBeNull();
         _event.Caption.ShouldBeEqualTo(PKSimConstants.UI.UpdateAvailable);
         _event.Url.ShouldBeEqualTo(Constants.PRODUCT_SITE_DOWNLOAD);
         _event.Notification.ShouldBeEqualTo(PKSimConstants.Information.NewVersionIsAvailable(_versionChecker.LatestVersion, Constants.PRODUCT_SITE));
      }
   }

   public class When_starting_the_post_launch_check_and_the_user_wants_to_be_notifed_of_new_version_of_the_app_and_a_new_version_is_available_but_the_check_was_already_dismissed_by_user : concern_for_PostLaunchChecker
   {
      private readonly VersionInfo _newVersion = new VersionInfo { Version = "1.2.3" };

      protected override async Task Context()
      {
         await base.Context();
         _userSettings.ShowUpdateNotification = true;
         A.CallTo(() => _versionChecker.NewVersionIsAvailableAsync()).Returns(true);
         A.CallTo(() => _versionChecker.LatestVersion).Returns(_newVersion);
         A.CallTo(() => _userSettings.LastIgnoredVersion).Returns(_newVersion.Version);
      }

      protected override async Task Because()
      {
         await sut.PerformPostLaunchCheckAsync();
      }

      [Observation]
      public void should_not_publish_a_new_version_notification()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<ShowNotificationEvent>._)).MustNotHaveHappened();
      }
   }

}