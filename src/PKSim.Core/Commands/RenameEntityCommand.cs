using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Events;

namespace PKSim.Core.Commands
{
   public class RenameEntityCommand : BuildingBlockStructureChangeCommand
   {
      private readonly string _entityId;
      protected readonly string _newName;
      protected IEntity _entity;
      protected string _oldName;

      public RenameEntityCommand(IEntity entity, string newName, IExecutionContext context)
      {
         _entity = entity;
         _newName = newName;
         _entityId = entity.Id;
         BuildingBlockId = context.BuildingBlockIdContaining(entity);

         ObjectType = context.TypeFor(entity);
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         Description = PKSimConstants.Command.RenameEntityCommandDescripiton(ObjectType, entity.Name, _newName);
      }

      protected override void ClearReferences()
      {
         _entity = null;
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new RenameEntityCommand(_entity, _oldName, context).AsInverseFor(this);
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _entity = context.Get<IEntity>(_entityId);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         var renameBuildingBlockTask = context.Resolve<IRenameBuildingBlockTask>();
         _oldName = _entity.Name;


         switch (_entity)
         {
            case Simulation simulation:
               //Renaming the simulation is performed in a special task 
               renameBuildingBlockTask.RenameSimulation(simulation, _newName);
               break;
            case IPKSimBuildingBlock entityAsBuildingBlock:
               //Renaming the building block is performed in a special task 
               renameBuildingBlockTask.RenameBuildingBlock(entityAsBuildingBlock, _newName);
               break;
            default:
               _entity.Name = _newName;
               break;
         }
      


         var buildingBlock = context.Get<IPKSimBuildingBlock>(BuildingBlockId);
         context.UpdateBuildingBlockPropertiesInCommand(this, buildingBlock);
         if (buildingBlock != null)
            buildingBlock.HasChanged = true;

         context.PublishEvent(new RenamedEvent(_entity));
      }
   }
}