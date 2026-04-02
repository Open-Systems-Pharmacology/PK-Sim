using OSPSuite.Core.Commands.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_set_simple_protocol_event_command : ContextSpecification<SetSimpleProtocolEventCommand>
   {
      protected IExecutionContext _context;
      protected SimpleProtocol _protocol;
      protected string _templateEventId;

      protected override void Context()
      {
         _context = A.Fake<IExecutionContext>();
         _protocol = new SimpleProtocol { Id = "protocol-id" };
         _templateEventId = "event-123";
         A.CallTo(() => _context.BuildingBlockContaining(_protocol)).Returns(_protocol);
         sut = new SetSimpleProtocolEventCommand(_protocol, _templateEventId, _context);
      }
   }

   public class When_executing_the_set_simple_protocol_event_command : concern_for_set_simple_protocol_event_command
   {
      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_set_the_template_event_id_on_the_protocol()
      {
         _protocol.TemplateEventId.ShouldBeEqualTo(_templateEventId);
      }
   }

   public class When_executing_the_set_simple_protocol_event_command_with_null : concern_for_set_simple_protocol_event_command
   {
      protected override void Context()
      {
         _context = A.Fake<IExecutionContext>();
         _protocol = new SimpleProtocol { Id = "protocol-id", TemplateEventId = "old-event" };
         A.CallTo(() => _context.BuildingBlockContaining(_protocol)).Returns(_protocol);
         sut = new SetSimpleProtocolEventCommand(_protocol, null, _context);
      }

      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_clear_the_template_event_id()
      {
         _protocol.TemplateEventId.ShouldBeNull();
      }

      [Observation]
      public void should_mark_protocol_as_having_no_event()
      {
         _protocol.HasEvent.ShouldBeFalse();
      }
   }

   public class The_inverse_of_set_simple_protocol_event_command : concern_for_set_simple_protocol_event_command
   {
      private ICommand<IExecutionContext> _result;

      protected override void Because()
      {
         _result = sut.InverseCommand(_context);
      }

      [Observation]
      public void should_be_a_set_simple_protocol_event_command()
      {
         _result.ShouldBeAnInstanceOf<SetSimpleProtocolEventCommand>();
      }

      [Observation]
      public void should_have_been_marked_as_inverse_for_the_add_command()
      {
         _result.IsInverseFor(sut).ShouldBeTrue();
      }
   }
}
