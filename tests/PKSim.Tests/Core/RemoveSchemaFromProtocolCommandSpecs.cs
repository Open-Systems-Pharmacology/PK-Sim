using System.Linq;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_RemoveSchemaFromProtocolCommand : ContextSpecification<RemoveSchemaFromProtocolCommand>
   {
      protected IExecutionContext _context;
      protected Schema _schema;
      protected  PKSim.Core.Model.AdvancedProtocol _protocol;

      protected override void Context()
      {
         _context = A.Fake<IExecutionContext>();
         _schema = A.Fake<Schema>();
         _protocol = new AdvancedProtocol();
         _protocol.AddSchema(_schema);
         sut = new RemoveSchemaFromProtocolCommand(_schema, _protocol, _context);
      }
   }

   
   public class When_executing_the_remove_schema_from_protocol_command : concern_for_RemoveSchemaFromProtocolCommand
   {
      protected override void Because()
      {
         sut.Execute(_context);
      }


      [Observation]
      public void should_remove_the_schema_from_the_protocol()
      {
         _protocol.AllSchemas.Contains(_schema).ShouldBeFalse();
      }
   }

   
   public class The_inverse_of_the_remove_schema_from_protocol_command : concern_for_RemoveSchemaFromProtocolCommand
   {
      private IReversibleCommand<IExecutionContext> _result;

      protected override void Because()
      {
         _result = sut.InverseCommand(_context);
      }

      [Observation]
      public void should_be_the_add_schema_to_protocol_command()
      {
         _result.ShouldBeAnInstanceOf<AddSchemaToProtocolCommand>();
      }

      [Observation]
      public void should_have_beeen_marked_as_inverse_for_the_add_command()
      {
         _result.IsInverseFor(sut).ShouldBeTrue();
      }
   }
}	