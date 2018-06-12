using System.Threading.Tasks;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationEngine
   {
      void Stop();
   }

   public interface ISimulationEngine<TSimulation> : ISimulationEngine where TSimulation : Simulation
   {
      /// <summary>
      ///    Run the simulation asynchronously (Hand returns right away to the caller)
      /// </summary>
      /// <param name="simulation">simulation to run</param>
      /// <param name="simulationRunOptions">Run options for this simulation run</param>
      Task RunAsync(TSimulation simulation, SimulationRunOptions simulationRunOptions);   
   }
}