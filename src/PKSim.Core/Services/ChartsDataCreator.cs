using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Data;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public interface IChartDataCreator<TX, TY> where TX : IXValue where TY : IYValue
   {
      ChartData<TX, TY> CreateFor(PivotResult pivotResult);
      ChartData<TX, TY> CreateFor<TPopulationAnalysis>(PopulationAnalysisChart<TPopulationAnalysis> populationAnalysisChart, Aggregate aggregate) where TPopulationAnalysis : PopulationPivotAnalysis;
      ChartData<TX, TY> CreateFor<TPopulationAnalysis>(TPopulationAnalysis populationAnalysis, IPopulationDataCollector populationDataCollector, ObservedDataCollection observedDataCollection, Aggregate aggregate) where TPopulationAnalysis : PopulationPivotAnalysis;
      IEnumerable<PopulationPKAnalysis> Aggregate(IEnumerable<StatisticalAggregation> selectedStatistics, IReadOnlyList<Compound> compounds, IEnumerable<QuantityPKParameter> pks, Simulation simulation);
   }

   public abstract class ChartDataCreator<TX, TY> : IChartDataCreator<TX, TY> where TX : IXValue where TY : IYValue
   {
      protected readonly IDimensionRepository _dimensionRepository;
      private readonly IPivotResultCreator _pivotResultCreator;
      private const string DEFAULT_ID = "Default";
      protected string _aggregationName;
      protected PopulationPivotAnalysis _analysis;
      protected DataTable _data;
      private IPopulationDataCollector _populationDataCollector;
      protected string _dataColumnName;
      protected ObservedDataCollection _observedDataCollection;

      protected ChartDataCreator(IDimensionRepository dimensionRepository, IPivotResultCreator pivotResultCreator)
      {
         _dimensionRepository = dimensionRepository;
         _pivotResultCreator = pivotResultCreator;
      }

      /// <summary>
      ///    Returns true if the charts can be created based on the analysis, otherwise false
      /// </summary>
      protected abstract bool CheckFields();

      protected abstract ChartData<TX, TY> BuildChartsData();
      
      public virtual ChartData<TX, TY> CreateFor(PivotResult pivotResult)
      {
         try
         {
            _analysis = pivotResult.Analysis;
            _data = pivotResult.PivotedData;
            _aggregationName = pivotResult.AggregationName;
            _dataColumnName = pivotResult.DataColumnName;
            _populationDataCollector = pivotResult.PopulationDataCollector;
            _observedDataCollection = pivotResult.ObservedDataCollection;

            if (!CheckFields())
               return null;

            var chart = BuildChartsData();

            chart.CreatePaneOrder();
            chart.Panes.Each(x => x.CreateCurveOrder());

            return chart;
         }
         finally
         {
            _analysis = null;
            _data = null;
            _populationDataCollector = null;
            _observedDataCollection = null;
         }
      }

      public ChartData<TX, TY> CreateFor<TPopulationAnalysis>(PopulationAnalysisChart<TPopulationAnalysis> populationAnalysisChart, Aggregate aggregate) where TPopulationAnalysis : PopulationPivotAnalysis
      {
         return CreateFor(populationAnalysisChart.PopulationAnalysis, populationAnalysisChart.Analysable.DowncastTo<IPopulationDataCollector>(), populationAnalysisChart.ObservedDataCollection, aggregate);
      }

      public ChartData<TX, TY> CreateFor<TPopulationAnalysis>(TPopulationAnalysis populationAnalysis, IPopulationDataCollector populationDataCollector, ObservedDataCollection observedDataCollection, Aggregate aggregate) where TPopulationAnalysis : PopulationPivotAnalysis
      {
         return CreateFor(CreateResult(populationAnalysis, populationDataCollector, observedDataCollection, aggregate));
      }

      protected virtual PivotResult CreateResult<TPopulationAnalysis>(TPopulationAnalysis populationAnalysis, IPopulationDataCollector populationDataCollector, ObservedDataCollection observedDataCollection, Aggregate aggregate) where TPopulationAnalysis : PopulationPivotAnalysis
      {
         return _pivotResultCreator.Create(populationAnalysis, populationDataCollector, observedDataCollection, aggregate);
      }

      protected virtual IReadOnlyDictionary<string, string> GetFieldValues(IEnumerable<string> fieldNames, DataRow row)
      {
         var dictionary = new Dictionary<string, string>();
         fieldNames.Each(fieldName => dictionary.Add(fieldName, row[fieldName].ToString()));
         return dictionary;
      }

      protected void CreatePrimaryKey(IEnumerable<string> seriesFieldNames, IEnumerable<string> paneFieldNames)
      {
         // first entry in PrimaryKey is DataField
         var pkColumnNames = new List<string> {_dataColumnName};
         pkColumnNames.AddRange(seriesFieldNames);
         pkColumnNames.AddRange(paneFieldNames);
         _data.PrimaryKey = pkColumnNames.Select(name => _data.Columns[name]).ToArray();
      }

      protected ChartData<TX, TY> CreateChart(
         INumericValueField xAxisField,
         IReadOnlyList<IComparer<object>> paneFieldValueComparers,
         IReadOnlyList<string> xFieldNames = null,
         IComparer<TX> xValueComparer = null)
      {
         return new ChartData<TX, TY>(CreateAxisDataFor(xAxisField), paneFieldValueComparers, xFieldNames, xValueComparer);
      }

      protected CurveData<TX, TY> GetCurveData(
         DataRow row,
         IReadOnlyList<string> paneFieldNames,
         IReadOnlyList<IComparer<object>> paneFieldValueComparers,
         IReadOnlyList<string> seriesFieldNames,
         IReadOnlyList<IComparer<object>> curveFieldValueComparers,
         ChartData<TX, TY> chart,
         INumericValueField yAxisField)
      {
         var paneFieldValues = GetFieldValues(paneFieldNames, row);
         var pane = getOrCreatePane(chart, paneFieldValues, curveFieldValueComparers, yAxisField);

         var seriesFieldValues = GetFieldValues(seriesFieldNames, row);
         var series = getOrCreateCurve(pane, seriesFieldValues, yAxisField);

         var quantityField = yAxisField as IQuantityField;
         if (quantityField != null)
            series.QuantityPath = quantityField.QuantityPath;

         SetCurveSettings(series, row);
         return series;
      }

      protected virtual void SetCurveSettings(CurveData<TX, TY> series, DataRow row)
      {
         var colorField = _analysis.ColorField;
         var symbolField = _analysis.SymbolField;

         if (colorField != null)
         {
            var colorGrouping = colorField.GroupingByName(row.StringAt(colorField.Name));
            series.Color = colorGrouping.Color;
            series.Symbol = colorGrouping.Symbol;
         }

         if (symbolField != null)
         {
            var symbolGrouping = symbolField.GroupingByName(row.StringAt(symbolField.Name));
            series.Symbol = symbolGrouping.Symbol;
         }
      }

      /// <summary>
      ///    Returns the field of type <typeparamref name="T" /> defined in the analysis whose named can be found
      ///    in the column <c>PivotResult.DATA_FIELD_NAME</c> from the given <paramref name="row" />
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="row">Current row from which the field name should be taken</param>
      protected T DataField<T>(DataRow row) where T : class
      {
         var fieldName = row.StringAt(_dataColumnName);
         var field = _analysis.FieldByName(fieldName) as T;
         if (field == null)
            throw new PKSimException(PKSimConstants.Error.NoDataFieldFoundFor(fieldName));

         return field;
      }

      /// <summary>
      ///    Returns the <see cref="DataView" /> of the data containing all rows where the value in column
      ///    <c>PivotResult.DATA_FIELD_NAME</c> is equal to <paramref name="fieldName" />
      /// </summary>
      protected IEnumerable<DataRow> RowsForDataField(string fieldName)
      {
         return from DataRowView dataRowView in new DataView(_data, $"{_dataColumnName} = '{fieldName}'", string.Empty, DataViewRowState.CurrentRows)
            select dataRowView.Row;
      }

      private PaneData<TX, TY> getOrCreatePane(ChartData<TX, TY> chart, IReadOnlyDictionary<string, string> paneFieldValues, IReadOnlyList<IComparer<object>> curveFieldValueComparers, INumericValueField axisField)
      {
         string paneCaption = captionFor(paneFieldValues.Values);
         string paneId = idFromCaption(paneCaption); // Id cannot be empty string         
         var pane = chart.Panes.FindById(paneId);
         if (pane != null)
            return pane;

         // add new pane 
         pane = new PaneData<TX, TY>(CreateAxisDataFor(axisField), paneFieldValues, curveFieldValueComparers)
         {
            Id = paneId,
            Caption = paneCaption
         };

         chart.AddPane(pane);
         return pane;
      }

      private CurveData<TX, TY> getOrCreateCurve(PaneData<TX, TY> pane, IReadOnlyDictionary<string, string> seriesFieldValues, INumericValueField yAxisField)
      {
         string curveCaption = captionFor(seriesFieldValues.Values);
         string curveId = idFromCaption(curveCaption);
         var series = pane.Curves[curveId];
         if (series != null)
            return series;

         // add new curve 
         series = new CurveData<TX, TY>(seriesFieldValues)
         {
            Id = curveId,
            Caption = curveCaption,
            //Save one merge dimension per curve so that we can always convert using the dimension matching the output field exactly (only necessary when converting between molar and mass)
            YDimension = createMergedDimensionFor(yAxisField)
         };
         pane.AddCurve(series);
         return series;
      }

      protected virtual AxisData CreateAxisDataFor(INumericValueField numericValueField)
      {
         if (numericValueField == null)
            return new AxisData(_dimensionRepository.NoDimension, _dimensionRepository.NoDimension.DefaultUnit, Scalings.Linear);

         var mergedDimension = createMergedDimensionFor(numericValueField);
         return new AxisData(mergedDimension, numericValueField.DisplayUnit, numericValueField.Scaling)
         {
            Caption = Constants.NameWithUnitFor(numericValueField.Name, numericValueField.DisplayUnit)
         };
      }

      private IDimension createMergedDimensionFor(INumericValueField numericValueField)
      {
         return _dimensionRepository.MergedDimensionFor(new NumericFieldContext(numericValueField, _populationDataCollector));
      }

      private static string captionFor(IEnumerable<string> fieldValues)
      {
         return fieldValues.DefaultIfEmpty(string.Empty).ToString(Constants.DISPLAY_PATH_SEPARATOR);
      }

      private static string idFromCaption(string caption)
      {
         return string.IsNullOrEmpty(caption) ? DEFAULT_ID : caption;
      }

      protected IReadOnlyList<IComparer<object>> GetFieldComparers(PivotArea area)
      {
         return _analysis.AllFieldsOn(area);
      }

      public virtual IEnumerable<PopulationPKAnalysis> Aggregate(IEnumerable<StatisticalAggregation> selectedStatistics, IReadOnlyList<Compound> compounds, IEnumerable<QuantityPKParameter> pks, Simulation simulation)
      {
         return null;
      }
   }
}