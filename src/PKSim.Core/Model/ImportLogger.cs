using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;

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

      public virtual void AddToLog(string message, NotificationType messageStatus = NotificationType.None)
      {
         _entries.Add(new LogEntry(messageStatus, message));
      }

      public virtual IEnumerable<LogEntry> Entries
      {
         get { return _entries; }
      }

      public virtual IEnumerable<string> Log
      {
         get { return _entries.Select(x => x.ToString()); }
      }

      public virtual NotificationType Status
      {
         get
         {
            var allStatus = _entries.Select(x => x.MessageStatus).Distinct().ToList();

            if (allStatus.Contains(NotificationType.Error))
               return NotificationType.Error;

            if (allStatus.Contains(NotificationType.Warning))
               return NotificationType.Warning;

            return NotificationType.Info;
         }
      }
   }
}