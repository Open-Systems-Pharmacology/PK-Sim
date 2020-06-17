using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using NHibernate.Mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PKSim.Infrastructure.Extensions;
using IOSPLogger = OSPSuite.Core.Services.ILogger;
using System.Collections.Concurrent;

namespace PKSim.Infrastructure.Services
{
  public class PKSimLogger : IOSPLogger
   {
    private const string DEFAULT_LOGGER_CATEGORY = "PK-Sim";
    private readonly ConcurrentDictionary<string, ILogger> _loggerDict;
    private List<Func<ILoggingBuilder, ILoggingBuilder>> _loggingBuilderConfigurations = new List<Func<ILoggingBuilder, ILoggingBuilder>>() { builder => builder };
    public void AddToLog(string message, LogLevel logLevel, string categoryName)
    {
      var logger = _loggerDict.GetOrAdd(categoryName, (_) => SetupLogger((string.IsNullOrEmpty(categoryName) ? DEFAULT_LOGGER_CATEGORY : categoryName)));
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

    private ILogger SetupLogger(string categoryName)
    {
      ILogger logger;

      using (var loggerFactory = LoggerFactory.Create(
          builder =>
          _loggingBuilderConfigurations.Aggregate(
              (f1, f2) => config => f1.Compose(f2, config)
          ).Invoke(builder)
      ))
      {
        logger = loggerFactory.CreateLogger(categoryName);
      };
      return logger;
    }
  }
}