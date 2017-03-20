using System;
using System.Linq;
using OSPSuite.Serializer.Xml;
using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services
{
   public class ApplicationSettingsPersitor : IApplicationSettingsPersitor
   {
      private readonly IStringSerializer _stringSerializer;
      private readonly IApplicationSettings _defaultApplicationSettings;
      private readonly IPKSimConfiguration _configuration;

      public ApplicationSettingsPersitor(IStringSerializer stringSerializer, IApplicationSettings defaultApplicationSettings, IPKSimConfiguration configuration)
      {
         _stringSerializer = stringSerializer;
         _defaultApplicationSettings = defaultApplicationSettings;
         _configuration = configuration;
      }

      public void Save(IApplicationSettings applicationSettings)
      {
         var xmlContent = _stringSerializer.Serialize(applicationSettings);
         XmlHelper.SaveXmlContentToFile(xmlContent, _configuration.ApplicationSettingsFilePath);
      }

      public IApplicationSettings Load()
      {
         try
         {
            foreach (var filePath in _configuration.ApplicationSettingsFilePaths.Where(FileHelper.FileExists))
            {
               var xmlContent = XmlHelper.XmlContentFromFile(filePath);
               return _stringSerializer.Deserialize<IApplicationSettings>(xmlContent);
            }
         }
         //We do not want to have a crash if the user has edited the configuration by hand
         catch (Exception)
         {
            return _defaultApplicationSettings;
         }

         return _defaultApplicationSettings;
      }
   }
}