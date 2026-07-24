using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class SetSchemaItemEventKeyCommand : BuildingBlockStructureChangeCommand
   {
      private readonly string _newEventKey;
      private readonly string _schemaItemId;
      private string _oldEventKey;
      private ISchemaItem _schemaItem;

      public SetSchemaItemEventKeyCommand(ISchemaItem schemaItem, string newEventKey, IExecutionContext context)
      {
         _schemaItem = schemaItem;
         _newEventKey = newEventKey;
         _schemaItemId = _schemaItem.Id;
         ObjectType = PKSimConstants.ObjectTypes.AdministrationProtocol;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         BuildingBlockId = context.BuildingBlockIdContaining(schemaItem);
         context.UpdateBuildingBlockPropertiesInCommand(this, context.BuildingBlockContaining(schemaItem));
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         _oldEventKey = _schemaItem.EventKey;
         _schemaItem.EventKey = _newEventKey;
         Description = PKSimConstants.Command.SetApplicationSchemaItemEventKeyDescription(_oldEventKey, _newEventKey);
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

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetSchemaItemEventKeyCommand(_schemaItem, _oldEventKey, context).AsInverseFor(this);
      }
   }
}
