using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Infrastructure.Reporting.Summary.Items;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class SimulationCompoundConfigurationReportBuilder : ReportBuilder<SimulationCompoundConfiguration>
   {
      private readonly IReportGenerator _reportGenerator;

      public SimulationCompoundConfigurationReportBuilder(IReportGenerator reportGenerator)
      {
         _reportGenerator = reportGenerator;
      }

      protected override void FillUpReport(SimulationCompoundConfiguration simulationCompoundConfiguration, ReportPart reportPart)
      {
         var (compoundName, compoundProperties, overwriteParameterSet) = simulationCompoundConfiguration;

         //Because the compound might be lazy loaded, it is potentially not available in the compound properties
         var compoundNameToUse = compoundProperties.Compound?.Name ?? compoundName;

         var calculationMethodReport = _reportGenerator.ReportFor(compoundProperties.AllCalculationMethods()).DowncastTo<TablePart>().WithTitle(compoundNameToUse);
         if (overwriteParameterSet != null)
            calculationMethodReport.AddIs(PKSimConstants.ObjectTypes.OverwriteParameterSet, overwriteParameterSet.Name);

         reportPart.AddPart(calculationMethodReport);
      }
   }
}