using System;
using System.Collections.Generic;
using System.IO;
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
      public string DimensionFilePath { get; }
      public string TemplateSystemDatabasePath { get; }
      public string LogConfigurationFile { get; }
      public string TemplateUserDatabaseTemplatePath { get; }
      public string DefaultTemplateUserDatabasePath { get; }
      public string SimModelSchemaPath { get; }
      public override string ProductName { get; } = CoreConstants.PRODUCT_NAME;
      public override Origin Product { get; } = Origins.PKSim;
      public override string ProductNameWithTrademark { get; } = CoreConstants.PRODUCT_NAME_WITH_TRADEMARK;
      public override ApplicationIcon Icon { get; } = ApplicationIcons.PKSim;
      public override string UserSettingsFileName { get; } = "UserSettings.xml";
      public override string IssueTrackerUrl { get; } = CoreConstants.ISSUE_TRACKER_URL;
      protected override string[] LatestVersionWithOtherMajor { get; } = {"6.3", "5.6"};
      public override string ChartLayoutTemplateFolderPath { get; }
      public override string TEXTemplateFolderPath { get; }
      public string ApplicationSettingsFilePath { get; }
      public string ApplicationSettingsFolderPath { get; }

      public PKSimConfiguration()
      {
         ApplicationSettingsFolderPath = applicationSettingsFolderPathFor(MajorVersion);

         createDefaultSettingsFolder();

         ApplicationSettingsFilePath = createAppDataPath("ApplicationSettings.xml");
         ChartLayoutTemplateFolderPath = createApplicationDataOrLocalPathForFolder(CoreConstants.APP_DATA_CHART_LAYOUT_FOLDER_NAME, CoreConstants.LOCAL_CHART_LAYOUT_FOLDER_NAME);
         TEXTemplateFolderPath = createApplicationDataOrLocalPathForFolder(CoreConstants.APP_DATA_TEX_TEMPLATE_FOLDER_NAME, CoreConstants.LOCAL_TEX_TEMPLATE_FOLDER_NAME);

         PKSimDbPath = createApplicationDataOrLocalPathForFile(CoreConstants.PK_SIM_DB_FILE);
         DimensionFilePath = createApplicationDataOrLocalPathForFile(CoreConstants.DIMENSION_FILE);
         PKParametersFilePath = createApplicationDataOrLocalPathForFile(CoreConstants.PK_PARAMETERS_FILE);
         TemplateSystemDatabasePath = createApplicationDataOrLocalPathForFile(CoreConstants.TEMPLATE_SYSTEM_DATABASE);
         LogConfigurationFile = createApplicationDataOrLocalPathForFile(CoreConstants.LOG_4_NET_CONFIG_FILE);
         TemplateUserDatabaseTemplatePath = createApplicationDataOrLocalPathForFile(CoreConstants.TEMPLATE_SYSTEM_DATABASE);

         SimModelSchemaPath = createLocalPath(CoreConstants.SIM_MODEL_SCHEMA_FILE);

         DefaultTemplateUserDatabasePath = createUserAppDataPath(CoreConstants.TEMPLATE_USER_DATABASE);
     }

      private void createDefaultSettingsFolder()
      {
         if (!DirectoryHelper.DirectoryExists(UserApplicationSettingsFolderPath))
            DirectoryHelper.CreateDirectory(UserApplicationSettingsFolderPath);

         if (!DirectoryHelper.DirectoryExists(ApplicationSettingsFolderPath))
            DirectoryHelper.CreateDirectory(ApplicationSettingsFolderPath);
      }

      private string createApplicationDataOrLocalPathForFile(string fileName) => createApplicationDataOrLocalPathFor(fileName, fileName, FileHelper.FileExists);

      private string createApplicationDataOrLocalPathForFolder(string folderNameAppData, string folderNameLocal) => createApplicationDataOrLocalPathFor(folderNameAppData, folderNameLocal, DirectoryHelper.DirectoryExists);

      private string createApplicationDataOrLocalPathFor(string appDataName, string localName,  Func<string, bool> existsFunc)
      {
         var applicationDataOrLocal = createAppDataPath(appDataName);
         if (existsFunc(applicationDataOrLocal))
            return applicationDataOrLocal;

         //try local if id does not exist
         var localPath = createLocalPath(localName);
         if (existsFunc(localPath))
            return localPath;

         //neither app data nor local exist, return app data
         return applicationDataOrLocal;
      }

      private string createLocalPath(string fileName) => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

      private string createUserAppDataPath(string fileName) => Path.Combine(UserApplicationSettingsFolderPath, fileName);

      private string createAppDataPath(string fileName) => Path.Combine(ApplicationSettingsFolderPath, fileName);

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

      private string applicationSettingsFolderPathFor(string version) => Path.Combine(EnvironmentHelper.ApplicationDataFolder(), ApplicationFolderPathWithRevision(version));

      public string UserApplicationSettingsFolderPath => Path.Combine(EnvironmentHelper.UserApplicationDataFolder(), ApplicationFolderPathWithRevision(MajorVersion));

      public IEnumerable<string> ApplicationSettingsFilePaths => SettingsFilePaths(ApplicationSettingsFilePath, applicationSettingsFolderPathFor);

      protected override string ApplicationFolderPathWithRevision(string version) => Path.Combine(CoreConstants.APPLICATION_FOLDER_PATH, version);
   }
}