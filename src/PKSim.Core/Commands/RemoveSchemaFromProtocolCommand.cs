using OSPSuite.Core.Commands.Core;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class RemoveSchemaFromProtocolCommand : RemoveEntityFromContainerCommand<Schema, AdvancedProtocol, RemoveSchemaFromProtocolEvent>
   {
      public RemoveSchemaFromProtocolCommand(Schema schemaToRemove, AdvancedProtocol protocol, IExecutionContext context)
         : base(schemaToRemove, protocol, context)
      {
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new AddSchemaToProtocolCommand(_entityToRemove, _parentContainer, context).AsInverseFor(this);
      }
   }
}