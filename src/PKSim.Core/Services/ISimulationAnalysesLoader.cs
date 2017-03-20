using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationAnalysesLoader
   {
      /// <summary>
      ///    Loads the PKAnalyses saved for the given <paramref name="populationSimulation" />
      /// </summary>
      void LoadAnalysesFor(PopulationSimulation populationSimulation);
   }
}