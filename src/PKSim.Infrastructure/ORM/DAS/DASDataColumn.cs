using System.Collections;
using System.Data;

namespace PKSim.Infrastructure.ORM.DAS
{
   /// <summary>
   /// This class is a collection for storing a list of string values.
   /// </summary>
   public class StringCollection : CollectionBase
   {
      /// <summary>
      /// This sub adds a string value to the collection.
      /// </summary>
      /// <param name="Value">Value to be added.</param>
      public void Add(string Value)
      {
         List.Add(Value);
      }

      /// <summary>
      /// This function returns the count of values.
      /// </summary>
      /// <returns>Count of values.</returns>
      public new int Count()
      {
         return base.Count;
      }

      /// <summary>
      /// This sub removes a value from the list.
      /// </summary>
      /// <param name="Value">Value to be removed.</param>
      public void Remove(string Value)
      {
         List.Remove(Value);
      }

      /// <summary>
      /// This sub removes a value specified by the index from the list.
      /// </summary>
      /// <param name="Index">Index of the value to be removed.</param>
      public new void RemoveAt(int Index)
      {
         base.RemoveAt(Index);
      }

      /// <summary>
      /// This function checks whether a given value is part of the list.
      /// </summary>
      /// <param name="Value">Value to be checked.</param>
      /// <returns><c>True</c>, if the list contains the value.</returns>
      public bool ContainsValue(string Value)
      {
         return List.Contains(Value);
      }

      /// <summary>
      /// This function returns the value specified by the index.
      /// </summary>
      /// <param name="Index">Index of the value.</param>
      /// <returns>Value</returns>
      public string ItemByIndex(int Index)
      {
         return (string)List[Index];
      }

      /// <summary>
      /// This function is needed to support for-each-loops.
      /// </summary>
      /// <returns><see cref="System.Collections.IEnumerator"></see> object.</returns>
      public new IEnumerator GetEnumerator()
      {
         return base.GetEnumerator();
      }
   }

   /// <summary>
   /// This class is an enhancement of DataColumn class.
   /// </summary>
   public class DASDataColumn : DataColumn
   {

      /// <summary>
      /// This enumeration defines known datatypes.
      /// </summary>
      public enum DASDataTypes
      {
         /// <summary>
         /// This is a datatype for textual values.
         /// </summary>
         DASSTRING,
         /// <summary>
         /// This is a datatype for numeric values without digits after decimal sign.
         /// </summary>
         DASLONG,
         /// <summary>
         /// This is a datatype for numeric values with digits after decimal sign.
         /// </summary>
         DASDOUBLE,
         /// <summary>
         /// This is a datatype for date values.
         /// </summary>
         DASDATE
      }

      private bool m_IsAutoValue;
      private string m_AutoValueCreator = string.Empty;

      private StringCollection m_ListOfValues = new StringCollection();
      /// <summary>
      /// This property must be overloaded to give a DASDataTable instead of a DataTable.
      /// </summary>
      public new DASDataTable Table
      {
         get { return (DASDataTable)base.Table; }
      }

      /// <summary>
      /// This property gives the column name used in the database.
      /// </summary>
      /// <returns>Name of the column in the database.</returns>
      public string DBColumnName { get; set; }

      /// <summary>
      /// This property gets and sets the DTSDataType of the column.
      /// </summary>
      /// <returns>Value of DTSDataTypes</returns>
      /// <exception cref="UnsupportedDataTypeException">Thrown when an unsupported data type occurs.</exception>
      public DASDataTypes DASDataType
      {
         get
         {
            if (ReferenceEquals(DataType, System.Type.GetType("System.String")))
               return DASDataTypes.DASSTRING;
            if (ReferenceEquals(DataType, System.Type.GetType("System.DateTime")))
               return DASDataTypes.DASDATE;
            if (ReferenceEquals(DataType, System.Type.GetType("System.Double")))
               return DASDataTypes.DASDOUBLE;
            if (ReferenceEquals(DataType, System.Type.GetType("System.Decimal")))
               return DASDataTypes.DASDOUBLE;
            if (ReferenceEquals(DataType, System.Type.GetType("System.Int32")))
               return DASDataTypes.DASLONG;
            throw new UnsupportedDataTypeException(DataType.ToString());
         }
         set
         {
            switch (value)
            {
               case DASDataTypes.DASDATE:
                  DataType = System.Type.GetType("System.DateTime");
                  break;
               case DASDataTypes.DASDOUBLE:
                  DataType = System.Type.GetType("System.Double");
                  break;
               case DASDataTypes.DASLONG:
                  DataType = System.Type.GetType("System.Int32");
                  break;
               case DASDataTypes.DASSTRING:
                  DataType = System.Type.GetType("System.String");
                  break;
               default:
                  throw new UnsupportedDataTypeException(value.ToString());
            }
         }
      }

