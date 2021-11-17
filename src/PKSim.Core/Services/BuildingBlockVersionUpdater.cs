using OSPSuite.Utility.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IBuildingBlockVersionUpdater
   {
      void UpdateBuildingBlockVersion(IBuildingBlockChangeCommand buildingBlockChangeCommand, IPKSimBuildingBlock buildingBlock);
   }

   public class BuildingBlockVersionUpdater : IBuildingBlockVersionUpdater
   {
      private readonly IBuildingBlockInProjectManager _buildingBlockInProjectManager;

      public BuildingBlockVersionUpdater(IBuildingBlockInProjectManager buildingBlockInProjectManager)
      {
         _buildingBlockInProjectManager = buildingBlockInProjectManager;
      }

      public void UpdateBuildingBlockVersion(IBuildingBlockChangeCommand buildingBlockChangeCommand, IPKSimBuildingBlock buildingBlock)
      {
         if (buildingBlock == null) return;
         if (!buildingBlockChangeCommand.ShouldChangeVersion) return;

         if (buildingBlockChangeCommand.IncrementVersion)
            buildingBlock.Version++;
         else
            buildingBlock.Version--;

         bool isStructureChangeCommand = buildingBlockChangeCommand.IsAnImplementationOf<IBuildingBlockStructureChangeCommand>();
         if (isStructureChangeCommand)
         {
            if (buildingBlockChangeCommand.IncrementVersion)
               buildingBlock.StructureVersion++;
            else
               buildingBlock.StructureVersion--;
         }

         var simulation = buildingBlock as Simulation;
         if (simulation != null)
            _buildingBlockInProjectManager.UpdateStatusForSimulation(simulation);
         else
            _buildingBlockInProjectManager.UpdateStatusForSimulationUsing(buildingBlock);
      }
   }
}