using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Chart;
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
         var paneFieldNames = new List<string> { _dataColumnName };
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
   }

   internal class BoxWhiskerXValueComparer : IComparer<BoxWhiskerXValue>
   {
      private readonly IReadOnlyList<IPopulationAnalysisField> _columnFields;

      public BoxWhiskerXValueComparer(IReadOnlyList<IPopulationAnalysisField> columnFields)
      {
         _columnFields = columnFields;
      }

      public int Compare(BoxWhiskerXValue arg1, BoxWhiskerXValue arg2)
      {
         if (!_columnFields.Any())
            return 0;

         if (arg1.Count != _columnFields.Count)
            throw new ArgumentException(PKSimConstants.Error.InconsistentXValuesLength(arg1.Count, _columnFields.Count));

         if (arg2.Count != _columnFields.Count)
            throw new ArgumentException(PKSimConstants.Error.InconsistentXValuesLength(arg2.Count, _columnFields.Count));

         int i;
         for (i = 0; i < _columnFields.Count - 1; i++)
         {
            int result = _columnFields[i].Compare(arg1[i], arg2[i]);
            if (result != 0)
               return result;
         }

         return _columnFields[i].Compare(arg1[i], arg2[i]);
      }
   }
}