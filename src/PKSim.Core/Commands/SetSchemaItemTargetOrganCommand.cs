using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class SetSchemaItemTargetOrganCommand : BuildingBlockStructureChangeCommand
   {
      private readonly string _newTargetOrgan;
      private readonly string _schemaItemId;
      private string _oldTargetOrgan;
      private ISchemaItem _schemaItem;

      public SetSchemaItemTargetOrganCommand(ISchemaItem schemaItem, string newTargetOrgan, IExecutionContext context)
      {
         _schemaItem = schemaItem;
         _newTargetOrgan = newTargetOrgan;
         _schemaItemId = _schemaItem.Id;
         ObjectType = PKSimConstants.ObjectTypes.AdministrationProtocol;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         BuildingBlockId = context.BuildingBlockIdContaining(schemaItem);
         context.UpdateBuildingBlockPropertiesInCommand(this, context.BuildingBlockContaining(schemaItem));
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         _oldTargetOrgan = _schemaItem.TargetOrgan;
         _schemaItem.TargetOrgan = _newTargetOrgan;
         Description = PKSimConstants.Command.SetApplicationSchemaItemTargetOrgan(_oldTargetOrgan, _newTargetOrgan);
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
         return new SetSchemaItemTargetOrganCommand(_schemaItem, _oldTargetOrgan, context).AsInverseFor(this);
      }
   }
}