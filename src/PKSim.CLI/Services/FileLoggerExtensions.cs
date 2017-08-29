using Microsoft.Extensions.Logging;

namespace PKSim.CLI.Services
{
   public static class FileLoggerExtensions
   {
      /// <summary>
      ///    Adds a file logger to <paramref name="logFileFullPath>" /> that is enabled for
      ///    <see cref="T:Microsoft.Extensions.Logging.LogLevel" />.Information or higher.
      /// </summary>
      public static ILoggerFactory AddFile(this ILoggerFactory factory, string logFileFullPath)
      {
         return AddFile(factory, logFileFullPath, LogLevel.Information);
      }

      /// <summary>
      ///    Adds a file logger to <paramref name="logFileFullPath>" /> that is enabled for the specififed
      ///    <paramref name="logLevel" /> or higher.
      /// </summary>
      public static ILoggerFactory AddFile(this ILoggerFactory factory, string logFileFullPath, LogLevel logLevel)
      {
         return AddFile(factory, logFileFullPath, logLevel, false);
      }

      /// <summary>
      ///    Adds a file logger to <paramref name="logFileFullPath>" /> that is enabled for the specififed
      ///    <paramref name="logLevel" /> or higher and specifies if the file should be appended or created
      /// </summary>
      public static ILoggerFactory AddFile(this ILoggerFactory factory, string logFileFullPath, LogLevel logLevel, bool append)
      {
         factory.AddProvider(new FileLoggerProvider(logFileFullPath, logLevel, append));
         return factory;
      }
   }
}