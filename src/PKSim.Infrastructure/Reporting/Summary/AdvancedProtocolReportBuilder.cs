using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class AdvancedProtocolReportBuilder : ReportBuilder<AdvancedProtocol>
   {
      private readonly IReportGenerator _reportGenerator;

      public AdvancedProtocolReportBuilder(IReportGenerator reportGenerator)
      {
         _reportGenerator = reportGenerator;
      }

      protected override void FillUpReport(AdvancedProtocol advancedProtocol, ReportPart reportPart)
      {
         reportPart.Title = PKSimConstants.UI.AdvancedProtocolMode;
         advancedProtocol.AllSchemas.Each(s => reportPart.AddPart(_reportGenerator.ReportFor(s)));
      }
   }
}