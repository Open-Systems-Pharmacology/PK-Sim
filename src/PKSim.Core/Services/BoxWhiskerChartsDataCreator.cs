using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public interface IBoxWhiskerChartDataCreator : IChartDataCreator<BoxWhiskerXValue, BoxWhiskerYValue>
   {
   }

   public class BoxWhiskerChartDataCreator : ChartDataCreator<BoxWhiskerXValue, BoxWhiskerYValue>, IBoxWhiskerChartDataCreator
   {
      public BoxWhiskerChartDataCreator(IDimensionRepository dimensionRepository, IPivotResultCreator pivotResultCreator) : base(dimensionRepository, pivotResultCreator)
      {
      }

      protected override bool CheckFields()
      {
         var boxWhiskerAnalysis = _analysis as PopulationBoxWhiskerAnalysis;
         if (boxWhiskerAnalysis == null) return false;

         return _analysis.AllFieldsOn(PivotArea.DataArea).Any();
      }

      protected override ChartData<BoxWhiskerXValue, BoxWhiskerYValue> BuildChartsData()
      {
         //to enable proper order of xValues (column field value combinations)
         var boxWhiskerAnalysis = _analysis.DowncastTo<PopulationBoxWhiskerAnalysis>();

         var xValueComparer = new BoxWhiskerXValueComparer(_analysis.AllFieldsOn(PivotArea.RowArea));
         var paneFieldNames = new List<string> {_dataColumnName};
         var paneFieldComparers = new List<IComparer<object>> {_analysis};
         var seriesFieldComparers = GetFieldComparers(PivotArea.ColumnArea);
         var xFieldNames = _analysis.StringFieldNamesOn(PivotArea.RowArea);
         var seriesFieldNames = _analysis.StringFieldNamesOn(PivotArea.ColumnArea);

         var chart = CreateChart(null, paneFieldComparers, xFieldNames, xValueComparer);
         foreach (DataRow row in _data.Rows)
         {
            var yAxisField = DataField<PopulationAnalysisNumericField>(row);
            var series = GetCurveData(row, paneFieldNames, paneFieldComparers, seriesFieldNames, seriesFieldComparers, chart, yAxisField);

            setSeriesValues(series, row, xFieldNames, boxWhiskerAnalysis);
         }

         chart.CreateXOrder();
         return chart;
      }

      private void setSeriesValues(CurveData<BoxWhiskerXValue, BoxWhiskerYValue> series, DataRow row, IEnumerable<string> xFieldNames, PopulationBoxWhiskerAnalysis boxWhiskerAnalysis)
      {
         var xValue = new BoxWhiskerXValue(GetFieldValues(xFieldNames, row).Values);
         var yValue = row[_aggreationName].DowncastTo<BoxWhiskerYValue>();
         if (!yValue.IsValid) return;

         if (!boxWhiskerAnalysis.ShowOutliers)
            yValue.ClearOutliers();

         series.Add(xValue, yValue);
      }

      public override ChartData<BoxWhiskerXValue, BoxWhiskerYValue> CreateFor(PivotResult pivotResult)
      {
         var chartData = base.CreateFor(pivotResult);
         var boxWhiskerAnalysis = pivotResult.Analysis as PopulationBoxWhiskerAnalysis;
         if (boxWhiskerAnalysis == null || chartData == null)
            return chartData;

         chartData.Panes.Each(p => updateIndividualIdForField(pivotResult.PopulationDataCollector, boxWhiskerAnalysis, p));

         return chartData;
      }

      private void updateIndividualIdForField(IPopulationDataCollector populationDataCollector, PopulationBoxWhiskerAnalysis boxWhiskerAnalysis, PaneData<BoxWhiskerXValue, BoxWhiskerYValue> dataPane)
      {
         var field = boxWhiskerAnalysis.FieldByName(dataPane.Id) as PopulationAnalysisNumericField;
         if (field == null)
            return;

         var fieldValues = field.GetValues(populationDataCollector).Where(x => x.IsValid()).ToFloatArray();
         var sortedValues = fieldValues.OrderBy(x => x).ToList();

         dataPane.Curves.SelectMany(x => x.YValues).Each(y => updateIndividualId(y, fieldValues, sortedValues));
      }

      private void updateIndividualId(BoxWhiskerYValue boxWhiskerYValue, float[] fieldValues, List<float> sortedValues)
      {
         boxWhiskerYValue.AllValues.Each(v =>
         {
            int index = sortedValues.BinarySearch(v);
            v.IndividualId = Array.IndexOf(fieldValues, index >= 0 ? v : sortedValues[~index]);
         });
      }
   }

   internal class BoxWhiskerXValueComparer : IComparer<BoxWhiskerXValue>
   {
      private readonly IReadOnlyList<IPopulationAnalysisField> _columnFields;

      public BoxWhiskerXValueComparer(IReadOnlyList<IPopulationAnalysisField> columnFields)
      {
         _columnFields = columnFields;
      }

      public int Compare(BoxWhiskerXValue xValue1, BoxWhiskerXValue xValue2)
      {
         if (!_columnFields.Any())
            return 0;

         if (xValue1.Count != _columnFields.Count)
            throw new ArgumentException(PKSimConstants.Error.InconsistentXValuesLength(xValue1.Count, _columnFields.Count));

         if (xValue2.Count != _columnFields.Count)
            throw new ArgumentException(PKSimConstants.Error.InconsistentXValuesLength(xValue2.Count, _columnFields.Count));

         int i;
         for (i = 0; i < _columnFields.Count - 1; i++)
         {
            int result = _columnFields[i].Compare(xValue1[i], xValue2[i]);
            if (result != 0)
               return result;
         }

         return _columnFields[i].Compare(xValue1[i], xValue2[i]);
      }
   }
}