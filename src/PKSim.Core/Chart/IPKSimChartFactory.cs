using System;
using OSPSuite.Core.Chart;
using PKSim.Core.Model;

namespace PKSim.Core.Chart
{
   public interface IPKSimChartFactory : IChartFactory
   {
      ChartWithObservedData Create(Type chartType);
      TChartType CreateChartFor<TChartType>(IndividualSimulation individualSimulation) where TChartType : ChartWithObservedData;
      ISimulationComparison CreateSummaryChart();
   }
}