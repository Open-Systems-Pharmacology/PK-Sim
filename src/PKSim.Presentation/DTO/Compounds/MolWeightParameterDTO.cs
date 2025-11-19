using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core.Model;
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
            .WithRule((dto, folder) => dto.Parameter.IsEffectiveMolWeightPositive(dto.Parameter.ConvertToBaseUnit(folder), dto._effectiveMolWeightParameter))
            .WithError(PKSimConstants.Error.EffectiveMolWeightCannotBeNegative);

         public static IEnumerable<IBusinessRule> All()
         {
            yield return effectiveMolWeightPositive;
         }
      }
   }
}