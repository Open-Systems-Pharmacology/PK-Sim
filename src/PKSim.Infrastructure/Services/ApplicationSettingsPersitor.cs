using System.Collections.Generic;
using PKSim.Core;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services
{
   public class ApplicationSettingsPersitor : SettingsPersistor<IApplicationSettings>, IApplicationSettingsPersitor
   {
      public ApplicationSettingsPersitor(IStringSerializer stringSerializer, IApplicationSettings defaultApplicationSettings, IPKSimConfiguration configuration)
         : base(stringSerializer, defaultApplicationSettings, configuration)
      {
      }

      protected override string SettingsFilePath => _configuration.ApplicationSettingsFilePath;
      protected override IEnumerable<string> SettingsFilePaths => _configuration.ApplicationSettingsFilePaths;
   }
}