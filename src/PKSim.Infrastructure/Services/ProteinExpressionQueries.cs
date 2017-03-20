using System;
using System.Data;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.DAS;

namespace PKSim.Infrastructure.Services
{
   public class ProteinExpressionQueries : IProteinExpressionQueries
   {
      private readonly DAS _databaseObject;

      //Queries
      private const string QRY_PROTEINS_BY_NAME = "QRY_PROTEINS_BY_NAME";
      private const string QRY_EXPRESSION_DATA_BY_ID = "QRY_EXPRESSION_DATA_BY_ID";
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
      private const string QRY_FIND_PROTEINS_COLUMNS = "ID, GENE_NAME, NAME_TYPE, SYMBOL, GENE_ID, OFFICIAL_FULL_NAME, HAS_DATA";

      private const string QRY_EXPRESSION_DATA_COLUMNS =
         "VARIANT_NAME, DATA_BASE, DATA_BASE_REC_ID, GENDER, TISSUE, HEALTH_STATE, SAMPLE_SOURCE, AGE_MIN, AGE_MAX, SAMPLE_COUNT, TOTAL_COUNT, RATIO, NORM_VALUE, UNIT";

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

      //cashed queries
      private DASDataTable _mappingContainerTissue;
      private DASDataTable _genderHints;
      private DASDataTable _tissueHints;
      private DASDataTable _healthStateHints;
      private DASDataTable _sampleSourceHints;
      private DASDataTable _unitHints;
      private DASDataTable _nameTypeHints;

      public ProteinExpressionQueries(IProteinExpressionDatabase database)
      {
         _databaseObject = database.DatabaseObject;
      }

      /// <summary>
      ///    This function retrieves a list of found proteins fulfilling the search criteria.
      /// </summary>
      public DataTable GetProteinsByName(string name)
      {
         DASDataTable retTable = null;
         try
         {
            _databaseObject.AddParameter(P_NAME, name, DAS.ParameterModes.PARM_IN,
               DAS.ServerTypes.ST_VARCHAR2);

            retTable = _databaseObject.CreateAndFillDataTable(
               $"SELECT {QRY_FIND_PROTEINS_COLUMNS} FROM {QRY_PROTEINS_BY_NAME} WHERE {P_NAME} = @{P_NAME}");
         }
         finally
         {
            _databaseObject.RemoveParameter(P_NAME);
         }
         return (retTable);
      }

      /// <summary>
      ///    This function retrieves expression data for a special protein.
      /// </summary>
      public DataTable GetExpressionDataByGeneId(int id)
      {
         DASDataTable retTable = null;
         try
         {
            _databaseObject.AddParameter(P_ID, id, DAS.ParameterModes.PARM_IN,
               DAS.ServerTypes.ST_NUMBER);

            retTable = _databaseObject.CreateAndFillDataTable(
               $"SELECT {QRY_EXPRESSION_DATA_COLUMNS} FROM {QRY_EXPRESSION_DATA_BY_ID} WHERE {P_ID} = @{P_ID}");
         }
         finally
         {
            _databaseObject.RemoveParameter(P_ID);
         }
         return (retTable);
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
         return (_mappingContainerTissue.Copy());
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
               DAS.ServerTypes.ST_VARCHAR2);
            _databaseObject.AddParameter(P_REC_ID, rec_id, DAS.ParameterModes.PARM_IN,
               DAS.ServerTypes.ST_VARCHAR2);

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
               DAS.ServerTypes.ST_VARCHAR2);
            _databaseObject.AddParameter(P_REC_ID, rec_id, DAS.ParameterModes.PARM_IN,
               DAS.ServerTypes.ST_VARCHAR2);

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
      private void ValidateQuery(string columns, string query)
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
            ValidateQuery(QRY_FIND_PROTEINS_COLUMNS, QRY_PROTEINS_BY_NAME);
            ValidateQuery(QRY_EXPRESSION_DATA_COLUMNS, QRY_EXPRESSION_DATA_BY_ID);
            ValidateQuery(QRY_CONTAINER_TISSUE_COLUMNS, QRY_CONTAINER_TISSUE);
            ValidateQuery(QRY_GENDER_HINT_COLUMNS, QRY_GENDER_HINT);
            ValidateQuery(QRY_TISSUE_HINT_COLUMNS, QRY_TISSUE_HINT);
            ValidateQuery(QRY_HEALTH_STATE_HINT_COLUMNS, QRY_HEALTH_STATE_HINT);
            ValidateQuery(QRY_SAMPLE_SOURCE_HINT_COLUMNS, QRY_SAMPLE_SOURCE_HINT);
            ValidateQuery(QRY_UNIT_HINT_COLUMNS, QRY_UNIT_HINT);
            ValidateQuery(QRY_NAME_TYPE_HINT_COLUMNS, QRY_NAME_TYPE_HINT);
            ValidateQuery(QRY_DATABASE_REC_PROPERTIES_COLUMNS, QRY_DATABASE_REC_PROPERTIES_BY_ID);
            ValidateQuery(QRY_DATABASE_REC_INFO_COLUMNS, QRY_DATABASE_REC_INFO_BY_ID);
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
   }
}