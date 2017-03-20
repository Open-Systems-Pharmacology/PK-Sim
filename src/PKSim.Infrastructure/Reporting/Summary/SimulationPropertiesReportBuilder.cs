using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class SimulationPropertiesReportBuilder : ReportBuilder<SimulationProperties>
   {
      private readonly IReportGenerator _reportGenerator;

      public SimulationPropertiesReportBuilder(IReportGenerator reportGenerator)
      {
         _reportGenerator = reportGenerator;
      }

      protected override void FillUpReport(SimulationProperties simulationProperties, ReportPart reportPart)
      {
         var part = new ReportPart { Title = PKSimConstants.UI.AllowAging };
         part.AddToContent(simulationProperties.AllowAging ? PKSimConstants.UI.Yes : PKSimConstants.UI.No);
         reportPart.AddPart(part);
         reportPart.AddPart(_reportGenerator.ReportFor(simulationProperties.ModelProperties));
        
         reportPart.AddPart(_reportGenerator.ReportFor(simulationProperties.EventProperties));
      }
   }
}