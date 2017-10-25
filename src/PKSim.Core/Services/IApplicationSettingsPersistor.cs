namespace PKSim.Core.Services
{
   public interface IApplicationSettingsPersistor : IPersistor<IApplicationSettings>
   {
      /// <summary>
      /// Saves current application settings
      /// </summary>
      void SaveCurrent();
   }
}