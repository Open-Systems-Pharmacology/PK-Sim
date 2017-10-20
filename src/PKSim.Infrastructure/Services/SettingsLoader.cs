using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Presentation;
using PKSim.Presentation.Services;

namespace PKSim.Infrastructure.Services
{
   public class SettingsLoader : IStartable
   {
      private readonly IUserSettingsPersistor _userSettingsPersistor;
      private readonly IApplicationSettingsPersitor _applicationSettingsPersistor;
      private readonly IPKSimConfiguration _configuration;

      public SettingsLoader(
         IUserSettingsPersistor userSettingsPersistor,
         IApplicationSettingsPersitor applicationSettingsPersistor,
         IPKSimConfiguration configuration)
      {
         _userSettingsPersistor = userSettingsPersistor;
         _applicationSettingsPersistor = applicationSettingsPersistor;
         _configuration = configuration;
      }

      public void Start()
      {
         var userSettings = _userSettingsPersistor.Load();
         _applicationSettingsPersistor.Load();

         createDefaultUserTemplate(userSettings);
      }

      private void createDefaultUserTemplate(IUserSettings userSettings)
      {
         //another file was already created and exists. nothing to do
         if (FileHelper.FileExists(userSettings.TemplateDatabasePath))
            return;

         //The default file was already created in the default location. nothing to do
         if (FileHelper.FileExists(_configuration.DefaultTemplateUserDatabasePath))
            return;

         userSettings.TemplateDatabasePath = _configuration.DefaultTemplateUserDatabasePath;
         FileHelper.Copy(_configuration.TemplateUserDatabaseTemplatePath, userSettings.TemplateDatabasePath);
      }
   }
}