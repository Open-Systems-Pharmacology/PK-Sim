using System.Collections.Generic;
using System.Data;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Chart;

namespace PKSim.Core.Mappers
{
   public class RangeChartDataToDataTableMapper : ChartDataToDataTableMapper<RangeXValue, RangeYValue>
   {
      private string _yLowerPercentileColumn;
      private string _yUpperPercentileColumn;
      private string _xMinimumColumn;
      private string _xMaximumColumn;
      private string _xNumberOfIndividualsColumn;
      private string _xValueColumn;
      private string _yValueColumn;

      protected override void AddSpecificChartColumns(DataTable dataTable, CurveData<RangeXValue, RangeYValue> curveData, bool exportForPivot)
      {
         _xValueColumn = curveData.XAxis.Caption;
         _yValueColumn = curveData.YAxis.Caption;
         _yLowerPercentileColumn = CoreConstants.CompositeNameFor(PKSimConstants.UI.LowerPercentile, _yValueColumn);
         _yUpperPercentileColumn = CoreConstants.CompositeNameFor(PKSimConstants.UI.UpperPercentile, _yValueColumn);
         _xMinimumColumn = CoreConstants.CompositeNameFor(PKSimConstants.UI.Minimum, _xValueColumn);
         _xMaximumColumn = CoreConstants.CompositeNameFor(PKSimConstants.UI.Maximum, _xValueColumn);
         _xNumberOfIndividualsColumn = PKSimConstants.UI.NumberOfIndividuals;

         dataTable.AddColumn<float>(_xMinimumColumn);
         dataTable.AddColumn<float>(_xValueColumn);
         dataTable.AddColumn<float>(_xMaximumColumn);
         dataTable.AddColumn<float>(_xNumberOfIndividualsColumn);
         dataTable.AddColumn<float>(_yLowerPercentileColumn);
         dataTable.AddColumn<float>(_yValueColumn);
         dataTable.AddColumn<float>(_yUpperPercentileColumn);
      }

      protected override IEnumerable<DataRow> AddSpecificChartValues(DataRow row, CurveData<RangeXValue, RangeYValue> curveData, bool exportForPivot)
      {
         var newRows = new List<DataRow>();

         for (var i = 0; i < curveData.XValues.Count; i++)
         {
            var newRow = row.Table.NewRow();
            var xValue = curveData.XValues[i];
            var yValue = curveData.YValues[i];

            newRow.ItemArray = row.ItemArray;
            newRow[_xMinimumColumn] = xValueForDataTableFor(curveData, xValue.Minimum);
            newRow[_xValueColumn] = xValueForDataTableFor(curveData, xValue.X);
            newRow[_xMaximumColumn] = xValueForDataTableFor(curveData, xValue.Maximum);
            newRow[_xNumberOfIndividualsColumn] = xValue.NumberOfItems;
            newRow[_yLowerPercentileColumn] = yValueForDataTableFor(curveData, yValue.LowerPercentile);
            newRow[_yValueColumn] = yValueForDataTableFor(curveData, yValue.Median);
            newRow[_yUpperPercentileColumn] = yValueForDataTableFor(curveData, yValue.UpperPercentile);
            newRows.Add(newRow);
         }

         return newRows;
      }
   }
}