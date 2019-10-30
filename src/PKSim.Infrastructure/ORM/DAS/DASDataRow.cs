using System;
using System.Data;
using System.Text;

namespace PKSim.Infrastructure.ORM.DAS
{
   public class DASDataRow : DataRow
   {
      /// <summary>
      /// This is the constructor of this class.
      /// </summary>
      /// <param name="Builder"><see cref=" DataRowBuilder "></see> object to build new rows.</param>
      public DASDataRow(DataRowBuilder Builder)
         : base(Builder)
      {
      }

      /// <summary>
      /// This property must be overloaded to give a DASDataTable instead of a DataTable.
      /// </summary>
      public new DASDataTable Table
      {
         get { return (DASDataTable)base.Table; }
      }

      /// <summary>
      /// This properties gives the used SQL statement.
      /// </summary>
      public string SQL { get; protected internal set; }

      /// <summary>
      /// This property informs whether the row has been changed.
      /// </summary>
      /// <returns><c>True</c>, if any column value has been changed.</returns>
      public bool Changed
      {
         get
         {
            foreach (DASDataColumn column in Table.Columns)
            {
               var cv = new DASColumnValue(this, column);
               if (cv.Changed)
                  return true; 
            }
            return false;
         }
      }

      /// <summary>
      /// This property gives the DBKey of the row.
      /// </summary>
      /// <returns>A <see cref=" string"></see> representation of primary key values.</returns>
      public string DBKey
      {
         get
         {
            var ReturnValue = new StringBuilder();
            var First = true;
            DataRowVersion rowVersion;

            switch (RowState)
            {
               case DataRowState.Added:
                  rowVersion = DataRowVersion.Current;
                  break;
               case DataRowState.Detached:
                  rowVersion = DataRowVersion.Default;
                  break;
               default:
                  rowVersion = DataRowVersion.Original;
                  break;
            }

            foreach (DataColumn Column in Table.PrimaryKey)
            {
               if (First)
                  First = false;
               else
                  ReturnValue.Append(Table.DAS.KeyDelimiter);
               ReturnValue.Append(this[Column, rowVersion].ToString());
            }

            return ReturnValue.ToString();
         }
      }

      /// <summary>
      /// This function checks whether the row does exists in the database.
      /// </summary>
      /// <returns><c>True</c>, if row exists.</returns>
      public bool ExistsInDB()
      {
         bool ReturnValue = false;
         var stringBuilder = new StringBuilder();
         var TableDAS = Table.DAS;
         DataRowVersion rowVersion;

         var dataTable = new DASDataTable(TableDAS) {DBTableName = Table.DBTableName};

         switch (RowState)
         {
            case DataRowState.Added:
               rowVersion = DataRowVersion.Current;
               break;
            case DataRowState.Detached:
               rowVersion = DataRowVersion.Default;
               break;
            default:
               rowVersion = DataRowVersion.Original;
               break;
         }

         long i = 1;
         foreach (DASDataColumn column in Table.PrimaryKey)
         {
            switch (column.DASDataType)
            {
               case DASDataColumn.DASDataTypes.DASDATE:
                  TableDAS.AddParameter(string.Format("@P{0}", i), this[column, rowVersion], DAS.ParameterModes.PARM_IN, DAS.ServerTypes.DATE);
                  break;
               case DASDataColumn.DASDataTypes.DASDOUBLE:
               case DASDataColumn.DASDataTypes.DASLONG:
                  TableDAS.AddParameter(string.Format("@P{0}", i), this[column, rowVersion], DAS.ParameterModes.PARM_IN, DAS.ServerTypes.NUMBER);
                  break;
               case DASDataColumn.DASDataTypes.DASSTRING:
                  TableDAS.AddParameter(string.Format("@P{0}", i), this[column, rowVersion], DAS.ParameterModes.PARM_IN, DAS.ServerTypes.STRING);
                  break;
               default:
                  throw new UnsupportedDataTypeException(column.DASDataType.ToString());
            }
            i++;
         }
         try
         {
            stringBuilder.AppendFormat("SELECT NULL FROM {0}", dataTable.DBTableName);
            i = 1;
            foreach (DASDataColumn column in Table.PrimaryKey)
            {
               if (i == 1)
                  stringBuilder.Append(" WHERE ");
               else
                  stringBuilder.Append(" AND ");
               stringBuilder.AppendFormat("{0} = @P{1}", column.ColumnName, i++);
            }

            TableDAS.FillDataTable(dataTable, stringBuilder.ToString());
         }
         catch (Exception ex)
         {
            throw new Exception(ex.Message, ex.InnerException);
         }
         finally
         {
            for (var k = 1; k <= Table.PrimaryKey.Length; k++ )
               TableDAS.RemoveParameter(string.Format("@P{0}", k));
         }

         if (dataTable.Rows.Count() == 1)
            ReturnValue = true;
         else if (dataTable.Rows.Count() > 1)
            throw new TooManyRowsFoundException();

         return ReturnValue;
      }

