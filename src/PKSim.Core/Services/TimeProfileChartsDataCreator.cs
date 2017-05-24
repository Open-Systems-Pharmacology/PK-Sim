﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Data;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Chart;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public interface ITimeProfileChartDataCreator : IChartDataCreator<TimeProfileXValue, TimeProfileYValue>
   {
   }

   public class TimeProfileChartDataCreator : ChartDataCreator<TimeProfileXValue, TimeProfileYValue>, ITimeProfileChartDataCreator
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IStatisticalDataCalculator _statisticalDataCalculator;
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IDataRepositoryToObservedCurveDataMapper _observedCurveDataMapper;
      private const string STATISTICAL_AGGREGATION = "STATISTICAL_AGGREGATION";
      private const string STATISTICAL_AGGREGATION_DISPLAY_NAME = "STATISTICAL_AGGREGATION_DISPLAY_NAME";
      private const string TIME_AND_VALUES = "TIME_AND_VALUES";

      public TimeProfileChartDataCreator(IDimensionRepository dimensionRepository, IPivotResultCreator pivotResultCreator, IRepresentationInfoRepository representationInfoRepository,
         IStatisticalDataCalculator statisticalDataCalculator, ILazyLoadTask lazyLoadTask, IDataRepositoryToObservedCurveDataMapper observedCurveDataMapper)
         : base(dimensionRepository, pivotResultCreator)
      {
         _representationInfoRepository = representationInfoRepository;
         _statisticalDataCalculator = statisticalDataCalculator;
         _lazyLoadTask = lazyLoadTask;
         _observedCurveDataMapper = observedCurveDataMapper;
      }

      protected override bool CheckFields()
      {
         var statisticalAnalysis = _analysis as PopulationStatisticalAnalysis;
         if (statisticalAnalysis == null) return false;

         return _analysis.AllFieldsOn(PivotArea.DataArea).Any() && statisticalAnalysis.SelectedStatistics.Any();
      }

      protected override ChartData<TimeProfileXValue, TimeProfileYValue> BuildChartsData()
      {
         var statisticalAnalysis = _analysis.DowncastTo<PopulationStatisticalAnalysis>();
         var paneFieldNames = _analysis.StringFieldNamesOn(PivotArea.RowArea);
         var paneFieldComparers = GetFieldComparers(PivotArea.RowArea);
         var seriesFieldNames = new List<string> {_dataColumnName, STATISTICAL_AGGREGATION_DISPLAY_NAME};
         if (_analysis.ColorField != null)
            seriesFieldNames.Add(_analysis.ColorField.Name);

         var seriesFieldComparers = GetFieldComparers(PivotArea.ColumnArea);
         var timeField = new TimeField {Dimension = _dimensionRepository.Time, DisplayUnit = statisticalAnalysis.TimeUnit};

         _data.AddColumn<StatisticalAggregation>(STATISTICAL_AGGREGATION);
         _data.AddColumn<string>(STATISTICAL_AGGREGATION_DISPLAY_NAME);
         _data.AddColumn<Tuple<QuantityValues, FloatMatrix>>(TIME_AND_VALUES);

         // Create rows for each combination of row<=>DATA_FIELD value and selected STATISTICAL_AGGREGATION
         var dataCopy = _data.Copy();
         _data.Clear();

         foreach (DataRow row in dataCopy.Rows)
         {
            var timeAndAllValues = getTimeAndAllValuesFor(row); // calculate once because expensive

            foreach (var statisticalAggregation in statisticalAnalysis.SelectedStatistics)
            {
               row[STATISTICAL_AGGREGATION] = statisticalAggregation;
               row[STATISTICAL_AGGREGATION_DISPLAY_NAME] = _representationInfoRepository.DisplayNameFor(statisticalAggregation);
               row[TIME_AND_VALUES] = timeAndAllValues;
               _data.ImportRow(row);
            }
         }

         var chart = CreateChart(timeField, paneFieldComparers);
         // Create series for each row (combination of DATA_FIELD value and selected STATISTICAL_AGGREGATION)
         foreach (DataRow row in _data.Rows)
         {
            var yAxisField = DataField<PopulationAnalysisOutputField>(row);
            var series = GetCurveData(row, paneFieldNames, paneFieldComparers, seriesFieldNames, seriesFieldComparers, chart, yAxisField);
            setSeriesValues(series, row);
         }

         //add observed data if availables
         addObservedDataToChart(chart);
         return chart;
      }

      private void addObservedDataToChart(ChartData<TimeProfileXValue, TimeProfileYValue> chart)
      {
         foreach (var observedData in _observedDataCollection)
         {
            foreach (var pane in chart.Panes)
            {
               if (_observedDataCollection.ApplyGroupingToObservedData)
               {
                  //Do not add to pane if the observed data meta data does not math to the pane grouping
                  if (pane.FieldNames.Any(f => !shouldDisplayObservedDataInPane(observedData, f, pane)))
                     continue;
               }

               var observedCurveData = _observedCurveDataMapper.MapFrom(observedData, _observedDataCollection, pane.Axis.Dimension);
               observedCurveData.Each(pane.AddObservedCurve);

               if (!_observedDataCollection.ApplyGroupingToObservedData)
                  continue;

               //switch the color for color grouping
               updateObservedDataColorBasedOnColorGrouping(observedData, observedCurveData);
            }
         }
      }

      private void updateObservedDataColorBasedOnColorGrouping(DataRepository observedData, IEnumerable<ObservedCurveData> observedCurveData)
      {
         if (_analysis.ColorField == null)
            return;

         var groupingValue = observedData.ExtendedPropertyValueFor(_analysis.ColorField.Name);
         if (groupingValue == null)
            return;

         var grouping = _analysis.ColorField.GroupingByName(groupingValue);
         if (grouping != null)
            observedCurveData.Each(c => c.Color = grouping.Color);
      }

      private static bool shouldDisplayObservedDataInPane(DataRepository observedData, string field, PaneData<TimeProfileXValue, TimeProfileYValue> pane)
      {
         if (!observedData.ExtendedProperties.Contains(field))
            return true;

         var extendedPropertyValue = observedData.ExtendedPropertyValueFor(field);
         var paneFieldValue = pane.FieldKeyValues[field];
         return string.Equals(extendedPropertyValue, paneFieldValue, StringComparison.InvariantCultureIgnoreCase);
      }

      private void setSeriesValues(CurveData<TimeProfileXValue, TimeProfileYValue> series, DataRow row)
      {
         var statisticalAggregation = row[STATISTICAL_AGGREGATION].DowncastTo<StatisticalAggregation>();

         var timeAndAllValues = (Tuple<QuantityValues, FloatMatrix>) row[TIME_AND_VALUES];

         QuantityValues time = timeAndAllValues.Item1;
         FloatMatrix allValues = timeAndAllValues.Item2;

         var results = getResultsFor(statisticalAggregation, allValues);
         for (int i = 0; i < time.Length; i++)
         {
            var yValue = results[i];
            if (yValue.IsValid)
               series.Add(new TimeProfileXValue(time[i]), yValue);
         }
      }

      protected override void SetCurveSettings(CurveData<TimeProfileXValue, TimeProfileYValue> series, DataRow row)
      {
         var outputField = _analysis.FieldByName(row[_dataColumnName].ToString()).DowncastTo<PopulationAnalysisOutputField>();
         series.Color = outputField.Color;

         var statisticalAggregation = row[STATISTICAL_AGGREGATION].DowncastTo<StatisticalAggregation>();
         series.LineStyle = statisticalAggregation.LineStyle;

         var colorField = _analysis.ColorField;

         var groupitem = colorField?.GroupingByName(row[colorField.Name].ToString());
         if (groupitem == null)
            return;

         series.Color = groupitem.Color;
      }

      protected override AxisData CreateAxisDataFor(INumericValueField axisField)
      {
         //override default implementation to set caption to [display unit], because different DATA_FIELDS are shown at one pane
         var axis = base.CreateAxisDataFor(axisField);
         if (axis != null)
            axis.Caption = Constants.NameWithUnitFor(_dimensionRepository.MergedDimensionFor(axisField).DisplayName, axisField.DisplayUnit);
         return axis;
      }

      private Tuple<QuantityValues, FloatMatrix> getTimeAndAllValuesFor(DataRow row)
      {
         var allValuesForQuantity = row[_aggreationName] as IReadOnlyList<QuantityValues>;

         if (allValuesForQuantity == null || allValuesForQuantity.Count == 0)
            return emptyTimeAndValues();

         var firstQuantityValue = allValuesForQuantity.First();
         if (firstQuantityValue.IsNull() || firstQuantityValue.Time.IsNull())
            return emptyTimeAndValues();

         var matrix = new FloatMatrix();
         var time = firstQuantityValue.Time;
         var length = time.Length;
         for (int i = 0; i < length; i++)
         {
            //values might have different lenght if using different time arrays. We base our arrays on the first time values
            var values = allValuesForQuantity.Where(qv => i < qv.Length)
               .Select(qv => qv.ValueAt(i));

            matrix.AddValuesAndSort(values);
         }

         return new Tuple<QuantityValues, FloatMatrix>(time, matrix);
      }

      private static Tuple<QuantityValues, FloatMatrix> emptyTimeAndValues()
      {
         return new Tuple<QuantityValues, FloatMatrix>(new QuantityValues(), new FloatMatrix());
      }

      protected override PivotResult CreateResult<TPopulationAnalysis>(TPopulationAnalysis populationAnalysis, IPopulationDataCollector populationDataCollector, ObservedDataCollection observedDataCollection, Aggregate aggregate)
      {
         _lazyLoadTask.LoadResults(populationDataCollector);
         return base.CreateResult(populationAnalysis, populationDataCollector, observedDataCollection, aggregate);
      }

      private class TimeField : PopulationAnalysisFieldBase, INumericValueField
      {
         public IDimension Dimension { get; set; }
         public Unit DisplayUnit { get; set; }
         public Scalings Scaling { get; set; }

         public TimeField() : base(typeof(double))
         {
            Scaling = Scalings.Linear;
            Name = PKSimConstants.UI.Time;
         }

         public override string Id => string.Empty;
      }

      private IReadOnlyList<TimeProfileYValue> getResultsFor(StatisticalAggregation statisticalAggregation, FloatMatrix quantityResults)
      {
         var curveValues = _statisticalDataCalculator.StatisticalDataFor(quantityResults, statisticalAggregation).ToList();
         //line curve
         if (curveValues.Count == 1)
            return timeProfilePointValues(curveValues[0]);

         //range
         if (curveValues.Count == 2)
            return timeProfileRangeValues(curveValues[0], curveValues[1]);

         return new List<TimeProfileYValue>();
      }

      private IReadOnlyList<TimeProfileYValue> timeProfilePointValues(IEnumerable<float> values)
      {
         return values.Select(value => new TimeProfileYValue {Y = value}).ToList();
      }

      private IReadOnlyList<TimeProfileYValue> timeProfileRangeValues(IReadOnlyList<float> lowerValues, IReadOnlyList<float> upperValues)
      {
         return lowerValues.Select((value, i) => new TimeProfileYValue
         {
            LowerValue = lowerValues[i],
            UpperValue = upperValues[i]
         }).ToList();
      }
   }
}