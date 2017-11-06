using System;
using Microsoft.Win32;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Configuration;
using OSPSuite.Utility;
using PKSim.Core;

namespace PKSim.Infrastructure
{
   public class PKSimConfiguration : OSPSuiteConfiguration, IPKSimConfiguration
   {
      public string PKSimDbPath { get; }
      public string TemplateSystemDatabasePath { get; }
      public string TemplateUserDatabaseTemplatePath { get; }
      public string DefaultTemplateUserDatabasePath { get; }
      public override string ProductName { get; } = CoreConstants.PRODUCT_NAME;
      public override Origin Product { get; } = Origins.PKSim;
      public override string ProductNameWithTrademark { get; } = CoreConstants.PRODUCT_NAME_WITH_TRADEMARK;
      public override ApplicationIcon Icon { get; } = ApplicationIcons.PKSim;
      public override string UserSettingsFileName { get; } = "UserSettings.xml";
      public override string ApplicationSettingsFileName { get; } = "ApplicationSettings.xml";
      public override string IssueTrackerUrl { get; } = CoreConstants.ISSUE_TRACKER_URL;
      protected override string[] LatestVersionWithOtherMajor { get; } = {"6.3", "5.6"};
      public override string WatermarkOptionLocation { get; } = "Options -> Settings -> Application";
      public override string ApplicationFolderPathName { get; } = CoreConstants.APPLICATION_FOLDER_PATH;

      public PKSimConfiguration()
      {
         createDefaultSettingsFolder();
         PKSimDbPath = AllUsersOrLocalPathForFile(CoreConstants.PK_SIM_DB_FILE);
         TemplateSystemDatabasePath = AllUsersOrLocalPathForFile(CoreConstants.TEMPLATE_SYSTEM_DATABASE);
         TemplateUserDatabaseTemplatePath = AllUsersOrLocalPathForFile(CoreConstants.TEMPLATE_USER_DATABASE_TEMPLATE);
         DefaultTemplateUserDatabasePath = CurrentUserFile(CoreConstants.TEMPLATE_USER_DATABASE);
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
               return (string) Registry.GetValue($@"HKEY_LOCAL_MACHINE\SOFTWARE\{Constants.RegistryPaths.MOBI_REG_PATH}{MajorVersion}", Constants.RegistryPaths.INSTALL_PATH, null);
            }
            catch (Exception)
            {
               return string.Empty;
            }
         }
      }
   }
}