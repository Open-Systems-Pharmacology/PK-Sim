using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace PKSim.CLI
{
   public class ErrorCountingLoggerProvider : ILoggerProvider
   {
      private int _errorCount;

      public bool HasErrors => _errorCount > 0;

      public ILogger CreateLogger(string categoryName) => new ErrorCountingLogger(this);

      private void increment() => Interlocked.Increment(ref _errorCount);

      public void Dispose()
      {
      }

      private class ErrorCountingLogger : ILogger
      {
         private readonly ErrorCountingLoggerProvider _provider;

         public ErrorCountingLogger(ErrorCountingLoggerProvider provider) => _provider = provider;

         public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
         {
            if (logLevel >= LogLevel.Error)
               _provider.increment();
         }

         public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Error;

         public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;
      }

      private class NullScope : IDisposable
      {
         public static readonly NullScope Instance = new NullScope();

         public void Dispose()
         {
         }
      }
   }
}
