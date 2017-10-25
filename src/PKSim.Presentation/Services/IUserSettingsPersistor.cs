using PKSim.Core.Services;

namespace PKSim.Presentation.Services
{
   public interface IUserSettingsPersistor : IPersistor<IUserSettings>
   {
      /// <summary>
      /// Saves current user settings
      /// </summary>
      void SaveCurrent();
   }
}