using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class SetSchemaItemTargetCompartmentCommand : BuildingBlockStructureChangeCommand
   {
      private readonly string _newTargetCompartment;
      private readonly string _schemaItemId;
      private string _oldTargetCompartment;
      private ISchemaItem _schemaItem;

      public SetSchemaItemTargetCompartmentCommand(ISchemaItem schemaItem, string newTargetCompartment, IExecutionContext context)
      {
         _schemaItem = schemaItem;
         _newTargetCompartment = newTargetCompartment;
         _schemaItemId = _schemaItem.Id;
         ObjectType = PKSimConstants.ObjectTypes.AdministrationProtocol;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         BuildingBlockId = context.BuildingBlockIdContaining(schemaItem);
         context.UpdateBuildingBlockPropertiesInCommand(this, context.BuildingBlockContaining(schemaItem));
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         _oldTargetCompartment = _schemaItem.TargetCompartment;
         _schemaItem.TargetCompartment = _newTargetCompartment;
         Description = PKSimConstants.Command.SetApplicationSchemaItemTargetCompartment(_oldTargetCompartment, _newTargetCompartment);
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
         return new SetSchemaItemTargetCompartmentCommand(_schemaItem, _oldTargetCompartment, context).AsInverseFor(this);
      }
   }
}