using System.Collections.Generic;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationComparisonTask
   {
      ISimulationComparison CreateIndividualSimulationComparison();
      ISimulationComparison CreatePopulationSimulationComparison();
      void ConfigurePopulationSimulationComparison(PopulationSimulationComparison populationSimulationComparison);
      bool Delete(IReadOnlyList<ISimulationComparison> simulationComparisons);
   }
}