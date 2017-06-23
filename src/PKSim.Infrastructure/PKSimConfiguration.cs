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
      public string TemplateSystemDatabasePath { get;  }

      private static readonly string[] LATEST_VERSION_WITH_OTHER_MAJOR = {"6.3", "5.6"};

      public PKSimConfiguration()
      {
         createDefaultSettingsFolder();
         PKSimDbPath = createApplicationDataOrLocalPathForFile(CoreConstants.PKSimDbFile);
         DimensionFilePath = createApplicationDataOrLocalPathForFile(CoreConstants.DimensionFile);
         PKParametersFilePath = createApplicationDataOrLocalPathForFile(CoreConstants.PKParametersFile);
         TemplateSystemDatabasePath = createApplicationDataOrLocalPathForFile(CoreConstants.TemplateSystemDatabase);
      }

      private void createDefaultSettingsFolder()
      {
         if (!Directory.Exists(UserApplicationSettingsFolderPath))
            Directory.CreateDirectory(UserApplicationSettingsFolderPath);

         if (!Directory.Exists(ApplicationSettingsFolderPath))
            Directory.CreateDirectory(ApplicationSettingsFolderPath);
      }

      public string LogConfigurationFile => createLocalPathForFile(CoreConstants.Log4NetConfigFile);

      public string TemplateUserDatabaseTemplatePath => createLocalPathForFile(CoreConstants.TemplateUserDatabaseTemplate);

      public string DefaultTemplateUserDatabasePath => Path.Combine(UserApplicationSettingsFolderPath, CoreConstants.TemplateUserDatabase);

      public string SimModelSchemaPath => createLocalPathForFile(CoreConstants.SimModelSchemaFile);

      private string createApplicationDataOrLocalPathForFile(string fileName)
      {
         var applicationDataPathForFile = Path.Combine(ApplicationSettingsFolderPath, fileName);
         if (FileHelper.FileExists(applicationDataPathForFile))
            return applicationDataPathForFile;

         //try local if id does not exist
         var localPathForFile = createLocalPathForFile(fileName);
         if (FileHelper.FileExists(localPathForFile))
            return localPathForFile;

         //neither app data nor local exist, return app data
         return applicationDataPathForFile;
      }

      private string createLocalPathForFile(string fileName) => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

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

      public override string ProductName => CoreConstants.ProductName;

      public override Origin Product => Origins.PKSim;

      public override string ProductNameWithTrademark => CoreConstants.ProductNameWithTrademark;

      public override ApplicationIcon Icon => ApplicationIcons.PKSim;

      public override string UserSettingsFileName => "UserSettings.xml";

      protected override string[] LatestVersionWithOtherMajor => LATEST_VERSION_WITH_OTHER_MAJOR;

      public override string ChartLayoutTemplateFolderPath => Path.Combine(ApplicationSettingsFolderPath, CoreConstants.ChartLayoutFolderPathName);

      public override string TEXTemplateFolderPath => Path.Combine(ApplicationSettingsFolderPath, CoreConstants.TEXTemplateFolderPathName);

      public string ApplicationSettingsFolderPath => applicationSettingsFolderPath(applicationFolderPathWithMajorVersion);

      public string ApplicationSettingsFilePath => applicationSettingsFilePath(MajorVersion);

      private string applicationSettingsFolderPath(string applicationFolderPath)
      {
         return Path.Combine(EnvironmentHelper.ApplicationDataFolder(), applicationFolderPath);
      }

      private string applicationSettingsFilePath(string revision)
      {
         return Path.Combine(EnvironmentHelper.ApplicationDataFolder(), ApplicationFolderPathWithRevision(revision), "ApplicationSettings.xml");
      }

      public string UserApplicationSettingsFolderPath => userApplicationSettingsFolderPath(applicationFolderPathWithMajorVersion);

      private string userApplicationSettingsFolderPath(string applicationFolderPath)
      {
         return Path.Combine(EnvironmentHelper.UserApplicationDataFolder(), applicationFolderPath);
      }

      public IEnumerable<string> ApplicationSettingsFilePaths => SettingsFilePaths(ApplicationSettingsFilePath, applicationSettingsFilePath);

      private string applicationFolderPathWithMajorVersion => ApplicationFolderPathWithRevision(MajorVersion);

      protected override string ApplicationFolderPathWithRevision(string version)
      {
         return Path.Combine(CoreConstants.ApplicationFolderPath, version);
      }

      public override string IssueTrackerUrl { get; } = CoreConstants.IssueTrackerUrl;
   }
}