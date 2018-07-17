using System;
using PKSim.Infrastructure.ORM.DAS;

namespace PKSim.Infrastructure.ORM.Core
{
   public abstract class SQLiteDatabase : Database
   {
      protected SQLiteDatabase(string password = null) : base(password, string.Empty)
      {
      }

      protected override DataProviders GetProvider() => DataProviders.SQLite;
   }

   public abstract class AccessDatabase : Database
   {
      protected AccessDatabase(string password = null) : base(password, String.Empty)
      {
      }
      protected override DataProviders GetProvider() => DataProviders.MSAccess;

   }
}