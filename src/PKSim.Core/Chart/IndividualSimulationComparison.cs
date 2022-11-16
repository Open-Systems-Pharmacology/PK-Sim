using System.Collections.Generic;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Chart
{
   public class IndividualSimulationComparison : ChartWithObservedData, ISimulationComparison<IndividualSimulation>
   {
      private readonly ICache<string, IndividualSimulation> _allSimulations;

      public OutputMappings OutputMappingsOfAllSimulations { get; private set; }
      public bool IsLoaded { get; set; }

      public IndividualSimulationComparison()
      {
         _allSimulations = new Cache<string, IndividualSimulation>(x => x.Id);
         OutputMappingsOfAllSimulations = new OutputMappings();
      }

      public void AddSimulation(IndividualSimulation simulation)
      {
         if (simulation == null) return;
         if (HasSimulation(simulation))
            return;

         _allSimulations.Add(simulation);

         foreach (var simulationOutputMapping in simulation.OutputMappings)
         {
            OutputMappingsOfAllSimulations.Add(simulationOutputMapping);
         }
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
         OutputMappingsOfAllSimulations.RemoveOutputsReferencing(simulation);
         removeAllOutputMappings();
      }

      private void removeAllOutputMappings()
      {
         OutputMappingsOfAllSimulations.Clear();
      }

      public void RemoveAllSimulations()
      {
         _allSimulations.Clear();
         removeAllOutputMappings();
      }
   }
}