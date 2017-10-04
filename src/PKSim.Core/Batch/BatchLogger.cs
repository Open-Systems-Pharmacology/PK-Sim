using System;
using System.Collections.Generic;
using System.IO;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Events;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Events;

namespace PKSim.Core.Batch
{
   public interface IBatchLogger : ILogger
   {
      void AddParameterValueToDebug(string parameterName, double value);
      void AddParameterValueToDebug(IParameter parameter);
      IEnumerable<string> Entries { get; }

      /// <summary>
      ///    Initialize the batch logger and specifies with notifcation will be monitored. 
      /// </summary>
      /// <param name="streamWriter">Optional stream where log will be written</param>
      /// <param name="notificationType">Type of notification that will be logged</param>
      void InitializeWith(StreamWriter streamWriter, NotificationType notificationType);

      /// <summary>
      /// Clear all notifications
      /// </summary>
      void Clear();
   }

   public class BatchLogger : IBatchLogger
   {
      private readonly IEventPublisher _eventPublisher;
      private readonly IList<string> _entries;
      private StreamWriter _streamWriter;
      private NotificationType _notificationType;
      public IEnumerable<string> Entries => _entries;

      public BatchLogger(IEventPublisher eventPublisher)
      {
         _entries = new List<string>();
         _eventPublisher = eventPublisher;
         _notificationType = NotificationType.All;
      }  

      public void AddToLog(string message, NotificationType messageStatus = NotificationType.None)
      {
         if (!messageStatus.Is(_notificationType))
            return;

         var logEntry = new LogEntry(messageStatus, message);
         var entryDisplay = logEntry.Display;
         _entries.Add(entryDisplay);
         _eventPublisher.PublishEvent(new LogEntryEvent(logEntry));
         addToStreamWriter(entryDisplay);
      }

      private void addToStreamWriter(string entryDisplay)
      {
         _streamWriter?.WriteLine(entryDisplay);
         _streamWriter?.Flush();
      }

      public void AddParameterValueToDebug(string parameterName, double value)
      {
         this.AddDebug($"Parameter '{parameterName}' set to '{value}'");
      }

      public void AddParameterValueToDebug(IParameter parameter)
      {
         AddParameterValueToDebug(parameter.Name, parameter.Value);
      }

      public void InitializeWith(StreamWriter streamWriter, NotificationType notificationType)
      {
         Clear();
         _streamWriter = streamWriter;
         _notificationType = notificationType;
      }

      public void Clear()
      {
         _entries.Clear();
      }
   }

   public class BatchLoggerDisposer : IDisposable
   {
      private readonly StreamWriter _streamWriter;

      public BatchLoggerDisposer(IBatchLogger batchLogger, string logFilePath, NotificationType notificationType)
      {
         ensureLogDirectoryExists(logFilePath);
         _streamWriter = new StreamWriter(logFilePath, append: false);
         batchLogger.InitializeWith(_streamWriter, notificationType);
      }

      private void ensureLogDirectoryExists(string logFilePath)
      {
         var directory = FileHelper.FolderFromFileFullPath(logFilePath);
         DirectoryHelper.CreateDirectory(directory);
      }

      protected virtual void Cleanup()
      {
         _streamWriter.Close();
         _streamWriter.Dispose();
      }

      #region Disposable properties

      private bool _disposed;

      public void Dispose()
      {
         if (_disposed) return;

         Cleanup();
         GC.SuppressFinalize(this);
         _disposed = true;
      }

      ~BatchLoggerDisposer()
      {
         Cleanup();
      }

      #endregion
   }
}