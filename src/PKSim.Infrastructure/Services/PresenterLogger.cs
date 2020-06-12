using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OSPSuite.Core.Events;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Events;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PKSim.Infrastructure.Services
{
   public class PresenterLogger : ILogger
   {
      private readonly IEventPublisher _eventPublisher;
      public string Name { get; }
      private readonly LogLevel _logLevel;

      public PresenterLogger(string name, LogLevel logLevel)
      {
         _eventPublisher = IoC.Resolve<IEventPublisher>();
         _logLevel = logLevel;
         Name = name;
      }

      public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
      {
         if (!IsEnabled(logLevel))
            return;

         if (formatter == null)
            throw new ArgumentNullException(nameof(formatter));

         string message = formatter(state, exception);
         if (string.IsNullOrEmpty(message) && exception == null)
            return;

         writeMessage(logLevel, Name, eventId.Id, message, exception);
      }

      private void writeMessage(LogLevel logLevel, string name, int eventIdId, string message, Exception exception)
      {
         var logEntry = new LogEntry(logLevel, message);
         _eventPublisher.PublishEvent(new LogEntryEvent(logEntry));
      }

      public bool IsEnabled(LogLevel logLevel)
      {
         return logLevel >= _logLevel;
      }

      public IDisposable BeginScope<TState>(TState state)
      {
            return NullLogger.Instance.BeginScope(state);
        }
   }
}