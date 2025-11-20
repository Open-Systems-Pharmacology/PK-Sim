using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Presentation.DTO.Parameters;

namespace PKSim.Presentation.DTO.Compounds
{
   public class MolWeightParameterDTO : ParameterDTO
   {
      private readonly IParameter _effectiveMolWeightParameter;

      /// <summary>
      /// Creates a DTO that adds a validation rule to ensure that the effective molecular weight is always greater than the minimum allowed
      /// </summary>
      /// <param name="parameterRelatedToEffectiveMolWeight">A parameter that affects the effective mol weight (mol weight, F, Br, Cl, I)</param>
      /// <param name="effectiveMolWeightParameter">The effective mol weight parameter</param>
      public MolWeightParameterDTO(IParameter parameterRelatedToEffectiveMolWeight, IParameter effectiveMolWeightParameter) : base(parameterRelatedToEffectiveMolWeight)
      {
         _effectiveMolWeightParameter = effectiveMolWeightParameter;
         Rules.AddRange(AllRules.All());
      }

      private static class AllRules
      {
         private static IBusinessRule effectiveMolWeightGreaterThanMinimum { get; } = CreateRule.For<MolWeightParameterDTO>()
            .Property(item => item.Value)
            .WithRule((dto, valueInDisplayUnit) =>
            {
               var oldValue = dto.Parameter.Value;
               try
               {
                  dto.Parameter.Value = dto.Parameter.ConvertToBaseUnit(valueInDisplayUnit);
                  return !dto._effectiveMolWeightParameter.MinValue.HasValue || dto._effectiveMolWeightParameter.Value >= dto._effectiveMolWeightParameter.MinValue;
               }
               finally
               {
                  dto.Parameter.Value = oldValue;
               }
               
            })
            .WithError((dto, value) => PKSimConstants.Error.EffectiveMolWeightMustBeGreaterThan(dto._effectiveMolWeightParameter.ConvertToDisplayUnit(dto._effectiveMolWeightParameter.MinValue), dto._effectiveMolWeightParameter.DisplayUnit.Name));

         public static IEnumerable<IBusinessRule> All()
         {
            yield return effectiveMolWeightGreaterThanMinimum;
         }
      }
   }
}