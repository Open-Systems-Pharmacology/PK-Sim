using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace PKSim.BatchTool.Services
{
   public class BatchLoggerProvider : ILoggerProvider
   {
      private readonly LogLevel _logLevel;
      private readonly ConcurrentDictionary<string, BatchLogger> _loggers = new ConcurrentDictionary<string, BatchLogger>();

      public BatchLoggerProvider(LogLevel logLevel)
      {
         _logLevel = logLevel;
      }

      public ILogger CreateLogger(string categoryName)
      {
         return _loggers.GetOrAdd(categoryName, createLoggerImplementation);
      }

      private BatchLogger createLoggerImplementation(string name)
      {
         return new BatchLogger(name, _logLevel);
      }

      public void Dispose()
      {
      }
   }
}