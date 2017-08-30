using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class LoadBuildingBlockFromSnapshotUICommand<TBuildingBlock> : IUICommand
      where TBuildingBlock : IPKSimBuildingBlock
   {
      private readonly IBuildingBlockTask<TBuildingBlock> _buildingBlockTask;

      public LoadBuildingBlockFromSnapshotUICommand(IBuildingBlockTask<TBuildingBlock> buildingBlockTask)
      {
         _buildingBlockTask = buildingBlockTask;
      }

      public void Execute()
      {
         _buildingBlockTask.LoadFromSnapshot();
      }
   }
}