using System;
using System.Data;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Infrastructure.Extensions;
using PKSim.Infrastructure.ORM.DAS;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_DAS : ContextSpecification<DAS>
   {
      protected DASDataTable _dataTable;

      protected override void Context()
      {
         sut = new DAS();
         sut.Connect(":memory:", string.Empty, string.Empty, DataProviders.SQLite);
      }

      public override void Cleanup()
      {
         sut.DisConnect();
         base.Cleanup();
      }
   }

   public class When_filling_a_data_table_from_a_query_whose_untyped_column_is_null_in_the_first_row : concern_for_DAS
   {
      protected override void Because()
      {
         _dataTable = sut.ExecuteQueryForDataTable("SELECT 1 AS ID, NULL AS NAME UNION ALL SELECT 2, 'CYP3A43'");
      }

      [Observation]
      public void should_type_the_column_from_the_first_non_null_value()
      {
         _dataTable.Columns.ItemByName("NAME").DataType.ShouldBeEqualTo(typeof(string));
      }

      [Observation]
      public void should_load_all_rows()
      {
         _dataTable.Rows.Count().ShouldBeEqualTo(2);
         _dataTable.Rows.ItemByIndex(1)["NAME"].ShouldBeEqualTo("CYP3A43");
      }

      [Observation]
      public void should_mark_the_loaded_rows_as_unchanged()
      {
         _dataTable.Rows.ItemByIndex(0).RowState.ShouldBeEqualTo(DataRowState.Unchanged);
      }
   }

   public class When_filling_a_data_table_from_a_query_whose_untyped_column_is_null_in_every_row : concern_for_DAS
   {
      protected override void Because()
      {
         _dataTable = sut.ExecuteQueryForDataTable("SELECT 1 AS ID, NULL AS NAME");
      }

      [Observation]
      public void should_create_an_untyped_column()
      {
         _dataTable.Columns.ItemByName("NAME").DataType.ShouldBeEqualTo(typeof(object));
      }

      [Observation]
      public void should_load_the_null_value()
      {
         _dataTable.Rows.ItemByIndex(0)["NAME"].ShouldBeEqualTo(DBNull.Value);
      }
   }

   public class When_filling_a_data_table_from_a_query_that_returns_no_rows : concern_for_DAS
   {
      protected override void Context()
      {
         base.Context();
         sut.ExecuteSQL("CREATE TABLE GENES (ID bigint, NAME text, VALUE double)");
      }

      protected override void Because()
      {
         _dataTable = sut.ExecuteQueryForDataTable("SELECT ID, NAME, VALUE, NAME || 'x' AS LABEL FROM GENES");
      }

      [Observation]
      public void should_type_the_declared_columns_from_their_declaration()
      {
         _dataTable.Columns.ItemByName("ID").DataType.ShouldBeEqualTo(typeof(long));
         _dataTable.Columns.ItemByName("NAME").DataType.ShouldBeEqualTo(typeof(string));
         _dataTable.Columns.ItemByName("VALUE").DataType.ShouldBeEqualTo(typeof(double));
      }

      [Observation]
      public void should_create_an_untyped_column_for_an_undeclared_column()
      {
         _dataTable.Columns.ItemByName("LABEL").DataType.ShouldBeEqualTo(typeof(object));
      }
   }

   public class When_filling_a_data_table_from_a_query_whose_declared_column_is_null_in_every_row : concern_for_DAS
   {
      protected override void Context()
      {
         base.Context();
         sut.ExecuteSQL("CREATE TABLE GENES (ID bigint, NAME text)");
         sut.ExecuteSQL("INSERT INTO GENES VALUES (1, NULL)");
      }

      protected override void Because()
      {
         _dataTable = sut.ExecuteQueryForDataTable("SELECT ID, NAME FROM GENES");
      }

      [Observation]
      public void should_type_the_column_from_its_declaration()
      {
         _dataTable.Columns.ItemByName("NAME").DataType.ShouldBeEqualTo(typeof(string));
      }
   }

   public class When_filling_a_data_table_whose_columns_are_already_defined : concern_for_DAS
   {
      protected override void Context()
      {
         base.Context();
         _dataTable = sut.ExecuteQueryForDataTable("SELECT 1 AS ID, 'A' AS NAME");
      }

      protected override void Because()
      {
         sut.FillDataTable(_dataTable, "SELECT 2 AS ID, 'B' AS NAME");
      }

      [Observation]
      public void should_reuse_the_existing_columns()
      {
         _dataTable.Columns.Count().ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_replace_the_rows()
      {
         _dataTable.Rows.Count().ShouldBeEqualTo(1);
         _dataTable.Rows.ItemByIndex(0)["NAME"].ShouldBeEqualTo("B");
      }
   }
}
