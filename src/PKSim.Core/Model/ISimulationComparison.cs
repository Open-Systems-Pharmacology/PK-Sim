using System.Collections.Generic;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public interface ISimulationComparison : ILazyLoadable, IObjectBase
   {
   }

   public interface ISimulationComparison<TSimulation> : ISimulationComparison where TSimulation : Simulation
   {
      void AddSimulation(TSimulation simulation);
      IEnumerable<TSimulation> AllSimulations();
      bool HasSimulation(TSimulation populationSimulation);
      void RemoveSimulation(TSimulation simulation);
      void RemoveAllSimulations();
   }
}