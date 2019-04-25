using System;

namespace PKSim.Infrastructure.ORM.Core
{
   public class DatabaseDisposer : IDisposable
   {
      private readonly IDatabase _database;

      public DatabaseDisposer(IDatabase database, string databasePath)
      {
         _database = database;
         _database.Connect(databasePath);
      }

      public void Dispose()
      {
         _database?.Disconnect();
      }
   }
}