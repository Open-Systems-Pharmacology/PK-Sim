using OSPSuite.Core.Commands.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;


namespace PKSim.Core
{
   public abstract class concern_for_set_schema_item_event_key_command : ContextSpecification<SetSchemaItemEventKeyCommand>
   {
      protected IExecutionContext _context;
      protected ISchemaItem _schemaItem;
      protected string _eventKey;

      protected override void Context()
      {
         _context = A.Fake<IExecutionContext>();
         _schemaItem = A.Fake<ISchemaItem>();
         _eventKey = "tralala";
         sut = new SetSchemaItemEventKeyCommand(_schemaItem, _eventKey, _context);
      }
   }


   public class When_executing_the_set_schema_item_event_key_command : concern_for_set_schema_item_event_key_command
   {
      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_change_the_event_key_in_the_schema_item()
      {
         _schemaItem.EventKey.ShouldBeEqualTo(_eventKey);
      }
   }


   public class The_inverse_of_set_schema_item_event_key_command : concern_for_set_schema_item_event_key_command
   {
      private ICommand<IExecutionContext> _result;

      protected override void Because()
      {
         _result = sut.InverseCommand(_context);
      }

      [Observation]
      public void should_be_a_set_schema_item_event_key_command()
      {
         _result.ShouldBeAnInstanceOf<SetSchemaItemEventKeyCommand>();
      }

      [Observation]
      public void should_have_been_marked_as_inverse_for_the_add_command()
      {
         _result.IsInverseFor(sut).ShouldBeTrue();
      }
   }
}
