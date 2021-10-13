using OSPSuite.Core;

namespace PKSim.Core
{
   public interface IPKSimConfiguration : IApplicationConfiguration
   {
      /// <summary>
      ///    Path of the PKSim Database
      /// </summary>
      string PKSimDbPath { get; set; }

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
      ///    Full path to MoBi application exe. This path is read from the registry entry
      /// </summary>
      string MoBiPath { get; }

      /// <summary>
      /// Full path of the main file containing the list of all available templates
      /// </summary>
      string RemoteTemplateSummaryPath { get; }

      string RemoteTemplateFolderPath { get; }
   }

}