using Serilog;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Logging
{
  public static class LoggingBuilderExtensions
  {
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string logFileFullPath)
    {
      builder.AddSerilog(
        new LoggerConfiguration()
          .WriteTo.File(logFileFullPath)
          .CreateLogger()
      );
      return builder;
    }
  }
}
