using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Infrastructure.Reporting.Summary.Items;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class CompoundPropertiesCalculationMethodsReportBuilder : ReportBuilder<CompoundPropertiesCalculationMethods>
   {
      private readonly IReportGenerator _reportGenerator;

      public CompoundPropertiesCalculationMethodsReportBuilder(IReportGenerator reportGenerator)
      {
         _reportGenerator = reportGenerator;
      }

      protected override void FillUpReport(CompoundPropertiesCalculationMethods compoundPropertiesCalculationMethods, ReportPart reportPart)
      {
         var (compoundName, compoundProperties) = compoundPropertiesCalculationMethods;
         var compoundNameToUse = compoundProperties.Compound?.Name ?? compoundName;

         var calculationMethodReportPart = _reportGenerator.ReportFor(compoundProperties.AllCalculationMethods());

         calculationMethodReportPart.SubParts.Each(x => reportPart.AddPart(x.WithTitle(compoundNameToUse)));
      }
   }
}