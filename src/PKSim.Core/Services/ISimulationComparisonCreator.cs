using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationComparisonCreator
   {
      ISimulationComparison CreateIndividualSimulationComparison();
      ISimulationComparison CreatePopulationSimulationComparison();
      void ConfigurePopulationSimulationComparison(PopulationSimulationComparison populationSimulationComparison);
   }
}