using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_PKSimParameterXmlSerializer<TParameter> : ContextForSerialization<Model> where TParameter : IParameter
   {
      protected TParameter _parameter;
      protected TParameter _deserializedParameter;
      protected Model _model;
      protected Container _container;

      protected override void Context()
      {
         base.Context();
         _container = new Container().WithName("C");
         _model = new Model().WithName("M");
         _model.Root = _container;
      }

      protected override void Because()
      {
         _container.Add(_parameter);
         var deserializedModel = SerializeAndDeserialize(_model);
         _deserializedParameter = deserializedModel.Root.FindByName(_parameter.Name).DowncastTo<TParameter>();
      }
   }

   public abstract class concern_for_PKSimParameterXmlSerializer : concern_for_PKSimParameterXmlSerializer<IParameter>
   {
   }

   public class When_serializing_a_constant_parameter_whose_value_was_not_fixed_by_the_user : concern_for_PKSimParameterXmlSerializer
   {
      protected override void Context()
      {
         base.Context();
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(5).WithName("toto");
         _parameter.IsFixedValue = false;
      }

      [Observation]
      public void should_be_able_to_restore_the_parameter_with_the_same_value()
      {
         _deserializedParameter.Value.ShouldBeEqualTo(_parameter.Value);
      }

      [Observation]
      public void should_be_able_to_restore_the_parameter_as_not_fixed()
      {
         _deserializedParameter.IsFixedValue.ShouldBeFalse();
      }
   }

   public class When_serializing_a_constant_parameter_that_was_fixed_by_the_user_of_type_simulation : concern_for_PKSimParameterXmlSerializer
   {
      protected override void Context()
      {
         base.Context();
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(5).WithName("toto");
         _parameter.IsFixedValue = true;
      }

      [Observation]
      public void should_be_able_to_restore_the_parameter_with_the_same_value()
      {
         _deserializedParameter.Value.ShouldBeEqualTo(_parameter.Value);
      }

      [Observation]
      public void should_be_able_to_restore_the_parameter_as_fixed()
      {
         _deserializedParameter.IsFixedValue.ShouldBeTrue();
      }
   }

   public class When_serializing_a_formula_parameter_whose_value_was_not_fixed_by_the_user : concern_for_PKSimParameterXmlSerializer
   {
      protected override void Context()
      {
         base.Context();
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(5).WithName("toto");
         _parameter.Formula = new ExplicitFormula("2*3").WithId("formula");
         _parameter.IsFixedValue = false;
      }

      [Observation]
      public void should_be_able_to_restore_the_parameter_with_the_same_value()
      {
         _deserializedParameter.Value.ShouldBeEqualTo(_parameter.Value);
      }

      [Observation]
      public void should_be_able_to_restore_the_parameter_as_not_fixed()
      {
         _deserializedParameter.IsFixedValue.ShouldBeFalse();
      }

      [Observation]
      public void should_have_restored_the_formula_as_defined_in_the_original_parameter()
      {
         var formula = _deserializedParameter.Formula.DowncastTo<ExplicitFormula>();
         formula.ShouldNotBeNull();
         formula.FormulaString.ShouldBeEqualTo("2*3");
      }
   }

   public class When_serializing_a_formula_parameter_whose_value_was_fixed_by_the_user : concern_for_PKSimParameterXmlSerializer<IParameter>
   {
      private IParameter _oneParameter;

      protected override void Context()
      {
         base.Context();
         var objectPathFactory = new ObjectPathFactoryForSpecs();
         _oneParameter = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("P1").WithId("1");
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(5).WithName("toto").WithId("2");
         _parameter.Formula = new ExplicitFormula("2*3").WithId("p");
         _container.Add(_oneParameter);
         _container.Add(_parameter);
         _parameter.Formula.AddObjectPath(objectPathFactory.CreateRelativeFormulaUsablePath(_parameter, _oneParameter));
         _parameter.Value = 10;
      }

      [Observation]
      public void should_be_able_to_restore_the_parameter_with_the_same_value()
      {
         _deserializedParameter.Value.ShouldBeEqualTo(_parameter.Value);
      }

      [Observation]
      public void should_be_able_to_restore_the_parameter_as_fixed()
      {
         _deserializedParameter.IsFixedValue.ShouldBeTrue();
      }

      [Observation]
      public void should_have_restored_the_formula_as_defined_in_the_original_parameter()
      {
         var formula = _deserializedParameter.Formula.DowncastTo<ExplicitFormula>();
         formula.ShouldNotBeNull();
         formula.FormulaString.ShouldBeEqualTo("2*3");
      }
   }

   public class When_serializing_a_distributed_parameter_whose_value_was_not_set_by_the_user : concern_for_PKSimParameterXmlSerializer<IDistributedParameter>
   {
      protected override void Context()
      {
         base.Context();
         _parameter = DomainHelperForSpecs.NormalDistributedParameter();
      }

      [Observation]
      public void should_be_able_to_restore_the_parameter_with_the_same_value()
      {
         _deserializedParameter.Value.ShouldBeEqualTo(_parameter.Value);
      }

      [Observation]
      public void should_be_able_to_restore_the_parameter_as_not_fixed()
      {
         _deserializedParameter.IsFixedValue.ShouldBeFalse();
      }

      [Observation]
      public void should_be_able_to_restore_the_same_percentile()
      {
         _deserializedParameter.Percentile.ShouldBeEqualTo(_parameter.Percentile);
      }
   }

   public class When_serializing_a_distributed_parameter_whose_value_was_set_by_the_user : concern_for_PKSimParameterXmlSerializer<IDistributedParameter>
   {
      protected override void Context()
      {
         base.Context();
         _parameter = DomainHelperForSpecs.NormalDistributedParameter();
         _parameter.Value = 1.2;
      }

      [Observation]
      public void should_be_able_to_restore_the_parameter_with_the_same_value()
      {
         _deserializedParameter.Value.ShouldBeEqualTo(_parameter.Value);
      }

      [Observation]
      public void should_be_able_to_restore_the_parameter_as_not_fixed()
      {
         _deserializedParameter.IsFixedValue.ShouldBeTrue();
      }

      [Observation]
      public void should_be_able_to_restore_the_same_percentile()
      {
         _deserializedParameter.Percentile.ShouldBeEqualTo(_parameter.Percentile, 1e-6);
      }
   }

   public class When_serializing_a_distributed_parameter_whose_percentile_was_set_by_the_user : concern_for_PKSimParameterXmlSerializer<IDistributedParameter>
   {
      protected override void Context()
      {
         base.Context();
         _parameter = DomainHelperForSpecs.NormalDistributedParameter();
         _parameter.Percentile = 0.3;
      }

      [Observation]
      public void should_be_able_to_restore_the_parameter_with_the_same_value()
      {
         _deserializedParameter.Value.ShouldBeEqualTo(_parameter.Value, 1e-6);
      }

      [Observation]
      public void should_be_able_to_restore_the_parameter_as_not_fixed()
      {
         _deserializedParameter.IsFixedValue.ShouldBeTrue();
      }

      [Observation]
      public void should_be_able_to_restore_the_same_percentile()
      {
         _deserializedParameter.Percentile.ShouldBeEqualTo(_parameter.Percentile);
      }
   }

   public class When_serializing_a_parameter_for_which_the_rhs_formula_was_set : concern_for_PKSimParameterXmlSerializer<IParameter>
   {
      protected override void Context()
      {
         base.Context();
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(5).WithName("toto");
         _parameter.Formula = new ExplicitFormula("2*3").WithId("formula");
         _parameter.RHSFormula = new ExplicitFormula("4*5").WithId("RhsFormula");
         _parameter.IsFixedValue = false;
      }

      [Observation]
      public void should_be_able_to_restore_the_rhs_formula()
      {
         var formula = _deserializedParameter.RHSFormula.DowncastTo<ExplicitFormula>();
         formula.ShouldNotBeNull();
         formula.FormulaString.ShouldBeEqualTo("4*5");
      }
   }

   public class When_serializing_a_parameter_with_a_min_value_set_to_null : concern_for_PKSimParameterXmlSerializer
   {
      protected override void Context()
      {
         base.Context();
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(5).WithName("toto");
         _parameter.Info.MinValue = null;
      }

      [Observation]
      public void the_deserialized_parameter_should_also_have_a_min_value_set_to_null()
      {
         _deserializedParameter.MinValue.ShouldBeNull();
      }
   }
}