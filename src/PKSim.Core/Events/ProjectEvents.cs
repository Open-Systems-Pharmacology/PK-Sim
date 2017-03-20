using System.Collections.Generic;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Events;

namespace PKSim.Core.Events
{
   public class SimulationComparisonCreatedEvent : ProjectEvent
   {
      public ISimulationComparison SimulationComparison { get; private set; }

      public SimulationComparisonCreatedEvent(IProject project, ISimulationComparison simulationComparison)
         : base(project)
      {
         SimulationComparison = simulationComparison;
      }
   }

   public class SimulationComparisonDeletedEvent : ProjectEvent
   {
      public ISimulationComparison Chart { get; private set; }

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