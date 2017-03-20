namespace PKSim.Infrastructure.ORM.DAS
{
   public enum DataProviders
   {
      /// <summary>
      /// This means connecting to a Microsoft Access database with standard security.
      /// </summary>
      MSAccess,
      /// <summary>
      /// This means connecting to a Microsoft Access database with database security.
      /// </summary>
      MSAccessWithDatabaseSecurity,
      /// <summary>
      /// This means connecting to a Microsoft Access database with workgroup security.
      /// </summary>
      MSAccessWithWorkgroupSecurity,
      /// <summary>
      /// This means connecting to an Oracle database using the data provider from Oracle.
      /// </summary>
      Oracle,
      /// <summary>
      /// This means connecting to an Oracle database using the Microsoft data provider.
      /// </summary>
      MSOracle,
      /// <summary>
      /// This means connecting to an SQLite database using the SQLite data provider.
      /// </summary>
      SQLite
   }
}