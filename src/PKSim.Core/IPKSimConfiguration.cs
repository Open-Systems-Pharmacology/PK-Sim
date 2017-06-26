using System.Collections.Generic;
using OSPSuite.Core;

namespace PKSim.Core
{
   public interface IPKSimConfiguration: IApplicationConfiguration
   {
      /// <summary>
      ///    Path of the PKSim Database
      /// </summary>
      string PKSimDbPath { get; }

      /// <summary>
      ///    Path of the dimension file
      /// </summary>
      string DimensionFilePath { get; }
      
      /// <summary>
      ///    Returns the path where the configuration file for the logger resides
      /// </summary>
      string LogConfigurationFile { get; }

      /// <summary>
      ///    Path of the System Template Database (Read only DB delivered with setup)
      /// </summary>
      string TemplateSystemDatabasePath { get; }

      /// <summary>
      ///    Path of the template used to create a user database (empty database to be copied and renamed)
      /// </summary>
      string TemplateUserDatabaseTemplatePath { get; }

      /// <summary>
      ///    Path of the default user template database created under the current user profile
      /// </summary>
      string DefaultTemplateUserDatabasePath { get; }

      /// <summary>
      ///    Path of the schema used to validate our models
      /// </summary>
      string SimModelSchemaPath { get; }

      /// <summary>
      ///    Folder path of the application specific settings (for all users)
      /// </summary>
      string ApplicationSettingsFolderPath { get; }

      /// <summary>
      ///    Path of the application specific settings file (for all users)
      /// </summary>
      string ApplicationSettingsFilePath { get; }

      /// <summary>
      /// Returns a possible enumeration containg the path of application settings that can be loaded. (Starting from the most recent one down to the first available one)
      /// </summary>
      IEnumerable<string> ApplicationSettingsFilePaths { get; } 

      /// <summary>
      ///    Folder path of the current user specific settings (current user only)
      /// </summary>
      string UserApplicationSettingsFolderPath { get; }

      /// <summary>
      ///    Paths of the current user specific settings file (current user only)
      /// </summary>
      string UserApplicationSettingsFilePath { get; }
   
      /// <summary>
      ///    Full path to MoBi application exe. This path is read from the registry entry
      /// </summary>
      string MoBiPath { get; }
   }
}