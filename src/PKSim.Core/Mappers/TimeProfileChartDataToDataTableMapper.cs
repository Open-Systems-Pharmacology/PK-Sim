using System.Collections.Generic;
using System.Data;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Chart;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Mappers
{
   public class TimeProfileChartDataToDataTableMapper : ChartDataToDataTableMapper<TimeProfileXValue, TimeProfileYValue>
   {
      private string _xValueColumn;
      private string _yValueColumn;
      private string _yLowerValueColumn;
      private string _yUpperValueColumn;

      public TimeProfileChartDataToDataTableMapper() : base(canExportPivotedValues: true)
      {
      }

      protected override void AddSpecificChartColumns(DataTable dataTable, CurveData<TimeProfileXValue, TimeProfileYValue> curveData, bool exportForPivot)
      {
         _xValueColumn = curveData.XAxis.Caption;
         _yValueColumn = curveData.YAxis.Caption;
         _yLowerValueColumn = CoreConstants.CompositeNameFor(PKSimConstants.UI.LowerValue, _yValueColumn);
         _yUpperValueColumn = CoreConstants.CompositeNameFor(PKSimConstants.UI.UpperValue, _yValueColumn);

         //Export xValueColumn as string to enable correct pivot on x values
         if (exportForPivot)
            dataTable.AddColumn<string>(_xValueColumn);
         else
            dataTable.AddColumn<float>(_xValueColumn);

         dataTable.AddColumn<float>(_yLowerValueColumn);
         dataTable.AddColumn<float>(_yValueColumn);
         dataTable.AddColumn<float>(_yUpperValueColumn);
      }

      private IEnumerable<DataRow> addSpecificChartValues<TY>(DataRow row, CurveData<TimeProfileXValue, TY> curveData, bool exportForPivot) where TY: ITimeProfileYValue
      {
         var newRows = new List<DataRow>();

         for (var i = 0; i < curveData.XValues.Count; i++)
         {
            var newRow = row.Table.NewRow();
            newRow.ItemArray = row.ItemArray;
            var xValue = xValueForDataTableFor(curveData,  curveData.XValues[i].X);
            var yValue = curveData.YValues[i];
            if (exportForPivot)
               newRow[_xValueColumn] = xValue.ConvertedTo<string>();
            else
               newRow[_xValueColumn] = xValue;

            newRow[_yLowerValueColumn] = yValueForDataTableFor(curveData, yValue.LowerValue);
            newRow[_yValueColumn] = yValueForDataTableFor(curveData, yValue.Y);
            newRow[_yUpperValueColumn] = yValueForDataTableFor(curveData,  yValue.UpperValue);
            newRows.Add(newRow); 
         }
         return newRows;
      }

      protected override void PerformSpecificTransformationOnPivotedTable(DataTable pivotTable)
      {
         //column xValueColumn was used as string in order to allow correct pivotation. We need the value as double for the export
         if (!pivotTable.Columns.Contains(_xValueColumn))
            return;

         var column = pivotTable.Columns[_xValueColumn];
         var xValueColumnAsString = $"{_xValueColumn}AsString";
         column.ColumnName = xValueColumnAsString;

         //make the column the first column of the pivot table
         pivotTable.AddColumn<float>(_xValueColumn).SetOrdinal(0);
         foreach (DataRow row in pivotTable.Rows)
         {
            row[_xValueColumn] = row.ValueAt<float>(xValueColumnAsString);
         }

         pivotTable.Columns.Remove(column);
      }

      protected override void AddObservedDataToTable(DataTable dataTable, PaneData<TimeProfileXValue, TimeProfileYValue> pane, bool exportForPivot)
      {
         AddCurvesToTable(dataTable, pane, pane.VisibleObservedCurveData, exportForPivot, addSpecificChartValues);
      }

      protected override IEnumerable<DataRow> AddSpecificChartValues(DataRow row, CurveData<TimeProfileXValue, TimeProfileYValue> curveData, bool exportForPivot)
      {
         return addSpecificChartValues(row, curveData, exportForPivot);
      }

      protected override IReadOnlyList<string> GetDataFields(CurveData<TimeProfileXValue, TimeProfileYValue> curveData)
      {
         return new[] {_yLowerValueColumn, _yValueColumn, _yUpperValueColumn};
      }

      protected override IReadOnlyList<string> GetRowFields(CurveData<TimeProfileXValue, TimeProfileYValue> curveData)
      {
         return new[] {_xValueColumn};
      }
   }
}