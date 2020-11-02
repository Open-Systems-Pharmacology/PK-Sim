using OSPSuite.Core.Commands.Core;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class AddSchemaToProtocolCommand : AddEntityToContainerCommand<Schema, AdvancedProtocol, AddSchemaToProtocolEvent>
   {
      public AddSchemaToProtocolCommand(Schema schemaToAdd, AdvancedProtocol protocol, IExecutionContext context)
         : base(schemaToAdd, protocol, context)
      {
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new RemoveSchemaFromProtocolCommand(_entityToAdd, _parentContainer, context).AsInverseFor(this);
      }
   }
}