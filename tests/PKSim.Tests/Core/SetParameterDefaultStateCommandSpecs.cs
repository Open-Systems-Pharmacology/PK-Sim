using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Commands;

namespace PKSim.Core
{
   public abstract class concern_for_SetParameterDefaultStateCommand : ContextSpecification<SetParameterDefaultStateCommand>
   {
      protected IParameter _parameter;
      protected IExecutionContext _context;

      protected override void Context()
      {
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(10).WithId("Id");
         _parameter.IsDefault = true;
         sut = new SetParameterDefaultStateCommand(_parameter, false);

         _context = A.Fake<IExecutionContext>();
         A.CallTo(() => _context.Get<IParameter>(_parameter.Id)).Returns(_parameter);
      }
   }

   public class When_executing_the_set_parameter_default_state_command_for_a_parameter : concern_for_SetParameterDefaultStateCommand
   {
      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_set_the_epxected_default_flag_in_the_parameter()
      {
         _parameter.IsDefault.ShouldBeFalse();
      }
   }

   public class When_executing_the_inverse_of_the_set_parameter_default_state_command : concern_for_SetParameterDefaultStateCommand
   {
      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_context);
      }

      [Observation]
      public void should_reset_the_default_state()
      {
         _parameter.IsDefault.ShouldBeTrue();
      }
   }
}