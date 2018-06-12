using Microsoft.Extensions.Logging;
using ILogger = OSPSuite.Core.Services.ILogger;

namespace PKSim.Infrastructure.Services
{
   public class PKSimLogger : ILogger
   {
      private readonly ILoggerFactory _loggerFactory;
      private const string DEFAULT_LOGGER_CATEGORY = "PK-Sim";

      public PKSimLogger(ILoggerFactory loggerFactory)
      {
         _loggerFactory = loggerFactory;
      }

      public void AddToLog(string message, LogLevel logLevel, string categoryName)
      {
         var logger = _loggerFactory.CreateLogger(string.IsNullOrEmpty(categoryName) ? DEFAULT_LOGGER_CATEGORY : categoryName);
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