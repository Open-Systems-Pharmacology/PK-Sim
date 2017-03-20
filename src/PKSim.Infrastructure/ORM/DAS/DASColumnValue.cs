using System;
using System.Data;

namespace PKSim.Infrastructure.ORM.DAS
{
   public class DASColumnValue
   {
      private readonly DASDataRow _row;

      private readonly DASDataColumn _column;
      /// <summary>
      /// This is the constructor of the class.
      /// </summary>
      /// <param name="Row"><see cref=" DASDataRow "></see> object needed for initialization.</param>
      /// <param name="Column"><see cref=" DASDataColumn "></see> object needed for initialization.</param>
      public DASColumnValue(DASDataRow Row, DASDataColumn Column)
      {
         _row = Row;
         _column = Column;
      }

      /// <summary>
      /// This sub initialized the column value.
      /// </summary>
      /// <param name="value">Value needed for initialization.</param>
      /// <remarks>
      /// For compatibility reasons.
      /// Sets current value of the column. Only usable when row has <see cref="DataRow.RowState"></see> =  <see cref="DataRowState.Added"></see>.
      /// </remarks>
      /// <exception cref="InvalidRowStateException">Thrown when <see cref="DataRow.RowState"></see> is not <see cref="DataRowState.Added"></see>.</exception>
      public void InitByValue(object value)
      {
         if (_row.RowState == DataRowState.Added | _row.RowState == DataRowState.Detached)
         {
            _row[_column] = value;
         }
         else
         {
            throw new InvalidRowStateException(_row.RowState);
         }
      }

      /// <summary>
      /// This sub initialized the column with null value.
      /// </summary>
      /// <remarks>
      /// For compatibility reasons.
      /// Sets current value of the column. Only usable when row has <see cref="DataRow.RowState"></see> = <see cref="DataRowState.Added"></see>.</remarks>
      /// <exception cref="InvalidRowStateException">Thrown when <see cref="DataRow.RowState"></see> is not <see cref="DataRowState.Added"></see>.</exception>
      public void InitAsNull()
      {
         if (_row.RowState == DataRowState.Added | _row.RowState == DataRowState.Detached)
         {
            _row[_column] = DBNull.Value;
         }
         else
         {
            throw new InvalidRowStateException(_row.RowState);
         }
      }

      /// <summary>
      /// This property gives the value of column with <see cref="DataRowVersion"></see> = <see cref="DataRowVersion.Original"></see>.
      /// </summary>
      /// <returns>Value of column with <see cref="DataRowVersion"></see> = <see cref="DataRowVersion.Original"></see>.</returns>
      public object DBValue
      {
         get { return _row[_column, DataRowVersion.Original]; }
      }

      /// <summary>
      /// This property gives the value of the column.
      /// </summary>
      /// <returns>Value</returns>
      public object Value
      {
         get { return _row[_column.ColumnName]; }
         set { _row[_column.ColumnName] = value; }
      }

      /// <summary>
      /// This property informs about whether the column has been changed.
      /// </summary>
      /// <returns><c>True</c>, if column value has been changed.</returns>
      /// <remarks>If the row has <see cref="DataRow.RowState"></see> = <see cref="DataRowState.Added"></see> the property always returns <c>False</c>.</remarks>
      public bool Changed
      {
         get
         {
            if (_row.RowState == DataRowState.Added)
               return false;
            if (!(NullValue | DBNullValue))
               return !(_row[_column.ColumnName] == _row[_column, DataRowVersion.Original]);
            if (NullValue)
               return !DBNullValue;
            return !NullValue;
         }
      }

      /// <summary>
      /// This property informs whether the value of column with <see cref="DataRowVersion"></see> = <see cref="DataRowVersion.Original"></see> is null.
      /// </summary>
      /// <returns><c>True</c>, if value of column with <see cref="DataRowVersion"></see> = <see cref="DataRowVersion.Original"></see> is null.</returns>
      public bool DBNullValue
      {
         get { return (ReferenceEquals(_row[_column, DataRowVersion.Original], DBNull.Value)); }
      }

