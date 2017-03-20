using System;
using System.Collections;
using System.Data;

namespace PKSim.Infrastructure.ORM.DAS
{
   public static class DASHelper
   {

      /// <summary>
      /// This enumeration declares the supported join types.
      /// </summary>
      public enum JoinType
      {
         /// <summary>
         /// The join type <c>Inner</c> means an inner join.
         /// </summary>
         /// <remarks>Only those rows where the column values are the same will be in the resulting table.</remarks>
         Inner,
         /// <summary>
         /// The join type <c>LeftOuter</c> means a left outer join.
         /// </summary>
         /// <remarks>All rows of the parent table and only those of the child table which column values are the same will be in the resulting table. </remarks>
         LeftOuter,
         /// <summary>
         /// The join type <c>RightOuter</c> means a right outer join.
         /// </summary>
         /// <remarks>All rows of the child table and only those of the parent table which column values are the same will be in the resulting table.</remarks>
         RightOuter,
         /// <summary>
         /// The join type <c>FullOuter</c> means a full outer join.
         /// </summary>
         /// <remarks>All rows of the child table and all rows of the parent table will be in the resulting table.</remarks>
         FullOuter
      }

      /// <summary>
      /// This function creates a new data table which is the result of a join of two data tables of a data set which is given by a data relation.
      /// </summary>
      /// <param name="Relation">The data relation object which specifies the relation of two tables in a data set.</param>
      /// <param name="JoinType">The type of join which is used by combining the two data tables.</param>
      /// <param name="TableName">The name of the new data table.</param>
      /// <returns>A data table object with all parent and all child columns.</returns>
      /// <remarks><para>The function can be used to join tables directly in memory without any database interaction.</para>
      /// <para>Child columns used for the relation are skipped.</para>
      /// <para>If a naming conflict occurs the child column will be named in a form <code>TableName.ColumnName</code>.</para></remarks>
      public static DataTable CreateDataJoin(DataRelation Relation, JoinType JoinType, string TableName)
      {

         if ((Relation == null))
            throw new ArgumentNullException("Relation");
         if ((string.IsNullOrEmpty(TableName)))
            throw new ArgumentNullException("TableName");

         var returnValue = new DataTable(TableName);

         // create new columns
         // add all parent columns
         foreach (DataColumn col in Relation.ParentTable.Columns)
         {
            returnValue.Columns.Add(col.ColumnName, col.DataType);
         }

         // add also all child columns not used in the relation
         // if there is a naming conflict take the table name of the child table as praefix
         foreach (DataColumn col in Relation.ChildTable.Columns)
         {
            IList newColumns = Relation.ChildColumns;
            if ((newColumns.Contains(col)))
               continue;
            if ((returnValue.Columns.Contains(col.ColumnName)))
            {
               returnValue.Columns.Add(string.Concat(Relation.ChildTable.TableName, ".", col.ColumnName), col.DataType);
            }
            else
            {
               returnValue.Columns.Add(col.ColumnName, col.DataType);
            }
         }

         returnValue.BeginInit();

         //do the inner join

         foreach (DataRow prow in Relation.ParentTable.Rows)
         {

            foreach (DataRow crow in prow.GetChildRows(Relation))
            {
               DataRow newRow = returnValue.NewRow();
               foreach (DataColumn col in returnValue.Columns)
               {
                  newRow[col] = DBNull.Value;
               }
               foreach (DataColumn col in Relation.ParentTable.Columns)
               {
                  newRow[col.ColumnName] = prow[col.ColumnName];
               }
               foreach (DataColumn col in Relation.ChildTable.Columns)
               {
                  IList newColumns = Relation.ChildColumns;
                  if (newColumns.Contains(col))
                     continue;
                  if (returnValue.Columns.Contains(string.Concat(Relation.ChildTable.TableName, ".", col.ColumnName)))
                  {
                     newRow[string.Concat(Relation.ChildTable.TableName, ".", col.ColumnName)] = crow[col];
                  }
                  else
                  {
                     newRow[col.ColumnName] = crow[col];
                  }
               }
               returnValue.Rows.Add(newRow);
            }
         }

         //do left outer join, add all parent rows having no child rows
         if (JoinType == JoinType.LeftOuter | JoinType == JoinType.FullOuter)
         {

            foreach (DataRow prow in Relation.ParentTable.Rows)
            {

               if ((prow.GetChildRows(Relation).Length == 0))
               {
                  DataRow newRow = returnValue.NewRow();
                  foreach (DataColumn col in returnValue.Columns)
                  {
                     newRow[col] = DBNull.Value;
                  }
                  foreach (DataColumn col in Relation.ParentTable.Columns)
                  {
                     newRow[col.ColumnName] = prow[col.ColumnName];
                  }
                  returnValue.Rows.Add(newRow);
               }
            }
         }

         // do right outer join, add all child rows having no parent row

         if (JoinType == JoinType.RightOuter | JoinType == JoinType.FullOuter)
         {
            foreach (DataRow crow in Relation.ChildTable.Rows)
            {

               if (crow.GetParentRows(Relation).Length == 0)
               {
                  DataRow newRow = returnValue.NewRow();
                  foreach (DataColumn col in returnValue.Columns)
                  {
                     newRow[col] = DBNull.Value;
                  }
                  foreach (DataColumn col in Relation.ChildTable.Columns)
                  {
                     IList newColumns = Relation.ChildColumns;
                     if (newColumns.Contains(col))
                        continue;
                     if ((returnValue.Columns.Contains(string.Concat(Relation.ChildTable.TableName, ".", col.ColumnName))))
                     {
                        newRow[string.Concat(Relation.ChildTable.TableName, ".", col.ColumnName)] = crow[col];
                     }
                     else
                     {
                        newRow[col.ColumnName] = crow[col];
                     }
                     returnValue.Rows.Add(newRow);
                  }
               }
            }
         }

         returnValue.EndInit();
         returnValue.AcceptChanges();

         return returnValue;
      }

   }
}
