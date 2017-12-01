using System.Linq;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISchemaTask
   {
      IPKSimCommand AddSchemaItemTo(Schema schema, SchemaItem schemaItemToDupicate);
      IPKSimCommand RemoveSchemaItemFrom(SchemaItem schemaItemToDelete, Schema schema);
      IPKSimCommand AddSchemaTo(AdvancedProtocol protocol);
      IPKSimCommand RemoveSchemaFrom(Schema schemaToRemove, AdvancedProtocol protocol);
      IPKSimCommand SetDosingInterval(SimpleProtocol protocol, DosingInterval dosingInterval);
      IPKSimCommand SetProtocolMode(Protocol protocol, ProtocolMode newProtocolMode);
   }

   public class SchemaTask : ISchemaTask
   {
      private readonly IExecutionContext _executionContext;
      private readonly ISchemaFactory _schemaFactory;
      private readonly ISchemaItemFactory _schemaItemFactory;

      public SchemaTask(IExecutionContext executionContext, ISchemaFactory schemaFactory, ISchemaItemFactory schemaItemFactory)
      {
         _executionContext = executionContext;
         _schemaFactory = schemaFactory;
         _schemaItemFactory = schemaItemFactory;
      }

      public IPKSimCommand AddSchemaItemTo(Schema schema, SchemaItem schemaItemToDupicate)
      {
         var schemaItemToAdd = _schemaItemFactory.CreateBasedOn(schemaItemToDupicate, schema);
         return new AddSchemaItemToSchemaCommand(schemaItemToAdd, schema, _executionContext).Run(_executionContext);
      }

      public IPKSimCommand RemoveSchemaItemFrom(SchemaItem schemaItemToDelete, Schema schema)
      {
         if (schema.SchemaItems.Count() <= 1)
            throw new CannotDeleteSchemaItemException();

         return new RemoveSchemaItemFromSchemaCommand(schemaItemToDelete, schema, _executionContext).Run(_executionContext);
      }

      public IPKSimCommand AddSchemaTo(AdvancedProtocol protocol)
      {
         var newSchema = _schemaFactory.CreateWithDefaultItem(ApplicationTypes.IntravenousBolus, protocol);
         return new AddSchemaToProtocolCommand(newSchema, protocol, _executionContext).Run(_executionContext);
      }

      public IPKSimCommand RemoveSchemaFrom(Schema schemaToRemove, AdvancedProtocol protocol)
      {
         if (protocol.AllSchemas.Count() <= 1)
            throw new CannotDeleteSchemaException();

         return new RemoveSchemaFromProtocolCommand(schemaToRemove, protocol, _executionContext).Run(_executionContext);
      }

      public IPKSimCommand SetDosingInterval(SimpleProtocol protocol, DosingInterval newDosingInterval)
      {
         return new SetProtocolDosingIntervalCommand(protocol, newDosingInterval, _executionContext).Run(_executionContext);
      }

      public IPKSimCommand SetProtocolMode(Protocol protocol, ProtocolMode newProtocolMode)
      {
         var oldProtocolMode = newProtocolMode == ProtocolMode.Advanced ? ProtocolMode.Simple : ProtocolMode.Advanced;
         return new SetProtocolModeCommand(protocol, oldProtocolMode, newProtocolMode, _executionContext).Run(_executionContext);
      }
   }
}