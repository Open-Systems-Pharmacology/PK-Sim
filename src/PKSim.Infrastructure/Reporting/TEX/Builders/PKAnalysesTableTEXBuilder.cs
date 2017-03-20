using System.Collections.Generic;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using PKSim.Infrastructure.Reporting.TeX.Items;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class PKAnalysesTableTeXBuilder : OSPSuiteTeXBuilder<PKAnalysesTable>
   {
      private readonly ITeXBuilderRepository _builderRepository;

      public PKAnalysesTableTeXBuilder(ITeXBuilderRepository builderRepository)
      {
         _builderRepository = builderRepository;
      }

      public override void Build(PKAnalysesTable pkAnalyses, OSPSuiteTracker buildTracker)
      {
         _builderRepository.Report(getReportObjectsFor(pkAnalyses, buildTracker), buildTracker);
      }

      private IEnumerable<object> getReportObjectsFor(PKAnalysesTable pkAnalyses, OSPSuiteTracker buildTracker)
      {
         var reportObjects = new List<object>();

         foreach (var curveName in pkAnalyses.AllCurveNames)
         {
            reportObjects.Add(buildTracker.GetStructureElementRelativeToLast(curveName, 1));
            reportObjects.Add(pkAnalyses.PKTableFor(curveName));
         }

         return reportObjects;
      }
   }
}