using System;
using Microsoft.Extensions.Logging;
using OSPSuite.Core.Domain;
using ILogger = OSPSuite.Core.Services.ILogger;

namespace PKSim.Infrastructure.Services
{
   public class PKSimLogger : ILogger
   {
      private readonly ILoggerFactory _loggerFactory;

      public PKSimLogger(ILoggerFactory loggerFactory)
      {
         _loggerFactory = loggerFactory;
      }

      public void AddToLog(string message, NotificationType messageStatus = NotificationType.None)
      {
         var logger = _loggerFactory.CreateLogger("PK-Sim");
         switch (messageStatus)
         {
            case NotificationType.Warning:
               logger.LogWarning(message);
               break;
            case NotificationType.Error:
               logger.LogError(message);
               break;
            case NotificationType.Info:
               logger.LogInformation(message);
               break;
            case NotificationType.Debug:
               logger.LogDebug(message);
               break;
            default:
               return;
         }
      }
   }
}