using Microsoft.Extensions.Logging;

namespace PKSim.Infrastructure.Services
{
   public static class PresenterLoggerExtensions
   {
      /// <summary>
      /// Adds a presenter logger that is enabled for <see cref="T:Microsoft.Extensions.Logging.LogLevel" />.Information or higher.
      /// </summary>
      public static ILoggerFactory AddPresenter(this ILoggerFactory factory)
      {
         return AddPresenter(factory, LogLevel.Information);
      }

      /// <summary>
      /// Adds a presenter logger that is enabled for  the specified <paramref name="logLevel"/> or higher.
      /// </summary>
      public static ILoggerFactory AddPresenter(this ILoggerFactory factory, LogLevel logLevel)
      {
         factory.AddProvider(new PresenterLoggerProvider(logLevel));
         return factory;
      }
   }
}