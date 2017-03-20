using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class SimulationReportBuilder : ReportBuilder<Simulation>
   {
      private readonly IReportGenerator _reportGenerator;

      public SimulationReportBuilder(IReportGenerator reportGenerator)
      {
         _reportGenerator = reportGenerator;
      }

      protected override void FillUpReport(Simulation simulation, ReportPart reportPart)
      {
         var title= string.Format("{0} '{1}'", PKSimConstants.ObjectTypes.Simulation, simulation.Name);
         reportPart.AddPart(_reportGenerator.ReportFor(simulation.UsedBuildingBlocks).WithTitle(title));
         reportPart.AddPart(_reportGenerator.ReportFor(simulation.Creation).WithTitle(PKSimConstants.UI.Origin));
         reportPart.AddPart(_reportGenerator.ReportFor(simulation.Properties));
      }
   }
}