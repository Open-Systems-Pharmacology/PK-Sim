using System.Collections.Generic;
using System.Linq;
using OSPSuite.Assets.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Items;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.Reporting.TeX.Reporters
{
   public class BuildingBlocksReporter : OSPSuiteTeXReporter<IReadOnlyCollection<IPKSimBuildingBlock>>
   {
      public override IReadOnlyCollection<object> Report(IReadOnlyCollection<IPKSimBuildingBlock> buildingBlocks, OSPSuiteTracker tracker)
      {
         var report = new List<object>();
         if (!buildingBlocks.Any())
            return report;

         var buildingBlockTypes = new[] {PKSimBuildingBlockType.Individual, PKSimBuildingBlockType.Population, PKSimBuildingBlockType.Compound, PKSimBuildingBlockType.Formulation, PKSimBuildingBlockType.Protocol, PKSimBuildingBlockType.Event};
         foreach (var buildingBlockType in buildingBlockTypes)
         {
            var currentBuildingBlockType = buildingBlockType;
            var buildingBlockGroup = buildingBlocks.Where(x => x.BuildingBlockType.Is(currentBuildingBlockType)).ToList();
            if (!buildingBlockGroup.Any()) continue;

            report.Add(new Chapter(currentBuildingBlockType.ToString().Pluralize()));
            report.AddRange(buildingBlockGroup);
         }

         return report;
      }
   }
}