using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;
using OSPSuite.Core.Commands.Core;

namespace PKSim.Core.Commands
{
   public class AddBuildingBlockToProjectCommand : PKSimReversibleCommand
   {
      private readonly string _buildingBlockToAddId;
      private IPKSimBuildingBlock _buildingBlockToAdd;

      public AddBuildingBlockToProjectCommand(IPKSimBuildingBlock buildingBlock, IExecutionContext context)
      {
         _buildingBlockToAdd = buildingBlock;
         _buildingBlockToAddId = _buildingBlockToAdd.Id;
         CommandType = PKSimConstants.Command.CommandTypeAdd;
         ObjectType = context.TypeFor(buildingBlock);
         context.UpdateBuildinBlockProperties(this, buildingBlock);
      }

      protected override void ExecuteWith(IExecutionContext context)
      {
         var project = context.CurrentProject;
         project.AddBuildingBlock(_buildingBlockToAdd);
         Description = PKSimConstants.Command.AddEntityToContainer(ObjectType, _buildingBlockToAdd.Name, context.TypeFor(context.CurrentProject), project.Name);
         ExtendedDescription = context.ReportFor(_buildingBlockToAdd);
         context.Register(_buildingBlockToAdd);
         context.PublishEvent(new BuildingBlockAddedEvent(_buildingBlockToAdd, context.CurrentProject));
      }

      protected override void ClearReferences()
      {
         _buildingBlockToAdd = null;
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         _buildingBlockToAdd = context.Get<IPKSimBuildingBlock>(_buildingBlockToAddId);
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new RemoveBuildingBlockFromProjectCommand(_buildingBlockToAdd, context).AsInverseFor(this);
      }
   }
}