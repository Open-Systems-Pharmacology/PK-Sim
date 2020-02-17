using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Chart;

namespace PKSim.Core.Extensions
{
   public static class WithDisplayUnitExtensions
   {
      private static readonly DoubleFormatter _doubleFormatter = new DoubleFormatter();

      public static string DisplayValueWithUnit(this IWithDisplayUnit withDisplayUnit, double valueInBaseUnit, IDimension valueDimension = null)
      {
         return _doubleFormatter.Format(DisplayValue(withDisplayUnit, valueInBaseUnit, valueDimension), withDisplayUnit.DisplayUnit);
      }

      public static double DisplayValue(this IWithDisplayUnit withDisplayUnit, double valueInBaseUnit, IDimension valueDimension = null)
      {
         var dimensionToUse = valueDimension ?? withDisplayUnit.Dimension;
         //This value now needs to be converted in the display value of the target unit
         return dimensionToUse.BaseUnitValueToUnitValue(withDisplayUnit.DisplayUnit, valueInBaseUnit);
      }
   }
}