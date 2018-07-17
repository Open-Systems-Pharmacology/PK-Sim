using System;
using System.Data;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.DAS;

namespace PKSim.Infrastructure.Services
{
   public class GeneExpressionQueries : IGeneExpressionQueries
   {
      private readonly DAS _databaseObject;

      //Queries
      private const string QRY_CONTAINER_TISSUE = "QRY_CONTAINER_TISSUE";
      private const string QRY_GENDER_HINT = "QRY_GENDER_HINT";
      private const string QRY_TISSUE_HINT = "QRY_TISSUE_HINT";
      private const string QRY_HEALTH_STATE_HINT = "QRY_HEALTH_STATE_HINT";
      private const string QRY_SAMPLE_SOURCE_HINT = "QRY_SAMPLE_SOURCE_HINT";
      private const string QRY_UNIT_HINT = "QRY_UNIT_HINT";
      private const string QRY_NAME_TYPE_HINT = "QRY_NAME_TYPE_HINT";
      private const string QRY_DATABASE_REC_PROPERTIES_BY_ID = "QRY_DATABASE_REC_PROPERTIES_BY_ID";
      private const string QRY_DATABASE_REC_INFO_BY_ID = "QRY_DATABASE_REC_INFO_BY_ID";

      //Columns

      private const string QRY_CONTAINER_TISSUE_COLUMNS = "CONTAINER, TISSUE";
      private const string QRY_DATABASE_REC_PROPERTIES_COLUMNS = "DATA_BASE, DATA_BASE_REC_ID, PROPERTY, PROPERTY_VALUE";

      private const string QRY_DATABASE_REC_INFO_COLUMNS = "DATA_BASE, DATA_BASE_REC_ID, LAST_REFRESH_DATE, DB_URL";
      private const string QRY_GENDER_HINT_COLUMNS = "GENDER, INFORMATION";
      private const string QRY_TISSUE_HINT_COLUMNS = "TISSUE, INFORMATION";
      private const string QRY_HEALTH_STATE_HINT_COLUMNS = "HEALTH_STATE, INFORMATION";
      private const string QRY_SAMPLE_SOURCE_HINT_COLUMNS = "SAMPLE_SOURCE, INFORMATION";
      private const string QRY_UNIT_HINT_COLUMNS = "UNIT, INFORMATION";
      private const string QRY_NAME_TYPE_HINT_COLUMNS = "NAME_TYPE, INFORMATION";

      //ParameterNames
      private const string P_ID = "P_ID";
      private const string P_NAME = "P_NAME";
      private const string P_DATABASE = "P_DATABASE";
      private const string P_REC_ID = "P_REC_ID";

      //cached queries
      private DataTable _mappingContainerTissue;
      private DataTable _genderHints;
      private DataTable _tissueHints;
      private DataTable _healthStateHints;
      private DataTable _sampleSourceHints;
      private DataTable _unitHints;
      private DataTable _nameTypeHints;

      public GeneExpressionQueries(IGeneExpressionDatabase database)
      {
         _databaseObject = database.DatabaseObject;
      }

