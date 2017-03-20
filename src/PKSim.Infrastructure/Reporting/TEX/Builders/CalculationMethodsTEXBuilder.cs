using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class CalculationMethodsTeXBuilder : OSPSuiteTeXBuilder<IEnumerable<CalculationMethod>>
   {
      private readonly ITeXBuilderRepository _texBuilderRepository;
      private readonly IReportGenerator _reportGenerator;

      public CalculationMethodsTeXBuilder(ITeXBuilderRepository texBuilderRepository, IReportGenerator reportGenerator)
      {
         _texBuilderRepository = texBuilderRepository;
         _reportGenerator = reportGenerator;
      }

      public override void Build(IEnumerable<CalculationMethod> calculationMethods, OSPSuiteTracker buildTracker)
      {
         var reportPart = _reportGenerator.ReportFor(calculationMethods);
         _texBuilderRepository.Report(reportPart, buildTracker);
      }
   }
}
