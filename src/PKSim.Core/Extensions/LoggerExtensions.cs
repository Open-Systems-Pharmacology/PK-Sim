using System;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Maths.Statistics;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Extensions
{
   public static class LoggerExtensions
   {
      public static void AddException(this ILogger logger, Exception exception)
      {
         if (isInfoException(exception))
            logger.AddWarning(exception.ExceptionMessage());
         else
            logger.AddError(exception.ExceptionMessageWithStackTrace());
      }

      //TODO make code from Exception Manager public
      private static bool isInfoException(Exception ex)
      {
         if (ex == null)
            return false;
         if (ex.IsWrapperException())
            return isInfoException(ex.InnerException);
         if (ex.IsAnImplementationOf<NotFoundException>())
            return false;
         return ex.IsAnImplementationOf<OSPSuiteException>() || ex.IsAnImplementationOf<DistributionException>();
      }
   }
}