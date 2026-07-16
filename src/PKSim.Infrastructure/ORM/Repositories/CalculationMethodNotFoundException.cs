using OSPSuite.Utility.Exceptions;
using PKSim.Assets;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class CalculationMethodNotFoundException : OSPSuiteException
   {
      public CalculationMethodNotFoundException(string calculationMethod)
         : base(string.Format(PKSimConstants.Error.CalculationMethodNotFound(calculationMethod)))
      {
      }
   }
}