using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class IndividualReportBuilder : DelegateReportBuilder<Individual>
   {
      public IndividualReportBuilder(IReportGenerator reportGenerator):base(reportGenerator)
      {
      }

      public override ReportPart Report(Individual individual)
      {
         return DelegateReportFor(individual.OriginData);
      }
   }
}