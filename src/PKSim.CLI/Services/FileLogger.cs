using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;

namespace PKSim.CLI.Services
{
   public class FileLogger : ILogger
   {
      private readonly StreamWriter _streamWriter;
      private readonly LogLevel _logLevel;
      private readonly string _name;

      public FileLogger(StreamWriter streamWriter, LogLevel logLevel, string name)
      {
         _streamWriter = streamWriter;
         _logLevel = logLevel;
         _name = name;
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

         writeMessage(logLevel, _name, eventId.Id, message, exception);
      }

      private void writeMessage(LogLevel logLevel, string name, int eventIdId, string message, Exception exception)
      {
         _streamWriter?.WriteLine($"{logLevel}: {message}");
         _streamWriter?.Flush();
      }

      public bool IsEnabled(LogLevel logLevel)
      {
         return logLevel >= _logLevel;
      }

      public IDisposable BeginScope<TState>(TState state)
      {
         return NullScope.Instance;
      }
   }
}