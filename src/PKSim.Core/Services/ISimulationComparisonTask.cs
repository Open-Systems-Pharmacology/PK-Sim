﻿using System.Collections.Generic;
using System.Threading.Tasks;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationComparisonTask
   {
      ISimulationComparison CreateIndividualSimulationComparison(IndividualSimulation individualSimulation = null);
      ISimulationComparison CreatePopulationSimulationComparison();
      void ConfigurePopulationSimulationComparison(PopulationSimulationComparison populationSimulationComparison);
      bool Delete(IReadOnlyList<ISimulationComparison> simulationComparisons);
      Task<ISimulationComparison> CloneSimulationComparision(ISimulationComparison simulationComparison);
   }
}