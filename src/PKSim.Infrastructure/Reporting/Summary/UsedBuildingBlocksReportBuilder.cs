using System.Collections.Generic;
using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class UsedBuildingBlocksReportBuilder : ReportBuilder<IEnumerable<UsedBuildingBlock>>
   {
      protected override void FillUpReport(IEnumerable<UsedBuildingBlock> usedBuildingBlocks, ReportPart reportPart)
      {
         foreach (var usedBuildingBlock in usedBuildingBlocks)
         {
            reportPart.AddToContent("Using {0} {1}", usedBuildingBlock.BuildingBlockType.ToString().ToLowerInvariant(), usedBuildingBlock.Name);
         }
      }
   }
}