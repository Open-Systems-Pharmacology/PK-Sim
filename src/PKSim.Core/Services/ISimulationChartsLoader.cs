using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationChartsLoader
   {
      /// <summary>
      /// Load the chart defined for the simulation and add them into the simulation
      /// </summary>
      /// <param name="simulation">Simulation for which the charts should be loaded</param>
      void LoadChartsFor(Simulation simulation);
   }
}