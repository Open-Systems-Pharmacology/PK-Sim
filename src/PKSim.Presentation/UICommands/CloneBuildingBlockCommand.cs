using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class CloneBuildingBlockCommand<TBuildingBlock> : ObjectUICommand<TBuildingBlock> where TBuildingBlock : class, IPKSimBuildingBlock
   {
      private readonly IBuildingBlockTask _buildingBlockTask;

      public CloneBuildingBlockCommand(IBuildingBlockTask buildingBlockTask)
      {
         _buildingBlockTask = buildingBlockTask;
      }

      protected override void PerformExecute()
      {
         _buildingBlockTask.Clone(Subject);
      }
   }
}