using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Serializer.Xml;
using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services
{
   public abstract class SettingsPersistor<TSettings> : IPersistor<TSettings>
   {
      private readonly IStringSerializer _stringSerializer;
      protected readonly TSettings _defaultSettings;
      protected readonly IPKSimConfiguration _configuration;

      protected SettingsPersistor(IStringSerializer stringSerializer, TSettings defaultSettings, IPKSimConfiguration configuration)
      {
         _stringSerializer = stringSerializer;
         _defaultSettings = defaultSettings;
         _configuration = configuration;
      }

      public virtual void Save(TSettings userSettings)
      {
         var xmlContent = _stringSerializer.Serialize(userSettings);
         XmlHelper.SaveXmlContentToFile(xmlContent, SettingsFilePath);
      }

      protected abstract string SettingsFilePath { get; }

      public virtual TSettings Load()
      {
         try
         {
            foreach (var filePath in SettingsFilePaths.Where(FileHelper.FileExists))
            {
               var xmlContent = XmlHelper.XmlContentFromFile(filePath);
               return _stringSerializer.Deserialize<TSettings>(xmlContent);
            }
         }
         //We do not want to have a crash if the user has edited the configuration by hand
         catch (Exception)
         {
            return _defaultSettings;
         }

         return _defaultSettings;
      }

      protected abstract IEnumerable<string> SettingsFilePaths { get; }

      public void SaveCurrent()
      {
         Save(_defaultSettings);
      }
   }
}