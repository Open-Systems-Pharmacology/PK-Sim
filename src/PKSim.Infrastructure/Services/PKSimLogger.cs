using Microsoft.Extensions.Logging;
using NHibernate.Mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ILogger = OSPSuite.Core.Services.ILogger;

namespace PKSim.Infrastructure.Services
{
   public class PKSimLogger : ILogger
   {
      private const string DEFAULT_LOGGER_CATEGORY = "PK-Sim";
      private ILoggerFactory _loggerFactory;
      private List<Func<ILoggingBuilder, ILoggingBuilder>> _loggingBuilderConfigurations = new List<Func<ILoggingBuilder, ILoggingBuilder>>() { builder => builder };
      private List<ILoggerProvider> _loggerProviders = new List<ILoggerProvider>();

      public PKSimLogger()
      {
         _loggerFactory = null;
      }

      public void AddToLog(string message, LogLevel logLevel, string categoryName)
      {
         if (_loggerFactory != null)// Maybe throw an error if null, i.e. not configured
            {
            var logger = _loggerFactory.CreateLogger(string.IsNullOrEmpty(categoryName) ? DEFAULT_LOGGER_CATEGORY : categoryName);
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
      }

        public PKSimLogger AddLoggingBuilderConfiguration(Func<ILoggingBuilder, ILoggingBuilder> configuration)
        {
            _loggingBuilderConfigurations.Add(configuration);
            SetupFactory();
            return this;
        }

        public PKSimLogger AddLoggerProvider(ILoggerProvider loggerprovider) 
        {
            _loggerProviders.Add(loggerprovider);
            SetupFactory();
            return this;
        }

        private void SetupFactory()
        {
            _loggerFactory = LoggerFactory.Create(
                builder =>
                _loggingBuilderConfigurations.Aggregate(
                    (f1, f2) => config => f2.Invoke(f1.Invoke(config))
                ).Invoke(builder)
            );
           _loggerProviders.ForEach(provider => _loggerFactory.AddProvider(provider));
        }
    }
}