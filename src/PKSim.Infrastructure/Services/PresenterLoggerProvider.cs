using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PKSim.Infrastructure.Services
{
   public class PresenterLoggerProvider : ILoggerProvider
   {
      private readonly ConcurrentDictionary<string, PresenterLogger> _loggers = new ConcurrentDictionary<string, PresenterLogger>();

      public ILogger CreateLogger(string categoryName)
      {
         return _loggers.GetOrAdd(categoryName, createLoggerImplementation);
      }

      private PresenterLogger createLoggerImplementation(string name)
      {
         return new PresenterLogger(name);
      }

      public void Dispose()
      {
      }
   }

  public static class PresenterLoggingBuilderExtensions
  {
    public static ILoggingBuilder AddPresenter(this ILoggingBuilder builder)
    {
      builder.Services.AddSingleton<ILoggerProvider, PresenterLoggerProvider>(serviceProvider => new PresenterLoggerProvider());
      return builder;
    }
  }
}