using System.Collections.Generic;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Events;
using OSPSuite.Core.Services;

namespace PKSim.Core.Batch
{
   public interface IBatchLogger : ILogger
   {
      void Clear();
      void AddParameterValueToDebug(string parameterName, double value);
      void AddParameterValueToDebug(IParameter parameter);
      IEnumerable<string> Entries { get; }
   }

   public class BatchLogger : IBatchLogger
   {
      private readonly IEventPublisher _eventPublisher;
      private readonly IList<string> _entries;

      public BatchLogger(IEventPublisher eventPublisher)
      {
         _entries = new List<string>();
         _eventPublisher = eventPublisher;
      }

      public void AddToLog(string message, NotificationType messageStatus = NotificationType.None)
      {
         var logEntry = new LogEntry(messageStatus, message);
         _entries.Add(logEntry.Display);
         _eventPublisher.PublishEvent(new LogEntryEvent(logEntry));
      }

      public void Clear()
      {
         _entries.Clear();
      }

      public void AddParameterValueToDebug(string parameterName, double value)
      {
         this.AddDebug($"Parameter '{parameterName}' set to '{value}'");
      }

      public void AddParameterValueToDebug(IParameter parameter)
      {
         AddParameterValueToDebug(parameter.Name, parameter.Value);
      }

      public IEnumerable<string> Entries => _entries;
   }
}