using System;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Events
{
   public class SimulationRunStartedEvent
   {
   }

   public class SimulationRunFinishedEvent
   {
      public Simulation Simulation { get; }
      public TimeSpan ExecutionTime { get; }

      public SimulationRunFinishedEvent(Simulation simulation, TimeSpan executionTime)
      {
         Simulation = simulation;
         ExecutionTime = executionTime;
      }
   }

   public class AddOutputIntervalToOutputSchemaEvent : AddEntityEvent<OutputInterval, OutputSchema>
   {
   }

   public class RemoveOutputIntervalFromOutputIntervalEvent : RemoveEntityEvent<OutputInterval, OutputSchema>
   {
   }

   public class SimulationUpdatedEvent
   {
      public Simulation Simulation { get; }

      public SimulationUpdatedEvent(Simulation simulation)
      {
         Simulation = simulation;
      }
   }
}