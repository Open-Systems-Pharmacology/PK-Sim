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
//   public interface IBatchLogger : ILogger
//   {
//
//      /// <summary>
//      ///    Initialize the batch logger and specifies with notifcation will be monitored. 
//      /// </summary>
//      /// <param name="streamWriter">Optional stream where log will be written</param>
//      /// <param name="notificationType">Type of notification that will be logged</param>
//      void InitializeWith(StreamWriter streamWriter, NotificationType notificationType);
//
//      /// <summary>
//      /// Clear all notifications
//      /// </summary>
//      void Clear();
//   }

//   public class BatchLogger : IBatchLogger
//   {
//      private readonly IEventPublisher _eventPublisher;
//      private StreamWriter _streamWriter;
//      private NotificationType _notificationType;
//
//      public BatchLogger(IEventPublisher eventPublisher)
//      {
//         _eventPublisher = eventPublisher;
//         _notificationType = NotificationType.All;
//      }  
//
//      public void AddToLog(string message, NotificationType messageStatus = NotificationType.None)
//      {
//         var logEntry = new LogEntry(messageStatus, message);
//         var entryDisplay = logEntry.Display;
//         _eventPublisher.PublishEvent(new LogEntryEvent(logEntry));
//
//         if(canAddToLogFile(messageStatus))
//            addToStreamWriter(entryDisplay);
//      }
//
//      private bool canAddToLogFile(NotificationType messageStatus) => messageStatus.Is(_notificationType);
//
//      private void addToStreamWriter(string entryDisplay)
//      {
//         _streamWriter?.WriteLine(entryDisplay);
//         _streamWriter?.Flush();
//      }
//
//      public void InitializeWith(StreamWriter streamWriter, NotificationType notificationType)
//      {
//         _streamWriter = streamWriter;
//         _notificationType = notificationType;
//      }
//      }

//   public class BatchLoggerDisposer : IDisposable
//   {
//      private readonly StreamWriter _streamWriter;
//
//      public BatchLoggerDisposer(IBatchLogger batchLogger, string logFilePath, NotificationType notificationType)
//      {
//         ensureLogDirectoryExists(logFilePath);
//         _streamWriter = streamWriterFor(logFilePath);
//         batchLogger.InitializeWith(_streamWriter, notificationType);
//      }
//
//      private static StreamWriter streamWriterFor(string logFilePath)
//      {
//         if (string.IsNullOrEmpty(logFilePath))
//            return null;
//
//         return new StreamWriter(logFilePath, append: false);
//      }
//
//      private void ensureLogDirectoryExists(string logFilePath)
//      {
//         if (string.IsNullOrEmpty(logFilePath))
//            return;
//
//         var directory = FileHelper.FolderFromFileFullPath(logFilePath);
//         DirectoryHelper.CreateDirectory(directory);
//      }
//
//      protected virtual void Cleanup()
//      {
//         _streamWriter?.Close();
//         _streamWriter?.Dispose();
//      }
//
//      #region Disposable properties
//
//      private bool _disposed;
//
//      public void Dispose()
//      {
//         if (_disposed) return;
//
//         Cleanup();
//         GC.SuppressFinalize(this);
//         _disposed = true;
//      }
//
//      ~BatchLoggerDisposer()
//      {
//         Cleanup();
//      }
//
//      #endregion
//   }
}