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
      ///    Path of the application specific settings file (for all users)
      /// </summary>
      string ApplicationSettingsFilePath { get; }

      /// <summary>
      /// Returns a possible enumeration containg the path of application settings that can be loaded. (Starting from the most recent one down to the first available one)
      /// </summary>
      IEnumerable<string> ApplicationSettingsFilePaths { get; } 
   
      /// <summary>
      ///    Full path to MoBi application exe. This path is read from the registry entry
      /// </summary>
      string MoBiPath { get; }
   }
}