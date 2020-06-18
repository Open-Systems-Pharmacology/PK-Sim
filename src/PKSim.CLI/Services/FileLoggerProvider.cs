using System.Collections.Concurrent;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OSPSuite.Utility;
using Serilog.Extensions.Logging;

namespace PKSim.CLI.Services
{
  public class FileLoggerProvider : ILoggerProvider
  {
    private readonly string _logFileFullPath;
    private readonly ConcurrentDictionary<string, FileLogger> _loggers = new ConcurrentDictionary<string, FileLogger>();
    private readonly StreamWriter _streamWriter;

    public FileLoggerProvider(string logFileFullPath, bool append)
    {
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
      return new FileLogger(_streamWriter, name);
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

  public static class FileLoggingBuilderExtensions
  {
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string logFileFullPath, bool append)
    {
      builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>(serviceProvider => new FileLoggerProvider(logFileFullPath, append));
      builder.Services.AddSingleton<ILoggerFactory>(serviceProvider => new SerilogLoggerFactory());
      return builder;
    }
  }
}