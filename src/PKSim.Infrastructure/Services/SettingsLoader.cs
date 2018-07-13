using OSPSuite.Utility;
using PKSim.Core.Services;
using PKSim.Presentation.Services;

namespace PKSim.Infrastructure.Services
{
   public class SettingsLoader : IStartable
   {
      private readonly IUserSettingsPersistor _userSettingsPersistor;
      private readonly IApplicationSettingsPersistor _applicationSettingsPersistor;

      public SettingsLoader(
         IUserSettingsPersistor userSettingsPersistor, IApplicationSettingsPersistor applicationSettingsPersistor
      )
      {
         _userSettingsPersistor = userSettingsPersistor;
         _applicationSettingsPersistor = applicationSettingsPersistor;
      }

      public void Start()
      {
         _userSettingsPersistor.Load();
         _applicationSettingsPersistor.Load();
      }
   }
}