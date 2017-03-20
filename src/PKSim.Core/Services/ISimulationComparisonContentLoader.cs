using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationComparisonContentLoader
   {
      void LoadContentFor(ISimulationComparison simulationComparison);
   }
}