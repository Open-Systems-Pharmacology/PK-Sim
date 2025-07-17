using PKSim.Assets;
using PKSim.Core.Model;
using OSPSuite.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class SubPopulationReportBuilder : ReportBuilder<SubPopulation>
   {
      protected override void FillUpReport(SubPopulation objectToReport, ReportPart reportPart)
      {
         reportPart.Title = PKSimConstants.UI.SubPopulation;
      }
   }
}