      /// <summary>
      /// This sub refreshs the data of the data row by selecting actual data from the data base.
      /// </summary>
      public void Refresh()
      {
         SelectFromDB();
      }

      /// <summary>
      /// This sub selects the data of a specified point in time from the data base.
      /// </summary>
      /// <exception cref="UnsupportedDataTypeException">Thrown when an unsupported data type occurs.</exception>
      /// <exception cref="RowNotFoundException">Thrown when the row could not be found.</exception>
      /// <exception cref="TooManyRowsFoundException">Thrown when more then one row has been found.</exception>
      public void SelectFromDB()
      {
         var tableDAS = Table.DAS;
         var stringBuilder = new StringBuilder();
         DataRowVersion rowVersion;

         switch (RowState)
         {
            case DataRowState.Added:
               rowVersion = DataRowVersion.Current;
               break;
            case DataRowState.Detached:
               rowVersion = DataRowVersion.Default;
               break;
            default:
               rowVersion = DataRowVersion.Original;
               break;
         }

         var dataTable = (DASDataTable)Table.Clone();

         long i = 1;
         foreach (DASDataColumn column in Table.PrimaryKey)
         {
            switch (column.DASDataType)
            {
               case DASDataColumn.DASDataTypes.DASDATE:
                  tableDAS.AddParameter(string.Format("@P{0}", i), this[column, rowVersion], DAS.ParameterModes.PARM_IN, DAS.ServerTypes.DATE);
                  break;
               case DASDataColumn.DASDataTypes.DASDOUBLE:
               case DASDataColumn.DASDataTypes.DASLONG:
                  tableDAS.AddParameter(string.Format("@P{0}", i), this[column, rowVersion], DAS.ParameterModes.PARM_IN, DAS.ServerTypes.NUMBER);
                  break;
               case DASDataColumn.DASDataTypes.DASSTRING:
                  tableDAS.AddParameter(string.Format("@P{0}", i), this[column, rowVersion], DAS.ParameterModes.PARM_IN, DAS.ServerTypes.STRING);
                  break;
               default:
                  throw new UnsupportedDataTypeException(column.DASDataType.ToString());
            }
            i++;
         }
         try
         {
            stringBuilder.Append(Table.BaseSQL);
            i = 1;
            foreach (DASDataColumn column in Table.PrimaryKey)
            {
               if (i == 1)
                  stringBuilder.Append(" WHERE ");
               else
                  stringBuilder.Append(" AND ");
               stringBuilder.AppendFormat("{0} = @P{1}", column.ColumnName, i++);
            }

            tableDAS.FillDataTable(dataTable, stringBuilder.ToString());
         }
         catch (Exception ex)
         {
            throw new Exception(ex.Message, ex.InnerException);
         }
         finally
         {
            for (var k = 1; k <= Table.PrimaryKey.Length; k++)
               tableDAS.RemoveParameter(string.Format("@P{0}", k));
         }

         if (dataTable.Rows.Count() == 0)
            throw new RowNotFoundException(DBKey);
         if (dataTable.Rows.Count() > 1)
            throw new TooManyRowsFoundException();
         //Set values of me to values just queried from data base
         var row = dataTable.Rows.ItemByIndex(0);
         foreach (DASDataColumn column in Table.Columns)
         {
            var readonlystate = column.ReadOnly;
            column.ReadOnly = false;
            this[column.ColumnName] = row[column.ColumnName];
            column.ReadOnly = readonlystate;
         }
         SQL = dataTable.SQL;

         if (RowState != DataRowState.Detached && RowState != DataRowState.Added)
            AcceptChanges();
      }
      /// <summary>
      /// This function inserts the row into the database.
      /// </summary>
      /// <returns>Number of rows affected.</returns>
      /// <remarks>Columns with property <see cref="DASDataColumn.IsAutoValue"></see> = <c>True</c> 
      /// must be set to their DASDataColumn.DefaultValue to get the auto value 
      /// by <see cref="InsertIntoDB"></see>.</remarks>
      /// <exception cref="NotEditableException">Thrown when the table is not editable.</exception>
      /// <exception cref="UnsupportedDataTypeException">Thrown when an unsupported data type occurs.</exception>
      /// <exception cref="InvalidRowStateException">Thrown when <see cref="DataRow.RowState"></see> is not equal <see cref=" DataRowState.Added"></see>.</exception>
      public virtual int InsertIntoDB()
      {
         int ReturnValue;

         if (!(Table.DBTableName.Length > 0))
            throw new NotInsertableException();

         if (RowState != DataRowState.Added)
            throw new InvalidRowStateException(RowState);

         var stringBuilder= new StringBuilder();
         var First = true;
         var TableDAS = Table.DAS;

         //check for autovalue columns
         foreach (DASDataColumn column in Table.Columns)
         {
            if (!column.IsAutoValue) continue;
            var curReadOnly = column.ReadOnly;
            column.ReadOnly = false;
            var ColumnValue = new DASColumnValue(this, column);
            if (ColumnValue.Value == column.DefaultValue)
               ColumnValue.Value = TableDAS.GetAutoValue(column.AutoValueCreator);
            column.ReadOnly = curReadOnly;
         }

         //Build SQL statement
         stringBuilder.AppendFormat("INSERT INTO {0} (", Table.DBTableName);
         foreach (DASDataColumn column in Table.Columns)
         {
            if (First)
               First = false;
            else
               stringBuilder.Append(", ");
            stringBuilder.Append(column.DBColumnName);
         }
         stringBuilder.Append(") VALUES (");

         for (long k = 1; k <= Table.Columns.Count(); k++)
         {
            if (k > 1)
               stringBuilder.Append(", ");
            stringBuilder.AppendFormat("@P{0}", k);
         }
         stringBuilder.Append(")");

         //Create Parameters
         long i = 1;
         foreach (DASDataColumn column in Table.Columns)
         {
            var ColumnValue = new DASColumnValue(this, column);
            switch (column.DASDataType)
            {
               case DASDataColumn.DASDataTypes.DASDATE:
                  TableDAS.AddParameter(string.Format("@P{0}", i), ColumnValue.Value, DAS.ParameterModes.PARM_IN,
                                        DAS.ServerTypes.DATE);
                  break;
               case DASDataColumn.DASDataTypes.DASDOUBLE:
               case DASDataColumn.DASDataTypes.DASLONG:
                  TableDAS.AddParameter(string.Format("@P{0}", i), ColumnValue.Value, DAS.ParameterModes.PARM_IN,
                                        DAS.ServerTypes.NUMBER);
                  break;
               case DASDataColumn.DASDataTypes.DASSTRING:
                  TableDAS.AddParameter(string.Format("@P{0}", i), ColumnValue.Value, DAS.ParameterModes.PARM_IN,
                                        DAS.ServerTypes.STRING);
                  break;
               default:
                  throw new UnsupportedDataTypeException(column.DASDataType.ToString());
            }
            i++;
         }

         try
         {
            ReturnValue = Table.DAS.ExecuteSQL(stringBuilder.ToString());
         }
         catch (Exception ex)
         {
            throw new Exception(ex.Message, ex.InnerException);
         }
         finally
         {
            //Remove Parameters
            for (long k = 1; k <= Table.Columns.Count(); k++)
               TableDAS.RemoveParameter(string.Format("@P{0}", k));
         }

         if (ReturnValue > 0)
         {
            AcceptChanges();
            SelectFromDB();
         }

         return ReturnValue;
      }

