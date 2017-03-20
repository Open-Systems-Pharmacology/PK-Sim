using System;
using System.Drawing;
using PKSim.Core.Events;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO
{
   public class SimulationLogDTO : DxValidatableDTO
   {
      public string SimulationName { get; private set; }
      public string Message { get; private set; }
      public NotificationType Status { get; private set; }

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
      public Image Image
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