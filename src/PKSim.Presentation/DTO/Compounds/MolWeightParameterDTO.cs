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

      public MolWeightParameterDTO(IParameter molWeightParameter, IParameter effectiveMolWeightParameter) : base(molWeightParameter)
      {
         _effectiveMolWeightParameter = effectiveMolWeightParameter;
         Rules.AddRange(AllRules.All());
      }

      private static class AllRules
      {
         private static IBusinessRule effectiveMolWeightPositive { get; } = CreateRule.For<MolWeightParameterDTO>()
            .Property(item => item.Value)
            .WithRule((dto, valueInDisplayUnit) =>
            {
               var oldValue = dto.Parameter.Value;
               try
               {
                  dto.Parameter.Value = dto.Parameter.ConvertToBaseUnit(valueInDisplayUnit);
                  return dto._effectiveMolWeightParameter.Value > 0;
               }
               finally
               {
                  dto.Parameter.Value = oldValue;
               }
               
            })
            .WithError(PKSimConstants.Error.EffectiveMolWeightCannotBeNegative);

         public static IEnumerable<IBusinessRule> All()
         {
            yield return effectiveMolWeightPositive;
         }
      }
   }
}