      /// <summary>
      /// This function updates the row in the database.
      /// </summary>
      /// <returns>Number of affected rows.</returns>
      /// <exception cref="NotEditableException">Thrown when the table is not editable.</exception>
      /// <exception cref="UnsupportedDataTypeException">Thrown when an unsupported data type occurs.</exception>
      /// <exception cref="InvalidRowStateException">Thrown when <see cref="DataRow.RowState"></see> is not equal <see cref=" DataRowState.Modified"></see>.</exception>
      public virtual int UpdateInDB()
      {
         int ReturnValue = 0;

         if (!Table.IsEditable)
            throw new NotEditableException();

         if (RowState == DataRowState.Modified)
         {

            if (Changed)
            {
               var tableDAS = Table.DAS;
               var stringBuilder = new StringBuilder();

               long i = 1;
               foreach (DASDataColumn column in Table.Columns)
               {
                  var ColumnValue = new DASColumnValue(this, column);
                  if (!ColumnValue.Changed) continue;
                  switch (column.DASDataType)
                  {
                     case DASDataColumn.DASDataTypes.DASDATE:
                        tableDAS.AddParameter(string.Format("@P{0}", i), ColumnValue.Value, DAS.ParameterModes.PARM_IN, DAS.ServerTypes.DATE);
                        break;
                     case DASDataColumn.DASDataTypes.DASDOUBLE:
                     case DASDataColumn.DASDataTypes.DASLONG:
                        tableDAS.AddParameter(string.Format("@P{0}", i), ColumnValue.Value, DAS.ParameterModes.PARM_IN, DAS.ServerTypes.NUMBER);
                        break;
                     case DASDataColumn.DASDataTypes.DASSTRING:
                        tableDAS.AddParameter(string.Format("@P{0}", i), ColumnValue.Value, DAS.ParameterModes.PARM_IN, DAS.ServerTypes.STRING);
                        break;
                     default:
                        throw new UnsupportedDataTypeException(column.DataType.ToString());
                  }
                  i++;
               }
               i = 1;
               foreach (DASDataColumn column in Table.PrimaryKey)
               {
                  var ColumnValue = new DASColumnValue(this, column);
                  switch (column.DASDataType)
                  {
                     case DASDataColumn.DASDataTypes.DASDATE:
                        tableDAS.AddParameter(string.Format("@PK{0}", i), ColumnValue.DBValue, DAS.ParameterModes.PARM_IN, DAS.ServerTypes.DATE);
                        break;
                     case DASDataColumn.DASDataTypes.DASDOUBLE:
                     case DASDataColumn.DASDataTypes.DASLONG:
                        tableDAS.AddParameter(string.Format("@PK{0}", i), ColumnValue.DBValue, DAS.ParameterModes.PARM_IN, DAS.ServerTypes.NUMBER);
                        break;
                     case DASDataColumn.DASDataTypes.DASSTRING:
                        tableDAS.AddParameter(string.Format("@PK{0}", i), ColumnValue.DBValue, DAS.ParameterModes.PARM_IN, DAS.ServerTypes.STRING);
                        break;
                     default:
                        throw new UnsupportedDataTypeException(column.DASDataType.ToString());
                  }
                  i++;
               }

               try
               {
                  stringBuilder.AppendFormat("UPDATE {0} SET ", Table.DBTableName);
                  i = 1;
                  foreach (DASDataColumn column in Table.Columns)
                  {
                     var ColumnValue = new DASColumnValue(this, column);
                     if (!ColumnValue.Changed) continue;
                     if (i > 1)
                        stringBuilder.Append(", ");
                     stringBuilder.AppendFormat("{0} = @P{1}", column.ColumnName, i++);
                  }
                  i = 1;
                  foreach (DASDataColumn column in Table.PrimaryKey)
                  {
                     if (i == 1)
                        stringBuilder.Append(" WHERE ");
                     else
                        stringBuilder.Append(" AND ");
                     stringBuilder.AppendFormat("{0} = @PK{1}", column.ColumnName, i++);
                  }
                  ReturnValue = tableDAS.ExecuteSQL(stringBuilder.ToString());
               }
               catch (Exception ex)
               {
                  throw new Exception(ex.Message, ex.InnerException);
               }
               finally
               {
                  for (var k = 1; k <= Table.PrimaryKey.Length; k++)
                     tableDAS.RemoveParameter(string.Format("@PK{0}", k));
                  i = 1;
                  foreach (DASDataColumn column in Table.Columns)
                  {
                     var ColumnValue = new DASColumnValue(this, column);
                     if (!ColumnValue.Changed) continue;
                     tableDAS.RemoveParameter(string.Format("@P{0}", i++));
                  }
               }

               if (ReturnValue > 0)
               {
                  AcceptChanges();
                  SelectFromDB();
               }

            }
         }
         else
         {
            throw new InvalidRowStateException(RowState);
         }
         return ReturnValue;
      }

