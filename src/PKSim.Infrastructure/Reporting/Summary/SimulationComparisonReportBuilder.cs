using PKSim.Assets;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public abstract class SimulationComparisonReportBuilder<T,S>: ReportBuilder<T> where T : ISimulationComparison<S> where S : Simulation
   {
      protected override void FillUpReport(T comparison, ReportPart reportPart)
      {
         reportPart.Title = PKSimConstants.UI.SimulationsUsedInComparison;
         foreach (var simulation in comparison.AllSimulations)
         {
            reportPart.AddToContent(simulation.Name);
         }
      }
   }

   public class IndividualSimulationComparisonReportBuilder : SimulationComparisonReportBuilder<IndividualSimulationComparison,IndividualSimulation>
   {
   }

   public class PopulationSimulationComparisonReportBuilder : SimulationComparisonReportBuilder<PopulationSimulationComparison,PopulationSimulation>
   {
      protected override void FillUpReport(PopulationSimulationComparison simulationComparison, ReportPart reportPart)
      {
         base.FillUpReport(simulationComparison, reportPart);
         var part = new ReportPart { Title = PKSimConstants.UI.ReferenceSimulation };
         if (simulationComparison.HasReference)
            part.AddToContent(simulationComparison.ReferenceSimulation.Name);
         else
            part.AddToContent(PKSimConstants.UI.None);

         reportPart.AddPart(part);
      }
   }
}