using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public interface IProtocolTask
   {
      IEnumerable<IParameter> AllDynamicParametersFor(ISchemaItem schemaItem);
      IEnumerable<string> AllFormulationKey();
      IPKSimCommand SetApplicationType(ISchemaItem schemaItem, ApplicationType applicationType);
      IPKSimCommand SetFormulationType(ISchemaItem schemaItem, string formulationType);
      IPKSimCommand SetDosingInterval(SimpleProtocol protocol, DosingInterval dosingInterval);
      IPKSimCommand AddSchemaTo(AdvancedProtocol protocol);
      IPKSimCommand RemoveSchemaFrom(Schema schemaToDelete, AdvancedProtocol protocol);
      IPKSimCommand AddSchemaItemTo(Schema schema, SchemaItem schemaItemToDupicate);
      IPKSimCommand RemoveSchemaItemFrom(SchemaItem schemaItemToDelete, Schema schema);
      IPKSimCommand SetTargetOrgan(ISchemaItem schemaItem, string targetOrgan, string targetCompartment);
      IPKSimCommand SetTargetCompartment(ISchemaItem schemaItem, string targetCompartment);
   }

   public class ProtocolTask : IProtocolTask
   {
      private readonly IFormulationKeyRepository _formulationKeyRepository;
      private readonly ISchemaTask _schemaTask;
      private readonly ISchemaItemParameterRetriever _schemaItemParameterRetriever;
      private readonly IExecutionContext _executionContext;

      public ProtocolTask(IFormulationKeyRepository formulationKeyRepository,
                          ISchemaTask schemaTask, ISchemaItemParameterRetriever schemaItemParameterRetriever, IExecutionContext executionContext)
      {
         _formulationKeyRepository = formulationKeyRepository;
         _schemaTask = schemaTask;
         _schemaItemParameterRetriever = schemaItemParameterRetriever;
         _executionContext = executionContext;
      }

      public IEnumerable<IParameter> AllDynamicParametersFor(ISchemaItem schemaItem)
      {
         return _schemaItemParameterRetriever.AllDynamicParametersFor(schemaItem).Where(p => p.Visible);
      }

      public IEnumerable<string> AllFormulationKey()
      {
         return _formulationKeyRepository.All();
      }

      public IPKSimCommand SetApplicationType(ISchemaItem schemaItem, ApplicationType applicationType)
      {
         return new SetSchemaItemApplicationTypeCommand(schemaItem, applicationType).Run(_executionContext);
      }

      public IPKSimCommand SetFormulationType(ISchemaItem schemaItem, string formulationType)
      {
         return new SetSchemaItemFormulationKeyCommand(schemaItem, formulationType, _executionContext).Run(_executionContext);
      }

      public IPKSimCommand SetDosingInterval(SimpleProtocol protocol, DosingInterval dosingInterval)
      {
         return _schemaTask.SetDosingInterval(protocol, dosingInterval);
      }

      public IPKSimCommand AddSchemaTo(AdvancedProtocol protocol)
      {
         return _schemaTask.AddSchemaTo(protocol);
      }

      public IPKSimCommand RemoveSchemaFrom(Schema schemaToDelete, AdvancedProtocol protocol)
      {
         return _schemaTask.RemoveSchemaFrom(schemaToDelete, protocol);
      }

      public IPKSimCommand AddSchemaItemTo(Schema schema, SchemaItem schemaItemToDupicate)
      {
         return _schemaTask.AddSchemaItemTo(schema, schemaItemToDupicate);
      }

      public IPKSimCommand RemoveSchemaItemFrom(SchemaItem schemaItemToDelete, Schema schema)
      {
         return _schemaTask.RemoveSchemaItemFrom(schemaItemToDelete, schema);
      }

      public IPKSimCommand SetTargetOrgan(ISchemaItem schemaItem, string targetOrgan, string targetCompartment)
      {
         var organCommand = new SetSchemaItemTargetOrganCommand(schemaItem, targetOrgan, _executionContext);
         if (schemaItem.TargetCompartment == targetCompartment)
            return organCommand.Run(_executionContext);

         var macroCommand = new PKSimMacroCommand {CommandType = organCommand.CommandType, Description = organCommand.Description, ObjectType = organCommand.ObjectType};
         organCommand.Visible = false;
         var compartmentCommand = new SetSchemaItemTargetCompartmentCommand(schemaItem, targetCompartment, _executionContext) {Visible = false};

         macroCommand.Add(organCommand, compartmentCommand);
         return macroCommand.Run(_executionContext);
      }

      public IPKSimCommand SetTargetCompartment(ISchemaItem schemaItem, string targetCompartment)
      {
         return new SetSchemaItemTargetCompartmentCommand(schemaItem, targetCompartment, _executionContext).Run(_executionContext);
      }
   }
}