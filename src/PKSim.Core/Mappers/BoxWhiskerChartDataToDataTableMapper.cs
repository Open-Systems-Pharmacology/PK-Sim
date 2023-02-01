using System.Collections.Generic;
using System.Data;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Chart;

namespace PKSim.Core.Mappers
{
   public class BoxWhiskerChartDataToDataTableMapper : ChartDataToDataTableMapper<BoxWhiskerXValue, BoxWhiskerYValue>
   {
      private const string _xValue = "X Value";
      private const string _lowerWhisker = "Lower Whisker";
      private const string _lowerBox = "Lower Box";
      private const string _median = "Median";
      private const string _upperBox = "Upper Box";
      private const string _upperWhisker = "Upper Whisker";
      private const string _variable = "Variable";

      protected override void AddSpecificChartColumns(DataTable dataTable, CurveData<BoxWhiskerXValue, BoxWhiskerYValue> curveData, bool exportForPivot)
      {
         dataTable.AddColumn(_xValue);
         dataTable.AddColumn<float>(_lowerWhisker);
         dataTable.AddColumn<float>(_lowerBox);
         dataTable.AddColumn<float>(_median);
         dataTable.AddColumn<float>(_upperBox);
         dataTable.AddColumn<float>(_upperWhisker);
         dataTable.AddColumn(_variable);
      }

      protected override IEnumerable<DataRow> AddSpecificChartValues(DataRow row, CurveData<BoxWhiskerXValue, BoxWhiskerYValue> curveData, bool exportForPivot)
      {
         var newRows = new List<DataRow>();

         for (var i = 0; i < curveData.XValues.Count; i++)
         {
            var newRow = row.Table.NewRow();
            var yValue = curveData.YValues[i];
            newRow.ItemArray = row.ItemArray;
            newRow[_xValue] = curveData.XValues[i].ToString(curveData.XAxis);
            newRow[_lowerWhisker] = yValueForDataTableFor(curveData, yValue.LowerWhisker);
            newRow[_lowerBox] = yValueForDataTableFor(curveData, yValue.LowerBox);
            newRow[_median] = yValueForDataTableFor(curveData, yValue.Median);
            newRow[_upperBox] = yValueForDataTableFor(curveData, yValue.UpperBox);
            newRow[_upperWhisker] = yValueForDataTableFor(curveData, yValue.UpperWhisker);
            newRow[_variable] = curveData.YAxis.Caption;
            newRows.Add(newRow);
         }

         return newRows;
      }
   }
}