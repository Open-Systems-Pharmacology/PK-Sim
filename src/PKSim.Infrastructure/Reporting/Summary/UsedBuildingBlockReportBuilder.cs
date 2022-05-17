using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class UsedBuildingBlockReportBuilder : ReportBuilder<UsedBuildingBlock>
   {
      private readonly IBuildingBlockInProjectManager _buildingBlockInProjectManager;

      public UsedBuildingBlockReportBuilder(IBuildingBlockInProjectManager buildingBlockInProjectManager)
      {
         _buildingBlockInProjectManager = buildingBlockInProjectManager;
      }

      protected override void FillUpReport(UsedBuildingBlock usedBuildingBlock, ReportPart reportPart)
      {
         var status = _buildingBlockInProjectManager.StatusFor(usedBuildingBlock);
         if (status == BuildingBlockStatus.Green) return;
         reportPart.Title = PKSimConstants.UI.Warning;
         reportPart.AddToContent(PKSimConstants.Information.BuildingBlockSettingsDoNotMatchWithTemplate(usedBuildingBlock.BuildingBlockType.ToString()));
      }
   }
}