using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class SetSchemaItemFormulationKeyCommand : BuildingBlockStructureChangeCommand
   {
      private readonly string _newFormulationKey;
      private readonly string _schemaItemId;
      private string _oldFormulationKey;
      private ISchemaItem _schemaItem;

      public SetSchemaItemFormulationKeyCommand(ISchemaItem schemaItem, string newFormulationKey, IExecutionContext context)
      {
         _schemaItem = schemaItem;
         _newFormulationKey = newFormulationKey;
         _schemaItemId = _schemaItem.Id;
         ObjectType = PKSimConstants.ObjectTypes.AdministrationProtocol;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         BuildingBlockId = context.BuildingBlockIdContaining(schemaItem);
         context.UpdateBuildinBlockPropertiesInCommand(this, context.BuildingBlockContaining(schemaItem));
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         _oldFormulationKey = _schemaItem.FormulationKey;
         _schemaItem.FormulationKey = _newFormulationKey;
         Description = PKSimConstants.Command.SetApplicationSchemaItemFormulationKeyDescription(_oldFormulationKey, _newFormulationKey);
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _schemaItem = context.Get<ISchemaItem>(_schemaItemId);
      }

      protected override void ClearReferences()
      {
         _schemaItem = null;
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetSchemaItemFormulationKeyCommand(_schemaItem, _oldFormulationKey, context).AsInverseFor(this);
      }
   }
}