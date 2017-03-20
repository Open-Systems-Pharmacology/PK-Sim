using System;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Events
{
   public class SimulationRunStartedEvent
   {
   }

   public class SimulationRunFinishedEvent
   {
      public Simulation Simulation { get; private set; }
      public TimeSpan ExecutionTime { get; private set; }

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
}