      /// <summary>
      /// This function deletes the row in the database.
      /// </summary>
      /// <returns>Number of affected rows.</returns>
      /// <exception cref="NotEditableException">Thrown when the table is not editable.</exception>
      /// <exception cref="InvalidRowStateException">Thrown when <see cref="DataRow.RowState"></see> is not equal <see cref=" DataRowState.Deleted"></see>.</exception>
      public virtual int DeleteFromDB()
      {
         int ReturnValue;

         if (!Table.IsEditable)
         {
            throw new NotEditableException();
         }

         if (RowState == DataRowState.Deleted)
         {
            var tableDAS = Table.DAS;
            var stringBuilder = new StringBuilder();

            long i = 1;
            foreach (DASDataColumn column in Table.PrimaryKey)
            {
               var ColumnValue = new DASColumnValue(this, column);
               switch (column.DASDataType)
               {
                  case DASDataColumn.DASDataTypes.DASDATE:
                     tableDAS.AddParameter(string.Format("@PK{0}", i), ColumnValue.DBValue, DAS.ParameterModes.PARM_IN, DAS.ServerTypes.DATE);
                     break;
                  case DASDataColumn.DASDataTypes.DASDOUBLE:
                  case DASDataColumn.DASDataTypes.DASLONG:
                     tableDAS.AddParameter(string.Format("@PK{0}", i), ColumnValue.DBValue, DAS.ParameterModes.PARM_IN, DAS.ServerTypes.NUMBER);
                     break;
                  case DASDataColumn.DASDataTypes.DASSTRING:
                     tableDAS.AddParameter(string.Format("@PK{0}", i), ColumnValue.DBValue, DAS.ParameterModes.PARM_IN, DAS.ServerTypes.STRING);
                     break;
                  default:
                     throw new UnsupportedDataTypeException(column.DASDataType.ToString());
               }
               i++;
            }
            try
            {
               stringBuilder.AppendFormat("DELETE FROM {0}", Table.DBTableName);
               i = 1;
               foreach (DASDataColumn column in Table.PrimaryKey)
               {
                  if (i == 1)
                     stringBuilder.Append(" WHERE ");
                  else
                     stringBuilder.Append(" AND ");
                  stringBuilder.AppendFormat("{0} = @PK{1}", column.ColumnName, i++);
               }
               ReturnValue = tableDAS.ExecuteSQL(stringBuilder.ToString());
            }
            catch (Exception ex)
            {
               throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
               for (var k = 1; k <= Table.PrimaryKey.Length; k++)
                  tableDAS.RemoveParameter(string.Format("@PK{0}", k));
            }
         }
         else
         {
            throw new InvalidRowStateException(RowState);
         }

         if (ReturnValue > 0)
         {
            AcceptChanges();
         }

         return ReturnValue;
      }

   }

