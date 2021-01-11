using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   public class EditObjectBaseDescriptionCommand : PKSimReversibleCommand
   {
      private readonly string _buildingBlockId;
      private readonly string _newDescription;
      private readonly string _objectBaseId;
      private IObjectBase _objectBase;
      private string _oldDescription;

      public EditObjectBaseDescriptionCommand(IObjectBase objectBase, string newDescription, IExecutionContext context)
      {
         _objectBase = objectBase;
         _newDescription = newDescription;
         _objectBaseId = objectBase.Id;
         var entity = objectBase as IEntity;

         if (entity != null)
         {
            _buildingBlockId = context.BuildingBlockIdContaining(entity);
            context.UpdateBuildingBlockPropertiesInCommand(this, context.BuildingBlockContaining(entity));
         }

         ObjectType = context.TypeFor(objectBase);
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         Description = PKSimConstants.Command.EditEntityDescriptionCommandDescripiton(ObjectType, objectBase.Name);
      }

      protected override void ExecuteWith(IExecutionContext context)
      {
         _oldDescription = _objectBase.Description;
         _objectBase.Description = _newDescription;
      }

      protected override void ClearReferences()
      {
         _objectBase = null;
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new EditObjectBaseDescriptionCommand(_objectBase, _oldDescription, context).AsInverseFor(this);
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         context.Get(_buildingBlockId);
         _objectBase = context.Get<IObjectBase>(_objectBaseId);
      }
   }
}