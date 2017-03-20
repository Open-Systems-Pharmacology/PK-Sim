using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_AddSchemaToProtocolCommand : ContextSpecification<AddSchemaToProtocolCommand>
   {
      protected IExecutionContext _context;
      protected Schema _schema;
      protected AdvancedProtocol _protocol;

      protected override void Context()
      {
         _context = A.Fake<IExecutionContext>();
         _schema = new Schema();
         _protocol = new AdvancedProtocol();
         sut = new AddSchemaToProtocolCommand(_schema, _protocol, _context);
      }
   }

   public class When_executing_the_add_schema_to_protocol_command : concern_for_AddSchemaToProtocolCommand
   {
      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_add_the_schema_to_the_protocol()
      {
         _protocol.AllSchemas.ShouldContain(_schema);
      }
   }

   public class The_inverse_of_the_add_schema_to_protocol_command : concern_for_AddSchemaToProtocolCommand
   {
      private IReversibleCommand<IExecutionContext> _result;

      protected override void Because()
      {
         _result = sut.InverseCommand(_context);
      }

      [Observation]
      public void should_be_a_remove_schema_from_protocol_command()
      {
         _result.ShouldBeAnInstanceOf<RemoveSchemaFromProtocolCommand>();
      }

      [Observation]
      public void should_have_beeen_marked_as_inverse_for_the_add_command()
      {
         _result.IsInverseFor(sut).ShouldBeTrue();
      }
   }
}