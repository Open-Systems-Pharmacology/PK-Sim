using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public abstract class AddBuildingBlockUICommand<TBuildingBlock, TBuildingBlockTask> : IUICommand where TBuildingBlock : class, IPKSimBuildingBlock
      where TBuildingBlockTask : IBuildingBlockTask<TBuildingBlock>
   {
      private readonly IBuildingBlockTask<TBuildingBlock> _buildingBlockTask;

      protected AddBuildingBlockUICommand(TBuildingBlockTask buildingBlockTask)
      {
         _buildingBlockTask = buildingBlockTask;
      }

      public void Execute()
      {
         _buildingBlockTask.AddToProject();
      }
   }
}