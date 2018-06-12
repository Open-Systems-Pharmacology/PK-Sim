using System;
using OSPSuite.Utility.Exceptions;

namespace PKSim.UI.Tests.Helpers
{
   public class ExceptionManagerForSpecs : IExceptionManager
   {
      public void LogException(Exception ex)
      {
         throw ex;
      }

      public void Execute(Action action)
      {
         action();
      }
   }
}