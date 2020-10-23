using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class ChangeApplicationTypeCommand : BuildingBlockChangeCommand
   {
      private readonly ApplicationType _newApplicationType;
      private readonly string _schemaItemId;
      private ApplicationType _oldApplicationType;
      private ISchemaItem _schemaItem;

      public ChangeApplicationTypeCommand(ISchemaItem schemaItem, ApplicationType newApplicationType, IExecutionContext context)
      {
         _schemaItem = schemaItem;
         _schemaItemId = _schemaItem.Id;
         _newApplicationType = newApplicationType;
         ObjectType = PKSimConstants.ObjectTypes.AdministrationProtocol;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         BuildingBlockId = context.BuildingBlockIdContaining(schemaItem);
         context.UpdateBuildingBlockPropertiesInCommand(this, context.BuildingBlockContaining(schemaItem));
      }

      protected override void ClearReferences()
      {
         _schemaItem = null;
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new ChangeApplicationTypeCommand(_schemaItem, _oldApplicationType, context).AsInverseFor(this);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         _oldApplicationType = _schemaItem.ApplicationType;
         _schemaItem.ApplicationType = _newApplicationType;
         Description = PKSimConstants.Command.SetApplicationSchemaItemApplicationTypeDescription(_oldApplicationType.ToString(), _newApplicationType.ToString());
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _schemaItem = context.Get<ISchemaItem>(_schemaItemId);
      }
   }
}