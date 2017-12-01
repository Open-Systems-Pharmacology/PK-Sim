using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.Core
{
   public class When_checking_if_a_parameter_is_a_distributed_parameter : StaticContextSpecification
   {
      private IParameter _distributedParameter;
      private IParameter _normalParameter;

      protected override void Context()
      {
         _distributedParameter = A.Fake<IDistributedParameter>();
         _normalParameter = A.Fake<IParameter>();
      }

      [Observation]
      public void should_return_true_for_a_distributed_parameter()
      {
         _distributedParameter.IsDistributed().ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_for_a_non_distributed_parameter()
      {
         _normalParameter.IsDistributed().ShouldBeFalse();
      }
   }

   public class When_defined_a_parameter_with_formula : StaticContextSpecification
   {
      private IParameter _parameter;
      private IFormula _formula;

      protected override void Context()
      {
         _formula = A.Fake<IFormula>();
         _parameter = A.Fake<IParameter>().WithFormula(_formula);
      }

      [Observation]
      public void should_set_the_parameter_formula_to_the_provided_formula()
      {
         _parameter.Formula.ShouldBeEqualTo(_formula);
      }
   }

   public class When_checking_if_a_parameter_is_an_expression_parameter : StaticContextSpecification
   {
      private IParameter _expressionParameter;
      private IParameter _otherParamter;

      protected override void Context()
      {
         _expressionParameter = A.Fake<IParameter>().WithName(CoreConstants.Parameter.REL_EXP);
         _otherParamter = A.Fake<IParameter>().WithName("toto");
      }

      [Observation]
      public void should_return_true_if_the_parameter_is_indeed_an_expression_parameter()
      {
         _expressionParameter.IsExpression().ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_for_any_parameter_that_is_not_defined_for_an_expression()
      {
         _otherParamter.IsExpression().ShouldBeFalse();
      }
   }

   public class When_checking_if_a_building_block_type_needs_a_default_value : StaticContextSpecification
   {
      private IParameter _parameter;

      protected override void Context()
      {
         base.Context();
         _parameter = A.Fake<IParameter>();
      }

      [Observation]
      public void should_return_true_if_the_types_is_individual_and_the_parameter_has_a_constant_formula()
      {
         A.CallTo(() => _parameter.BuildingBlockType).Returns(PKSimBuildingBlockType.Individual);
         _parameter.Formula = new ConstantFormula();
         _parameter.NeedsDefault().ShouldBeTrue();
      }

      [Observation]
      public void should_return_true_if_the_types_is_individual_and_the_parameter_has_a_distributed_formula()
      {
         A.CallTo(() => _parameter.BuildingBlockType).Returns(PKSimBuildingBlockType.Individual);
         _parameter.Formula = new LogNormalDistributionFormula();
         _parameter.NeedsDefault().ShouldBeTrue();
      }

      [Observation]
      public void should_return_true_if_the_types_is_simulation_and_the_parameter_has_a_constant_formula()
      {
         A.CallTo(() => _parameter.BuildingBlockType).Returns(PKSimBuildingBlockType.Simulation);
         _parameter.Formula = new ConstantFormula();
         _parameter.NeedsDefault().ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_the_types_is_simulation_and_the_parameter_does_not_have_a_constant_formula()
      {
         A.CallTo(() => _parameter.BuildingBlockType).Returns(PKSimBuildingBlockType.Simulation);
         _parameter.Formula = new ExplicitFormula();
         _parameter.NeedsDefault().ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_if_the_types_is_population_and_the_parameter_has_a_constant_formula()
      {
         A.CallTo(() => _parameter.BuildingBlockType).Returns(PKSimBuildingBlockType.Population);
         _parameter.Formula = new ConstantFormula();
         _parameter.NeedsDefault().ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_the_parameter_is_named_after_a_distribution_parameter()
      {
         _parameter.Name = Constants.Distribution.GEOMETRIC_DEVIATION;
         A.CallTo(() => _parameter.BuildingBlockType).Returns(PKSimBuildingBlockType.Individual);
         _parameter.NeedsDefault().ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_otherwise()
      {
         A.CallTo(() => _parameter.BuildingBlockType).Returns(PKSimBuildingBlockType.Compound);
         _parameter.NeedsDefault().ShouldBeFalse();

         A.CallTo(() => _parameter.BuildingBlockType).Returns(PKSimBuildingBlockType.Event);
         _parameter.NeedsDefault().ShouldBeFalse();

         A.CallTo(() => _parameter.BuildingBlockType).Returns(PKSimBuildingBlockType.Protocol);
         _parameter.NeedsDefault().ShouldBeFalse();
      }
   }

   public class When_checking_if_a_parameter_value_differs_from_its_default : StaticContextSpecification
   {
      private IParameter _parameter;

      protected override void Context()
      {
         _parameter = A.Fake<IParameter>();
      }

      [Observation]
      public void should_return_false_if_the_parameter_has_no_default()
      {
         _parameter.Editable = true;
         _parameter.DefaultValue = null;
         _parameter.ValueDiffersFromDefault().ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_parameter_is_locked()
      {
         _parameter.Editable = false;
         _parameter.ValueDiffersFromDefault().ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_if_the_parameter_has_a_fixed_value_and_the_formula_is_explicit()
      {
         _parameter.Editable = true;
         _parameter.Formula = new ExplicitFormula("1+2");
         _parameter.IsFixedValue = true;
         _parameter.ValueDiffersFromDefault().ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_the_parameter_value_is_not_fixed_and_the_formula_is_explicit()
      {
         _parameter.Editable = true;
         _parameter.Formula = new ExplicitFormula("1+2");
         _parameter.IsFixedValue = false;
         _parameter.ValueDiffersFromDefault().ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_parameter_values_are_equal_with_a_relative_difference_of_less_that_1_percent()
      {
         _parameter.Editable = true;
         _parameter.Value = 100;
         _parameter.DefaultValue = 100.9;
         _parameter.ValueDiffersFromDefault().ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_if_the_parameter_values_differs_of_more_that_1_percent_in_relative_difference()
      {
         _parameter.Editable = true;
         _parameter.Value = 100;
         _parameter.DefaultValue = 101.1;
         _parameter.ValueDiffersFromDefault().ShouldBeTrue();
      }
   }
}