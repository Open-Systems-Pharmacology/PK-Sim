using System;
using OSPSuite.Utility.Exceptions;

namespace PKSim.Core
{
   public class PKSimException : OSPSuiteException
   {
      public PKSimException()
      {
      }

      public PKSimException(string message) : base(message)
      {
      }

      public PKSimException(string message, Exception innerException) : base(message, innerException)
      {
      }
   }
}