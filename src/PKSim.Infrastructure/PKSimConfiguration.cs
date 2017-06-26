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
      public override string ProductName { get; } = CoreConstants.ProductName;
      public override Origin Product { get; } = Origins.PKSim;
      public override string ProductNameWithTrademark { get; } = CoreConstants.ProductNameWithTrademark;
      public override ApplicationIcon Icon { get; } = ApplicationIcons.PKSim;
      public override string UserSettingsFileName { get; } = "UserSettings.xml";
      public override string IssueTrackerUrl { get; } = CoreConstants.IssueTrackerUrl;
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
         ChartLayoutTemplateFolderPath = createAppDataPath(CoreConstants.ChartLayoutFolderPathName);
         TEXTemplateFolderPath = createAppDataPath(CoreConstants.TEXTemplateFolderPathName);

         PKSimDbPath = createApplicationDataOrLocalPathForFile(CoreConstants.PKSimDbFile);
         DimensionFilePath = createApplicationDataOrLocalPathForFile(CoreConstants.DimensionFile);
         PKParametersFilePath = createApplicationDataOrLocalPathForFile(CoreConstants.PKParametersFile);
         TemplateSystemDatabasePath = createApplicationDataOrLocalPathForFile(CoreConstants.TemplateSystemDatabase);
         LogConfigurationFile = createApplicationDataOrLocalPathForFile(CoreConstants.TemplateSystemDatabase);
         TemplateUserDatabaseTemplatePath = createApplicationDataOrLocalPathForFile(CoreConstants.TemplateSystemDatabase);

         SimModelSchemaPath = createLocalPathForFile(CoreConstants.TemplateSystemDatabase);

         DefaultTemplateUserDatabasePath = createUserAppDataPath(CoreConstants.TemplateUserDatabase);
     }

      private void createDefaultSettingsFolder()
      {
         if (!DirectoryHelper.DirectoryExists(UserApplicationSettingsFolderPath))
            DirectoryHelper.CreateDirectory(UserApplicationSettingsFolderPath);

         if (!DirectoryHelper.DirectoryExists(ApplicationSettingsFolderPath))
            DirectoryHelper.CreateDirectory(ApplicationSettingsFolderPath);
      }

      private string createApplicationDataOrLocalPathForFile(string fileName)
      {
         var applicationDataPathForFile = createAppDataPath(fileName);
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

      protected override string ApplicationFolderPathWithRevision(string version) => Path.Combine(CoreConstants.ApplicationFolderPath, version);
   }
}