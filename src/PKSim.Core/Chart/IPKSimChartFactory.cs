using System;
using PKSim.Core.Model;
using OSPSuite.Core.Chart;

namespace PKSim.Core.Chart
{
   public interface IPKSimChartFactory : IChartFactory
   {
      ChartWithObservedData Create(Type chartType);
      SimulationTimeProfileChart CreateChartFor(IndividualSimulation individualSimulation);
      SimulationPredictedVsObservedChart CreatePredictedVsObservedChartFor(IndividualSimulation individualSimulation);
      ISimulationComparison CreateSummaryChart();
   }
}
