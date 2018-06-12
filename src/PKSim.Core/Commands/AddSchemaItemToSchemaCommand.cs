using OSPSuite.Core.Commands.Core;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class AddSchemaItemToSchemaCommand : AddEntityToContainerCommand<SchemaItem, Schema, AddSchemaItemToSchemaEvent>
   {
      public AddSchemaItemToSchemaCommand(SchemaItem schemaItemToAdd, Schema schema, IExecutionContext context) : base(schemaItemToAdd, schema, context)
      {
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new RemoveSchemaItemFromSchemaCommand(_entityToAdd, _parentContainer, context).AsInverseFor(this);
      }
   }
}