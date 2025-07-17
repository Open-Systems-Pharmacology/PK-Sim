using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class ImportPopulationReportBuilder : DelegateReportBuilder<ImportPopulation>
   {
      public ImportPopulationReportBuilder(IReportGenerator reportGenerator) : base(reportGenerator)
      {
      }

      public override ReportPart Report(ImportPopulation population)
      {
         return DelegateReportFor(population.Settings);
      }
   }
}