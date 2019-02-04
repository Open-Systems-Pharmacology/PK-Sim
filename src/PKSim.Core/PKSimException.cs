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

   public class QualificationRunException : OSPSuiteException
   {
      public QualificationRunException()
      {
      }

      public QualificationRunException(string message) : base(message)
      {
      }

      public QualificationRunException(string message, Exception innerException) : base(message, innerException)
      {
      }
   }
}