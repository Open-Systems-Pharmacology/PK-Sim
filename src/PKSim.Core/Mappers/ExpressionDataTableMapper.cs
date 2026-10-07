using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using static PKSim.Core.Model.ExpressionDataFields;

namespace PKSim.Core.Mappers
{
   /// <summary>
   ///    Converts expression data records and tissue to container mappings from and to the data tables of the gene
   ///    expression database queries
   /// </summary>
   public interface IExpressionDataTableMapper
   {
      IReadOnlyList<ExpressionDataRecord> RecordsFrom(DataTable expressionDataTable);
      IReadOnlyList<TissueContainerMapping> MappingFrom(DataTable mappingTable);
      DataTable MappingTableFrom(IEnumerable<TissueContainerMapping> mapping);

      /// <summary>
      ///    Returns a table with a row for each record assigned to a container, with a column for each record field and for the
      ///    name and display name of the container
      /// </summary>
      DataTable DataTableFrom(IEnumerable<ContainerExpressionDataRecord> containerRecords);

      /// <summary>
      ///    Returns the data set saved with an expression query, containing the records and the mapping of the query
      /// </summary>
      DataSet DataSetFrom(ExpressionQuery query);
   }

   public class ExpressionDataTableMapper : IExpressionDataTableMapper
   {
      public const string EXPRESSION_DATA = "ExpressionData";
      public const string MAPPING_DATA = "MappingData";
      private const string REL_TISSUE = "REL_TISSUE";

      private static readonly string[] _textColumns = {VARIANT_NAME, DATA_BASE, DATA_BASE_REC_ID, GENDER, TISSUE, HEALTH_STATE, SAMPLE_SOURCE};
      private static readonly string[] _numericColumns = {AGE_MIN, AGE_MAX, SAMPLE_COUNT, TOTAL_COUNT, RATIO, NORM_VALUE};

      public IReadOnlyList<ExpressionDataRecord> RecordsFrom(DataTable expressionDataTable) => expressionDataTable.Rows.Cast<DataRow>().Select(recordFrom).ToList();

      public IReadOnlyList<TissueContainerMapping> MappingFrom(DataTable mappingTable) =>
         mappingTable.Rows.Cast<DataRow>().Select(x => new TissueContainerMapping(textFrom(x, TISSUE), textFrom(x, CONTAINER))).ToList();

      public DataTable MappingTableFrom(IEnumerable<TissueContainerMapping> mapping)
      {
         var mappingTable = new DataTable(MAPPING_DATA);
         mappingTable.Columns.Add(CONTAINER, typeof(string));
         mappingTable.Columns.Add(TISSUE, typeof(string));
         mapping.Each(x => mappingTable.Rows.Add(valueOrNull(x.Container), valueOrNull(x.Tissue)));
         mappingTable.AcceptChanges();
         return mappingTable;
      }

      public DataTable DataTableFrom(IEnumerable<ContainerExpressionDataRecord> containerRecords)
      {
         var dataTable = createExpressionDataTable();
         dataTable.Columns.Add(CONTAINER, typeof(string));
         dataTable.Columns.Add(CONTAINER_DISPLAY_NAME, typeof(string));
         containerRecords.Each(x => dataTable.Rows.Add(valuesFor(x.Record).Concat(new object[] {x.Container.ContainerName, x.Container.ContainerDisplayName}).ToArray()));
         return dataTable;
      }

      public DataSet DataSetFrom(ExpressionQuery query)
      {
         var expressionDataTable = createExpressionDataTable();
         query.Records.Each(x => expressionDataTable.Rows.Add(valuesFor(x).ToArray()));
         var mappingTable = MappingTableFrom(query.Mapping);

         var dataSet = new DataSet(EXPRESSION_DATA);
         dataSet.Tables.Add(expressionDataTable);
         dataSet.Tables.Add(mappingTable);
         dataSet.Relations.Add(REL_TISSUE, expressionDataTable.Columns[TISSUE], mappingTable.Columns[TISSUE], false);
         return dataSet;
      }

      private static DataTable createExpressionDataTable()
      {
         var dataTable = new DataTable(EXPRESSION_DATA);
         _textColumns.Each(x => dataTable.Columns.Add(x, typeof(string)));
         _numericColumns.Each(x => dataTable.Columns.Add(x, typeof(double)));
         dataTable.Columns.Add(UNIT, typeof(string));
         return dataTable;
      }

      private static IEnumerable<object> valuesFor(ExpressionDataRecord record) => new[]
      {
         valueOrNull(record.VariantName),
         valueOrNull(record.Database),
         valueOrNull(record.DatabaseRecId),
         valueOrNull(record.Gender),
         valueOrNull(record.Tissue),
         valueOrNull(record.HealthState),
         valueOrNull(record.SampleSource),
         valueOrNull(record.AgeMin),
         valueOrNull(record.AgeMax),
         valueOrNull(record.SampleCount),
         valueOrNull(record.TotalCount),
         valueOrNull(record.Ratio),
         valueOrNull(record.NormValue),
         valueOrNull(record.Unit)
      };

      private static ExpressionDataRecord recordFrom(DataRow row) => new ExpressionDataRecord
      {
         VariantName = textFrom(row, VARIANT_NAME),
         Database = textFrom(row, DATA_BASE),
         DatabaseRecId = textFrom(row, DATA_BASE_REC_ID),
         Gender = textFrom(row, GENDER),
         Tissue = textFrom(row, TISSUE),
         HealthState = textFrom(row, HEALTH_STATE),
         SampleSource = textFrom(row, SAMPLE_SOURCE),
         AgeMin = numberFrom(row, AGE_MIN),
         AgeMax = numberFrom(row, AGE_MAX),
         SampleCount = numberFrom(row, SAMPLE_COUNT),
         TotalCount = numberFrom(row, TOTAL_COUNT),
         Ratio = numberFrom(row, RATIO),
         NormValue = numberFrom(row, NORM_VALUE),
         Unit = textFrom(row, UNIT)
      };

      private static object valueOrNull(object value) => value ?? DBNull.Value;

      private static string textFrom(DataRow row, string column) => row.IsNull(column) ? null : row[column].ToString();

      private static double? numberFrom(DataRow row, string column) => row.IsNull(column) ? null : Convert.ToDouble(row[column]);
   }
}
