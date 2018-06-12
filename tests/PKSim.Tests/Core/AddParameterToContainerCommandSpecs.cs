using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using FakeItEasy;

namespace PKSim.Core
{
   public abstract class concern_for_AddParameterToContainerCommand : ContextSpecification<AddParameterToContainerCommand>
   {
      protected IExecutionContext _executionContext;
      protected IParameter _parameterToAdd;
      protected IContainer _parentContainer;

      protected override void Context()
      {
         _parameterToAdd =A.Fake<IParameter>();
         _executionContext = A.Fake<IExecutionContext>();
         _parentContainer = A.Fake<IContainer>();
         sut = new AddParameterToContainerCommand(_parameterToAdd,_parentContainer,_executionContext);
      }
   }

   
   public class When_a_command_is_adding_a_parameter_to_a_container : concern_for_AddParameterToContainerCommand
   {
      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void the_parameter_should_have_been_added_to_the_container()
      {
         A.CallTo(() => _parentContainer.Add(_parameterToAdd)).MustHaveHappened();
      }
   }

   
   public class The_inverse_command_of_an_add_parameter_to_container_command : concern_for_AddParameterToContainerCommand
   {
      private IReversibleCommand<IExecutionContext> _result;

      protected override void Because()
      {
         _result = sut.InverseCommand(_executionContext);
      }

      [Observation]
      public void should_be_a_remove_parameter_from_container_command()
      {
         _result.ShouldBeAnInstanceOf<RemoveParameterFromContainerCommand>();
      }

      [Observation]
      public void should_have_beeen_marked_as_inverse_for_the_add_command()
      {
         _result.IsInverseFor(sut).ShouldBeTrue();
      }
   }
}	