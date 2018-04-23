using System.Collections.Generic;
using OSPSuite.Presentation.UICommands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class SaveBuildingBlockAsSystemTemplateCommand<TBuildingBlock> : ObjectUICommand<IReadOnlyList<TBuildingBlock>> where TBuildingBlock : class, IPKSimBuildingBlock
   {
      private readonly IBuildingBlockTask<TBuildingBlock> _buildingBlockTask;

      public SaveBuildingBlockAsSystemTemplateCommand(IBuildingBlockTask<TBuildingBlock> buildingBlockTask)
      {
         _buildingBlockTask = buildingBlockTask;
      }

      protected override void PerformExecute()
      {
         _buildingBlockTask.SaveAsSystemTemplate(Subject);
      }
   }
}