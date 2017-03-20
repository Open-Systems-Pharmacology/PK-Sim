using OSPSuite.Utility.Reflection;

namespace PKSim.Presentation.DTO
{
   public class SpeciesDatabaseMapDTO : Notifier
   {
      /// <summary>
      ///   Name of the species for which the database path is specified
      /// </summary>
      public string SpeciesName { get; set; }

      /// <summary>
      ///   Name of the species for which the database path is specified
      /// </summary>
      public string SpeciesDisplayName { get; set; }

      private string _databaseFullPath;

      public bool HasChanged
      {
         get { return !string.Equals(DatabaseFullPath, OriginalDatabasePathFullPath); }
      }

      public bool WasDeleted
      {
         get { return !string.IsNullOrEmpty(OriginalDatabasePathFullPath) && string.IsNullOrEmpty(DatabaseFullPath); }
      }

      /// <summary>
      ///   Path of the protein expression database for the given species
      /// </summary>
      public string DatabaseFullPath
      {
         get { return _databaseFullPath; }
         set
         {
            _databaseFullPath = value;
            OnPropertyChanged(() => DatabaseFullPath);
         }
      }

      /// <summary>
      ///   Path of the protein expression database for the given species
      /// </summary>
      public string OriginalDatabasePathFullPath { get; set; }
   }
}