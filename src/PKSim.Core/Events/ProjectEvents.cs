using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Events
{
   public class SimulationComparisonCreatedEvent : ProjectEvent
   {
      public ISimulationComparison SimulationComparison { get; }

      public SimulationComparisonCreatedEvent(IProject project, ISimulationComparison simulationComparison)
         : base(project)
      {
         SimulationComparison = simulationComparison;
      }
   }

   public class SimulationComparisonDeletedEvent : ProjectEvent
   {
      public ISimulationComparison Chart { get; }

      public SimulationComparisonDeletedEvent(IProject project, ISimulationComparison chart)
         : base(project)
      {
         Chart = chart;
      }
   }

   public class SimulationConvertedEvent
   {
      public IEnumerable<SimulationLog> SimulationLogs { get; private set; }

      public SimulationConvertedEvent(IEnumerable<SimulationLog> simulationLogs)
      {
         SimulationLogs = simulationLogs;
      }
   }
}