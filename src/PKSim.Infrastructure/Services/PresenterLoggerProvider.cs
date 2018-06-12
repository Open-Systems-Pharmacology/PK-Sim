using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace PKSim.Infrastructure.Services
{
   public class PresenterLoggerProvider : ILoggerProvider
   {
      private readonly LogLevel _logLevel;
      private readonly ConcurrentDictionary<string, PresenterLogger> _loggers = new ConcurrentDictionary<string, PresenterLogger>();

      public PresenterLoggerProvider(LogLevel logLevel)
      {
         _logLevel = logLevel;
      }

      public ILogger CreateLogger(string categoryName)
      {
         return _loggers.GetOrAdd(categoryName, createLoggerImplementation);
      }

      private PresenterLogger createLoggerImplementation(string name)
      {
         return new PresenterLogger(name, _logLevel);
      }

      public void Dispose()
      {
      }
   }
}