using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_SetParameterValueCommand : ContextSpecification<IPKSimReversibleCommand>
   {
      protected IParameter _parameter;
      protected double _valueToSet;
      protected Unit _unit;
      protected IExecutionContext _executionContext;
      protected double _oldValue;
      protected IDimension _dimension;

      protected override void Context()
      {
         _valueToSet = 20;
         _oldValue = 30;
         _unit = A.Fake<Unit>();
         A.CallTo(() => _unit.Name).Returns("_unit");
         _dimension = A.Fake<IDimension>();
         var container = new Container();
         var objectPathFactory = new ObjectPathFactoryForSpecs();

         var anotherParameter = DomainHelperForSpecs.ConstantParameterWithValue(10)
            .WithId("P2")
            .WithName("P2");

         container.Add(anotherParameter);

         _parameter = new PKSimParameter().WithFormula(new ExplicitFormula($"{_oldValue}"))
            .WithId("P1")
            .WithName("P1")
            .WithDimension(_dimension);

         container.Add(_parameter);
         _parameter.Formula.AddObjectPath(objectPathFactory.CreateRelativeFormulaUsablePath(_parameter, anotherParameter));

         A.CallTo(() => _dimension.Unit(_unit.Name)).Returns(_unit);
         _parameter.DisplayUnit = _unit;
         _executionContext = A.Fake<IExecutionContext>();
         A.CallTo(() => _executionContext.Get<IParameter>(_parameter.Id)).Returns(_parameter);
         A.CallTo(() => _executionContext.BuildingBlockContaining(_parameter)).Returns(A.Fake<IPKSimBuildingBlock>());
         sut = new SetParameterValueCommand(_parameter, _valueToSet);
      }
   }

   public class when_executing_the_set_parameter_value_command_for_a_parameter : concern_for_SetParameterValueCommand
   {
      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_set_the_value_to_the_parameter()
      {
         _parameter.Value.ShouldBeEqualTo(_valueToSet);
      }
   }

   public class when_executing_the_set_parameter_value_command_for_a_default_parameter_that_has_unchanged_method_and_source : concern_for_SetParameterValueCommand
   {
      protected override void Context()
      {
         base.Context();
         _parameter.ValueOrigin.Default = true;
         _parameter.ValueOrigin.Method = ValueOriginDeterminationMethods.Undefined;
         _parameter.ValueOrigin.Source = ValueOriginSources.Undefined;
      }

      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_set_the_default_flag_to_false()
      {
         _parameter.ValueOrigin.Default.ShouldBeFalse();
      }


      [Observation]
      public void should_set_the_value_origin_method_to_undefined()
      {
         _parameter.ValueOrigin.Method.ShouldBeEqualTo(ValueOriginDeterminationMethods.Undefined);
      }

      [Observation]
      public void should_set_the_value_origin_source_to_unknown()
      {
         _parameter.ValueOrigin.Source.ShouldBeEqualTo(ValueOriginSources.Unknown);
      }

   }

   public class When_executing_the_set_parameter_value_command_for_a_constant_parameter_of_type_simulation : concern_for_SetParameterValueCommand
   {
      protected override void Context()
      {
         base.Context();
         var container = new Container();
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(_oldValue)
            .WithId("Id")
            .WithDimension(_dimension)
            .WithName("P1");

         _parameter.BuildingBlockType = PKSimBuildingBlockType.Simulation;
         container.Add(_parameter);
         _parameter.DisplayUnit = _unit;
         sut = new SetParameterValueCommand(_parameter, _valueToSet);
      }

      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_have_set_the_is_fixed_value_to_true()
      {
         _parameter.IsFixedValue.ShouldBeTrue();
      }

      [Observation]
      public void should_have_set_the_is_value_to_the_given_value()
      {
         _parameter.Value.ShouldBeEqualTo(_valueToSet);
      }
   }

   public class When_executing_the_set_parameter_value_inverse_command_for_a_parameter_whose_value_was_set_for_the_first_time_in_the_command : concern_for_SetParameterValueCommand
   {
      protected override void Context()
      {
         base.Context();
         _parameter.IsFixedValue = false;
      }

      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_executionContext);
      }

      [Observation]
      public void should_mark_the_parameter_as_using_the_origin_formula()
      {
         _parameter.IsFixedValue.ShouldBeFalse();
      }
   }

   public class When_executing_the_reverse_command_of_a_set_parameter_value_inverse_command_for_a_parameter_whose_value_was_set_for_the_first_time_in_the_command :
      concern_for_SetParameterValueCommand
   {
      private IReversibleCommand<IExecutionContext> _reverseCommand;

      protected override void Context()
      {
         base.Context();
         _parameter.IsFixedValue = false;

         sut.Execute(_executionContext); //=> IsFixedValue = true, command did set the value for the first time

         //Inverse Command_should reset the parameter value to the formula
         sut.RestoreExecutionData(_executionContext);
         var command = sut.InverseCommand(_executionContext);
         command.Execute(_executionContext);

         //Inverse of Inverse command = first command
         command.RestoreExecutionData(_executionContext);
         _reverseCommand = command.InverseCommand(_executionContext);
      }

      protected override void Because()
      {
         _reverseCommand.Execute(_executionContext);
      }

      [Observation]
      public void should_mark_the_parameter_as_set_by_the_user()
      {
         _parameter.IsFixedValue.ShouldBeTrue();
      }

      [Observation]
      public void the_value_of_the_parameter_should_have_been_set_to_the_desire_value()
      {
         _parameter.Value.ShouldBeEqualTo(_valueToSet);
      }
   }

   public class When_executing_the_reverse_command_of_a_set_parameter_value_inverse_command_for_a_parameter_that_is_a_default_parameter_with_changed_method_or_source :
      concern_for_SetParameterValueCommand
   {
      private IReversibleCommand<IExecutionContext> _reverseCommand;

      protected override void Context()
      {
         base.Context();
         _parameter.ValueOrigin.Default = true;
         _parameter.ValueOrigin.Method = ValueOriginDeterminationMethods.InVitroAssay;
         _parameter.ValueOrigin.Source = ValueOriginSources.Internet;

         sut.Execute(_executionContext); //=> Default should be false and value origin set to unknown
         //Inverse Command_should reset the value origin to default
         var command = sut.InvokeInverse(_executionContext);

         //Inverse of Inverse command = first command
         command.RestoreExecutionData(_executionContext);
         _reverseCommand = command.InverseCommand(_executionContext);
      }

      protected override void Because()
      {
         _reverseCommand.Execute(_executionContext);
      }

      [Observation]
      public void should_mark_the_parameter_as_set_by_the_user()
      {
         _parameter.ValueOrigin.Default.ShouldBeFalse();
      }

      [Observation]
      public void should_not_have_updated_the_value_method_and_source()
      {
         _parameter.ValueOrigin.Method.ShouldBeEqualTo(ValueOriginDeterminationMethods.InVitroAssay); ;
         _parameter.ValueOrigin.Source.ShouldBeEqualTo(ValueOriginSources.Internet); 
      }
   }

   public class When_executing_the_reverse_command_of_a_set_parameter_value_inverse_command_for_a_parameter_that_is_a_default_parameter_with_undefined_method_and_source :
      concern_for_SetParameterValueCommand
   {
      private IReversibleCommand<IExecutionContext> _reverseCommand;

      protected override void Context()
      {
         base.Context();
         _parameter.ValueOrigin.Default = true;
         _parameter.ValueOrigin.Method = ValueOriginDeterminationMethods.Undefined;
         _parameter.ValueOrigin.Source = ValueOriginSources.Undefined;

         sut.Execute(_executionContext); //=> Default should be false and value origin set to unknown
         //Inverse Command_should reset the value origin to default
         var command = sut.InvokeInverse(_executionContext);

         //Inverse of Inverse command = first command
         command.RestoreExecutionData(_executionContext);
         _reverseCommand = command.InverseCommand(_executionContext);
      }

      protected override void Because()
      {
         _reverseCommand.Execute(_executionContext);
      }

      [Observation]
      public void should_mark_the_parameter_as_set_by_the_user()
      {
         _parameter.ValueOrigin.Default.ShouldBeFalse();
      }

      [Observation]
      public void should_have_updated_the_value_method_and_source()
      {
         _parameter.ValueOrigin.Method.ShouldBeEqualTo(ValueOriginDeterminationMethods.Undefined); ;
         _parameter.ValueOrigin.Source.ShouldBeEqualTo(ValueOriginSources.Unknown);
      }
   }


   public class When_setting_the_value_for_a_parameter_whose_formula_cannot_be_evaluated : concern_for_SetParameterValueCommand
   {
      protected override void Context()
      {
         base.Context();
         _parameter.Formula = new ExplicitFormula("Undefined formula");
      }

      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_be_able_to_set_the_value()
      {
         _parameter.Value.ShouldBeEqualTo(_valueToSet);
      }
   }

   public class When_setting_the_value_for_a_distributed_parameter : concern_for_SetParameterValueCommand
   {
      protected override void Context()
      {
         base.Context();
         var container = new Container();
         _valueToSet = 1.2;
         _parameter = DomainHelperForSpecs.NormalDistributedParameter()
            .WithId("Id")
            .WithDimension(_dimension)
            .WithName("P1");
         container.Add(_parameter);
         _parameter.DisplayUnit = _unit;
         _executionContext = A.Fake<IExecutionContext>();
         A.CallTo(() => _executionContext.Get<IParameter>(_parameter.Id)).Returns(_parameter);
         A.CallTo(() => _executionContext.BuildingBlockContaining(_parameter)).Returns(A.Fake<IPKSimBuildingBlock>());

         _oldValue = _parameter.Value;
         sut = new SetParameterValueCommand(_parameter, _valueToSet);
      }

      protected override void Because()
      {
         //Inverse Command_should reset the parameter value to the formula
         sut.ExecuteAndInvokeInverse(_executionContext);
      }

      [Observation]
      public void executing_the_reverse_command_should_reset_the_parameter_to_its_default_value()
      {
         _parameter.Value.ShouldBeEqualTo(_oldValue);
      }
   }

   public class When_setting_the_value_of_a_parameter_with_the_default_flag_set_to_false_hereby_indicating_that_the_parameter_was_either_changed_by_user_or_is_an_input_parameter : concern_for_SetParameterValueCommand
   {
      protected override void Context()
      {
         base.Context();
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(_oldValue)
            .WithId("Id")
            .WithDimension(_dimension)
            .WithName("P1");

         _parameter.ValueOrigin.Default = false;
      }

      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_executionContext);
      }

      [Observation]
      public void should_not_reset_the_flag_to_default_when_inversing_the_command()
      {
         _parameter.ValueOrigin.Default.ShouldBeFalse();
      }
   }

   public class When_setting_the_value_of_a_parameter_with_the_default_flag_set_to_true : concern_for_SetParameterValueCommand
   {
      protected override void Context()
      {
         base.Context();
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(_oldValue)
            .WithId("Id")
            .WithDimension(_dimension)
            .WithName("P1");

         _parameter.ValueOrigin.Default = true;
         _parameter.ValueOrigin.Source = ValueOriginSources.Database;
         _parameter.ValueOrigin.Method = ValueOriginDeterminationMethods.ManualFit;
      }

      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_executionContext);
      }

      [Observation]
      public void should_reset_the_default_flag_to_true()
      {
         _parameter.ValueOrigin.Default.ShouldBeTrue();
         _parameter.ValueOrigin.Source.ShouldBeEqualTo(ValueOriginSources.Database);
         _parameter.ValueOrigin.Method.ShouldBeEqualTo(ValueOriginDeterminationMethods.ManualFit);
      }
   }
}