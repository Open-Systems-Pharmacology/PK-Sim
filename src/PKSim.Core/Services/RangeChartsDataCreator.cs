using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Chart;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;

namespace PKSim.Core.Services
{
   public interface IRangeChartDataCreator : IChartDataCreator<RangeXValue, RangeYValue>
   {
   }

   public class RangeChartDataCreator : ChartDataCreator<RangeXValue, RangeYValue>, IRangeChartDataCreator
   {
      private readonly IBinIntervalsCreator _binIntervalsCreator;
      private const int BORDER_PERCENTILE = 5;

      public RangeChartDataCreator(IDimensionRepository dimensionRepository, IPivotResultCreator pivotResultCreator, IBinIntervalsCreator binIntervalsCreator) : base(dimensionRepository, pivotResultCreator)
      {
         _binIntervalsCreator = binIntervalsCreator;
      }

      protected override bool CheckFields()
      {
         return _analysis.AllFieldsOn<PopulationAnalysisNumericField>(PivotArea.DataArea).Count >= 2;
      }

      protected override ChartData<RangeXValue, RangeYValue> BuildChartsData()
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
            var curveData = GetCurveData(row, paneFieldNames, paneFieldComparers, seriesFieldNames, seriesFieldComparers, chart, yAxisField);
            setCurveDataValues(curveData, row, yAxisField.Name);
         }

         connectCurvesDefinedIn(chart);
         return chart;
      }

      private void connectCurvesDefinedIn(ChartData<RangeXValue, RangeYValue> chart)
      {
         chart.Panes.Each(connectCurvesDefinedIn);
      }

      private void connectCurvesDefinedIn(PaneData<RangeXValue, RangeYValue> panes)
      {
         var orderedCurveByXvalue = panes.Curves.Where(x => x.XValues.Any())
            .OrderBy(x => x.XValues[0]).ToList();

         //Some curves are invalid, we do not try to connect anything in that case
         if (orderedCurveByXvalue.Count != panes.Curves.Count)
            return;

         for (int i = 1; i < orderedCurveByXvalue.Count; i++)
         {
            var firstCurve = orderedCurveByXvalue[i - 1];
            var secondCurve = orderedCurveByXvalue[i];
            firstCurve.Add(secondCurve.XValues[0], secondCurve.YValues[0]);
         }
      }

      private void setCurveDataValues(CurveData<RangeXValue, RangeYValue> series, DataRow xFieldRow, string yFieldName)
      {
         // find row with yField / data for y-Values
         var pkValues = xFieldRow.GetPrimaryKeyValues();
         pkValues[0] = yFieldName; // DATA_FIELD-column first, see CreatePrimaryKey
         DataRow yFieldRow = _data.Rows.Find(pkValues);

         var xValues = xFieldRow[_aggreationName] as float[];
         var yValues = yFieldRow[_aggreationName] as float[];

         //possible if no values are defined for the row filter
         if (xValues == null || yValues == null)
            return;

         if (xValues.Length != yValues.Length)
            throw new InvalidDataException(PKSimConstants.Error.DifferentVectorLengths);

         // create range curve on grid. Do not allow uniform interval distribution as data might be misleading
         var xValueIntervals = _binIntervalsCreator.CreateIntervalsFor(xValues.ToDoubleArray());
         foreach (var xValueInterval in xValueIntervals)
         {
            var yValuesForXInterval = yValues.Where((y, i) => xValueInterval.Contains(xValues[i])).ToList();
            var rangeYValue = createRangeYValue(yValuesForXInterval);
            if (rangeYValue.IsValid)
               series.Add(createRangeXValue(xValueInterval, yValuesForXInterval.Count), rangeYValue);
         }
      }

      private static RangeXValue createRangeXValue(BinInterval xValueInterval, int numberOfItems)
      {
         return new RangeXValue(xValueInterval.MeanValue.ToFloat())
         {
            Minimum = xValueInterval.Min.ToFloat(),
            Maximum = xValueInterval.Max.ToFloat(),
            NumberOfItems = numberOfItems
         };
      }

      private RangeYValue createRangeYValue(IEnumerable<float> valuesInInterval)
      {
         var sortedArray = valuesInInterval.OrderedAndPurified();
         return new RangeYValue
         {
            LowerPercentile = sortedArray.Percentile(BORDER_PERCENTILE),
            Median = sortedArray.Median(),
            UpperPercentile = sortedArray.Percentile(100 - BORDER_PERCENTILE),
         };
      }
   }
}