using System;
using System.Data;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid;
using FakeItEasy;
using OSPSuite.Core.Extensions;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Mappers;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Extensions;

namespace PKSim.UI.Tests
{
   public abstract class concern_for_PivotGridCellsToDataTableMapper : ContextSpecification<PivotGridCellsToDataTableMapper>
   {
      protected override void Context()
      {
         sut = new PivotGridCellsToDataTableMapper();
      }
   }

   public class when_mapping_from_pivot_grid_cells_to_data_table : concern_for_PivotGridCellsToDataTableMapper
   {
      private DataTable _result;
      private PKAnalysisPivotGridControl _pivotGrid;

      protected override void Context()
      {
         base.Context();
         _pivotGrid = new PKAnalysisPivotGridControl(A.Fake<IClipboardTask>()) {DataSource = generateSourceDataTable()};
         var curveField = new PivotGridField("column", PivotArea.ColumnArea);
         var parameterField = new PivotGridField("row", PivotArea.RowArea);
         var valueField = new PivotGridField("data", PivotArea.DataArea) {SummaryType = PivotSummaryType.Custom};
         _pivotGrid.AddParameterField(parameterField);
         _pivotGrid.AddValueField(valueField);
         _pivotGrid.Fields.Add(curveField);
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_pivotGrid.Cells, s => $"{s} [ml]");
      }

      private DataTable generateSourceDataTable()
      {
         var dataTable = new DataTable("table");

         dataTable.AddColumn("column");
         dataTable.AddColumn("row");
         dataTable.AddColumn<double>("data");

         generateRow(dataTable, 1, 1);
         generateRow(dataTable, 2, 2);
         generateRow(dataTable, 3, double.NaN);
         return dataTable;
      }

      private static void generateRow(DataTable dataTable, int i, double value)
      {
         var row = dataTable.NewRow();
         row["column"] = "column" + i;
         row["row"] = "row" + i;
         row["data"] = value;
         dataTable.Rows.Add(row);
      }

      [Observation]
      public void table_row_names_should_use_the_retriever()
      {
         _result.Rows[0][0].ShouldBeEqualTo("row1 [ml]");
         _result.Rows[1][0].ShouldBeEqualTo("row2 [ml]");
      }

      [Observation]
      public void table_column_names_should_be_mapped_from_the_original_data_table()
      {
         _result.Columns[0].Caption.ShouldBeEmpty();
         _result.Columns[1].Caption.ShouldBeEqualTo("column1");
         _result.Columns[2].Caption.ShouldBeEqualTo("column2");
      }

      [Observation]
      public void table_contains_correct_values_in_correct_locations()
      {
         _result.Rows[0][1].ShouldBeEqualTo(1.0);
         _result.Rows[1][2].ShouldBeEqualTo(2.0);
         _result.Rows[2][3].ShouldBeEqualTo(DBNull.Value);
      }

      [Observation]
      public void should_map_a_table_with_the_correct_number_of_columns()
      {
         // adds a column for the parameter display name
         _result.Columns.Count.ShouldBeEqualTo(_pivotGrid.Cells.ColumnCount + 1);
      }

      [Observation]
      public void should_map_a_table_one_row_per_row_in_source_data()
      {
         _result.Rows.Count.ShouldBeEqualTo(_pivotGrid.Cells.RowCount);
      }
   }
}