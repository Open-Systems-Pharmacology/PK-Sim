using PKSim.Assets;
using OSPSuite.Utility.Exceptions;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class CalculationMethodNotLicensedException : OSPSuiteException
   {
      public CalculationMethodNotLicensedException(string calculationMethod)
         : base(string.Format(PKSimConstants.Error.CalculationMethodIsNotLicensed(calculationMethod)))
      {
      }
   }
}