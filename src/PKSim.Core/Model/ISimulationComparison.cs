using System.Collections.Generic;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public interface ISimulationComparison : ILazyLoadable, IObjectBase, IUsesObservedData
   {
      void RemoveAllSimulations();
      IReadOnlyCollection<Simulation> AllBaseSimulations { get; }
   }

   public interface ISimulationComparison<TSimulation> : ISimulationComparison where TSimulation : Simulation
   {
      IReadOnlyCollection<TSimulation> AllSimulations { get; }
      void AddSimulation(TSimulation simulation);
      bool HasSimulation(TSimulation simulation);
      void RemoveSimulation(TSimulation simulation);
   }
}