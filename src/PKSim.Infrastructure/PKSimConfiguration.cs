using System;
using System.IO;
using Microsoft.Win32;
using OSPSuite.Assets;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;
using PKSim.Core;

namespace PKSim.Infrastructure
{
   public class PKSimConfiguration : OSPSuiteConfiguration, IPKSimConfiguration
   {
      public string PKSimDbPath { get; set; }
      public string TemplateSystemDatabasePath { get; }
      public string TemplateUserDatabaseTemplatePath { get; }
      public string RemoteTemplateSummaryPath { get; }
      public string RemoteTemplateFolderPath { get; }
      public string DefaultTemplateUserDatabasePath { get; }
      public override string ProductName { get; } = CoreConstants.PRODUCT_NAME;
      public override int InternalVersion { get; } = ProjectVersions.Current;
      public override Origin Product { get; } = Origins.PKSim;
      public override string ProductNameWithTrademark { get; } = CoreConstants.PRODUCT_NAME_WITH_TRADEMARK;
      public override string IconName { get; } = ApplicationIcons.PKSim.IconName;
      public override string UserSettingsFileName { get; } = "UserSettings.xml";
      public override string ApplicationSettingsFileName { get; } = "ApplicationSettings.xml";
      public override string IssueTrackerUrl { get; } = CoreConstants.ISSUE_TRACKER_URL;
      protected override string[] LatestVersionWithOtherMajor { get; } = {"10.0", "9.1", "9.0", "8.0"};
      public override string WatermarkOptionLocation { get; } = "Options -> Settings -> Application";
      public override string ApplicationFolderPathName { get; } = CoreConstants.APPLICATION_FOLDER_PATH;

      public PKSimConfiguration()
      {
         createDefaultSettingsFolder();
         PKSimDbPath = LocalOrAllUsersPathForFile(CoreConstants.PK_SIM_DB_FILE);
         TemplateSystemDatabasePath = LocalOrAllUsersPathForFile(CoreConstants.TEMPLATE_SYSTEM_DATABASE);
         TemplateUserDatabaseTemplatePath = LocalOrAllUsersPathForFile(CoreConstants.TEMPLATE_USER_DATABASE_TEMPLATE);
         RemoteTemplateSummaryPath = LocalOrAllUsersPathForFile(CoreConstants.REMOTE_TEMPLATE_SUMMARY);
         DefaultTemplateUserDatabasePath = CurrentUserFile(CoreConstants.TEMPLATE_USER_DATABASE);
         RemoteTemplateFolderPath = Path.Combine(CurrentUserFolderPath, CoreConstants.REMOTE_FOLDER_PATH); 
         if (!DirectoryHelper.DirectoryExists(RemoteTemplateFolderPath))
            DirectoryHelper.CreateDirectory(RemoteTemplateFolderPath);
      }

      private void createDefaultSettingsFolder()
      {
         if (!DirectoryHelper.DirectoryExists(CurrentUserFolderPath))
            DirectoryHelper.CreateDirectory(CurrentUserFolderPath);

         if (!DirectoryHelper.DirectoryExists(AllUsersFolderPath))
            DirectoryHelper.CreateDirectory(AllUsersFolderPath);

      
      }

      public string MoBiPath
      {
         get
         {
            try
            {
               return (string) Registry.GetValue($@"HKEY_LOCAL_MACHINE\SOFTWARE\{Constants.RegistryPaths.MOBI_REG_PATH}{Major}", Constants.RegistryPaths.INSTALL_PATH, null);
            }
            catch (Exception)
            {
               return string.Empty;
            }
         }
      }
   }
}