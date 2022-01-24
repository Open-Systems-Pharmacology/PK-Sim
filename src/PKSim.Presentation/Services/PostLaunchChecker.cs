using System;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Events;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Repositories;
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
      private readonly IRemoteTemplateRepository _remoteTemplateRepository;

      public PostLaunchChecker(
         IVersionChecker versionChecker, 
         IWatermarkStatusChecker watermarkStatusChecker,
         ITemplateDatabaseCreator templateDatabaseCreator,
         IEventPublisher eventPublisher, 
         IUserSettings userSettings,
         IRemoteTemplateRepository remoteTemplateRepository)
      {
         _versionChecker = versionChecker;
         _watermarkStatusChecker = watermarkStatusChecker;
         _templateDatabaseCreator = templateDatabaseCreator;
         _eventPublisher = eventPublisher;
         _userSettings = userSettings;
         _remoteTemplateRepository = remoteTemplateRepository;
      }

      public async Task PerformPostLaunchCheckAsync()
      {
         await checkForNewSoftwareVersionAsync();

         await checkForNewTemplateVersionAsync();

         _watermarkStatusChecker.CheckWatermarkStatus();

         _templateDatabaseCreator.CreateDefaultTemplateDatabase();

      }

      private async Task checkForNewTemplateVersionAsync()
      {
         try
         {
            await _versionChecker.DownloadLatestVersionInfoAsync();
            var hasNewVersion = _versionChecker.NewVersionIsAvailableFor(CoreConstants.TEMPLATES_PRODUCT_NAME, _remoteTemplateRepository.Version);
            if (!hasNewVersion) 
               return;

            await _remoteTemplateRepository.UpdateLocalTemplateSummaryFile();
         }
         catch (Exception)
         {
            //no need to do anything if version cannot be returned
            return;
         }


      }
      private async Task checkForNewSoftwareVersionAsync()
      {
         if (!_userSettings.ShowUpdateNotification)
            return;

         try
         {
            await _versionChecker.DownloadLatestVersionInfoAsync();
            var hasNewVersion =  _versionChecker.NewVersionIsAvailable;
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