using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ILazyLoadTask : OSPSuite.Core.Domain.Services.ILazyLoadTask
   {
      /// <summary>
      ///    Loads the simulation results for the given simulation (if not already loaded)
      /// </summary>
      void LoadResults<TSimulation>(TSimulation simulation) where TSimulation : Simulation;

      /// <summary>
      ///    Loads the simulation results for the given <paramref name="populationDataCollector" /> (if not already loaded)
      /// </summary>
      void LoadResults(IPopulationDataCollector populationDataCollector);
   }
}