      /// <summary>
      ///    This function retrieves a list of found proteins fulfilling the search criteria.
      /// </summary>
      public DataTable GetProteinsByName(string name)
      {
         try
         {
            var query = @"SELECT 
                      proteinsbyname.ID, 
                      Min(proteinsbyname.GENE_NAME) AS GENE_NAME, 
                      Min(proteinsbyname.NAME_TYPE) AS NAME_TYPE, 
                      proteinsbyname.SYMBOL, 
                      proteinsbyname.GENE_ID, 
                      proteinsbyname.OFFICIAL_FULL_NAME, 
                      case when Exists(SELECT NULL FROM TAB_GENE_VARIANTS v, TAB_EXPRESSION_DATA_VALUES e WHERE e.VARIANT_ID = v.VARIANT_ID AND v.GENE_ID = proteinsbyname.ID) then 1 else 0 end AS HAS_DATA 
                  FROM
                      (SELECT 
                          gn.GENE_ID AS ID, 
                          gn.GENE_NAME, 
                          gn.NAME_TYPE, 
                          (SELECT Min(s.GENE_NAME) FROM TAB_GENE_NAMES s WHERE s.GENE_ID = gn.GENE_ID AND s.NAME_TYPE = 'SYMBOL') AS SYMBOL, 
                          (SELECT Min(s.GENE_NAME) FROM TAB_GENE_NAMES s WHERE s.GENE_ID = gn.GENE_ID AND s.NAME_TYPE = 'GENE_ID') AS GENE_ID, 
                          (SELECT Min(s.GENE_NAME) FROM TAB_GENE_NAMES s WHERE s.GENE_ID = gn.GENE_ID AND s.NAME_TYPE = 'OFFICIAL_FULL_NAME') AS OFFICIAL_FULL_NAME
                      FROM TAB_GENE_NAMES AS gn WHERE gn.GENE_NAME like @P_NAME) as proteinsbyname

                  GROUP BY proteinsbyname.ID, proteinsbyname.SYMBOL, proteinsbyname.GENE_ID, proteinsbyname.OFFICIAL_FULL_NAME";

            _databaseObject.AddParameter(P_NAME, name, DAS.ParameterModes.PARM_IN, DAS.ServerTypes.STRING);
            return executeStatementForDataTable(query);
         }
         finally
         {
            _databaseObject.RemoveParameter(P_NAME);
         }
      }

      /// <summary>
      ///    This function retrieves expression data for a special protein.
      /// </summary>
      public DataTable GetExpressionDataByGeneId(long id)
      {
         try
         {
            var query1 = @"SELECT TAB_GENE_VARIANTS.VARIANT_NAME, TAB_EXPRESSION_DATA_RECORDS.DATA_BASE, TAB_EXPRESSION_DATA_RECORDS.DATA_BASE_REC_ID, TAB_EXPRESSION_DATA_RECORDS.GENDER, TAB_EXPRESSION_DATA_RECORDS.TISSUE, TAB_EXPRESSION_DATA_RECORDS.HEALTH_STATE, TAB_EXPRESSION_DATA_RECORDS.SAMPLE_SOURCE, TAB_EXPRESSION_DATA_AGES.AGE_MIN, TAB_EXPRESSION_DATA_AGES.AGE_MAX, TAB_EXPRESSION_DATA_VALUES.SAMPLE_COUNT, TAB_EXPRESSION_DATA_VALUES.TOTAL_COUNT, [SAMPLE_COUNT]/[TOTAL_COUNT] AS RATIO, ([SAMPLE_COUNT]/[TOTAL_COUNT])/[AVG] AS NORM_VALUE, TAB_EXPRESSION_DATA_VALUES.UNIT
                           FROM TAB_GENES INNER JOIN ((TAB_EXPRESSION_DATA_AGES INNER JOIN TAB_EXPRESSION_DATA_RECORDS ON TAB_EXPRESSION_DATA_AGES.AGE_ID = TAB_EXPRESSION_DATA_RECORDS.AGE_ID) INNER JOIN (TAB_GENE_VARIANTS INNER JOIN (TAB_EXPRESSION_DATA_VALUES INNER JOIN TAB_GLOBAL_STATISTICS ON TAB_EXPRESSION_DATA_VALUES.UNIT = TAB_GLOBAL_STATISTICS.UNIT) ON TAB_GENE_VARIANTS.VARIANT_ID = TAB_EXPRESSION_DATA_VALUES.VARIANT_ID) ON TAB_EXPRESSION_DATA_RECORDS.DATA_SOURCE_ID = TAB_EXPRESSION_DATA_VALUES.DATA_SOURCE_ID) ON TAB_GENES.GENE_ID = TAB_GENE_VARIANTS.GENE_ID
                           WHERE (((TAB_EXPRESSION_DATA_RECORDS.TISSUE) Is Not Null) AND ((TAB_GENES.GENE_ID)=@P_ID))";

            var query2 = @"SELECT TAB_GENE_VARIANTS.VARIANT_NAME, TAB_EXPRESSION_DATA_RECORDS.DATA_BASE, TAB_EXPRESSION_DATA_RECORDS.DATA_BASE_REC_ID, TAB_EXPRESSION_DATA_RECORDS.GENDER, TAB_EXPRESSION_DATA_RECORDS.TISSUE, TAB_EXPRESSION_DATA_RECORDS.HEALTH_STATE, TAB_EXPRESSION_DATA_RECORDS.SAMPLE_SOURCE, Null AS AGE_MIN, Null AS AGE_MAX, TAB_EXPRESSION_DATA_VALUES.SAMPLE_COUNT, TAB_EXPRESSION_DATA_VALUES.TOTAL_COUNT, [SAMPLE_COUNT]/[TOTAL_COUNT] AS RATIO, ([SAMPLE_COUNT]/[TOTAL_COUNT])/[AVG] AS NORM_VALUE, TAB_EXPRESSION_DATA_VALUES.UNIT
                           FROM TAB_GENES INNER JOIN (TAB_EXPRESSION_DATA_RECORDS INNER JOIN (TAB_GENE_VARIANTS INNER JOIN (TAB_EXPRESSION_DATA_VALUES INNER JOIN TAB_GLOBAL_STATISTICS ON TAB_EXPRESSION_DATA_VALUES.UNIT = TAB_GLOBAL_STATISTICS.UNIT) ON TAB_GENE_VARIANTS.VARIANT_ID = TAB_EXPRESSION_DATA_VALUES.VARIANT_ID) ON TAB_EXPRESSION_DATA_RECORDS.DATA_SOURCE_ID = TAB_EXPRESSION_DATA_VALUES.DATA_SOURCE_ID) ON TAB_GENES.GENE_ID = TAB_GENE_VARIANTS.GENE_ID
                           WHERE (((TAB_EXPRESSION_DATA_RECORDS.TISSUE) Is Not Null) AND ((TAB_EXPRESSION_DATA_RECORDS.AGE_ID) Is Null) AND ((TAB_GENES.GENE_ID)=@P_ID))";


            var query = $"SELECT * from ({query1}) UNION SELECT * from ({query2})";

            _databaseObject.AddParameter(P_ID, id, DAS.ParameterModes.PARM_IN, DAS.ServerTypes.NUMBER);
            return executeStatementForDataTable(query);
         }
         finally
         {
            _databaseObject.RemoveParameter(P_ID);
         }
      }

