using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public static class NumericValueFieldExtensions
   {
      public static double ValueInDisplayUnit(this INumericValueField numericValueField, double valueInBaseUnit)
      {
         return numericValueField.ConvertToDisplayUnit(valueInBaseUnit);
      }

      public static double ValueInCoreUnit(this INumericValueField numericValueField, double valueInDisplayUnit)
      {
         return numericValueField.ConvertToBaseUnit(valueInDisplayUnit);
      }

      internal static IDimension UpdateDimension(this INumericValueField numericValueField, IDimension dimension)
      {
         var dimensionToUse = dimension ?? Constants.Dimension.NO_DIMENSION;
         numericValueField.DisplayUnit = dimensionToUse.DefaultUnit;
         return dimensionToUse;
      }
   }
}