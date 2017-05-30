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
      Task RunAsync(TSimulation simulation);

      /// <summary>
      ///    Run the simulaton synchronously and does not raise any UI events
      /// </summary>
      /// <param name="simulation">Simulation to run</param>
      void Run(TSimulation simulation);


      /// <summary>
      /// Run the simulation as a batch run an ensure that all outputs are available
      /// </summary>
      Task RunForBatch(TSimulation simulation, bool checkNegativeValues);
   }
}