      /// <summary>
      /// This property is true if the values for this column are created automatically by the database.
      /// </summary>
      /// <returns><c>True</c>, if column value is created automatically.</returns>
      public bool IsAutoValue
      {
         get { return m_IsAutoValue; }
      }

      /// <summary>
      /// Use this property to get and set the auto value creator object information.
      /// </summary>
      /// <value>Name of the oracle sequence object or a string representing table name and column name in a form like <c>table.column</c>.</value>
      /// <returns>The auto value creator object information.</returns>
      /// <remarks>Only useful for datatype <see cref="DASDataTypes.DASLONG"></see>.
      /// Sets also property <see cref="IsAutoValue"></see> to <c>True</c>.
      /// Property <see cref="DataColumn.DefaultValue"></see> is set to <c>0</c>.</remarks>
      /// <exception cref="UnsupportedDataTypeForAutoValueCreationException">Thrown when column has an unsupported data type.</exception>
      public string AutoValueCreator
      {
         get { return m_AutoValueCreator; }
         set
         {
            if (DASDataType != DASDataTypes.DASLONG)
               throw new UnsupportedDataTypeForAutoValueCreationException(DataType.ToString());
            m_AutoValueCreator = value;
            m_IsAutoValue = true;
            DefaultValue = 0;
         }
      }

      /// <summary>
      /// This property is true if this column should not be shown in a control.
      /// </summary>
      public bool Hidden { get; set; }

      /// <summary>
      /// This properties gives a list of values.
      /// </summary>
      /// <remarks>In the database only these values are allowed.</remarks>
      public StringCollection ListOfValues
      {
         get { return m_ListOfValues; }
         set { m_ListOfValues = value; }
      }

   }

   /// <summary>
   /// This class is an encapsulation of DataColumnCollection class. 
   /// </summary>
   /// <remarks>It is no real collection object. It just encapsulates the columns collection of the DataTable.</remarks>
   public class DASDataColumnCollection
   {

      private readonly DataColumnCollection m_Columns;
      /// <summary>
      /// This is the constructor of the class.
      /// </summary>
      /// <param name="Columns">Columns collection use in the DataTable object.</param>
      public DASDataColumnCollection(DataColumnCollection Columns)
      {
         m_Columns = Columns;
      }

      /// <summary>
      /// This sub removes all columns from the collection.
      /// </summary>
      public void Clear()
      {
         m_Columns.Clear();
      }

      /// <summary>
      /// This sub adds the given column to the collection.
      /// </summary>
      /// <param name="Column"><see cref=" DASDataColumn "></see> object to be added.</param>
      public void Add(DASDataColumn Column)
      {
         m_Columns.Add(Column);
      }

      /// <summary>
      /// This function gives the count of columns.
      /// </summary>
      /// <returns>Count of columns.</returns>
      public int Count()
      {
         return m_Columns.Count;
      }

      /// <summary>
      /// This sub removes the given column from the collection.
      /// </summary>
      /// <param name="Column"><see cref=" DASDataColumn "></see> object to be removed.</param>
      public void Remove(DASDataColumn Column)
      {
         m_Columns.Remove(Column);
      }

      /// <summary>
      /// This sub removes the column specified by the given index from the collection.
      /// </summary>
      /// <param name="Index">Index of <see cref=" DASDataColumn "></see> object to be removed.</param>
      public void RemoveAt(int Index)
      {
         m_Columns.RemoveAt(Index);
      }

      /// <summary>
      /// This function checks whether a column with specified name exists.
      /// </summary>
      /// <param name="Name">Name of the column.</param>
      /// <returns><c>True</c>, if column is part of the columns collection.</returns>
      public bool ContainsName(string Name)
      {
         return m_Columns.Contains(Name);
      }

      /// <summary>
      /// This function checks whether a column exists.
      /// </summary>
      /// <param name="Column"><see cref=" DASDataColumn "></see> to be checked.</param>
      /// <returns><c>True</c>, if column is part of the columns collection.</returns>
      public bool ContainsColumn(DASDataColumn Column)
      {
         return m_Columns.Contains(Column.ColumnName);
      }

      /// <summary>
      /// This function gives the column by the specified name.
      /// </summary>
      /// <param name="Name">Name of the column.</param>
      /// <returns>DASDataColumn object.</returns>
      public DASDataColumn ItemByName(string Name)
      {
         return (DASDataColumn)m_Columns[m_Columns.IndexOf(Name)];
      }

      /// <summary>
      /// This function gives the column by the specified index.
      /// </summary>
      /// <param name="Index">Index of the column.</param>
      /// <returns>DASDataColumn object.</returns>
      public DASDataColumn ItemByIndex(int Index)
      {
         return (DASDataColumn)m_Columns[Index];
      }

      /// <summary>
      /// This function is needed to support for-each-loops.
      /// </summary>
      /// <returns><see cref="System.Collections.IEnumerator"></see> object.</returns>
      public IEnumerator GetEnumerator()
      {
         return m_Columns.GetEnumerator();
      }
   }
}