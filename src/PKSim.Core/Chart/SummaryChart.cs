using System.Collections.Generic;
using BTS.Utility.Collections;
using BTS.Utility.Visitor;
using PKSim.Core.Model;
using SBSuite.Core.Chart;
using SBSuite.Core.Domain;
using SBSuite.Core.Domain.Services;

namespace PKSim.Core.Chart
{
   public class SummaryChart : ChartWithObservedData, ISimulationComparison<IndividualSimulation>
   {
      private readonly ICache<string, IndividualSimulation> _allSimulations;
      public bool IsLoaded { get; set; }
      public string Icon { get; set; }

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

      public IEnumerable<IndividualSimulation> AllSimulations()
      {
         return _allSimulations;
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

      public virtual void AcceptVisitor(IVisitor visitor)
      {
         visitor.Visit(this);
      }

      public void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         var originalChart = source as SummaryChart;
         if (originalChart == null) return;
         UpdateFrom(originalChart);
      }
   }
}