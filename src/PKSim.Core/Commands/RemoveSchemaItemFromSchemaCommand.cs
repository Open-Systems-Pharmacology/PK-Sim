using OSPSuite.Core.Commands.Core;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class RemoveSchemaItemFromSchemaCommand : RemoveEntityFromContainerCommand<ISchemaItem, Schema, RemoveSchemaItemFromSchemaEvent>
   {
      public RemoveSchemaItemFromSchemaCommand(ISchemaItem entityToRemove, Schema parentContainer, IExecutionContext context)
         : base(entityToRemove, parentContainer, context)
      {
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new AddSchemaItemToSchemaCommand(_entityToRemove, _parentContainer, context).AsInverseFor(this);
      }
   }
}