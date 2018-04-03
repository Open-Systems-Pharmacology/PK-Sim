using System.Data;
using OSPSuite.TeXReporting.Data;
using OSPSuite.Utility.Extensions;

namespace PKSim.Infrastructure.Reporting.Extensions
{
   public static class DataTableExtensions
   {
      public static DataColumn AddHiddenColumn(this DataTable dataTable, string columnName)
      {
         return dataTable.AddColumn<string>(columnName).AsHidden();
      }
   }
}