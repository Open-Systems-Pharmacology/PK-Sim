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

      public static DataRow ExecuteQueryForSingleRowOrNull(this DAS connection, string query)
      {
         var rows = ExecuteQueryForDataTable(connection, query).Rows;
         return rows.Count() > 0 ? rows.ItemByIndex(0) : null;
      }
   }
}