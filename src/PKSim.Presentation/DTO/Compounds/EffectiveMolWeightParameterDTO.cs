using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Format;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Presentation.DTO.Parameters;

namespace PKSim.Presentation.DTO.Compounds
{
   public class EffectiveMolWeightParameterDTO : ParameterDTO
   {
      private readonly IParameter _effectiveMolWeightParameter;

      /// <summary>
      ///    Creates a DTO that adds a validation rule to ensure that the effective molecular weight is always greater than the
      ///    minimum allowed
      /// </summary>
      /// <param name="effectiveMolWeightParameter">The effective mol weight parameter</param>
      public EffectiveMolWeightParameterDTO(IParameter effectiveMolWeightParameter) : base(effectiveMolWeightParameter)
      {
         _effectiveMolWeightParameter = effectiveMolWeightParameter;
         Rules.AddRange(AllRules.All());
      }

      private static class AllRules
      {
         private static readonly NumericFormatter<double> _numericFormatter = new NumericFormatter<double>(NumericFormatterOptions.Instance);

         private static IBusinessRule effectiveMolWeightGreaterThanMinimum { get; } = CreateRule.For<EffectiveMolWeightParameterDTO>()
            .Property(item => item.Value)
            .WithRule((dto, valueInDisplayUnit) =>
            {
               var valueInBaseUnit = dto.Parameter.ConvertToBaseUnit(valueInDisplayUnit);

               // Nan values are invalid
               if (double.IsNaN(valueInBaseUnit))
                  return false;

               // evaluate without updating the parameter because writing a value will trigger another validation
               if (valueInBaseUnit == dto.Parameter.Value)
                  return effectiveMolWeightIsValid(dto);

               // Update the parameter value to evaluate if the effective mol weight will be valid.
               // Reset to the old value after
               var oldValue = dto.Parameter.Value;
               try
               {
                  dto.Parameter.Value = valueInBaseUnit;
                  return effectiveMolWeightIsValid(dto);
               }
               finally
               {
                  dto.Parameter.Value = oldValue;
               }
            })
            .WithError((dto, value) =>
               PKSimConstants.Error.EffectiveMolWeightMustBeGreaterThan(
                  _numericFormatter.Format(dto._effectiveMolWeightParameter.ConvertToDisplayUnit(dto._effectiveMolWeightParameter.MinValue)),
                  dto._effectiveMolWeightParameter.DisplayUnit.Name));

         private static bool effectiveMolWeightIsValid(EffectiveMolWeightParameterDTO dto) => !dto._effectiveMolWeightParameter.MinValue.HasValue || dto._effectiveMolWeightParameter.Value >= dto._effectiveMolWeightParameter.MinValue;

         public static IEnumerable<IBusinessRule> All()
         {
            yield return effectiveMolWeightGreaterThanMinimum;
         }
      }
   }
}