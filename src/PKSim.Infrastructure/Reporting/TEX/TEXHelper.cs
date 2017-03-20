using System;
using System.Globalization;
using System.Linq;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.TeXReporting.TeX.PGFPlots;

namespace PKSim.Infrastructure.Reporting.TeX
{
   /// <summary>
   /// This class has some often used translation helper functions.
   /// </summary>
   static class TEXHelper
   {

      public static PlotOptions.ErrorTypes GetErrorType(AuxiliaryType auxiliaryType)
      {
         switch (auxiliaryType)
         {
            case AuxiliaryType.Undefined:
               return PlotOptions.ErrorTypes.arithmetic;
            case AuxiliaryType.ArithmeticStdDev:
               return PlotOptions.ErrorTypes.arithmetic;
            case AuxiliaryType.GeometricStdDev:
               return PlotOptions.ErrorTypes.geometric;
            case AuxiliaryType.ArithmeticMeanPop:
               return PlotOptions.ErrorTypes.arithmetic;
            case AuxiliaryType.GeometricMeanPop:
               return PlotOptions.ErrorTypes.geometric;
            default:
               throw new ArgumentOutOfRangeException("auxiliaryType");
         }
      }

      public static PlotOptions.Markers GetMarker(Symbols symbol)
      {
         switch (symbol)
         {
            case Symbols.None:
               return PlotOptions.Markers.None;
            case Symbols.Circle:
               return PlotOptions.Markers.Circle;
            case Symbols.Diamond:
               return PlotOptions.Markers.Diamond;
            case Symbols.Triangle:
               return PlotOptions.Markers.Triangle;
            case Symbols.Square:
               return PlotOptions.Markers.Square;
            default:
               throw new ArgumentOutOfRangeException("symbol");
         }
      }

      public static string GetOpacityFor(int transparency)
      {
         return (1 - transparency / 255D).ToString("0.00", CultureInfo.InvariantCulture);
      }

      // manages conversion from float to double and back
      public static float ValueInDisplayUnit(IDimension dimension, Unit displayUnit, float baseUnitValue)
      {
         double dBaseUnitValue = Convert.ToDouble(baseUnitValue);
         double dDisplayUnitValue = dimension.BaseUnitValueToUnitValue(displayUnit, dBaseUnitValue);
         return Convert.ToSingle(dDisplayUnitValue);
      }

      public static float[] ValueInDisplayUnit(IDimension dimension, Unit displayUnit, float[] baseUnitValues)
      {
         return baseUnitValues.Select(baseUnitValue => ValueInDisplayUnit(dimension, displayUnit, baseUnitValue)).ToArray();
      }

   }
}
