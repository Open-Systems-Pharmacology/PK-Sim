using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO: Remove
namespace PKSim.Infrastructure.Services
{
   public interface ILoggerCreator
   {
      ILogger GetOrCreateLogger(string categoryName);

      ILoggerCreator AddLoggingBuilderConfiguration(Func<ILoggingBuilder, ILoggingBuilder> configuration);
   }
}
