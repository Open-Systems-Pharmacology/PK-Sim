using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class ProcessSelectionGroupTeXBuilder : OSPSuiteTeXBuilder<ProcessSelectionGroup>
   {
      private readonly IReportGenerator _reportGenerator;
      private readonly ITeXBuilderRepository _builderRepository;

      public ProcessSelectionGroupTeXBuilder(IReportGenerator reportGenerator, ITeXBuilderRepository builderRepository)
      {
         _reportGenerator = reportGenerator;
         _builderRepository = builderRepository;
      }

      public override void Build(ProcessSelectionGroup processSelectionGroup, OSPSuiteTracker buildTracker)
      {
         _builderRepository.Report(_reportGenerator.ReportFor(processSelectionGroup), buildTracker);
      }
   }
}