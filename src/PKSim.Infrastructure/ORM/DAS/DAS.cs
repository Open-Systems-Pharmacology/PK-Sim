using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace PKSim.Infrastructure.ORM.DAS
{
   /// <summary>
   ///    This class is the main class of the DAS-Layer. It supports helpfull properties and methods to connect and access
   ///    databases.
   /// </summary>
   /// <remarks>It uses Ole DB data providers for accessing oracle or microsoft access databases.</remarks>
   public class DAS : IDisposable
   {
      private const string STR_FROM = " FROM ";

      private const string STR_WHERE = " WHERE ";
      private DbProviderFactory _providerFactory;

      public DbConnection Connection { get; private set; }

      //this is just a dummy command to create a parameters collection to store parameters
      private DbCommand _command;
      private DbParameterCollection _parameters;
      private DbTransaction _transaction;
      private string _provider;
      private string _connectionString;
      private bool _transactionOpen;

      /// <summary>
      ///    This enumeration classifies the parameters you can define on the database to support parameterized statements.
      /// </summary>
      public enum ParameterModes
      {
         /// <summary>
         ///    This should be used for input parameters only.
         /// </summary>
         PARM_IN = ParameterDirection.Input,

         /// <summary>
         ///    This should be used for output parameters only.
         /// </summary>
         PARM_OUT = ParameterDirection.Output,

         /// <summary>
         ///    This should be used for parameters that are both input and output.
         /// </summary>
         PARM_INOUT = ParameterDirection.InputOutput,

         /// <summary>
         ///    This should be used for parameters that are used as return value of stored procedures.
         /// </summary>
         PARM_RETURN = ParameterDirection.ReturnValue
      }

      /// <summary>
      ///    This enumeration classified the datatype of a parameter you can define on the database to support parameterized
      ///    statements.
      /// </summary>
      public enum ServerTypes
      {
         /// <summary>
         ///    This means the parameter is used to hold numeric values.
         /// </summary>
         NUMBER = DbType.Double,

         /// <summary>
         ///    This means the parameter is used to hold textual values.
         /// </summary>
         STRING = DbType.String,

         /// <summary>
         ///    This means the parameter is used to hold date values.
         /// </summary>
         DATE = DbType.Date
      }

      /// <summary>
      ///    This function creates a database connection using the given parameters.
      /// </summary>
      /// <param name="dataProvider">Name of the data provider which should be used for the connection.</param>
      /// <param name="connectionString">A valid connection string for the specified data provider.</param>
      /// <returns><c>True</c>, if connection could be established.</returns>
      /// <exception cref="AlreadyConnectedException">Thrown when there is already an established connection.</exception>
      /// <exception cref="UnknownDataProvider">Thrown when the specified DataProvider is unknown.</exception>
      public bool Connect(string dataProvider, string connectionString)
      {
         bool returnValue;

         if (!IsConnected)
         {
            if ((dataProvider == "System.Data.SQLite"))
            {
               _providerFactory = new SQLiteFactory();
            }
            else
            {
               try
               {
                  _providerFactory = DbProviderFactories.GetFactory(dataProvider);
               }
               catch (Exception)
               {
                  throw new UnknownDataProvider();
               }
            }

            Connection = _providerFactory.CreateConnection();
            Connection.ConnectionString = connectionString;
            Connection.Open();
            if (Connection.State == ConnectionState.Open)
            {
               _provider = dataProvider;
               _connectionString = connectionString;
               _command = _providerFactory.CreateCommand();
               _parameters = _command.Parameters;
               returnValue = true;
            }
            else
            {
               _provider = "";
               _connectionString = "";
               returnValue = false;
            }
         }
         else
         {
            throw new AlreadyConnectedException();
         }

         _transactionOpen = false;

         return returnValue;
      }

      /// <summary>
      ///    This function creates a database connection using the given parameters.
      /// </summary>
      /// <param name="dbName">
      ///    Name of the database to connect to.
      ///    <para>
      ///       If <see cref="DataProviders.MSAccess"></see> or <see cref="DataProviders.MSAccessWithDatabaseSecurity"></see> or
      ///       <see cref="DataProviders.MSAccessWithWorkgroupSecurity"></see> is specified as provider it has to be the filename
      ///       of the Microsoft Access database file (*.mdb).
      ///    </para>
      ///    <para>
      ///       If <see cref="DataProviders.Oracle"></see> is specified as provider the alias for a Oracle database used in
      ///       the SQL*Net enviroment must be specified (tnsnames.ora).
      ///    </para>
      /// </param>
      /// <param name="workgroupDb">Filename of the workgroup database.</param>
      /// <param name="user">Name of the database user used for the connection.</param>
      /// <param name="password">Password of the database user used for the connection.</param>
      /// <param name="dataProvider">
      ///    Provider for connection.
      ///    <para><see cref="DataProviders.MSAccess"></see> means a connection to a Microsoft Access database.</para>
      ///    <para>
      ///       <see cref="DataProviders.MSAccessWithDatabaseSecurity"></see> means a connection to a Microsoft Access database
      ///       with database security.
      ///       That means there is a password defined within the database.
      ///    </para>
      ///    <para>
      ///       <see cref="DataProviders.MSAccessWithWorkgroupSecurity"></see> means a connection to a Microsoft Access
      ///       database with workgroup security.
      ///    </para>
      ///    <para>
      ///       <see cref="DataProviders.Oracle"></see> means a connection to an Oracle database using the oracle data
      ///       provider.
      ///    </para>
      /// <para>
      ///    <see cref="DataProviders.MSOracle"></see> means a connection to an Oracle database using the microsoft data
      ///    provider.
      /// </para>
      /// </param>
      /// <returns><c>True</c>, if connection could be established.</returns>
      /// <exception cref="AlreadyConnectedException">Thrown when there is already an established connection.</exception>
      /// <exception cref="UnknownDataProvider">Thrown when the specified DataProvider is unknown.</exception>
      public bool Connect(string dbName, string workgroupDb, string user, string password, DataProviders dataProvider)
      {
         bool returnValue = false;

         if (!IsConnected)
         {
            var connBuilder = new DbConnectionStringBuilder();
            string sProvider;

            switch (dataProvider)
            {
               case DataProviders.MSAccess:
                  sProvider = "System.Data.OleDb";
                  connBuilder.Add("Provider", "Microsoft.Jet.OLEDB.4.0");
                  connBuilder.Add("Data Source", dbName);
                  connBuilder.Add("User ID", user);
                  connBuilder.Add("Password", password);
                  try
                  {
                     returnValue = Connect(sProvider, connBuilder.ConnectionString);
                  }
                  catch (Exception)
                  {
                  }

                  if (!IsConnected)
                  {
                     connBuilder["Provider"] = "Microsoft.ACE.OLEDB.12.0";
                  }

                  break;

               case DataProviders.SQLite:
                  sProvider = "System.Data.SQLite";
                  connBuilder.Add("Data Source", dbName);
                  if (!string.IsNullOrEmpty(password))
                  {
                     connBuilder.Add("Password", password);
                  }

                  break;
               default:
                  throw new UnknownDataProvider();
            }

            if (!IsConnected)
            {
               returnValue = Connect(sProvider, connBuilder.ConnectionString);
            }
         }
         else
         {
            throw new AlreadyConnectedException();
         }

         return returnValue;
      }

      public bool Connect(string dbName, string user, string password, DataProviders dataProvider)
      {
         return Connect(dbName, string.Empty, user, password, dataProvider);
      }

          /// <summary>
      ///    This property gives the information whether there is an opened transaction.
      /// </summary>
      /// <exception cref="NotConnectedException">Thrown when there is no established connection.</exception>
      public bool IsTransactionOpen
      {
         get
         {
            if (IsConnected)
            {
               return _transactionOpen;
            }

            throw new NotConnectedException();
         }
      }

      /// <summary>
      ///    This property gives the information whether you are connected to a database.
      /// </summary>
      public bool IsConnected
      {
         get
         {
            if (Connection != null)
            {
               return (Connection.State == ConnectionState.Open);
            }

            return false;
         }
      }

      /// <summary>
      ///    This property gives the name of the database you are connected to.
      /// </summary>
      /// <exception cref="NotConnectedException">Thrown when there is no established connection.</exception>
      public string DatabaseName
      {
         get
         {
            if (IsConnected)
               return String.IsNullOrEmpty(Connection.Database)
                  ? Connection.DataSource
                  : Connection.Database;
            throw new NotConnectedException();
         }
      }

      /// <summary>
      ///    This function can be used to execute a given SQL in the connected database.
      /// </summary>
      /// <param name="sql">SQL-statement to be executed. Can be a DML or a DDL statement.</param>
      /// <returns>The number of affected rows.</returns>
      /// <remarks>
      ///    A DDL (Data Definition Language) statement makes an implicit commit.
      ///    Therefor if an transaction is opened this transaction is also commited.
      /// </remarks>
      /// <exception cref="NotConnectedException">Thrown when there is no established connection.</exception>
      public int ExecuteSQL(string sql)
      {
         int functionReturnValue;
         if (IsConnected)
         {
            var cmd = _providerFactory.CreateCommand();
            cmd.CommandText = sql;
            cmd.Connection = Connection;
            foreach (DbParameter param in _parameters)
            {
               if (!sql.Contains(param.ParameterName)) continue;
               var newParam = _providerFactory.CreateParameter();
               newParam.ParameterName = param.ParameterName;
               newParam.Direction = param.Direction;
               newParam.DbType = param.DbType;
               newParam.Value = param.Value;

               cmd.Parameters.Add(newParam);
            }

            cmd.Transaction = _transaction;

            functionReturnValue = cmd.ExecuteNonQuery();
            foreach (DbParameter param in cmd.Parameters)
               if (param.Direction != ParameterDirection.Input)
                  _parameters[param.ParameterName].Value = param.Value;
         }
         else
         {
            throw new NotConnectedException();
         }

         return functionReturnValue;
      }

      private static string getDbTableName(string sql)
      {
         var returnValue = sql.ToUpper();
         if (returnValue.Contains(STR_FROM))
         {
            returnValue = returnValue.Substring(returnValue.IndexOf(STR_FROM) + STR_FROM.Length);
            if (returnValue.Contains(STR_WHERE))
               returnValue = returnValue.Substring(0, returnValue.IndexOf(STR_WHERE));

            if (returnValue.Contains(",") | returnValue.Contains("("))
               returnValue = string.Empty;
            else
            {
               var sa = returnValue.Trim().Split(' ');
               returnValue = sa[0];
            }
         }
         else
            returnValue = string.Empty;

         return returnValue;
      }

      /// <summary>
      ///    This property gives the character used to build the DBKey.
      ///    The DBKey is a <see cref="String"></see> representation of the primary key column values.
      /// </summary>
      /// <remarks>You can use this to build DBKey value by yourself.</remarks>
      public char KeyDelimiter { get; set; } = '|';

      /// <summary>
      ///    This sub creates a given <see cref="DASDataTable"></see> object with the schema information found for the given
      ///    table.
      /// </summary>
      /// <param name="tableName">Name of the database table which schema should be used.</param>
      /// <exception cref="NotConnectedException">Thrown when there is no established connection.</exception>
      public DASDataTable CreateDASDataTable(string tableName = "Query")
      {
         if ((string.IsNullOrEmpty(tableName)))
            throw new ArgumentNullException(nameof(tableName));

         return CreateDASDataTable("*", tableName);
      }

      /// <summary>
      ///    This sub creates a given <see cref="DASDataTable"></see> object with the schema information found for the given
      ///    table.
      /// </summary>
      /// <param name="columnList">String with a comma separated list of all columns.</param>
      /// <param name="tableName">Name of the database table which schema should be used.</param>
      /// <exception cref="ArgumentNullException"></exception>
      /// <remarks>
      ///    <para>
      ///       You can use the column list to select the needed columns. You can also rename the database column by using
      ///       aliases in the list.
      ///    </para>
      ///    <para>
      ///       To use aliases just name them like in normal sql. For example
      ///       <quote>column1 as mycolumn1, column2 as mycolumn2</quote>.
      ///    </para>
      /// </remarks>
      /// <exception cref="NotConnectedException">Thrown when there is no established connection.</exception>
      public DASDataTable CreateDASDataTable(string columnList, string tableName)
      {
         if ((string.IsNullOrEmpty(columnList)))
            throw new ArgumentNullException(nameof(columnList));

         if ((string.IsNullOrEmpty(tableName)))
            throw new ArgumentNullException(nameof(tableName));

         if (IsConnected)
         {
            var dataTable = new DASDataTable(this);

            var cmd = _providerFactory.CreateCommand();
            cmd.CommandText = $"SELECT {columnList} FROM {tableName}";
            cmd.Connection = Connection;
            cmd.Transaction = _transaction;
            dataTable.Load(cmd.ExecuteReader(CommandBehavior.SchemaOnly));

            var dasDataTable = new DASDataTable(this);
            foreach (DataColumn column in dataTable.Columns)
            {
               var dasColumn = new DASDataColumn
               {
                  ColumnName = column.ColumnName,
                  DBColumnName = column.ColumnName,
                  DataType = column.DataType,
                  MaxLength = column.MaxLength,
                  AllowDBNull = column.AllowDBNull,
                  Unique = column.Unique
               };
               dasDataTable.Columns.Add(dasColumn);
            }

            var keys = new DASDataColumn[dataTable.PrimaryKey.Length + 1];
            var i = 0;
            foreach (var column in dataTable.PrimaryKey)
            {
               keys[i] = dasDataTable.Columns.ItemByName(column.ColumnName);
               i += 1;
            }

            dasDataTable.PrimaryKey = keys;
            dasDataTable.DBTableName = tableName;
            dasDataTable.BuildAndSetBaseSQL();
            return dasDataTable;
         }

         throw new NotConnectedException();
      }

      /// <summary>
      ///    This sub returns <see cref="DASDataTable"></see> object with the resulting rows of the given SQL Query.
      /// </summary>
      /// <param name="sql">SQL-Statement to query data. Must be a SELECT-Statement.</param>
      /// <remarks>
      ///    You can use parameters in the SQL-Statement to increase performance and making the SQL-statement reusable.
      ///    <seealso cref="AddParameter(string,object,PKSim.Infrastructure.ORM.DAS.DAS.ParameterModes)"></seealso>
      ///    <seealso
      ///       cref="AddParameter(string,object,PKSim.Infrastructure.ORM.DAS.DAS.ParameterModes,PKSim.Infrastructure.ORM.DAS.DAS.ServerTypes)">
      ///    </seealso>
      ///    Don't forget to remove the parameter later on with <see cref="RemoveParameter "></see>, because a parameter
      ///    name is unique within the whole session and trying to create some with an used name will cause an error.
      /// </remarks>
      /// <exception cref="NotConnectedException">Thrown when there is no established connection.</exception>
      public DASDataTable CreateAndFillDataTable(string sql)
      {
         var dt = CreateDASDataTable(getDbTableName(sql));
         FillDataTable(dt, sql);
         return dt;
      }

      /// <summary>
      ///    This sub fills a given <see cref="DASDataTable"></see> object with the resulting rows of the given SQL Query.
      /// </summary>
      /// <param name="dataTable"><see cref="DASDataTable"></see> object to be filled.</param>
      /// <param name="sql">SQL-Statement to query data. Must be a SELECT-Statement.</param>
      /// <remarks>
      ///    You can use parameters in the SQL-Statement to increase performance and making the SQL-statement reusable.
      ///    <seealso cref="AddParameter(string,object,PKSim.Infrastructure.ORM.DAS.DAS.ParameterModes)"></seealso>
      ///    <seealso
      ///       cref="AddParameter(string,object,PKSim.Infrastructure.ORM.DAS.DAS.ParameterModes,PKSim.Infrastructure.ORM.DAS.DAS.ServerTypes)">
      ///    </seealso>
      ///    Don't forget to remove the parameter later on with <see cref="RemoveParameter "></see>, because a parameter
      ///    name is unique within the whole session and trying to create some with an used name will cause an error.
      /// </remarks>
      /// <exception cref="NotConnectedException">Thrown when there is no established connection.</exception>
      public void FillDataTable(DASDataTable dataTable, string sql)
      {
         if (!IsConnected)
            throw new NotConnectedException();

         var paramValues = new ParameterValue[_parameters.Count];
         var i = 0;

         var cmd = _providerFactory.CreateCommand();
         cmd.CommandText = sql;
         cmd.Connection = Connection;
         cmd.Transaction = _transaction;

         foreach (DbParameter param in _parameters)
         {
            if (!sql.Contains(param.ParameterName)) continue;
            var newParam = _providerFactory.CreateParameter();
            newParam.ParameterName = param.ParameterName;
            newParam.Direction = param.Direction;
            newParam.DbType = param.DbType;
            newParam.Value = param.Value;
            cmd.Parameters.Add(newParam);
         }

         //store actual parameters and their values to support refreshing later on
         foreach (DbParameter parameter in _parameters)
         {
            var paramValue = new ParameterValue {ParameterName = parameter.ParameterName, Value = parameter.Value};

            switch (parameter.Direction)
            {
               case ParameterDirection.Input:
                  paramValue.ParameterMode = ParameterModes.PARM_IN;
                  break;
               case ParameterDirection.Output:
                  paramValue.ParameterMode = ParameterModes.PARM_OUT;
                  break;
               case ParameterDirection.InputOutput:
                  paramValue.ParameterMode = ParameterModes.PARM_INOUT;
                  break;
               case ParameterDirection.ReturnValue:
                  paramValue.ParameterMode = ParameterModes.PARM_RETURN;
                  break;
            }

            switch (parameter.DbType)
            {
               case DbType.Date:
               case DbType.DateTime:
               case DbType.Time:
                  paramValue.ServerType = ServerTypes.DATE;
                  break;
               case DbType.Decimal:
               case DbType.Double:
               case DbType.Int16:
               case DbType.Int32:
               case DbType.Int64:
               case DbType.Single:
               case DbType.UInt16:
               case DbType.UInt32:
               case DbType.UInt64:
                  paramValue.ServerType = ServerTypes.NUMBER;
                  break;
               case DbType.AnsiString:
               case DbType.AnsiStringFixedLength:
               case DbType.String:
               case DbType.StringFixedLength:
                  paramValue.ServerType = ServerTypes.STRING;
                  break;
               default:
                  throw new UnsupportedDataTypeException(parameter.DbType.ToString());
            }

            paramValues[i] = paramValue;
            i += 1;
         }

         dataTable.ParameterValues = paramValues;

         dataTable.SQL = sql;
         dataTable.Rows.Clear();
         try
         {
            using (var adapter = _providerFactory.CreateDataAdapter())
            {
               adapter.SelectCommand = cmd;
               adapter.Fill(dataTable);
            }
         }
         catch (Exception ex)
         {
            throw new Exception($"Error occurred with statement <{sql}>.", ex);
         }
      }

      /// <summary>
      ///    This function queries the next value of the given AutoValueCreator object.
      /// </summary>
      /// <param name="autoValueCreator">Name of the database table and column separated by a ".".</param>
      /// <returns>Generated AutoValue</returns>
      public object GetAutoValue(string autoValueCreator)
      {
         object returnValue;

         if (IsConnected)
         {
            DbCommand cmd = _providerFactory.CreateCommand();
            if (autoValueCreator.Contains(".") & autoValueCreator.Split('.').Length == 2)
            {
               cmd.CommandText = string.Format("SELECT MAX({0}) + 1 FROM {1}", autoValueCreator.Split('.')[0], autoValueCreator.Split('.')[1]);
            }
            else
            {
               cmd.CommandText = string.Format("SELECT {0}.NEXTVAL FROM DUAL", autoValueCreator);
            }

            cmd.Connection = Connection;
            cmd.Transaction = _transaction;
            returnValue = cmd.ExecuteScalar();
         }
         else
         {
            throw new NotConnectedException();
         }

         return returnValue;
      }

      /// <summary>
      ///    This sub creates a new parameter.
      /// </summary>
      /// <param name="paramName">Name of the parameter.</param>
      /// <param name="paramValue">Value for the parameter.</param>
      /// <param name="paramType">Type of the parameter.</param>
      /// <param name="serverType">ServerType of the parameter.</param>
      /// <remarks>
      ///    <para>
      ///       Each parameter has an identifying name and an associated value.
      ///       You can automatically bind a parameter to SQL and PL/SQL statements of other objects,
      ///       by using the parameter’s name as a placeholder (@Name) in the SQL statement.
      ///       Such use of parameters can simplify dynamic queries and increase program performance.
      ///    </para>
      ///    <para>
      ///       Although each parameter has a name the mapping to the sql statement is done by position.
      ///       That means if you have several parameters in your sql statement the sequence of all distinct
      ///       parameters must match the sequence of your parameters.
      ///    </para>
      ///    <para>
      ///       For example:
      ///       SELECT * FROM TABLE WHERE COLUMN1 = @P1 AND COLUMN2 = @P2
      ///    </para>
      ///    <para>
      ///       Now your parameters must be added in the sequence @P1 and then @P2.
      ///       If you would add them as @P2 and then @P1 the value of @P2 would be matched to @P1!
      ///    </para>
      ///    <para>
      ///       If you set an incorrect <paramref name=" paramType "></paramref>, such as
      ///       <see cref="ParameterModes.PARM_INOUT"></see>
      ///       for a stored procedure parameter type IN, this can result in errors.
      ///       In other words <see cref="ParameterModes.PARM_INOUT"></see> means "for IN OUT parameters only".
      ///       It does not mean that you should use the parameter against one stored procedure that has an IN parameter
      ///       and then use it in another that has an OUT parameter.
      ///       In such a case you should use two parameters.
      ///       Errors caused in this way are rare, but in the case of parameter-related errors, you should verify
      ///       that the <paramref name=" paramType "></paramref> is correct.
      ///    </para>
      ///    <para>
      ///       Don't forget to remove the parameter later on with <see cref="RemoveParameter "></see>, because a parameter
      ///       name is unique within the whole session and trying to create some with an used name will cause an error.
      ///    </para>
      ///    <example>
      ///       This example shows how to use parameters:
      ///       <code lang="C#">
      /// private DASAccess.DASDataTable Query(DAS das, string param1, long param2)
      /// {
      ///   var dt = das.CreateDASDataTable("Table");
      ///   const string PARAM1 = "@Param1";
      ///   const string PARAM2 = "@Param2";
      /// 
      ///   string sql = String.Format(String.Concat(dt.BaseSQL, " WHERE t.COL1 = {0} AND t.COL2 = {1}", PARAM1, PARAM2));
      /// 
      ///   try 
      ///   {
      ///     das.AddParameter(PARAM1, param1, DAS.ParameterModes.PARM_IN, DAS.ServerTypes.STRING);
      ///     das.AddParameter(PARAM2, param2, DAS.ParameterModes.PARM_IN, DAS.ServerTypes.NUMBER);
      /// 
      ///     das.FillDataTable(ref dt, sql);
      ///   }
      ///   finally 
      ///   {
      ///     das.RemoveParameter(PARAM1);
      ///     das.RemoveParameter(PARAM2);
      ///   }
      /// 
      ///   return dt;
      /// }
      /// </code>
      ///    </example>
      /// </remarks>
      /// <exception cref="NotConnectedException">Thrown when there is no established connection.</exception>
      public void AddParameter(string paramName, object paramValue, ParameterModes paramType, ServerTypes serverType)
      {
         if (IsConnected)
         {
            DbParameter param = _providerFactory.CreateParameter();
            param.Direction = (ParameterDirection) paramType;
            param.ParameterName = paramName.StartsWith("@") ? paramName : string.Concat("@", paramName);
            switch (serverType)
            {
               case ServerTypes.DATE:
                  param.DbType = DbType.DateTime;
                  param.Value = paramValue;
                  break;
               case ServerTypes.NUMBER:
                  int variable;
                  if (Int32.TryParse(paramValue.ToString(), out variable))
                  {
                     param.DbType = DbType.Int32;
                     param.Value = variable;
                  }
                  else
                  {
                     param.DbType = DbType.Double;
                     param.Value = Convert.ToDouble(paramValue);
                  }

                  break;
               case ServerTypes.STRING:
                  param.DbType = DbType.String;
                  param.Value = paramValue;
                  break;
            }

            _parameters.Add(param);
         }
         else
         {
            throw new NotConnectedException();
         }
      }

      /// <summary>
      ///    This sub creates a new parameter.
      /// </summary>
      /// <param name="paramName">Name of the parameter.</param>
      /// <param name="paramValue">Value for the parameter.</param>
      /// <param name="paramType">Type of the parameter.</param>
      /// <remarks>
      ///    <para>
      ///       Each parameter has an identifying name and an associated value.
      ///       You can automatically bind a parameter to SQL and PL/SQL statements of other objects,
      ///       by using the parameter’s name as a placeholder (@Name) in the SQL statement.
      ///       Such use of parameters can simplify dynamic queries and increase program performance.
      ///    </para>
      ///    <para>
      ///       Although each parameter has a name the mapping to the sql statement is done by position.
      ///       That means if you have several parameters in your sql statement the sequence of all distinct
      ///       parameters must match the sequence of your parameters.
      ///    </para>
      ///    <para>
      ///       For example:
      ///       SELECT * FROM TABLE WHERE COLUMN1 = @P1 AND COLUMN2 = @P2
      ///    </para>
      ///    <para>
      ///       Now your parameters must be added in the sequence @P1 and then @P2.
      ///       If you would add them as @P2 and then @P1 the value of @P2 would be matched to @P1!
      ///    </para>
      ///    <para>
      ///       If you set an incorrect <paramref name=" paramType "></paramref>, such as
      ///       <see cref="ParameterModes.PARM_INOUT"></see>
      ///       for a stored procedure parameter type IN, this can result in errors.
      ///       In other words <see cref="ParameterModes.PARM_INOUT"></see> means "for IN OUT parameters only".
      ///       It does not mean that you should use the parameter against one stored procedure that has an IN parameter
      ///       and then use it in another that has an OUT parameter.
      ///       In such a case you should use two parameters.
      ///    </para>
      ///    <para>
      ///       Errors caused in this way are rare, but in the case of parameter-related errors, you should verify
      ///       that the <paramref name=" paramType "></paramref> is correct.
      ///    </para>
      ///    <para>
      ///       Don't forget to remove the parameter later on with <see cref="RemoveParameter "></see>, because a parameter
      ///       name is unique within the whole session and trying to create some with an used name will cause an error.
      ///    </para>
      ///    <example>
      ///       This example shows how to use parameters:
      ///       <code lang="C#">
      /// private DASAccess.DASDataTable Query(DAS das, string param1, long param2)
      /// {
      ///   var dt = das.CreateDASDataTable("Table");
      ///   const string PARAM1 = "@Param1";
      ///   const string PARAM2 = "@Param2";
      /// 
      ///   string sql = String.Format(String.Concat(dt.BaseSQL, " WHERE t.COL1 = {0} AND t.COL2 = {1}", PARAM1, PARAM2));
      /// 
      ///   try 
      ///   {
      ///     das.AddParameter(PARAM1, param1, DAS.ParameterModes.PARM_IN, DAS.ServerTypes.STRING);
      ///     das.AddParameter(PARAM2, param2, DAS.ParameterModes.PARM_IN, DAS.ServerTypes.NUMBER);
      /// 
      ///     das.FillDataTable(ref dt, sql);
      ///   }
      ///   finally 
      ///   {
      ///     das.RemoveParameter(PARAM1);
      ///     das.RemoveParameter(PARAM2);
      ///   }
      /// 
      ///   return dt;
      /// }
      /// </code>
      ///    </example>
      /// </remarks>
      /// <exception cref="NotConnectedException">Thrown when there is no established connection.</exception>
      public void AddParameter(string paramName, object paramValue, ParameterModes paramType)
      {
         if (IsConnected)
         {
            DbParameter param = _providerFactory.CreateParameter();
            param.Direction = (ParameterDirection) paramType;
            param.ParameterName = paramName.StartsWith("@") ? paramName : string.Concat("@", paramName);
            param.Value = paramValue;

            _parameters.Add(param);
         }
         else
         {
            throw new NotConnectedException();
         }
      }

      /// <summary>
      ///    This function gives the value of a parameter.
      /// </summary>
      /// <param name="paramName">Name of the parameter.</param>
      /// <returns>Value of the parameter.</returns>
      /// <remarks>
      ///    Each parameter has an identifying name and an associated value.
      ///    You can automatically bind a parameter to SQL and PL/SQL statements of other objects,
      ///    by using the parameter’s name as a placeholder (:Name) in the SQL or PL/SQL statement.
      ///    Such use of parameters can simplify dynamic queries and increase program performance.
      ///    If you set an incorrect <see cref="ParameterModes"></see>, such as <see cref="ParameterModes.PARM_INOUT"></see>
      ///    for a stored procedure parameter type IN, this can result in errors.
      ///    In other words <see cref="ParameterModes.PARM_INOUT"></see> means "for IN OUT parameters only".
      ///    It does not mean that you should use the parameter against one stored procedure that has an IN parameter
      ///    and then use it in another that has an OUT parameter.
      ///    In such a case you should use two parameters.
      ///    Errors caused in this way are rare, but in the case of parameter-related errors, you should verify
      ///    that the <see cref="ParameterModes"></see> is correct.
      /// </remarks>
      /// <exception cref="NotConnectedException">Thrown when there is no established connection.</exception>
      public object GetParameterValue(string paramName)
      {
         if (IsConnected)
         {
            if (paramName.StartsWith("@"))
            {
               return _parameters[paramName].Value;
            }

            return _parameters[string.Concat("@", paramName)].Value;
         }

         throw new NotConnectedException();
      }

      /// <summary>
      ///    This sub removes a parameter.
      /// </summary>
      /// <param name="paramName">Name of the parameter.</param>
      /// <seealso cref="AddParameter(string,object,PKSim.Infrastructure.ORM.DAS.DAS.ParameterModes)"></seealso>
      /// <seealso
      ///    cref="AddParameter(string,object,PKSim.Infrastructure.ORM.DAS.DAS.ParameterModes,PKSim.Infrastructure.ORM.DAS.DAS.ServerTypes)">
      /// </seealso>
      /// <exception cref="NotConnectedException">Thrown when there is no established connection.</exception>
      public void RemoveParameter(string paramName)
      {
         if (IsConnected)
            if (paramName.StartsWith("@"))
               _parameters.RemoveAt(paramName);
            else
               _parameters.RemoveAt(string.Concat("@", paramName));
         else
            throw new NotConnectedException();
      }

      /// <summary>
      ///    This sub disconnects from the connected database.
      /// </summary>
      /// <exception cref="NotConnectedException">Thrown when there is no established connection.</exception>
      public void DisConnect()
      {
         if (!IsConnected)
            throw new NotConnectedException();
         Connection.Close();
         Connection = null;
      }

      /// <summary>
      ///    This sub starts a transaction in the connected database.
      /// </summary>
      /// <remarks>
      ///    After this method has been called, no database transactions are committed until a <see cref=" CommitTrans "></see>
      ///    is issued.
      ///    Alternatively, the session can be rolled back using <see cref=" Rollback"></see>.
      ///    If a transaction has already been started, repeated use of <see cref="BeginTrans"></see> causes an error.
      ///    If <see cref="DASDataRow.UpdateInDB "></see> or <see cref="DASDataRow.DeleteFromDB "></see> methods fail on a given
      ///    row in a
      ///    global transaction after you issued a BeginTrans, be aware that locks will remain on those previously locked rows.
      ///    These locks will persist until you call <see cref=" CommitTrans "></see> or <see cref=" Rollback"></see>.
      /// </remarks>
      /// <exception cref="AlreadyOpenTransactionException">Thrown when there is already an opened transaction.</exception>
      /// <exception cref="NotConnectedException">Thrown when there is no established connection.</exception>
      public void BeginTrans()
      {
         if (!IsConnected)
            throw new NotConnectedException();
         if (_transactionOpen)
            throw new AlreadyOpenTransactionException();
         _transaction = Connection.BeginTransaction();
         _transactionOpen = true;
      }

      /// <summary>
      ///    This sub commits the actual transaction.
      /// </summary>
      /// <remarks>
      ///    <see cref=" CommitTrans "></see> commits all transactions present within the session.
      ///    The transaction is closed afterwards and all locks are unlocked.
      ///    <see cref=" CommitTrans "></see> is valid only when a transaction has been started.
      ///    If a transaction has not been started, use of <see cref=" CommitTrans "></see> causes an error.
      /// </remarks>
      /// <exception cref="NoOpenTransactionException">Thrown when there is no opened transaction.</exception>
      /// <exception cref="NotConnectedException">Thrown when there is no established connection.</exception>
      public void CommitTrans()
      {
         if (!IsConnected)
            throw new NotConnectedException();
         if (!_transactionOpen)
            throw new NoOpenTransactionException();
         _transaction.Commit();
         _transactionOpen = false;
      }

      /// <summary>
      ///    This sub rollbacks the actual transaction.
      /// </summary>
      /// <remarks>
      ///    <see cref=" Rollback"></see> rolls back all pending transactions within the specified session.
      ///    The transaction is closed afterwards and all locks are unlocked.
      ///    <see cref=" Rollback"></see> is valid only when a transaction has been started.
      ///    If a transaction has not been started, use of <see cref=" Rollback"></see> results in an error.
      /// </remarks>
      /// <exception cref="NoOpenTransactionException">Thrown when there is no opened transaction.</exception>
      /// <exception cref="NotConnectedException">Thrown when there is no established connection.</exception>
      public void Rollback()
      {
         if (!IsConnected)
            throw new NotConnectedException();
         if (!_transactionOpen)
            throw new NoOpenTransactionException();
         _transaction.Rollback();
         _transactionOpen = false;
      }

      public void Dispose()
      {
         Connection = null;
      }
   }
}