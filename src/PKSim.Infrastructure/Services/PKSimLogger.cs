using Microsoft.Extensions.Logging;
using IOSPLogger = OSPSuite.Core.Services.ILogger;

// TODO: Remove
namespace PKSim.Infrastructure.Services
{
   public class PKSimLogger : IOSPLogger
   {
      private const string DEFAULT_LOGGER_CATEGORY = "PK-Sim";
      private readonly ILoggerCreator _loggerCreator;

      public PKSimLogger(ILoggerCreator loggerCreator)
      {
         _loggerCreator = loggerCreator;
      }
      public void AddToLog(string message, LogLevel logLevel, string categoryName)
      {
         var logger = _loggerCreator.GetOrCreateLogger(string.IsNullOrEmpty(categoryName) ? DEFAULT_LOGGER_CATEGORY : categoryName);
         switch (logLevel)
         {
            case LogLevel.Trace:
               logger.LogTrace(message);
               break;
            case LogLevel.Debug:
               logger.LogDebug(message);
               break;
            case LogLevel.Information:
               logger.LogInformation(message);
               break;
            case LogLevel.Warning:
               logger.LogWarning(message);
               break;
            case LogLevel.Error:
               logger.LogError(message);
               break;
            case LogLevel.Critical:
               logger.LogCritical(message);
               break;
         }
      }
   }
}