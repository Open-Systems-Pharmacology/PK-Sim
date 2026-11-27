using System.Data;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using static PKSim.Core.ExpressionQueryForSpecs;

namespace PKSim.Core
{
   public abstract class concern_for_ExpressionDataTableMapper : ContextSpecification<IExpressionDataTableMapper>
   {
      protected override void Context()
      {
         sut = new ExpressionDataTableMapper();
      }
   }

   public class When_mapping_container_records_to_a_data_table : concern_for_ExpressionDataTableMapper
   {
      private DataTable _result;

      protected override void Because()
      {
         _result = sut.DataTableFrom(new ExpressionQueryCalculator().ContainerRecordsFor(Query(), Containers()));
      }

      [Observation]
      public void should_have_a_column_for_each_record_field_and_for_the_container()
      {
         _result.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ShouldOnlyContainInOrder(
            ExpressionDataFields.VARIANT_NAME, ExpressionDataFields.DATA_BASE, ExpressionDataFields.DATA_BASE_REC_ID, ExpressionDataFields.GENDER,
            ExpressionDataFields.TISSUE, ExpressionDataFields.HEALTH_STATE, ExpressionDataFields.SAMPLE_SOURCE, ExpressionDataFields.AGE_MIN,
            ExpressionDataFields.AGE_MAX, ExpressionDataFields.SAMPLE_COUNT, ExpressionDataFields.TOTAL_COUNT, ExpressionDataFields.RATIO,
            ExpressionDataFields.NORM_VALUE, ExpressionDataFields.UNIT, ExpressionDataFields.CONTAINER, ExpressionDataFields.CONTAINER_DISPLAY_NAME);
      }

      [Observation]
      public void should_have_a_row_for_each_record_assigned_to_a_container()
      {
         var intestineRows = _result.Rows.Cast<DataRow>().Where(x => Equals(x[ExpressionDataFields.TISSUE], "intestine")).ToList();
         intestineRows.Select(x => x[ExpressionDataFields.CONTAINER_DISPLAY_NAME]).ShouldOnlyContain("Duodenum D", "Jejunum D");
      }

      [Observation]
      public void should_store_missing_values_as_null_values()
      {
         _result.Rows.Cast<DataRow>().Count(x => x.IsNull(ExpressionDataFields.NORM_VALUE)).ShouldBeEqualTo(1);
      }
   }

   public class When_mapping_a_tissue_container_mapping_to_a_table_and_back : concern_for_ExpressionDataTableMapper
   {
      private DataTable _mappingTable;

      protected override void Because()
      {
         _mappingTable = sut.MappingTableFrom(Mapping());
      }

      [Observation]
      public void should_store_unmapped_tissues_with_a_null_container()
      {
         _mappingTable.Rows.Cast<DataRow>().Single(x => Equals(x[ExpressionDataFields.TISSUE], "skin")).IsNull(ExpressionDataFields.CONTAINER).ShouldBeTrue();
      }

      [Observation]
      public void should_return_the_same_mapping()
      {
         sut.MappingFrom(_mappingTable).Select(x => $"{x.Tissue}->{x.Container}").ShouldOnlyContainInOrder(Mapping().Select(x => $"{x.Tissue}->{x.Container}"));
      }
   }
}
