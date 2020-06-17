using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using NHibernate.Mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PKSim.Infrastructure.Extensions;
using ILogger = OSPSuite.Core.Services.ILogger;
using System.Collections.Concurrent;

namespace PKSim.Infrastructure.Services
{
  public class PKSimLogger : ILogger
  {
    private const string DEFAULT_LOGGER_CATEGORY = "PK-Sim";
    private ConcurrentDictionary<string, Microsoft.Extensions.Logging.ILogger> _logger;
    private List<Func<ILoggingBuilder, ILoggingBuilder>> _loggingBuilderConfigurations = new List<Func<ILoggingBuilder, ILoggingBuilder>>() { builder => builder };

    public void AddToLog(string message, LogLevel logLevel, string categoryName)
    {
      var logger = _logger.GetOrAdd(categoryName, (_) => SetupFactory((string.IsNullOrEmpty(categoryName) ? DEFAULT_LOGGER_CATEGORY : categoryName)));
      switch (logLevel)
      {
        case LogLevel.Trace:
          logger.LogTrace(message);
          break;
        case LogLevel.Debug:
          logger.LogDebug(message);
          break;
        case LogLevel.Information:
          logger.LogInformation(message);
          break;
        case LogLevel.Warning:
          logger.LogWarning(message);
          break;
        case LogLevel.Error:
          logger.LogError(message);
          break;
        case LogLevel.Critical:
          logger.LogCritical(message);
          break;
      }
    }

    public PKSimLogger AddLoggingBuilderConfiguration(Func<ILoggingBuilder, ILoggingBuilder> configuration)
    {
      _loggingBuilderConfigurations.Add(configuration);
      return this;
    }

    private Microsoft.Extensions.Logging.ILogger SetupFactory(string categoryName)
    {
      Microsoft.Extensions.Logging.ILogger result;
      using (var loggerFactory = LoggerFactory.Create(
          builder =>
          _loggingBuilderConfigurations.Aggregate(
              (f1, f2) => config => f1.Compose(f2, config)
          ).Invoke(builder)
      ))
      {
        result = loggerFactory.CreateLogger(categoryName);
      };
      return result;
    }
  }
}