      /// <summary>
      ///    This function retrieves the default container tissue mapping.
      /// </summary>
      public DataTable GetContainerTissueMapping()
      {
         if (_mappingContainerTissue == null)
         {
            _mappingContainerTissue = _databaseObject.CreateAndFillDataTable($"SELECT {QRY_CONTAINER_TISSUE_COLUMNS} FROM {QRY_CONTAINER_TISSUE}");
         }

         return _mappingContainerTissue.Copy();
      }

      /// <summary>
      ///    This function retrieves information for a gender.
      /// </summary>
      public string GetGenderHint(string gender)
      {
         if (_genderHints == null)
         {
            _genderHints = _databaseObject.CreateAndFillDataTable($"SELECT {QRY_GENDER_HINT_COLUMNS} FROM {QRY_GENDER_HINT}");
         }

         DataRow[] dr = _genderHints.Select($"GENDER = '{gender}'");
         if (dr.Length == 0) return string.Empty;
         return (string) dr[0]["INFORMATION"];
      }

      /// <summary>
      ///    This function retrieves information for a tissue.
      /// </summary>
      public string GetTissueHint(string tissue)
      {
         if (_tissueHints == null)
         {
            _tissueHints = _databaseObject.CreateAndFillDataTable($"SELECT {QRY_TISSUE_HINT_COLUMNS} FROM {QRY_TISSUE_HINT}");
         }

         DataRow[] dr = (_tissueHints.Select($"TISSUE = '{tissue}'"));
         if (dr.Length == 0) return string.Empty;
         return (string) dr[0]["INFORMATION"];
      }

