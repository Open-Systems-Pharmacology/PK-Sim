using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Core.Services
{
   public interface ISimulationCommandDescriptionBuilder
   {
      ReportPart BuildDifferenceBetween(Simulation sourceSimulation, Simulation targetSimulation);
      ReportPart BuildFor(Simulation simulation);
   }

   public class SimulationCommandDescriptionBuilder : ISimulationCommandDescriptionBuilder
   {
      private readonly IReportGenerator _reportGenerator;

      public SimulationCommandDescriptionBuilder(IReportGenerator reportGenerator)
      {
         _reportGenerator = reportGenerator;
      }

      public ReportPart BuildDifferenceBetween(Simulation sourceSimulation, Simulation targetSimulation)
      {
         //compare building blocks
         var reportPart = new ReportPart();
         var sourceReport = BuildFor(sourceSimulation).ToStringReport().Split('\n').ToList();
         var targetReport = BuildFor(targetSimulation).ToStringReport().Split('\n').ToList();

         var tempSourceReport = new List<string>(sourceReport);
         foreach (var line in tempSourceReport)
         {
            if (!targetReport.Contains(line))
               continue;

            targetReport.Remove(line);
            sourceReport.Remove(line);
         }

         var beforePart = new ReportPart().WithTitle("Before");
         beforePart.AddToContent(sourceReport.ToString("\n"));
         var afterPart = new ReportPart().WithTitle("After");
         afterPart.AddToContent(targetReport.ToString("\n"));

         reportPart.AddPart(beforePart);
         reportPart.AddPart(afterPart);
         return reportPart;
      }

      public ReportPart BuildFor(Simulation simulation)
      {
         return _reportGenerator.ReportFor(simulation);
      }
   }
}