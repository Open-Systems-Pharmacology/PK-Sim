using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class OriginDataTeXBuilder : OSPSuiteTeXBuilder<OriginData>
   {
      private readonly ITeXBuilderRepository _texBuilderRepository;
      private readonly IReportGenerator _reportGenerator;

      public OriginDataTeXBuilder(ITeXBuilderRepository texBuilderRepository, IReportGenerator reportGenerator)
      {
         _texBuilderRepository = texBuilderRepository;
         _reportGenerator = reportGenerator;
      }

      public override void Build(OriginData originData, OSPSuiteTracker buildTracker)
      {
         var reportPart = _reportGenerator.ReportFor(originData);
         _texBuilderRepository.Report(reportPart, buildTracker);
      }
   }
}