   /// <summary>
   /// This class is an encapsulation of DataRowCollection class. 
   /// </summary>
   /// <remarks>It is no real collection object. It just encapsulates the row collection of the DataTable.</remarks>
   public class DASDataRowCollection
   {

      private readonly DataRowCollection m_Rows;
      /// <summary>
      /// This is the constructor of the class.
      /// </summary>
      /// <param name="Rows"><see cref=" DataRowCollection "></see> object needed for initialization.</param>
      internal DASDataRowCollection(DataRowCollection Rows)
      {
         m_Rows = Rows;
      }

      /// <summary>
      /// This sub removes all rows from the collection.
      /// </summary>
      public void Clear()
      {
         m_Rows.Clear();
      }

      /// <summary>
      /// This sub adds given row to the collection.
      /// </summary>
      /// <param name="Row"><see cref=" DASDataRow "></see> object to be added.</param>
      public void Add(DASDataRow Row)
      {
         m_Rows.Add(Row);
      }

      /// <summary>
      /// This function returns the count of rows.
      /// </summary>
      public int Count()
      {
         return m_Rows.Count;
      }

      /// <summary>
      /// This sub removes the row from the collection.
      /// </summary>
      /// <param name="Row"><see cref=" DASDataRow "></see> object to be removed.</param>
      /// <remarks>It does not delete the row from the database.</remarks>
      public void Remove(DASDataRow Row)
      {
         m_Rows.Remove(Row);
      }

