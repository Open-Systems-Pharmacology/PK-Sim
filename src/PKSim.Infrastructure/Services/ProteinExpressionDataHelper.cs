using System;
using System.Collections;
using System.Data;
using System.Reflection;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.DAS;
using PKSim.Presentation.Presenters.ProteinExpression;

namespace PKSim.Infrastructure.Services
{
  
   public class ProteinExpressionDataHelper : IProteinExpressionDataHelper
   {
      public DataTable CreateDataJoin(DataRelation dr, JoinType jt, string tableName)
      {
         DASHelper.JoinType dasJt;
         switch (jt)
         {
            case JoinType.Inner:
               dasJt = DASHelper.JoinType.Inner;
               break;
            case JoinType.LeftOuter:
               dasJt = DASHelper.JoinType.LeftOuter;
               break;
            case JoinType.RightOuter:
               dasJt = DASHelper.JoinType.RightOuter;
               break;
            case JoinType.FullOuter:
               dasJt = DASHelper.JoinType.FullOuter;
               break;
            default:
               dasJt = DASHelper.JoinType.Inner;
               break;
         }

         return DASHelper.CreateDataJoin(dr, dasJt, tableName);
      }

      /// <summary>
      /// Fills the given data table with given object data.
      /// </summary>
      private static void fillData(PropertyInfo[] properties, DataTable dt, Object o)
      {
         DataRow dr = dt.NewRow();
         foreach (PropertyInfo pi in properties)
         {
            dr[pi.Name] = pi.GetValue(o, null) ?? DBNull.Value;
         }

         dt.Rows.Add(dr);
      }

      /// <summary>
      /// All the Properties for the class array are converted to columns.
      /// </summary>
      private static DataTable createDataTable(PropertyInfo[] properties)
      {
         DataTable dt = new DataTable();
         DataColumn dc = null;
         foreach (PropertyInfo propInfo in properties)
         {
            dc = new DataColumn();
            dc.ColumnName = propInfo.Name;
            Type propType = propInfo.PropertyType;
            if (propType.IsGenericType &&
                propType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
               propType = Nullable.GetUnderlyingType(propType);
            }
            dc.DataType = propType;

            dt.Columns.Add(dc);
         }
         return dt;
      }

      /// <summary>
      /// Converts an arraylist of a class object to a data table object.
      /// </summary>
      public DataTable ConvertToDataTable(object[] array)
      {
         PropertyInfo[] properties = array.GetType().GetElementType().GetProperties();
         DataTable dt = createDataTable(properties);
         if (array.Length == 0) return dt;
         foreach (object o in array)
            fillData(properties, dt, o);
         return dt;
      }

   
      /// <summary>
      /// This helping method retrieves a string collection with all distinct values of the given column.
      /// </summary>
      public IList GetDistinctLoV(DataColumn column)
      {
         IList ret = new StringCollection();
         foreach (DataRow row in column.Table.Select(null, column.ColumnName))
         {
            if (!ret.Contains(row[column].ToString()))
            {
               ret.Add(row[column].ToString());
            }
         }
         return ret;
      }
   }

}