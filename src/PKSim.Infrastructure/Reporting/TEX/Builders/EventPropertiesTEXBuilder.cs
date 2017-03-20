using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class EventPropertiesTeXBuilder : OSPSuiteTeXBuilder<EventProperties>
   {
      private readonly ITeXBuilderRepository _texBuilderRepository;
      private readonly IReportGenerator _reportGenerator;

      public EventPropertiesTeXBuilder(ITeXBuilderRepository texBuilderRepository, IReportGenerator reportGenerator)
      {
         _texBuilderRepository = texBuilderRepository;
         _reportGenerator = reportGenerator;
      }

      public override void Build(EventProperties objectToReport, OSPSuiteTracker buildTracker)
      {
         _texBuilderRepository.Report(_reportGenerator.ReportFor(objectToReport), buildTracker);
      }
   }
}
