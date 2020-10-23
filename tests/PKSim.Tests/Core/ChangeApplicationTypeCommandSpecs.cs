using OSPSuite.Core.Commands.Core;
using PKSim.Core.Commands;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using FakeItEasy;

namespace PKSim.Core
{
   public abstract class concern_for_ChangeApplicationTypeCommand : ContextSpecification<ChangeApplicationTypeCommand>
   {
      protected IExecutionContext _executionContext;
      protected ISchemaItem _schemaItem;
      protected ApplicationType _newApplicationType;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _schemaItem =new SchemaItem {ApplicationType = ApplicationTypes.IntravenousBolus};
         _newApplicationType =ApplicationTypes.Intravenous;
         sut = new ChangeApplicationTypeCommand(_schemaItem,_newApplicationType,_executionContext);
      }
   }

   
   public class When_executing_the_change_application_type_command : concern_for_ChangeApplicationTypeCommand
   {
      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_change_the_application_type_of_the_schema_item()
      {
         _schemaItem.ApplicationType.ShouldBeEqualTo(_newApplicationType); 
      }
   }

   
   public class The_inverse_of_the_change_application_type_command : concern_for_ChangeApplicationTypeCommand
   {
      private ICommand<IExecutionContext> _result;

      protected override void Because()
      {
         _result = sut.InverseCommand(_executionContext);
      }

      [Observation]
      public void should_be_a_change_application_type_command()
      {
         _result.ShouldBeAnInstanceOf<ChangeApplicationTypeCommand>();
      }

      [Observation]
      public void should_have_been_marked_as_inverse_for_the_add_command()
      {
         _result.IsInverseFor(sut).ShouldBeTrue();
      }
   }
}	