using System.Data;
using PKSim.Core;
using PKSim.Infrastructure.ORM.DAS;

namespace PKSim.Infrastructure.ORM.Core
{
   public interface IDbGateway
   {
      DataTable ExecuteStatementForDataTable(string selectStatement);
   }

   public class SimpleDbGateway : IDbGateway
   {
      private readonly IModelDatabase _modelDatabase;
      private readonly IPKSimConfiguration _configuration;

      public SimpleDbGateway(IModelDatabase modelDatabase, IPKSimConfiguration configuration)
      {
         _modelDatabase = modelDatabase;
         _configuration = configuration;
         _modelDatabase.Connect(_configuration.PKSimDbPath);
      }

      public virtual DataTable ExecuteStatementForDataTable(string selectStatement)
      {
         var dataTable = new DASDataTable(databaseConnection);
         databaseConnection.FillDataTable(dataTable, selectStatement);
         return dataTable;
      }

      private DAS.DAS databaseConnection => _modelDatabase.DatabaseObject;
   }
}