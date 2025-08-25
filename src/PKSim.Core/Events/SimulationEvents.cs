using System;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Events
{
   public class SimulationRunStartedEvent
   {
      public SimulationRunStartedEvent(Simulation simulation)
      {
         Simulation = simulation;
      }

      public Simulation Simulation { get; }
   }

   public class SimulationRunCanceledEvent
   {
      public SimulationRunCanceledEvent(Simulation simulation)
      {
         Simulation = simulation;
      }

      public Simulation Simulation { get; }
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

   /// <summary>
   /// Event is thrown typically when a simulation is updated from building block
   /// </summary>
   public class SimulationUpdatedEvent
   {
      public Simulation Simulation { get; }

      public SimulationUpdatedEvent(Simulation simulation)
      {
         Simulation = simulation;
      }
   }
}