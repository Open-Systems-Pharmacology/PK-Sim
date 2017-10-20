using System.Collections.Generic;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Presentation;
using PKSim.Presentation.Services;

namespace PKSim.Infrastructure.Services
{
   public class UserSettingsPersistor : SettingsPersistor<IUserSettings>, IUserSettingsPersistor
   {
      public UserSettingsPersistor(IStringSerializer stringSerializer, IUserSettings defaultUserSettings,
         IPKSimConfiguration configuration) : base(stringSerializer, defaultUserSettings, configuration)
      {
      }

      public override void Save(IUserSettings userSettings)
      {
         userSettings.SaveLayout();
         base.Save(userSettings);
      }

      protected override string SettingsFilePath => _configuration.UserSettingsFilePath;
      protected override IEnumerable<string> SettingsFilePaths => _configuration.UserSettingsFilePaths;
   }
}