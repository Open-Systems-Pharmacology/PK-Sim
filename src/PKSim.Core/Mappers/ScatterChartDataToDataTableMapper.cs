using System.Collections.Generic;
using System.Data;
using PKSim.Core.Chart;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Mappers
{
   public class ScatterChartDataToDataTableMapper : ChartDataToDataTableMapper<ScatterXValue, ScatterYValue>
   {
      protected override void AddSpecificChartColumns(DataTable dataTable, CurveData<ScatterXValue, ScatterYValue> curveData, bool exportForPivot)
      {
         dataTable.AddColumn<float>(curveData.XAxis.Caption);
         dataTable.AddColumn<float>(curveData.YAxis.Caption);
      }

      protected override IEnumerable<DataRow> AddSpecificChartValues(DataRow row, CurveData<ScatterXValue, ScatterYValue> curveData, bool exportForPivot)
      {
         var newRows = new List<DataRow>();

         for (var i = 0; i < curveData.XValues.Count; i++)
         {
            var newRow = row.Table.NewRow();
            newRow.ItemArray = row.ItemArray;
            newRow[curveData.XAxis.Caption] = ValueForDataTableFor(curveData.XAxis, curveData.XValues[i].X);
            newRow[curveData.YAxis.Caption] = ValueForDataTableFor(curveData.YAxis, curveData.YValues[i].Y);
            newRows.Add(newRow);
         }
         return newRows;
      }
   }
}