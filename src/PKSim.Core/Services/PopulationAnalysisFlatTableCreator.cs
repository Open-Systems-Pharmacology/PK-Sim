using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Services
{
   public interface IPopulationAnalysisFlatTableCreator
   {
      /// <summary>
      ///    Creates the data that will be displayed in the pivot presenter based on the given
      ///    <paramref name="populationAnalysisFields" /> and the <paramref name="populationDataCollector" />
      /// </summary>
      DataTable Create(IPopulationDataCollector populationDataCollector, IReadOnlyCollection<IPopulationAnalysisField> populationAnalysisFields);

      /// <summary>
      ///    Creates the data that will be displayed in the pivot presenter using the fields defined in the given
      ///    <paramref name="populationAnalysis" /> and the <paramref name="populationDataCollector" />
      /// </summary>
      DataTable Create(IPopulationDataCollector populationDataCollector, PopulationAnalysis populationAnalysis);
   }

   public class PopulationAnalysisFlatTableCreator : IPopulationAnalysisFlatTableCreator
   {
      private readonly Cache<string, IReadOnlyList<object>> _allDataFields = new Cache<string, IReadOnlyList<object>>();
      private readonly Dictionary<string, string> _allCovariateFields = new Dictionary<string, string>();
      private DataTable _dataTable;
      private IPopulationDataCollector _populationDataCollector;
      private string _simulationNameColumnName;

      public DataTable Create(IPopulationDataCollector populationDataCollector, IReadOnlyCollection<IPopulationAnalysisField> populationAnalysisFields)
      {
         _dataTable = new DataTable();
         _populationDataCollector = populationDataCollector;
         try
         {
            //create one column per numeric data field
            numericDataFieldsFrom(populationAnalysisFields).Each(addDataField);

            //Special handling for covariate fields
            covariateFieldsFrom(populationAnalysisFields).Each(addCovariateField);

            //add simulation name to support reference simulation
            _simulationNameColumnName = addReferenceSimulationNameColumn();

            //Fill data fields 
            _dataTable.BeginLoadData();
            for (int i = 0; i < populationDataCollector.NumberOfItems; i++)
            {
               var row = _dataTable.NewRow();
               foreach (var fieldValues in _allDataFields.KeyValues)
               {
                  row[fieldValues.Key] = fieldValues.Value[i];
               }
               _dataTable.Rows.Add(row);
            }
            _dataTable.EndLoadData();

            //add an expression field for each covariate field for reference simulation handling
            foreach (var covariateField in _allCovariateFields)
            {
               var column = createExpressionFieldFor(covariateField);
               _dataTable.Columns.Add(column);
               addReferenceGroupingItemTo(covariateField.Key, populationAnalysisFields);
            }

            //add derived fields. This needs to be done AFTER the data fields are filled
            derivedFieldsFrom(populationAnalysisFields).Each(addDerivedField);

            return _dataTable;
         }
         finally
         {
            _allDataFields.Clear();
            _allCovariateFields.Clear();
            _dataTable = null;
            _populationDataCollector = null;
            _simulationNameColumnName = string.Empty;
         }
      }

      private bool hasReferenceSimulation
      {
         get
         {
            var populationComparison = _populationDataCollector as PopulationSimulationComparison;
            return populationComparison != null && populationComparison.HasReference;
         }
      }

      private PopulationSimulationComparison comparison
      {
         get { return _populationDataCollector.DowncastTo<PopulationSimulationComparison>(); }
      }

      private IEnumerable<PopulationAnalysisDataField> numericDataFieldsFrom(IEnumerable<IPopulationAnalysisField> populationAnalysisFields)
      {
         return fieldsFrom<PopulationAnalysisDataField>(populationAnalysisFields, x => !x.IsAnImplementationOf<PopulationAnalysisCovariateField>());
      }

      private IEnumerable<PopulationAnalysisDerivedField> derivedFieldsFrom(IEnumerable<IPopulationAnalysisField> populationAnalysisFields)
      {
         return fieldsFrom<PopulationAnalysisDerivedField>(populationAnalysisFields);
      }

      private IEnumerable<PopulationAnalysisCovariateField> covariateFieldsFrom(IEnumerable<IPopulationAnalysisField> populationAnalysisFields)
      {
         return fieldsFrom<PopulationAnalysisCovariateField>(populationAnalysisFields);
      }

      private void addReferenceGroupingItemTo(string covariateFieldName, IEnumerable<IPopulationAnalysisField> populationAnalysisFields)
      {
         if (!hasReferenceSimulation) return;

         var field = populationAnalysisFields.FindByName(covariateFieldName).DowncastTo<PopulationAnalysisCovariateField>();
         field.ReferenceGroupingItem = comparison.ReferenceGroupingItem;
      }

      private DataColumn createExpressionFieldFor(KeyValuePair<string, string> covariateField)
      {
         if (!hasReferenceSimulation)
            return null;

         return new DataColumn(covariateField.Key, typeof (string))
         {
            Expression = getReferenceExpression(
               comparison.ReferenceSimulation.Name,
               comparison.ReferenceGroupingItem.Label,
               $"[{covariateField.Value}]")
         };
      }

      private void addCovariateField(PopulationAnalysisCovariateField covariateField)
      {
         if (!hasReferenceSimulation)
            addDataField(covariateField);
         else
         {
            var covariateFieldInternalName = ShortGuid.NewGuid().ToString();
            _allCovariateFields.Add(covariateField.Name, covariateFieldInternalName);
            addDataField(covariateField, covariateFieldInternalName);
         }
      }

      private void addDataField(PopulationAnalysisDataField dataField)
      {
         addDataField(dataField, dataField.Name);
      }

      private void addDataField(PopulationAnalysisDataField dataField, string fieldName)
      {
         addDataField(dataField.GetValuesAsObjects(_populationDataCollector), dataField.DataType, fieldName);
      }

      private void addDataField(IReadOnlyList<object> values, Type dataType, string fieldName)
      {
         _dataTable.AddColumn(fieldName, dataType);
         _allDataFields.Add(fieldName, values);
      }

      private void addDerivedField(PopulationAnalysisDerivedField derivedField)
      {
         derivedField.UpdateExpression(_populationDataCollector);
         var column = _dataTable.AddColumn(derivedField.Name, derivedField.DataType);
         column.Expression = derivedField.Expression;
         updateColumnsForReferenceSimulation(column, derivedField);
      }

      private void updateColumnsForReferenceSimulation(DataColumn column, PopulationAnalysisDerivedField derivedField)
      {
         if (!hasReferenceSimulation) return;
         //make modifications to support reference simulation
         updateExpressionWithSimulationName(column, comparison.ReferenceSimulation, comparison.ReferenceGroupingItem);
         storeReferenceGroupingItemOn(derivedField, comparison.ReferenceGroupingItem);
      }

      private string addReferenceSimulationNameColumn()
      {
         if (!hasReferenceSimulation)
            return string.Empty;

         var columnName = ShortGuid.NewGuid().ToString();
         addDataField(comparison.AllSimulationNames, typeof (string), columnName);

         return columnName;
      }

      private static void storeReferenceGroupingItemOn(PopulationAnalysisDerivedField derivedField, GroupingItem referenceGroupingItem)
      {
         var groupingField = derivedField as PopulationAnalysisGroupingField;
         if (groupingField == null) return;

         groupingField.ReferenceGroupingItem = referenceGroupingItem;
      }

      private string getReferenceExpression(string referenceSimulationName, string referenceLabel, string elseCase)
      {
         return $"iif([{_simulationNameColumnName}]='{referenceSimulationName}', '{referenceLabel}', {elseCase})";
      }

      private void updateExpressionWithSimulationName(DataColumn column, PopulationSimulation referenceSimulation, GroupingItem referenceGroupingItem)
      {
         column.Expression = getReferenceExpression(
            referenceSimulation.Name,
            referenceGroupingItem.Label,
            column.Expression);
      }

      public DataTable Create(IPopulationDataCollector populationDataCollector, PopulationAnalysis populationAnalysis)
      {
         return Create(populationDataCollector, populationAnalysis.AllFields);
      }

      private IEnumerable<T> fieldsFrom<T>(IEnumerable<IPopulationAnalysisField> populationAnalysisFields) where T : class, IPopulationAnalysisField
      {
         return fieldsFrom<T>(populationAnalysisFields, x => true);
      }

      private IEnumerable<T> fieldsFrom<T>(IEnumerable<IPopulationAnalysisField> populationAnalysisFields, Func<T, bool> query) where T : class, IPopulationAnalysisField
      {
         return from f in populationAnalysisFields
            let cast = f as T
            where cast != null
            where query(cast)
            select cast;
      }
   }
}