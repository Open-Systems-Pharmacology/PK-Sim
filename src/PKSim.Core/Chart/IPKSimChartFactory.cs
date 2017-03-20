using System;
using PKSim.Core.Model;
using OSPSuite.Core.Chart;

namespace PKSim.Core.Chart
{
   public interface IPKSimChartFactory : IChartFactory
   {
      IChartWithObservedData Create(Type chartType);
      SimulationTimeProfileChart CreateChartFor(IndividualSimulation individualSimulation);
      ISimulationComparison CreateSummaryChart();
   }
}
