using OSPSuite.Core.Domain;

namespace PKSim.Core.Events
{
   public class SimulationLog
   {
      public string SimulationName { get; private set; }
      public string Message { get; private set; }
      public NotificationType Status { get; private set; }

      public SimulationLog(string simulationName, string content, NotificationType status)
      {
         SimulationName = simulationName;
         Status = status;
         Message = content;
      }
   }
}