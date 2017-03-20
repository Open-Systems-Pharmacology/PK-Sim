using PKSim.Core.Chart;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.Reporting.TeX.Items
{
   public class IndividualPKAnalyses
   {
      public Simulation Simulation { get; private set; }
      public SimulationTimeProfileChart SimulationChart { get; private set; }

      public IndividualPKAnalyses(Simulation simulation, SimulationTimeProfileChart simulationChart)
      {
         Simulation = simulation;
         SimulationChart = simulationChart;
      }
   }
}