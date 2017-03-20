using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Services
{
   public class SimulationSelector : ISimulationSelector
   {
      public bool SimulationCanBeUsedForIdentification(ISimulation simulation)
      {
         return simulationIsIndividual(simulation);
      }

      public bool SimulationCanBeUsedForSensitivityAnalysis(ISimulation simulation)
      {
         return simulationIsIndividual(simulation);
      }

      private bool simulationIsIndividual(ISimulation simulation)
      {
         return simulation.IsAnImplementationOf<IndividualSimulation>();
      }
   }
}
