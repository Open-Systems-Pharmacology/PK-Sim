using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Format;
using OSPSuite.Utility.Validation;

namespace PKSim.Presentation.DTO.Compounds;

public class OverwriteParameterValueDTO : DxValidatableDTO
{
   private static readonly NumericFormatter<double> _numericFormatter = new(NumericFormatterOptions.Instance);

   public OverwriteParameterValueDTO(ParameterValue parameterValue)
   {
      ParameterValue = parameterValue;
      Rules.AddRange(AllRules.All());
   }

   public ParameterValue ParameterValue { get; }

   public string Path => ParameterValue.Path.ToString();

   public double? Value => ParameterValue.Value.HasValue ? ParameterValue.ConvertToDisplayUnit(ParameterValue.Value) : null;

   public string Unit => ParameterValue.DisplayUnit.Name;

   public string ValueOrigin => ParameterValue.ValueOrigin.Display;

   public double ValueInBaseUnit(double valueInDisplayUnit) => ParameterValue.ConvertToBaseUnit(valueInDisplayUnit);

   private string displayValueFor(double? valueInBaseUnit) => _numericFormatter.Format(ParameterValue.ConvertToDisplayUnit(valueInBaseUnit));

   private static class AllRules
   {
      private static IBusinessRule valueIsInAllowedRange { get; } = CreateRule.For<OverwriteParameterValueDTO>()
         .Property(dto => dto.Value)
         .WithRule((dto, valueInDisplayUnit) => errorFor(dto, valueInDisplayUnit) == null)
         .WithError((dto, valueInDisplayUnit) => errorFor(dto, valueInDisplayUnit));

      private static string errorFor(OverwriteParameterValueDTO dto, double? valueInDisplayUnit)
      {
         var info = dto.ParameterValue.Info;
         //no allowed range is known unless the entry was created by committing a simulation parameter, so anything goes
         if (info == null || !valueInDisplayUnit.HasValue)
            return null;

         var value = dto.ValueInBaseUnit(valueInDisplayUnit.Value);
         var parameterName = dto.ParameterValue.Name;

         if (info.MinValue.HasValue)
         {
            if (value < info.MinValue.Value)
               return Validation.ValueBiggerThanMin(parameterName, dto.displayValueFor(info.MinValue), dto.Unit);

            if (value == info.MinValue.Value && !info.MinIsAllowed)
               return Validation.ValueStrictBiggerThanMin(parameterName, dto.displayValueFor(info.MinValue), dto.Unit);
         }

         if (info.MaxValue.HasValue)
         {
            if (value > info.MaxValue.Value)
               return Validation.ValueSmallerThanMax(parameterName, dto.displayValueFor(info.MaxValue), dto.Unit);

            if (value == info.MaxValue.Value && !info.MaxIsAllowed)
               return Validation.ValueStrictSmallerThanMax(parameterName, dto.displayValueFor(info.MaxValue), dto.Unit);
         }

         return null;
      }

      public static IEnumerable<IBusinessRule> All()
      {
         yield return valueIsInAllowedRange;
      }
   }
}
