using System;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using PKSim.Core.Events;

namespace PKSim.Presentation.DTO
{
   public class SimulationLogDTO : DxValidatableDTO
   {
      public string SimulationName { get; }
      public string Message { get; }
      public NotificationType Status { get; }

      public SimulationLogDTO(SimulationLog simulationLog)
      {
         SimulationName = simulationLog.SimulationName;
         Message = simulationLog.Message;
         Status = simulationLog.Status;
      }

      /// <summary>
      ///    Status of the selection (Image that will be displayed to the end user indicating if the mapping
      ///    appears to be allowed or not)
      /// </summary>
      public ApplicationIcon Image
      {
         get
         {
            switch (Status)
            {
               case NotificationType.Info:
                  return ApplicationIcons.OK;
               case NotificationType.Error:
                  return ApplicationIcons.Error;
               case NotificationType.Warning:
                  return ApplicationIcons.Warning;
               case NotificationType.Debug:
                  return ApplicationIcons.Warning;
               default:
                  throw new ArgumentOutOfRangeException();
            }
         }
      }
   }
}