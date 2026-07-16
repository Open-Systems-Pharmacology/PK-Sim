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
         private static readonly NumericFormatter<double> _numericFormatter = new(NumericFormatterOptions.Instance);

         private static IBusinessRule effectiveMolWeightGreaterThanMinimum { get; } = CreateRule.For<EffectiveMolWeightParameterDTO>()
            .Property(item => item.Value)
            .WithRule((dto, valueInDisplayUnit) =>
            {
               // Nan values are invalid
               if (double.IsNaN(valueInDisplayUnit))
                  return false;

               // For effective mol weight is a formula-only calculation and is updated through halogens and/or mol weight
               // and never directly from user input. The value at time of validation is always current because the triggering
               // update has already occurred. Either mol weight was updated, or halogen count
               return effectiveMolWeightIsValid(dto);
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