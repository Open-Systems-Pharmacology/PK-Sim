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
      protected IParameter _parameter;
      protected TableFormulaMapper _tableFormulaMapper;

      protected override void Context()
      {
         _tableFormulaMapper = A.Fake<TableFormulaMapper>();

         sut = new ParameterMapper(_tableFormulaMapper);

         //5 mm is the value
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(10)
            .WithName("P1")
            .WithDescription("P1 description")
            .WithDimension(DomainHelperForSpecs.LengthDimensionForSpecs());

         _parameter.DisplayUnit = _parameter.Dimension.Unit("mm");
      }
   }

   public class When_mapping_a_parameter_to_snapshot : concern_for_ParameterMapper
   {
      private SnapshotParameter _snapshotParameter;

      protected override void Because()
      {
         _snapshotParameter = sut.MapToSnapshot(_parameter);
      }

      [Observation]
      public void should_return_an_object_having_all_default_properties_of_the_object()
      {
         _snapshotParameter.Name.ShouldBeEqualTo(_parameter.Name);
         _snapshotParameter.Unit.ShouldBeEqualTo(_parameter.DisplayUnit.Name);
      }

      [Observation]
      public void should_have_reset_the_snapshot_description()
      {
         _snapshotParameter.Description.ShouldBeNull();
      }

      [Observation]
      public void should_have_set_the_value_in_display_unit()
      {
         _snapshotParameter.Value.ShouldBeEqualTo(_parameter.ValueInDisplayUnit);
      }
   }

   public class When_mapping_a_parameter_having_a_table_formula_as_formula_to_snapshot : concern_for_ParameterMapper
   {
      private SnapshotParameter _snapshotParameter;
      private TableFormula _snapshotTableFormula;

      protected override void Context()
      {
         base.Context();
         var tableFormula = new OSPSuite.Core.Domain.Formulas.TableFormula();
         _parameter.Formula = tableFormula;
         _snapshotTableFormula = new TableFormula();
         A.CallTo(() => _tableFormulaMapper.MapToSnapshot(tableFormula)).Returns(_snapshotTableFormula);
      }

      protected override void Because()
      {
         _snapshotParameter = sut.MapToSnapshot(_parameter);
      }

      [Observation]
      public void should_use_the_formula_mapper_to_map_a_snapshot_of_the_table_formula()
      {
         _snapshotParameter.TableFormula.ShouldBeEqualTo(_snapshotTableFormula);
      }
   }

   public class When_updating_a_parameter_from_a_snapshot_object : concern_for_ParameterMapper
   {
      private SnapshotParameter _snapshotParameter;

      protected override void Context()
      {
         base.Context();
         _snapshotParameter = sut.MapToSnapshot(_parameter);
         _snapshotParameter.Value = 50; //50 mm
         _snapshotParameter.ValueDescription = "The value description for this value";
      }

      protected override void Because()
      {
         sut.UpdateParameterFromSnapshot(_parameter, _snapshotParameter);
      }

      [Observation]
      public void should_update_the_standard_parameter_properties_from_the_parameter_snapshot()
      {
         _parameter.ValueInDisplayUnit.ShouldBeEqualTo(_snapshotParameter.Value);
         _parameter.ValueDescription.ShouldBeEqualTo(_snapshotParameter.ValueDescription);
      }
   }

   public class When_updating_a_paraneter_from_a_snapshot_object_using_a_table_formula_and_the_value_was_not_changed_by_the_user : concern_for_ParameterMapper
   {
      private SnapshotParameter _snapshotParameter;
      private double _parameterValue;

      protected override void Context()
      {
         base.Context();
         _parameterValue = 1500;
         _parameter.Value = _parameterValue;
         _snapshotParameter = sut.MapToSnapshot(_parameter);
         _snapshotParameter.Value = _parameter.ValueInDisplayUnit;
         _snapshotParameter.TableFormula = new TableFormula();
         var modelTableFormula = new OSPSuite.Core.Domain.Formulas.TableFormula();
         A.CallTo(() => _tableFormulaMapper.MapToModel(_snapshotParameter.TableFormula)).Returns(modelTableFormula);
         
         //Ensure that the first value is the parameter value
         modelTableFormula.AddPoint(0, _parameterValue);

         //set some dummy value to ensure reset
         _parameter.Value = 12345;
      }

      protected override void Because()
      {
         sut.UpdateParameterFromSnapshot(_parameter, _snapshotParameter);
      }

      [Observation]
      public void should_hsave_set_the_table_formula_into_the_parameter_and_respected_the_fixed_value_flag()
      {
         _parameter.Formula.ShouldBeAnInstanceOf<OSPSuite.Core.Domain.Formulas.TableFormula>();
         _parameter.Value.ShouldBeEqualTo(_parameterValue);
         _parameter.IsFixedValue.ShouldBeFalse();
      }
   }

   public class When_updating_a_paraneter_from_a_snapshot_object_using_a_table_formula_and_the_value_was_changed_by_the_user : concern_for_ParameterMapper
   {
      private SnapshotParameter _snapshotParameter;
      private double _parameterValue;

      protected override void Context()
      {
         base.Context();
         _parameterValue = 1500;
         _parameter.Value = _parameterValue;
         _snapshotParameter = sut.MapToSnapshot(_parameter);
         _snapshotParameter.Value = _parameter.ValueInDisplayUnit;
         _snapshotParameter.TableFormula = new TableFormula();
         var modelTableFormula = new OSPSuite.Core.Domain.Formulas.TableFormula();
         A.CallTo(() => _tableFormulaMapper.MapToModel(_snapshotParameter.TableFormula)).Returns(modelTableFormula);

         //Set a first value that is not the parameter value
         modelTableFormula.AddPoint(0, 1122);

         //set some dummy value to ensure reset
         _parameter.Value = 12345;
      }

      protected override void Because()
      {
         sut.UpdateParameterFromSnapshot(_parameter, _snapshotParameter);
      }

      [Observation]
      public void should_hsave_set_the_table_formula_into_the_parameter_and_respected_the_fixed_value_flag()
      {
         _parameter.Formula.ShouldBeAnInstanceOf<OSPSuite.Core.Domain.Formulas.TableFormula>();
         _parameter.Value.ShouldBeEqualTo(_parameterValue);
         _parameter.IsFixedValue.ShouldBeTrue();
      }
   }
}