using System;
using System.Linq;
using OSPSuite.Serializer.Xml;
using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Presentation;
using PKSim.Presentation.Services;

namespace PKSim.Infrastructure.Services
{
   public class UserSettingsPersistor : IUserSettingsPersistor
   {
      private readonly IStringSerializer _stringSerializer;
      private readonly IUserSettings _defaultUserSettings;
      private readonly IPKSimConfiguration _configuration;

      public UserSettingsPersistor(IStringSerializer stringSerializer, IUserSettings defaultUserSettings,
         IPKSimConfiguration configuration)
      {
         _stringSerializer = stringSerializer;
         _defaultUserSettings = defaultUserSettings;
         _configuration = configuration;
      }

      public void Save(IUserSettings userSettings)
      {
         userSettings.SaveLayout();

         var xmlContent = _stringSerializer.Serialize(userSettings);
         XmlHelper.SaveXmlContentToFile(xmlContent, _configuration.UserApplicationSettingsFilePath);
      }

      public IUserSettings Load()
      {
         try
         {
            foreach (var filePath in _configuration.UserApplicationSettingsFilePaths.Where(FileHelper.FileExists))
            {
               var xmlContent = XmlHelper.XmlContentFromFile(filePath);
               return _stringSerializer.Deserialize<IUserSettings>(xmlContent);
            }
         }
         //We do not want to have a crash if the user has edited the configuration by hand
         catch (Exception)
         {
            return _defaultUserSettings;
         }

         return _defaultUserSettings;
      }
   }
}