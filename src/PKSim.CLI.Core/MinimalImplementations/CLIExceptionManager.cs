using System;
using OSPSuite.Utility.Exceptions;

namespace PKSim.CLI.Core.MinimalImplementations
{
   public class CLIExceptionManager : ExceptionManagerBase
   {
      public override void LogException(Exception ex)
      {
         throw ex;
      }
   }

}