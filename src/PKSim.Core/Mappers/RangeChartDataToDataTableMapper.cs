using System.Collections.Generic;
using System.Data;
using PKSim.Assets;
using PKSim.Core.Chart;
using OSPSuite.Core.Extensions;

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
            newRow.ItemArray = row.ItemArray;
            newRow[_xMinimumColumn] = ValueForDataTableFor(curveData.XAxis, curveData.XValues[i].Minimum);
            newRow[_xValueColumn] = ValueForDataTableFor(curveData.XAxis, curveData.XValues[i].X);
            newRow[_xMaximumColumn] = ValueForDataTableFor(curveData.XAxis, curveData.XValues[i].Maximum);
            newRow[_xNumberOfIndividualsColumn] = curveData.XValues[i].NumberOfItems;
            newRow[_yLowerPercentileColumn] = ValueForDataTableFor(curveData.YAxis, curveData.YValues[i].LowerPercentile);
            newRow[_yValueColumn] = ValueForDataTableFor(curveData.YAxis, curveData.YValues[i].Median);
            newRow[_yUpperPercentileColumn] = ValueForDataTableFor(curveData.YAxis, curveData.YValues[i].UpperPercentile);
            newRows.Add(newRow);
         }
         return newRows;
      }
   }
}