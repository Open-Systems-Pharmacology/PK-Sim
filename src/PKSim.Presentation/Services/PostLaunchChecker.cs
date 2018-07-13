using System;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Events;
using PKSim.Assets;
using PKSim.Core.Services;
using PKSim.Presentation.Events;

namespace PKSim.Presentation.Services
{
   public class PostLaunchChecker : IPostLaunchChecker
   {
      private readonly IVersionChecker _versionChecker;
      private readonly IWatermarkStatusChecker _watermarkStatusChecker;
      private readonly ITemplateDatabaseCreator _templateDatabaseCreator;
      private readonly IEventPublisher _eventPublisher;
      private readonly IUserSettings _userSettings;

      public PostLaunchChecker(
         IVersionChecker versionChecker, 
         IWatermarkStatusChecker watermarkStatusChecker,
         ITemplateDatabaseCreator templateDatabaseCreator,
         IEventPublisher eventPublisher, 
         IUserSettings userSettings)
      {
         _versionChecker = versionChecker;
         _watermarkStatusChecker = watermarkStatusChecker;
         _templateDatabaseCreator = templateDatabaseCreator;
         _eventPublisher = eventPublisher;
         _userSettings = userSettings;
      }

      public async Task PerformPostLaunchCheckAsync()
      {
         await checkForNewVersionAsync();

         _watermarkStatusChecker.CheckWatermarkStatus();

         _templateDatabaseCreator.CreateDefaultTemplateDatabase();

      }

      private async Task checkForNewVersionAsync()
      {
         if (!_userSettings.ShowUpdateNotification)
            return;

         try
         {
            var hasNewVersion = await _versionChecker.NewVersionIsAvailableAsync();
            if (!hasNewVersion) return;
         }
         catch (Exception)
         {
            //no need to do anything if version cannot be returned
            return;
         }

         if (string.Equals(_versionChecker.LatestVersion, _userSettings.LastIgnoredVersion))
            return;

         var message = PKSimConstants.Information.NewVersionIsAvailable(_versionChecker.LatestVersion, Constants.PRODUCT_SITE);
         _eventPublisher.PublishEvent(new ShowNotificationEvent(PKSimConstants.UI.UpdateAvailable, message, Constants.PRODUCT_SITE_DOWNLOAD));
      }
   }
}