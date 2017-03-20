using System.Collections.Generic;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class FormulationTeXBuilder : BuildingBlockTeXBuilder<Formulation>
   {
      public FormulationTeXBuilder(ITeXBuilderRepository builderRepository, IReportGenerator reportGenerator, ILazyLoadTask lazyLoadTask) : base(builderRepository, reportGenerator, lazyLoadTask)
      {
      }

      protected override IEnumerable<object> BuildingBlockReport(Formulation formulation, OSPSuiteTracker tracker)
      {
         return new[] {_reportGenerator.ReportFor(formulation)};
      }
   }
}