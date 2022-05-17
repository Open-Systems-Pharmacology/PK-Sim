using System;
using System.Globalization;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Utility.Format;
using PKSim.Assets;

namespace PKSim.Core.Model
{
   public static class ParameterMessages
   {
      private static readonly NumericFormatter<double> _numericFormatter = new NumericFormatter<double>(NumericFormatterOptions.Instance);

      public static string SetParameterValue(IParameter parameter, string parameterDisplayName, string oldValue, string valueToSet)
      {
         return PKSimConstants.Command.SetParameterValueDescription(parameterDisplayName,
            oldValue,
            valueToSet);
      }

      public static string SetParameterValue(IParameter parameter, string parameterDisplayName, double oldValue, double valueToSet)
      {
         return SetParameterValue(parameter, parameterDisplayName, DisplayValueFor(parameter, oldValue), DisplayValueFor(parameter, valueToSet));
      }

      public static string SetParameterUnit(IParameter parameter, string parameterDisplayName, Unit oldUnit, Unit newUnit)
      {
         return PKSimConstants.Command.SetParameterUnitDescription(parameterDisplayName,
            displayFor(parameter.ValueInDisplayUnit),
            oldUnit.ToString(),
            newUnit.ToString());
      }

      public static string SetParameterDisplayUnit(string parameterDisplayName, Unit oldUnit, Unit newUnit)
      {
         return PKSimConstants.Command.SetParameterDisplayUnitDescription(parameterDisplayName,
            oldUnit.ToString(),
            newUnit.ToString());
      }

      public static string SetParameterPercentile(string parameterDisplayName, double oldPercentile, double newPercentile)
      {
         return string.Format(PKSimConstants.Command.SetPercentileValueDescription, parameterDisplayName, oldPercentile, newPercentile);
      }

      public static string SetParameterFormula(string parameterDisplayName)
      {
         return string.Format(PKSimConstants.Command.SetParameterFormulaDescription, parameterDisplayName);
      }

      public static string UpdateTableParameterFormula(string parameterDisplayName)
      {
         return string.Format(PKSimConstants.Command.UpdateTableParameterFormula, parameterDisplayName);
      }

      public static string ResetParameterValue(IParameter parameter, string parameterDisplayName, double oldValue)
      {
         return PKSimConstants.Command.ResetParameterValueDescription(parameterDisplayName,
            DisplayValueFor(parameter, oldValue),
            DisplayValueFor(parameter, tryGetValue(parameter)));
      }

      public static string DisplayValueFor(IParameter parameter, double baseValue, bool numericalDisplayOnly = false)
      {
         if (double.IsNaN(baseValue))
            return string.Empty;

         var displayValue = displayFor(parameter, baseValue, numericalDisplayOnly);
         var displayUnit = DisplayUnitFor(parameter);

         if (string.IsNullOrEmpty(displayUnit) || numericalDisplayOnly)
            return displayValue;

         return $"{displayValue} {displayUnit}";
      }

      private static string displayFor(IParameter parameter, double value, bool numericalDisplayOnly)
      {
         if (parameter.NameIsOneOf(Constants.Parameters.AllBooleanParameters))
         {
            if (numericalDisplayOnly)
               return value.ToString(CultureInfo.InvariantCulture);

            return value == 1 ? PKSimConstants.UI.Yes : PKSimConstants.UI.No;
         }

         return displayFor(parameter.ConvertToDisplayUnit(value));
      }

      /// <summary>
      ///    Returns the formatted display value for the <paramref name="parameter" />. If the flag
      ///    <paramref name="numericalDisplayOnly" /> is set to <c>true</c>, the returned display value will only contain
      ///    numbers. Otherwise unit will be added to display and boolean will be converted to true or false.
      ///    Default value if <c>false</c>
      /// </summary>
      public static string DisplayValueFor(IParameter parameter, bool numericalDisplayOnly = false)
      {
         return DisplayValueFor(parameter, tryGetValue(parameter), numericalDisplayOnly);
      }

      public static string DisplayUnitFor(IParameter parameter)
      {
         var displayUnit = parameter.DisplayUnit;
         return string.IsNullOrEmpty(displayUnit?.Name) ? string.Empty : displayUnit.Name;
      }

      private static double tryGetValue(IParameter parameter)
      {
         try
         {
            return parameter.Value;
         }
         catch (Exception)
         {
            return double.NaN;
         }
      }

      private static string displayFor(double value)
      {
         return _numericFormatter.Format(value);
      }
   }
}