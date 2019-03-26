using System.Collections.Generic;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class PKSimObserverBuildingBlockTexBuilder : BuildingBlockTeXBuilder<PKSimObserverBuildingBlock>
   {
      public PKSimObserverBuildingBlockTexBuilder(ITeXBuilderRepository builderRepository, IReportGenerator reportGenerator, ILazyLoadTask lazyLoadTask) : base(builderRepository, reportGenerator, lazyLoadTask)
      {
      }

      protected override IEnumerable<object> BuildingBlockReport(PKSimObserverBuildingBlock observerBuildingBlock, OSPSuiteTracker tracker)
      {
         return observerBuildingBlock.Observers;
      }
   }
}