using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Chart;
using PKSim.Core.Mappers;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Utility.Extensions;

namespace PKSim.Presentation.Mappers
{
   public class DataRepositoryToObservedCurveDataMapper : IDataRepositoryToObservedCurveDataMapper
   {
      private readonly IQuantityPathToQuantityDisplayPathMapper _displayPathMapper;
      private readonly IDimensionRepository _dimensionRepository;

      public DataRepositoryToObservedCurveDataMapper(IQuantityPathToQuantityDisplayPathMapper displayPathMapper, IDimensionRepository dimensionRepository)
      {
         _displayPathMapper = displayPathMapper;
         _dimensionRepository = dimensionRepository;
      }

      public IReadOnlyList<ObservedCurveData> MapFrom(DataRepository observedData, ObservedDataCollection observedDataCollection, IDimension yAxisDimension)
      {
         var observedCurveData = new List<ObservedCurveData>();
         observedData.ObservationColumns().Each(col => observedCurveData.Add(mapFrom(col, observedDataCollection.ObservedDataCurveOptionsFor(col), yAxisDimension)));
         return observedCurveData;
      }

      private ObservedCurveData mapFrom(DataColumn observedDataColumn, ObservedDataCurveOptions observedDataCurveOptions, IDimension yAxisDimension)
      {
         var observedCurve = createCurveData(observedDataColumn, observedDataCurveOptions);
         var errorColumn = errorColumnFor(observedDataColumn);
         var baseGrid = observedDataColumn.BaseGrid;
         var yDimension = _dimensionRepository.MergedDimensionFor(observedDataColumn);
         var yErrorDimension = yDimension;

         if (observedCurve.ErrorType == AuxiliaryType.GeometricStdDev)
            yErrorDimension = errorColumn.Dimension;

         baseGrid.Values.Each((value, i) =>
         {
            var observedDataYValue = new ObservedDataYValue
            {
               Mean = convertToAxisBaseValue(yAxisDimension, yDimension, observedDataColumn[i]),
               ErrorType = observedCurve.ErrorType,
               Error = convertToAxisBaseValue(yAxisDimension, yErrorDimension, errorColumn[i])
            };
            observedCurve.Add(new TimeProfileXValue(value), observedDataYValue);
         });

         return observedCurve;
      }

      private static float convertToAxisBaseValue(IDimension yAxisDimension, IDimension columnDimension, float valueInColumnBaseUnit)
      {
         if (!columnDimension.HasUnit(yAxisDimension.BaseUnit.Name))
            return valueInColumnBaseUnit;

         return columnDimension.BaseUnitValueToUnitValue(yAxisDimension.BaseUnit, valueInColumnBaseUnit).ToFloat();
      }

      private ObservedCurveData createCurveData(DataColumn observedDataColumn, ObservedDataCurveOptions observedDataCurveOptions)
      {
         if (string.IsNullOrEmpty(observedDataCurveOptions.Caption))
            //use null here as observed data do not belong to any simulation
            observedDataCurveOptions.Caption = _displayPathMapper.DisplayPathAsStringFor(null, observedDataColumn);

         var observedCurve = new ObservedCurveData(errorTypeFrom(observedDataColumn))
         {
            Id = observedDataColumn.Id,
            Caption = observedDataCurveOptions.Caption,
         };

         updateCurveOptions(observedDataCurveOptions.CurveOptions, observedCurve);

         return observedCurve;
      }

      private DataColumn errorColumnFor(DataColumn observedDataColumn)
      {
         var errorType = errorTypeFrom(observedDataColumn);
         if (errorType != AuxiliaryType.Undefined)
            return observedDataColumn.GetRelatedColumn(errorType);

         //Dummy Error column for confident programming
         return new DataColumn("Error", Constants.Dimension.NO_DIMENSION, observedDataColumn.BaseGrid)
         {
            Values = new float[observedDataColumn.Values.Count].InitializeWith(float.NaN)
         };
      }

      private static void updateCurveOptions(CurveOptions curveOptions, ObservedCurveData observedCurve)
      {
         observedCurve.Color = curveOptions.Color;
         observedCurve.LineStyle = curveOptions.LineStyle;
         observedCurve.Symbol = curveOptions.Symbol;
         observedCurve.Visible = curveOptions.Visible;
      }

      private AuxiliaryType errorTypeFrom(DataColumn column)
      {
         if (!column.RelatedColumns.Any())
            return AuxiliaryType.Undefined;

         return column.RelatedColumns.ElementAt(0).DataInfo.AuxiliaryType;
      }
   }
}