      /// <summary>
      ///    This function retrieves information for a health state.
      /// </summary>
      public string GetHealthStateHint(string healthState)
      {
         if (_healthStateHints == null)
         {
            _healthStateHints =
               _databaseObject.CreateAndFillDataTable($"SELECT {QRY_HEALTH_STATE_HINT_COLUMNS} FROM {QRY_HEALTH_STATE_HINT}");
         }

         DataRow[] dr = (_healthStateHints.Select($"HEALTH_STATE = '{healthState}'"));
         if (dr.Length == 0) return string.Empty;
         return (string) dr[0]["INFORMATION"];
      }

      /// <summary>
      ///    This function retrieves information for a sample source.
      /// </summary>
      public string GetSampleSourceHint(string sampleSource)
      {
         if (_sampleSourceHints == null)
         {
            _sampleSourceHints =
               _databaseObject.CreateAndFillDataTable($"SELECT {QRY_SAMPLE_SOURCE_HINT_COLUMNS} FROM {QRY_SAMPLE_SOURCE_HINT}");
         }

         DataRow[] dr = (_sampleSourceHints.Select($"SAMPLE_SOURCE = '{sampleSource}'"));
         if (dr.Length == 0) return string.Empty;
         return (string) dr[0]["INFORMATION"];
      }

      /// <summary>
      ///    This function retrieves information for a unit.
      /// </summary>
      public string GetUnitHint(string unit)
      {
         if (_unitHints == null)
         {
            _unitHints =
               _databaseObject.CreateAndFillDataTable($"SELECT {QRY_UNIT_HINT_COLUMNS} FROM {QRY_UNIT_HINT}");
         }

         DataRow[] dr = (_unitHints.Select($"UNIT = '{unit}'"));
         if (dr.Length == 0) return string.Empty;
         return (string) dr[0]["INFORMATION"];
      }

      /// <summary>
      ///    This function retrieves information for a name type.
      /// </summary>
      public string GetNameTypeHint(string nameType)
      {
         if (_nameTypeHints == null)
         {
            _nameTypeHints =
               _databaseObject.CreateAndFillDataTable($"SELECT {QRY_NAME_TYPE_HINT_COLUMNS} FROM {QRY_NAME_TYPE_HINT}");
         }

         DataRow[] dr = (_nameTypeHints.Select($"NAME_TYPE = '{nameType}'"));
         if (dr.Length == 0) return string.Empty;
         return (string) dr[0]["INFORMATION"];
      }

      /// <summary>
      ///    This function retrieves a list of property strings for a data base record id.
      /// </summary>
      public string[] GetDataBaseRecProperties(string database, string rec_id)
      {
         DASDataTable dt = null;
         string[] ret;
         try
         {
            _databaseObject.AddParameter(P_DATABASE, database, DAS.ParameterModes.PARM_IN,
               DAS.ServerTypes.STRING);
            _databaseObject.AddParameter(P_REC_ID, rec_id, DAS.ParameterModes.PARM_IN,
               DAS.ServerTypes.STRING);

            dt = _databaseObject.CreateAndFillDataTable(
               $"SELECT {QRY_DATABASE_REC_PROPERTIES_COLUMNS} FROM {QRY_DATABASE_REC_PROPERTIES_BY_ID} WHERE {P_DATABASE} = @{P_DATABASE} AND {P_REC_ID} = @{P_REC_ID}");
            ret = new string[dt.Rows.Count()];
            int i = 0;
            foreach (DASDataRow dr in dt.Rows)
            {
               DASColumnValue property = new DASColumnValue(dr, dr.Table.Columns.ItemByName("PROPERTY"));
               DASColumnValue propertyValue = new DASColumnValue(dr, dr.Table.Columns.ItemByName("PROPERTY_VALUE"));
               ret[i++] += $"{property.GetValueAsString()}: {propertyValue.GetValueAsString()}";
            }
         }
         finally
         {
            _databaseObject.RemoveParameter(P_DATABASE);
            _databaseObject.RemoveParameter(P_REC_ID);
         }

         return (ret);
      }

