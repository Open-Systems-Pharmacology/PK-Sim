using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using ILogger = OSPSuite.Core.Services.ILogger;

namespace PKSim.Core.Model
{
   public interface IImportLogger : ILogger
   {
      IEnumerable<LogEntry> Entries { get; }
      IEnumerable<string> Log { get; }
      NotificationType Status { get; }
   }

   public class ImportLogger : IImportLogger
   {
      private readonly List<LogEntry> _entries;

      public ImportLogger()
      {
         _entries = new List<LogEntry>();
      }

      public void AddToLog(string message, LogLevel logLevel, string categoryName)
      {
         _entries.Add(new LogEntry(logLevel, message));
      }

      public virtual IEnumerable<LogEntry> Entries => _entries;

      public virtual IEnumerable<string> Log
      {
         get { return _entries.Select(x => x.ToString()); }
      }

      public virtual NotificationType Status
      {
         get
         {
            var allStatus = _entries.Select(x => x.Level).Distinct().ToList();

            if (allStatus.Contains(LogLevel.Error) || allStatus.Contains(LogLevel.Critical))
               return NotificationType.Error;

            if (allStatus.Contains(LogLevel.Warning))
               return NotificationType.Warning;

            return NotificationType.Info;
         }
      }
   }
}