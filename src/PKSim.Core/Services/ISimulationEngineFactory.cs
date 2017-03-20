using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationEngineFactory
   {
      ISimulationEngine<TSimulation> Create<TSimulation>() where TSimulation : Simulation;
   }
}