      /// <summary>
      /// This property gets and sets the Value to null.
      /// </summary>
      /// <returns><c>True</c>, if column value is null.</returns>
      public bool NullValue
      {
         get { return (ReferenceEquals(_row[_column.ColumnName], DBNull.Value)); }
         set { if (value) _row[_column.ColumnName] = DBNull.Value; }
      }

      /// <summary>
      /// This function gets the value of column with <see cref="DataRowVersion"></see> = <see cref="DataRowVersion.Original"></see> in a string representation.
      /// </summary>
      /// <returns>Value of column with <see cref="DataRowVersion"></see> = <see cref="DataRowVersion.Original"></see> in a string representation.</returns>
      /// <remarks>The value is not formatted with a predefined format pattern. To get the formatted value use the overload method.</remarks>
      public string GetDBValueAsString()
      {
         return DBNullValue ? string.Empty : DBValue.ToString();
      }

      /// <summary>
      /// This function gets the value of column with <see cref="DataRowVersion"></see> = <see cref="DataRowVersion.Original"></see> in a formatted string representation.
      /// </summary>
      /// <param name="format">A valid format for function <see cref="string.Format(System.IFormatProvider,string,object[])"></see>.</param>
      /// <returns>value of column with <see cref="DataRowVersion"></see> = <see cref="DataRowVersion.Original"></see> in a formatted string representation.</returns>
      /// <remarks>Usefull for columns with <see cref="DASDataColumn.DASDataType"></see> = <see cref="DASDataColumn.DASDataTypes.DASDATE "></see>.
      /// <seealso cref="string.Format(System.IFormatProvider,string,object[])"></seealso></remarks>
      public string GetDBValueAsString(string format)
      {
         return DBNullValue ? string.Empty : string.Format(string.Concat("{0:", format, "}"), DBValue);
      }

      /// <summary>
      /// This function gets the value of the column in a string representation.
      /// </summary>
      /// <returns>Column value in a string representation.</returns>
      /// <remarks>The value is not formatted with a predefined format pattern. To get the formatted value use the overload method.</remarks>
      public string GetValueAsString()
      {
         return NullValue ? string.Empty : Value.ToString();
      }

      /// <summary>
      /// This function gets the value of the column in a formatted string representation.
      /// </summary>
      /// <param name="format">A valid format for function <see cref="string.Format(System.IFormatProvider,string,object[])"></see>.</param>
      /// <returns>Column value in a formatted string representation.</returns>
      /// <remarks>Usefull for columns with <see cref="DASDataColumn.DASDataType"></see> = <see cref="DASDataColumn.DASDataTypes.DASDATE "></see>.
      /// <seealso cref="string.Format(System.IFormatProvider,string,object[])"></seealso></remarks>
      public string GetValueAsString(string format)
      {
         return NullValue ? string.Empty : string.Format(string.Concat("{0:", format, "}"), Value);
      }

      /// <summary>
      /// This sub sets the column value by the given string value.
      /// </summary>
      /// <param name="value">Value to be set.</param>
      /// <exception cref="UnsupportedDataTypeException">Thrown when an unsupported data type occurs.</exception>
      public void SetValueAsString(string value)
      {
         if (value.Length == 0)
            Value = DBNull.Value;
         else
         {
            switch (_column.DASDataType)
            {
               case DASDataColumn.DASDataTypes.DASDATE:
                  Value = Convert.ToDateTime(value);
                  break;
               case DASDataColumn.DASDataTypes.DASDOUBLE:
                  Value = Convert.ToDouble(value);
                  break;
               case DASDataColumn.DASDataTypes.DASLONG:
                  Value = Convert.ToInt32(value);
                  break;
               case DASDataColumn.DASDataTypes.DASSTRING:
                  Value = value;
                  break;
               default:
                  throw new UnsupportedDataTypeException(value.GetType().ToString());
            }
         }
      }

   }
}
