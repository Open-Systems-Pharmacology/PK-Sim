using System.Collections.Generic;
using OSPSuite.Presentation.UICommands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class SaveBuildingBlockAsTemplateCommand<TBuildingBlock> : ObjectUICommand<TBuildingBlock> where TBuildingBlock : class, IPKSimBuildingBlock
   {
      private readonly IBuildingBlockTask<TBuildingBlock> _buildingBlockTask;

      public SaveBuildingBlockAsTemplateCommand(IBuildingBlockTask<TBuildingBlock> buildingBlockTask)
      {
         _buildingBlockTask = buildingBlockTask;
      }

      protected override void PerformExecute()
      {
         _buildingBlockTask.SaveAsTemplate(Subject);
      }
   }

   public class SaveBuildingBlocksAsTemplateCommand<TBuildingBlock> : ObjectUICommand<IReadOnlyList<TBuildingBlock>> where TBuildingBlock : class, IPKSimBuildingBlock
   {
      private readonly IBuildingBlockTask _buildingBlockTask;

      public SaveBuildingBlocksAsTemplateCommand(IBuildingBlockTask buildingBlockTask)
      {
         _buildingBlockTask = buildingBlockTask;
      }

      protected override void PerformExecute()
      {
         _buildingBlockTask.SaveAsTemplate(Subject, TemplateDatabaseType.User);
      }
   }
}