      /// <summary>
      ///    This function retrieves a list of information strings for a data base record id.
      /// </summary>
      public string[] GetDataBaseRecInfos(string database, string rec_id)
      {
         DASDataTable dt = null;
         string[] ret;
         try
         {
            _databaseObject.AddParameter(P_DATABASE, database, DAS.ParameterModes.PARM_IN,
               DAS.ServerTypes.STRING);
            _databaseObject.AddParameter(P_REC_ID, rec_id, DAS.ParameterModes.PARM_IN,
               DAS.ServerTypes.STRING);

            dt = _databaseObject.CreateAndFillDataTable(
               $"SELECT {QRY_DATABASE_REC_INFO_COLUMNS} FROM {QRY_DATABASE_REC_INFO_BY_ID} WHERE {P_DATABASE} = @{P_DATABASE} AND {P_REC_ID} = @{P_REC_ID}");
            ret = new string[dt.Columns.Count() - 2];

            foreach (DASDataRow dr in dt.Rows)
            {
               for (int i = 2; i < dt.Columns.Count(); i++)
               {
                  var colval = new DASColumnValue(dr, dt.Columns.ItemByIndex(i));
                  if (colval.DBNullValue) continue;
                  if (colval.Value is DateTime)
                     ret[i - 2] += $"{dt.Columns.ItemByIndex(i).ColumnName}: {colval.GetValueAsString("yyyy-MM-dd")}";
                  else
                     ret[i - 2] += $"{dt.Columns.ItemByIndex(i).ColumnName}: {colval.GetValueAsString()}";
               }
            }
         }
         finally
         {
            _databaseObject.RemoveParameter(P_DATABASE);
            _databaseObject.RemoveParameter(P_REC_ID);
         }

         return (ret);
      }

      /// <summary>
      ///    This method validates the schema of a database query.
      /// </summary>
      /// <param name="columns">List of column names separated by colon.</param>
      /// <param name="query">Name of query to be checked.</param>
      private void validateQuery(string columns, string query)
      {
         var schemaTable = _databaseObject.CreateDASDataTable(query);
         foreach (var col in columns.Split(','))
         {
            if (!schemaTable.Columns.ContainsName(col.Trim()))
               throw new PKSimException(($"Column {col.Trim()} could not be found in query {query}."));
         }
      }

      /// <summary>
      ///    This method validates the schema of the database.
      /// </summary>
      /// <exception cref="PKSimException">Thrown when an error occured.</exception>
      public void ValidateDatabase()
      {
         try
         {
            validateQuery(QRY_CONTAINER_TISSUE_COLUMNS, QRY_CONTAINER_TISSUE);
            validateQuery(QRY_GENDER_HINT_COLUMNS, QRY_GENDER_HINT);
            validateQuery(QRY_TISSUE_HINT_COLUMNS, QRY_TISSUE_HINT);
            validateQuery(QRY_HEALTH_STATE_HINT_COLUMNS, QRY_HEALTH_STATE_HINT);
            validateQuery(QRY_SAMPLE_SOURCE_HINT_COLUMNS, QRY_SAMPLE_SOURCE_HINT);
            validateQuery(QRY_UNIT_HINT_COLUMNS, QRY_UNIT_HINT);
            validateQuery(QRY_NAME_TYPE_HINT_COLUMNS, QRY_NAME_TYPE_HINT);
         }
         catch (Exception ex)
         {
            throw new PKSimException($"Database {_databaseObject.DatabaseName} is not a valid expression database!", ex);
         }
      }

      public void ClearCache()
      {
         _mappingContainerTissue = null;
         _genderHints = null;
         _tissueHints = null;
         _healthStateHints = null;
         _sampleSourceHints = null;
         _unitHints = null;
         _nameTypeHints = null;
      }

      private DataTable executeStatementForDataTable(string selectStatement)
      {
         var dataTable = new DASDataTable(_databaseObject);
         try
         {
            _databaseObject.FillDataTable(dataTable, selectStatement);
            return dataTable;
         }
         catch (Exception e)
         {
            var t = dataTable.GetErrors();
            Console.WriteLine(e);
            throw;
         }
      }
   }
}