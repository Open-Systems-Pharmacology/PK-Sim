using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Commands;

namespace PKSim.Core
{
   public abstract class concern_for_SetRelativeExpressionFromNormalizedCommand : ContextSpecification<SetRelativeExpressionFromNormalizedCommand>
   {
      protected double _newNormalizedValue;
      protected IParameter _normalizedParameter;
      protected IParameter _relativeExpressionParameter;
      protected IExecutionContext _context;

      protected override void Context()
      {
         _context = A.Fake<IExecutionContext>();
         var container = new Container();

         _normalizedParameter = new Parameter
         {
            Id = "normalizedId",
            GroupName = CoreConstants.Groups.RELATIVE_EXPRESSION,
            Name = CoreConstants.Parameter.RelExpNorm,
            Value = 0.5
         };
         
         _newNormalizedValue = 4.0;

         _relativeExpressionParameter = new Parameter
         {
            Id = "relativeId",
            GroupName = CoreConstants.Groups.RELATIVE_EXPRESSION,
            Name = CoreConstants.Parameter.RelExp,
            Value = 1.0
         };

         container.Add(_relativeExpressionParameter);
         container.Add(_normalizedParameter);

         A.CallTo(() => _context.Get<IParameter>(_relativeExpressionParameter.Id)).Returns(_relativeExpressionParameter);
         A.CallTo(() => _context.Get<IParameter>(_normalizedParameter.Id)).Returns(_normalizedParameter);

         sut = new SetRelativeExpressionFromNormalizedCommand(_normalizedParameter, _newNormalizedValue);
      }
   }

   public class When_executing_and_reversing_the_command_to_set_relative_expression_from_normalized_relative_expression : concern_for_SetRelativeExpressionFromNormalizedCommand
   {
      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_context);
      }

      [Observation]
      public void the_value_of_the_relative_expression_should_be_reversed()
      {
         _relativeExpressionParameter.Value.ShouldBeEqualTo(1);
      }
   }

   public class When_executing_the_command_to_set_relative_expression_from_normalized_relative_expression : concern_for_SetRelativeExpressionFromNormalizedCommand
   {
      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void the_relative_expression_should_be_scaled_with_the_normalized_value()
      {
         _relativeExpressionParameter.Value.ShouldBeEqualTo(8.0);
      }
   }
}
