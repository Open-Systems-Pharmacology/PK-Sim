using System.Collections.Generic;
using OSPSuite.Core.Chart;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Chart
{
   public class SummaryChart : ChartWithObservedData, ISimulationComparison<IndividualSimulation>
   {
      private readonly ICache<string, IndividualSimulation> _allSimulations;
      public bool IsLoaded { get; set; }

      public IReadOnlyCollection<IndividualSimulation> AllSimulations => _allSimulations;

      public IReadOnlyCollection<Simulation> AllBaseSimulations => _allSimulations;

      public SummaryChart()
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

      public bool HasSimulation(IndividualSimulation populationSimulation)
      {
         return _allSimulations.Contains(populationSimulation.Id);
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