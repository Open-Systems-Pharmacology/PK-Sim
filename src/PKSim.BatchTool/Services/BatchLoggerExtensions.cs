using Microsoft.Extensions.Logging;

namespace PKSim.BatchTool.Services
{
   public static class BatchLoggerExtensions
   {
      /// <summary>
      /// Adds a batch logger that is enabled for <see cref="T:Microsoft.Extensions.Logging.LogLevel" />.Information or higher.
      /// </summary>
      public static ILoggerFactory AddBatch(this ILoggerFactory factory)
      {
         return AddBatch(factory, LogLevel.Information);
      }

      /// <summary>
      /// Adds a batch logger that is enabled for  the specified <paramref name="logLevel"/> or higher.
      /// </summary>
      public static ILoggerFactory AddBatch(this ILoggerFactory factory, LogLevel logLevel)
      {
         factory.AddProvider(new BatchLoggerProvider(logLevel));
         return factory;
      }
   }
}