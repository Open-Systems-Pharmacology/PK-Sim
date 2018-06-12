using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Events;

namespace PKSim.Core.Commands
{
   public class RemoveBuildingBlockFromProjectIrreversibleCommand : PKSimCommand
   {
      private IPKSimBuildingBlock _buildingBlockToRemove;

      public RemoveBuildingBlockFromProjectIrreversibleCommand(IPKSimBuildingBlock buildingBlock, IExecutionContext context)
      {
         _buildingBlockToRemove = buildingBlock;
         CommandType = PKSimConstants.Command.CommandTypeDelete;
         ObjectType = context.TypeFor(buildingBlock);
         context.UpdateBuildinBlockPropertiesInCommand(this, buildingBlock);
      }

      protected override void ExecuteWith(IExecutionContext context)
      {
         var project = context.CurrentProject;

         project.RemoveBuildingBlock(_buildingBlockToRemove);
         Description = PKSimConstants.Command.RemoveEntityFromContainer(ObjectType, _buildingBlockToRemove.Name, context.TypeFor(project), project.Name);

         removeResultsForSimulation(_buildingBlockToRemove as Simulation);

         context.Unregister(_buildingBlockToRemove);

         context.PublishEvent(new BuildingBlockRemovedEvent(_buildingBlockToRemove, context.CurrentProject));
         var simulation = _buildingBlockToRemove as ISimulation;
         if (simulation != null)
            context.PublishEvent(new SimulationRemovedEvent(simulation));
      }

      protected override void ClearReferences()
      {
         _buildingBlockToRemove = null;
      }

      private void removeResultsForSimulation(Simulation simulation)
      {
         simulation?.ClearResults();
      }
   }
}