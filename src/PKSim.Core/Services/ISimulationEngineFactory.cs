using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationEngineFactory
   {
      ISimulationEngine<TSimulation, TResult> Create<TSimulation, TResult>() where TSimulation : Simulation;
   }
}