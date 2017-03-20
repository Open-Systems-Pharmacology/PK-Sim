using System.Collections.Generic;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Items;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.Reporting.TeX.Reporters
{
   public class BuildingBlockReporter : OSPSuiteTeXReporter<IPKSimBuildingBlock>
   {
      public override IReadOnlyCollection<object> Report(IPKSimBuildingBlock buildingBlock, OSPSuiteTracker buildTracker)
      {
         return new List<object> {new Chapter(buildingBlock.BuildingBlockType.ToString()), buildingBlock};
      }
   }
}