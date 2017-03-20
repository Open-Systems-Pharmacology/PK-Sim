using System.Linq;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_TableFormulaToDataRepositoryMapper : ContextSpecification<ITableFormulaToDataRepositoryMapper>
   {
      private IIdGenerator _idGenerator;

      protected override void Context()
      {
         _idGenerator = new IdGenerator();
         sut = new TableFormulaToDataRepositoryMapper(_idGenerator);
      }
   }

   
   public class When_mapping_a_table_formula_to_a_data_repository : concern_for_TableFormulaToDataRepositoryMapper
   {
      private DataRepository _dataRep;
      private TableFormula _table;

      protected override void Context()
      {
         base.Context();
         _table = new TableFormula();
         _table.Dimension = DomainHelperForSpecs.LengthDimensionForSpecs();
         _table.XDimension = DomainHelperForSpecs.NoDimension();
         _table.AddPoint(1,10);
         _table.AddPoint(2,20);
      }

      protected override void Because()
      {
         _dataRep = sut.MapFrom(_table);
      }

      [Observation]
      public void should_return_a_data_repository_containing_two_columns()
      {
         _dataRep.Columns.Count().ShouldBeEqualTo(2);
      }

      [Observation]
      public void the_base_grid_column_should_have_the_xdimension_of_the_table_formula ()
      {
         var baseGrid = _dataRep.BaseGrid;
         baseGrid.Dimension.ShouldBeEqualTo(_table.XDimension);
      }

      [Observation]
      public void the_base_grid_should_contain_the_values_defined_in_the_table_formula()
      {
         var baseGrid = _dataRep.BaseGrid;
         baseGrid.Values.ShouldOnlyContain(1,2);
      }

      [Observation]
      public void the_value_column_should_contain_the_values_defined_in_the_table_formula()
      {
         var valueColumn= _dataRep.First(x => !x.IsBaseGrid());
         valueColumn.Values.ShouldOnlyContain(10, 20);
      }
   }
}	