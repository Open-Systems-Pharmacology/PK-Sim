using System.Collections.Generic;
using System.Data;
using PKSim.Core.Chart;
using OSPSuite.Core.Extensions;

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
            newRow.ItemArray = row.ItemArray;
            newRow[_xValue] = curveData.XValues[i].ToString(curveData.XAxis);
            newRow[_lowerWhisker] = ValueForDataTableFor(curveData.YAxis, curveData.YValues[i].LowerWhisker);
            newRow[_lowerBox] = ValueForDataTableFor(curveData.YAxis, curveData.YValues[i].LowerBox);
            newRow[_median] = ValueForDataTableFor(curveData.YAxis, curveData.YValues[i].Median);
            newRow[_upperBox] = ValueForDataTableFor(curveData.YAxis, curveData.YValues[i].UpperBox);
            newRow[_upperWhisker] = ValueForDataTableFor(curveData.YAxis, curveData.YValues[i].UpperWhisker);
            newRow[_variable] = curveData.YAxis.Caption;
            newRows.Add(newRow);
         }
         return newRows;
      }
   }
}