namespace PKSim.Core
{
   public class SpeciesDatabaseMap
   {
      /// <summary>
      ///    Name of the species for which the database path is specified
      /// </summary>
      public string Species { get; set; }

      /// <summary>
      ///    Path of the protein expression database for the given species
      /// </summary>
      public string DatabaseFullPath { get; set; }
   }
}