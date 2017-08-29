using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using SnapshotParameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterMapper : ContextSpecification<ParameterMapper>
   {
      protected IParameter _constantParameter;
      protected TableFormulaMapper _tableFormulaMapper;

      protected override void Context()
      {
         _tableFormulaMapper = A.Fake<TableFormulaMapper>();

         sut = new ParameterMapper(_tableFormulaMapper);

         //5 mm is the value
         _constantParameter = DomainHelperForSpecs.ConstantParameterWithValue(10)
            .WithName("P1")
            .WithDescription("P1 description")
            .WithDimension(DomainHelperForSpecs.LengthDimensionForSpecs());

         _constantParameter.DisplayUnit = _constantParameter.Dimension.Unit("mm");
      }
   }

   public class When_mapping_a_parameter_to_snapshot : concern_for_ParameterMapper
   {
      private SnapshotParameter _snapshotParameter;

      protected override void Because()
      {
         _snapshotParameter = sut.MapToSnapshot(_constantParameter);
      }

      [Observation]
      public void should_return_an_object_having_all_default_properties_of_the_object()
      {
         _snapshotParameter.Name.ShouldBeEqualTo(_constantParameter.Name);
         _snapshotParameter.Unit.ShouldBeEqualTo(_constantParameter.DisplayUnit.Name);
      }

      [Observation]
      public void should_have_reset_the_snapshot_description()
      {
         _snapshotParameter.Description.ShouldBeNull();
      }

      [Observation]
      public void should_have_set_the_value_in_display_unit()
      {
         _snapshotParameter.Value.ShouldBeEqualTo(_constantParameter.ValueInDisplayUnit);
      }
   }

   public class When_mapping_a_parameter_having_a_table_formula_as_formula : concern_for_ParameterMapper
   {
      private SnapshotParameter _snapshotParameter;
      private TableFormula _snapshotTableFormula;

      protected override void Context()
      {
         base.Context();
         var tableFormula = new OSPSuite.Core.Domain.Formulas.TableFormula();
         _constantParameter.Formula = tableFormula;
         _snapshotTableFormula = new TableFormula();
         A.CallTo(() => _tableFormulaMapper.MapToSnapshot(tableFormula)).Returns(_snapshotTableFormula);
      }

      protected override void Because()
      {
         _snapshotParameter = sut.MapToSnapshot(_constantParameter);
      }

      [Observation]
      public void should_use_the_formula_mapper_to_map_a_snapshot_of_the_table_formula()
      {
         _snapshotParameter.TableFormula.ShouldBeEqualTo(_snapshotTableFormula);
      }
   }
}