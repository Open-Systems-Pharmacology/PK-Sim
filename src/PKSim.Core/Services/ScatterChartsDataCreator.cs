using System.Data;
using System.IO;
using PKSim.Assets;
using PKSim.Core.Chart;
using PKSim.Core.Extensions;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Services
{
   public interface IScatterChartDataCreator : IChartDataCreator<ScatterXValue, ScatterYValue>
   {
   }

   public class ScatterChartDataCreator : ChartDataCreator<ScatterXValue, ScatterYValue>, IScatterChartDataCreator
   {
      public ScatterChartDataCreator(IDimensionRepository dimensionRepository, IPivotResultCreator pivotResultCreator)
         : base(dimensionRepository, pivotResultCreator)
      {
      }

      protected override bool CheckFields()
      {
         return _analysis.AllFieldsOn<PopulationAnalysisNumericField>(PivotArea.DataArea).Count >= 2;
      }

      protected override ChartData<ScatterXValue, ScatterYValue> BuildChartsData()
      {
         var paneFieldNames = _analysis.StringFieldNamesOn(PivotArea.RowArea);
         var seriesFieldNames = _analysis.StringFieldNamesOn(PivotArea.ColumnArea);
         var paneFieldComparers = GetFieldComparers(PivotArea.RowArea);
         var seriesFieldComparers = GetFieldComparers(PivotArea.ColumnArea);
         var dataFields = _analysis.AllFieldsOn<PopulationAnalysisNumericField>(PivotArea.DataArea);

         // due to CheckFields is assured, that fields are available
         var xAxisField = dataFields[0];
         var yAxisField = dataFields[1];

         CreatePrimaryKey(seriesFieldNames, paneFieldNames);
         var chart = CreateChart(xAxisField, paneFieldComparers);

         foreach (var row in RowsForDataField(xAxisField.Name))
         {
            var series = GetCurveData(row, paneFieldNames, paneFieldComparers, seriesFieldNames, seriesFieldComparers, chart, yAxisField);
            setSeriesValues(series, row, yAxisField.Name);
         }

         return chart;
      }

      private void setSeriesValues(CurveData<ScatterXValue, ScatterYValue> series, DataRow row, string yFieldName)
      {
         // find row with yField / data for y-Values
         var pkValues = row.GetPrimaryKeyValues();
         pkValues[0] = yFieldName;
         DataRow yFieldRow = _data.Rows.Find(pkValues);

         var xValues = (float[]) row[_aggreationName];
         var yValues = (float[]) yFieldRow[_aggreationName];

         if (xValues.Length != yValues.Length)
            throw new InvalidDataException(PKSimConstants.Error.DifferentVectorLengths);

         for (int i = 0; i < xValues.Length; i++)
         {
            var scatterValue = new ScatterYValue(yValues[i]);
            if (scatterValue.IsValid)
               series.Add(new ScatterXValue(xValues[i]), scatterValue);
         }
      }
   }
}