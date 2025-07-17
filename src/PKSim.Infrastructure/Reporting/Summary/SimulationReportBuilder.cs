using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Infrastructure.Reporting.Summary.Items;

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
         var title = $"{PKSimConstants.ObjectTypes.Simulation} '{simulation.Name}'";
         reportPart.AddPart(_reportGenerator.ReportFor(simulation.UsedBuildingBlocks).WithTitle(title));
         reportPart.AddPart(_reportGenerator.ReportFor(simulation.Creation).WithTitle(PKSimConstants.UI.Origin));
         reportPart.AddPart(_reportGenerator.ReportFor(simulation.Properties));

         //Add special node with calculation methods per compound. Because the compound might be lazy loaded,
         //it is potentially not available in the compound properties
         simulation.UsedBuildingBlocksInSimulation(PKSimBuildingBlockType.Compound).ToList().Each((x, index) =>
         {
            var compoundPropertiesWithName = new CompoundPropertiesCalculationMethods(x.Name, simulation.CompoundPropertiesList[index]);
            reportPart.AddPart(_reportGenerator.ReportFor(compoundPropertiesWithName));
         });
      }
   }
}