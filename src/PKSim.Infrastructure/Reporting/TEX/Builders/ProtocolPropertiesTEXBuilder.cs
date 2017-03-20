using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class ProtocolPropertiesTeXBuilder : OSPSuiteTeXBuilder<ProtocolProperties>
   {
      private readonly ITeXBuilderRepository _texBuilderRepository;
      private readonly IReportGenerator _reportGenerator;

      public ProtocolPropertiesTeXBuilder(ITeXBuilderRepository texBuilderRepository, IReportGenerator reportGenerator)
      {
         _texBuilderRepository = texBuilderRepository;
         _reportGenerator = reportGenerator;
      }

      public override void Build(ProtocolProperties objectToReport, OSPSuiteTracker buildTracker)
      {
         _texBuilderRepository.Report(_reportGenerator.ReportFor(objectToReport.Protocol), buildTracker);
      }
   }
}
