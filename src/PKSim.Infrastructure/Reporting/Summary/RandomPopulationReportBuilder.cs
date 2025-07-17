using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class RandomPopulationReportBuilder : DelegateReportBuilder<RandomPopulation>
   {
      public RandomPopulationReportBuilder(IReportGenerator reportGenerator) : base(reportGenerator)
      {
      }

      public override ReportPart Report(RandomPopulation randomPopulation)
      {
         return DelegateReportFor(randomPopulation.Settings);
      }
   }
}