using System;
using System.Collections.Generic;
using System.IO;
using OSPSuite.Assets;
using OSPSuite.Utility;
using Microsoft.Win32;
using PKSim.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Configuration;

namespace PKSim.Infrastructure
{
   public class PKSimConfiguration : OSPSuiteConfiguration, IPKSimConfiguration
   {
      public string PKSimDb { get; set; }
      public string DimensionFilePath { get; set; }
      public string TemplateSystemDatabasePath { get; set; }
      public bool IsToken { get; set; }

      private static readonly string[] LATEST_VERSION_WITH_OTHER_MAJOR = { "5.6" };

      public PKSimConfiguration()
      {
         createDefaultSettingsFolder();
         PKSimDb = createApplicationDataPathForFile(CoreConstants.PKSimDb);
         DimensionFilePath = createApplicationDataPathForFile(CoreConstants.DimensionFile);
         PKParametersFilePath = createApplicationDataPathForFile(CoreConstants.PKParametersFile);
         TemplateSystemDatabasePath = createApplicationDataPathForFile(CoreConstants.TemplateSystemDatabase);
      }

      private void createDefaultSettingsFolder()
      {
         if (!Directory.Exists(UserApplicationSettingsFolderPath))
            Directory.CreateDirectory(UserApplicationSettingsFolderPath);

         if (!Directory.Exists(ApplicationSettingsFolderPath))
            Directory.CreateDirectory(ApplicationSettingsFolderPath);
      }

      public string LogConfigurationFile => createAbsolutePathForFile(CoreConstants.Log4NetConfigFile);

      public string TemplateUserDatabaseTemplatePath => createAbsolutePathForFile(CoreConstants.TemplateUserDatabaseTemplate);

      public string DefaultTemplateUserDatabasePath => Path.Combine(UserApplicationSettingsFolderPath, CoreConstants.TemplateUserDatabase);

      public string SimModelSchemaPath => createAbsolutePathForFile(CoreConstants.SimModelSchema);

      private string createApplicationDataPathForFile(string fileName)
      {
         return Path.Combine(ApplicationSettingsFolderPath, fileName);
      }

      private string createAbsolutePathForFile(string fileName)
      {
         return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
      }

      public string MoBiPath
      {
         get
         {
            try
            {
               return (string) Registry.GetValue($@"HKEY_LOCAL_MACHINE\SOFTWARE\{CoreConstants.MoBiRegPath}{MajorVersion}", "InstallPath", null);
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