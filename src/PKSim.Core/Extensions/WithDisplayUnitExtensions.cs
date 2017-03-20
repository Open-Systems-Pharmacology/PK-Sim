using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Extensions
{
   public static class WithDisplayUnitExtensions
   {
      private static readonly DoubleFormatter _doubleFormatter = new DoubleFormatter();

      public static string DisplayValue(this IWithDisplayUnit withDisplayUnit, double valueInCoreUnit)
      {
         return _doubleFormatter.Format(withDisplayUnit.ConvertToDisplayUnit(valueInCoreUnit), withDisplayUnit.DisplayUnit);
      }
   }
}