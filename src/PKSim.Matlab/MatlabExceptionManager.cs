using System;
using OSPSuite.Utility.Exceptions;

namespace PKSim.Matlab
{
   public class MatlabExceptionManager : ExceptionManagerBase
   {
      public override void LogException(Exception ex)
      {
         throw ex;
      }
   }
}