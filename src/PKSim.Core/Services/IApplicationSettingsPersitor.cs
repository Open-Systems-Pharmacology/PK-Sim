namespace PKSim.Core.Services
{
   public interface IApplicationSettingsPersitor : IPersistor<IApplicationSettings>
   {
      /// <summary>
      /// Saves current application settings
      /// </summary>
      void SaveCurrent();
   }
}