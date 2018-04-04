using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;

namespace PKSim.Infrastructure.Reporting.TeX.Items
{
   public class PKAnalysesTable
   {
      private readonly DataTable _pkTable;

      public PKAnalysesTable(DataTable pkTable)
      {
         _pkTable = pkTable;
      }

      public IReadOnlyList<string> AllCurveNames => _pkTable.AllValuesInColumn<string>(PKSimConstants.PKAnalysis.CurveName).Distinct().ToList();

      public DataTable PKTableFor(string curveName)
      {
         var view = new DataView(_pkTable, $"{PKSimConstants.PKAnalysis.CurveName} = '{curveName}'", string.Empty, DataViewRowState.CurrentRows);
         var tableName = $"{PKSimConstants.UI.PKAnalyses} for {curveName}";
         var allColumnNames = _pkTable.Columns.OfType<DataColumn>().Select(x => x.ColumnName).ToList();
         allColumnNames.Remove(PKSimConstants.PKAnalysis.CurveName);
         return view.ToTable(tableName, distinct: false, columnNames: allColumnNames.ToArray());
      }
   }
}