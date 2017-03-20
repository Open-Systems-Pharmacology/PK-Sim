using System;
using System.Collections;
using System.Data;

namespace PKSim.Infrastructure.ORM.DAS
{
   internal class ParameterValue
   {
      /// <summary>
      /// This property is the name of the parameter.
      /// </summary>
      /// <returns>Name of the parameter.</returns>
      public string ParameterName { get; set; }

      /// <summary>
      /// This property is the value of the parameter.
      /// </summary>
      /// <returns>Value of the parameter.</returns>
      public object Value { get; set; }

      /// <summary>
      /// This property is the <see cref="DAS.ServerTypes"></see> of the parameter.
      /// </summary>
      /// <returns>Value of <see cref="DAS.ServerTypes"></see>.</returns>
      public DAS.ServerTypes ServerType { get; set; }

      /// <summary>
      /// This property is the mode of the parameter.
      /// </summary>
      /// <returns>Value of <see cref="DAS.ParameterModes"></see>.</returns>
      public DAS.ParameterModes ParameterMode { get; set; }
   }

   public class DASDataTable : DataTable, IEnumerable
   {

      private readonly DAS m_DAS;
      private readonly DASDataColumnCollection m_Columns;
      private readonly DASDataRowCollection m_Rows;
      private string m_DBTableName = string.Empty;
      private string m_BaseSQL = string.Empty;
      private string m_SQL = string.Empty;

      private ParameterValue[] m_ParameterValues;
      /// <summary>
      /// This is the constructor of this class.
      /// </summary>
      public DASDataTable(DAS DAS)
      {
         m_DAS = DAS;
         m_Columns =  new DASDataColumnCollection(base.Columns);
         m_Rows = new DASDataRowCollection(base.Rows);
      }

      /// <summary>
      /// This property gives the row of the given index.
      /// </summary>
      /// <param name="index">Index of the row.</param>
      /// <returns><see cref="DASDataRow"></see> object.</returns>
      public DASDataRow this[int index]
      {
         get { return Rows.ItemByIndex(index); }
      }

      /// <summary>
      /// This property gives the columns of this table.
      /// </summary>
      public new DASDataColumnCollection Columns
      {
         get { return m_Columns; }
      }

      /// <summary>
      /// This property gives the rows of this table.
      /// </summary>
      public new DASDataRowCollection Rows
      {
         get { return m_Rows; }
      }

      /// <summary>
      /// This Function must be overridden to create a DASDataTable instead of a DataTable.
      /// </summary>
      protected override DataTable CreateInstance()
      {
         return new DASDataTable(m_DAS);
      }

      /// <summary>
      /// This Function must be overridden to create a DASDataRow instead of a DataRow.
      /// </summary>
      /// <param name="builder"><see cref="DataRowBuilder "></see> object to build new rows.</param>
      protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
      {
         return new DASDataRow(builder);
      }

      /// <summary>
      /// This property gives the name of table in the database.
      /// </summary>
      public string DBTableName
      {
         get { return m_DBTableName; }
         protected internal set { m_DBTableName = value; }
      }

      /// <summary>
      /// This gives the SQL statement which has been used on retrieving the data from database.
      /// </summary>
      public string SQL
      {
         get { return m_SQL; }
         internal set { m_SQL = value; }
      }

      /// <summary>
      /// This property is used internally to store the values of parameters needed for a refresh.
      /// </summary>
      /// <remarks>If you are using parameters in your query the 
      /// corresponding values are stored to support the refresh method.</remarks>
      internal ParameterValue[] ParameterValues
      {
         get { return m_ParameterValues; }
         set { m_ParameterValues = value; }
      }

      /// <summary>
      /// This is the basic sql to get data of the table.
      /// </summary>
      /// <returns>SQL statement.</returns>
      public string BaseSQL
      {
         get { return m_BaseSQL; }
      }

      /// <summary>
      /// This sub builds and sets the base sql.
      /// </summary>
      protected internal void BuildAndSetBaseSQL()
      {
         var sb = new System.Text.StringBuilder();
         bool first = true;

         sb.Append("SELECT ");
         foreach (DASDataColumn column in Columns)
         {
            if (!first)
            {
               sb.Append(", ");
            }
            else
            {
               first = false;
            }
            sb.AppendFormat("t.{0}", column.DBColumnName);
         }
         sb.AppendFormat(" FROM {0} t", DBTableName);

         m_BaseSQL = sb.ToString();
      }

      /// <summary>
      /// This property gives the information whether the DASDataTable is editable.
      /// </summary>
      /// <returns><c>True</c> if table is editable (UPDATE and DELETE). </returns>
      /// <remarks>A table is editable, if it knows its <see cref="DBTableName"></see> and a primary key must be set.</remarks>
      public bool IsEditable
      {
         get { return ((DBTableName.Length > 0) & (PrimaryKey.Length > 0)); }
      }

      /// <summary>
      /// This property gives the DAS object.
      /// </summary>
      /// <returns>DAS</returns>
      public DAS DAS
      {
         get { return m_DAS; }
      }

      /// <summary>
      /// This sub refreshs the data of the data table by selecting actual data from the data base.
      /// </summary>
      public void Refresh()
      {
         try
         {
            foreach (ParameterValue value in m_ParameterValues)
            {
               DAS.AddParameter(value.ParameterName, value.Value, value.ParameterMode, value.ServerType);
            }
            m_DAS.FillDataTable(this, SQL);
         }
         catch (Exception ex)
         {
            throw new Exception(ex.Message, ex.InnerException);
         }
         finally
         {
            foreach (ParameterValue value in m_ParameterValues)
            {
               DAS.RemoveParameter(value.ParameterName);
            }
         }
      }

      /// <summary>
      /// This method overloads the base class method to support DAS specific properties.
      /// </summary>
      /// <param name="table">Table to be merged.</param>
      /// <param name="preserveChanges"><c>True</c> means all changes are preserved.</param>
      /// <param name="missingSchemaAction"><seealso cref="System.Data.MissingSchemaAction"></seealso></param>
      public void Merge(DASDataTable table, bool preserveChanges, MissingSchemaAction missingSchemaAction)
      {
         base.Merge(table, preserveChanges, missingSchemaAction);
         foreach (DASDataRow row in table.Rows)
         {
            var _with1 = Rows.ItemByDBKey(row.DBKey);
            _with1.SQL = row.SQL;
         }
      }

      /// <summary>
      /// This method overloads the base class method to support DAS specific properties.
      /// </summary>
      /// <param name="table">Table to be merged.</param>
      /// <param name="preserveChanges"><c>True</c> means all changes are preserved.</param>
      public void Merge(DASDataTable table, bool preserveChanges)
      {
         Merge(table, preserveChanges, MissingSchemaAction.Error);
      }

      /// <summary>
      /// This method overloads the base class method to support DAS specific properties.
      /// </summary>
      /// <param name="table">Table to be merged.</param>
      public void Merge(DASDataTable table)
      {
         Merge(table, true, MissingSchemaAction.Error);
      }

      /// <summary>
      /// This function is needed to support for each loops.
      /// </summary>
      public IEnumerator GetEnumerator()
      {

         return Rows.GetEnumerator();
      }
   }
}