      /// <summary>
      /// This sub removes the row specified by the index from the collection.
      /// </summary>
      /// <param name="Index">Index of the row in the collection.</param>
      /// <remarks>It does not delete the row from the database.</remarks>
      public void RemoveAt(int Index)
      {
         m_Rows.RemoveAt(Index);
      }

      /// <summary>
      /// This functions checks whether a given row is part of the collection.
      /// </summary>
      /// <param name="Row"><see cref=" DASDataRow "></see> object to be checked.</param>
      /// <returns><c>True</c>, if collection contains the row.</returns>
      public bool ContainsValue(DASDataRow Row)
      {
         return m_Rows.Contains(Row);
      }

      /// <summary>
      /// This function gives the row specified by the index.
      /// </summary>
      /// <param name="Index">Index of the row in the rows collection.</param>
      /// <returns><see cref=" DASDataRow "></see> object.</returns>
      public DASDataRow ItemByIndex(int Index)
      {
         return (DASDataRow)m_Rows[Index];
      }

      /// <summary>
      /// This function gives the row specified by the DBKey.
      /// </summary>
      /// <param name="DBKey"><see cref=" string"></see> representation of primary key values.</param>
      /// <returns><see cref=" DASDataRow"></see> object.</returns>
      public DASDataRow ItemByDBKey(string DBKey)
      {
         DASDataRow ReturnValue = null;
         foreach (DASDataRow row in m_Rows)
         {
            if (row.DBKey == DBKey)
            {
               ReturnValue = row;
               break; 
            }
         }

         return ReturnValue;
      }

      /// <summary>
      /// This function is needed to support for-each-loops.
      /// </summary>
      /// <returns><see cref="System.Collections.IEnumerator"></see> object.</returns>
      public System.Collections.IEnumerator GetEnumerator()
      {
         return m_Rows.GetEnumerator();
      }
   }
}