using OSPSuite.Core.Commands.Core;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using FakeItEasy;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_SetRelativeExpressionCommand : ContextSpecification<SetRelativeExpressionCommand>
   {
      protected IExecutionContext _context;
      private IndividualEnzyme _enzyme;
      protected double _value;
      protected IParameter _parameterExpression;

      protected override void Context()
      {
         _context = A.Fake<IExecutionContext>();
         _enzyme = A.Fake<IndividualEnzyme>();
         _value = 5;
         _parameterExpression = DomainHelperForSpecs.ConstantParameterWithValue(2);
         A.CallTo(() => _context.BuildingBlockContaining(_parameterExpression)).Returns(A.Fake<IPKSimBuildingBlock>());
         sut = new SetRelativeExpressionCommand(_parameterExpression,_value);
      }
   }

   
   public class When_executing_the_set_relative_expression_command : concern_for_SetRelativeExpressionCommand
   {
      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_change_the_value_of_the_expression()
      {
         _parameterExpression.Value.ShouldBeEqualTo(_value);
      }
   }

   
   public class The_inverse_of_the_set_relative_expression_command : concern_for_SetRelativeExpressionCommand
   {
      private IReversibleCommand<IExecutionContext> _result;

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
      public void should_have_beeen_marked_as_inverse_for_the_add_command()
      {
         _result.IsInverseFor(sut).ShouldBeTrue();
      }
   }
}	