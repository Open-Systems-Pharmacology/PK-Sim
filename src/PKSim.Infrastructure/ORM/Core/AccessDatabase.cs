using PKSim.Core;
using PKSim.Infrastructure.ORM.DAS;

namespace PKSim.Infrastructure.ORM.Core
{
   public abstract class AccessDatabase : Database
   {
      protected AccessDatabase(string password=null) : base(password, CoreConstants.AccessUserName)
      {
      }

      protected override DataProviders GetProvider()
      {
         return string.IsNullOrEmpty(_password) ? DataProviders.MSAccess : DataProviders.MSAccessWithDatabaseSecurity;
      }
   }
}