using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationResultsLoader
   {
      void LoadResultsFor(Simulation simulation);
   }
}