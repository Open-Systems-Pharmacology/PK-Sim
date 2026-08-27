using System.Collections.Generic;
using OSPSuite.Core.Chart;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Chart
{
   public class IndividualSimulationComparison : ChartWithObservedData, ISimulationComparison<IndividualSimulation>
   {
      private readonly ICache<string, IndividualSimulation> _allSimulations;

      //see ILazyLoadable: lazy loading reads this flag without holding the lock that guards the load, so it is
      //published with a volatile field to make the writes that loaded the object visible with it
      private volatile bool _isLoaded;

      public bool IsLoaded
      {
         get => _isLoaded;
         set => _isLoaded = value;
      }

      public IndividualSimulationComparison()
      {
         _allSimulations = new Cache<string, IndividualSimulation>(x => x.Id);
      }

      public void AddSimulation(IndividualSimulation simulation)
      {
         if (simulation == null) return;
         if (HasSimulation(simulation))
            return;

         _allSimulations.Add(simulation);
      }

      public IReadOnlyCollection<IndividualSimulation> AllSimulations => _allSimulations;

      public IReadOnlyCollection<Simulation> AllBaseSimulations => _allSimulations;

      public bool HasSimulation(IndividualSimulation simulation)
      {
         return _allSimulations.Contains(simulation.Id);
      }

      public void RemoveSimulation(IndividualSimulation simulation)
      {
         if (!HasSimulation(simulation))
            return;

         _allSimulations.Remove(simulation.Id);
         RemoveCurvesForDataRepository(simulation.DataRepository);
      }

      public void RemoveAllSimulations()
      {
         _allSimulations.Clear();
      }
   }
}