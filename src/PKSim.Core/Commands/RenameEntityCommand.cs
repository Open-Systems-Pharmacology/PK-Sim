using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
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

         var simulation = _entity as Simulation;

         if (simulation == null)
         {
            _entity.Name = _newName;
            if (_entity.IsAnImplementationOf<IPKSimBuildingBlock>())
               renameBuildingBlockTask.RenameUsageOfBuildingBlockInProject(_entity.DowncastTo<IPKSimBuildingBlock>(), _oldName);
         }
         else
            //Renaming the simulation is performed in a special task 
            renameBuildingBlockTask.RenameSimulation(simulation, _newName);


         var buildingBlock = context.Get<IPKSimBuildingBlock>(BuildingBlockId);
         context.UpdateBuildingBlockPropertiesInCommand(this, buildingBlock);
         if (buildingBlock != null)
            buildingBlock.HasChanged = true;

         context.PublishEvent(new RenamedEvent(_entity));
      }
   }
}