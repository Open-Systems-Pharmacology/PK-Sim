using System.Data;
using PKSim.Infrastructure.ORM.DAS;

namespace PKSim.Infrastructure.Extensions
{
   public static class DASExtensions
   {
      public static DASDataTable ExecuteQueryForDataTable(this DAS connection, string query)
      {
         var table = new DASDataTable(connection);
         connection.FillDataTable(table, query);
         return table;
      }

      public static DataRow ExecuteQueryForSingleRow(this DAS connection, string query)
      {
         return ExecuteQueryForDataTable(connection, query).Rows.ItemByIndex(0);
      }
   }
}