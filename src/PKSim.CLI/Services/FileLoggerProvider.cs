using System.Collections.Concurrent;
using System.IO;
using Microsoft.Extensions.Logging;
using OSPSuite.Utility;

namespace PKSim.CLI.Services
{
   public class FileLoggerProvider : ILoggerProvider
   {
      private readonly LogLevel _logLevel;
      private readonly string _logFileFullPath;
      private readonly ConcurrentDictionary<string, FileLogger> _loggers = new ConcurrentDictionary<string, FileLogger>();
      private readonly StreamWriter _streamWriter;

      public FileLoggerProvider(string logFileFullPath, LogLevel logLevel, bool append)
      {
         _logLevel = logLevel;
         _logFileFullPath = logFileFullPath;
         ensureLogDirectoryExists();
         _streamWriter = new StreamWriter(_logFileFullPath, append);
      }

      public ILogger CreateLogger(string categoryName)
      {
         return _loggers.GetOrAdd(categoryName, createLoggerImplementation);
      }

      private FileLogger createLoggerImplementation(string name)
      {
         return new FileLogger(_streamWriter, _logLevel, name);
      }

      private void ensureLogDirectoryExists()
      {
         var directory = FileHelper.FolderFromFileFullPath(_logFileFullPath);
         DirectoryHelper.CreateDirectory(directory);
      }

      public void Dispose()
      {
         _streamWriter.Close();
         _streamWriter.Dispose();
      }
   }
}