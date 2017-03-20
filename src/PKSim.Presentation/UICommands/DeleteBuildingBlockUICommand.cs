using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class DeleteBuildingBlockUICommand : ObjectUICommand<IPKSimBuildingBlock>
   {
      private readonly IBuildingBlockTask _buildingBlockTask;

      public DeleteBuildingBlockUICommand(IBuildingBlockTask buildingBlockTask)
      {
         _buildingBlockTask = buildingBlockTask;
      }

      protected override void PerformExecute()
      {
         _buildingBlockTask.Delete(Subject);
      }
   }
}