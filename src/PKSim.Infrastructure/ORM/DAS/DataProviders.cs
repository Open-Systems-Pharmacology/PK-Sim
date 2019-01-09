namespace PKSim.Infrastructure.ORM.DAS
{
   public enum DataProviders
   {
      /// <summary>
      /// This means connecting to a Microsoft Access database with standard security.
      /// </summary>
      MSAccess,
      /// <summary>
      /// This means connecting to an SQLite database using the SQLite data provider.
      /// </summary>
      SQLite
   }
}