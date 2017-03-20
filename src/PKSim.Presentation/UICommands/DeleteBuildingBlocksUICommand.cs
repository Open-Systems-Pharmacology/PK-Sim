using System.Collections.Generic;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class DeleteBuildingBlocksUICommand : ObjectUICommand<IReadOnlyList<IPKSimBuildingBlock>>
   {
      private readonly IBuildingBlockTask _buildingBlockTask;

      public DeleteBuildingBlocksUICommand(IBuildingBlockTask buildingBlockTask)
      {
         _buildingBlockTask = buildingBlockTask;
      }

      protected override void PerformExecute()
      {
         _buildingBlockTask.Delete(Subject);
      }
   }
}