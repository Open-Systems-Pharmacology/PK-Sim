using OSPSuite.Core.Commands.Core;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using FakeItEasy;
using OSPSuite.Core.Domain;


namespace PKSim.Core
{
   public abstract class concern_for_SetRelativeExpressionCommand : ContextSpecification<SetExpressionProfileValueCommand>
   {
      protected IExecutionContext _context;
      protected double _value = 5;
      protected IParameter _parameterExpression;

      protected override void Context()
      {
         _context = A.Fake<IExecutionContext>();
         _parameterExpression = DomainHelperForSpecs.ConstantParameterWithValue(2);
         A.CallTo(() => _context.BuildingBlockContaining(_parameterExpression)).Returns(A.Fake<IPKSimBuildingBlock>());
         sut = new SetExpressionProfileValueCommand(_parameterExpression,_value);
      }
   }

   
   public class When_executing_the_set_relative_expression_command : concern_for_SetRelativeExpressionCommand
   {
      protected override void Context()
      {
         base.Context();
         _parameterExpression.IsDefault = true;
      }

      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_change_the_value_of_the_expression()
      {
         _parameterExpression.Value.ShouldBeEqualTo(_value);
      }

      [Observation]
      public void should_have_set_the_is_default_parameter_to_false_if_the_value_is_different_from_zero()
      {
         _parameterExpression.IsDefault.ShouldBeFalse();
      }
   }

   public class When_setting_the_relative_expression_value_to_zero : concern_for_SetRelativeExpressionCommand
   {
      protected override void Context()
      {
         _value = 0;
         base.Context();
         _parameterExpression.IsDefault = false;
      }

      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_change_the_value_of_the_expression()
      {
         _parameterExpression.Value.ShouldBeEqualTo(_value);
      }

      [Observation]
      public void should_have_set_the_is_default_parameter_to_true()
      {
         _parameterExpression.IsDefault.ShouldBeTrue();
      }
   }

   public class The_inverse_of_the_set_relative_expression_command : concern_for_SetRelativeExpressionCommand
   {
      private ICommand<IExecutionContext> _result;

      protected override void Because()
      {
         _result = sut.InverseCommand(_context);
      }

      [Observation]
      public void should_be_a_set_parameter_default_value_command()
      {
         _result.ShouldBeAnInstanceOf<SetParameterValueCommand>();
      }

      [Observation]
      public void should_have_been_marked_as_inverse_for_the_add_command()
      {
         _result.IsInverseFor(sut).ShouldBeTrue();
      }
   }
}	